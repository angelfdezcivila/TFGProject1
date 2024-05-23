using System;
using System.Collections.Generic;
using Cellular;
using JetBrains.Annotations;
using StageGenerator;
using UnityEngine;

namespace Pedestrians
{
    public class Pedestrian : MonoBehaviour
    {
        /// <summary>
        /// Minimum desirability of a cell so that it is never 0.
        /// </summary>
        private const double DESIRABILITY_EPSILON = 0.00001;

        #region Private variables

        /// <summary>
        /// Class counter to generate unique identifiers for pedestrians.
        /// </summary>
        private static int _nextIdentifier = 0;

        /// <summary>
        /// Each pedestrian has a unique identifier.
        /// </summary>
        private int _identifier;

        /// <summary>
        /// Row in scenario where pedestrian is currently located.
        /// </summary>
        private int _row;

        /// <summary>
        /// Column in scenario where pedestrian is currently located.
        /// </summary>
        private int _column;

        /// <summary>
        /// Number of steps currently taken by pedestrian.
        /// </summary>
        private int _numberOfSteps;

        /// <summary>
        /// Number of discrete time steps elapsed when pedestrian exited the scenario.
        /// </summary>
        private int _exitTimeSteps;

        /// <summary>
        /// Parameters describing this pedestrian.
        /// </summary>
        private PedestrianParameters _parameters;

        /// <summary>
        /// Automaton where this pedestrian is running.
        /// </summary>
        private CellularAutomaton _automaton;

        /// <summary>
        /// Path followed by pedestrian in scenario during simulation.
        /// </summary>
        private List<Location> _path;

        #endregion

        #region Public Properties

        /// <summary>
        /// Unique identifier corresponding to this pedestrian.
        /// </summary>
        public int Identifier => _identifier;

        /// <summary>
        /// Row in scenario where this pedestrian is currently located.
        /// </summary>
        public int Row => _row;

        /// <summary>
        /// Column in scenario where this pedestrian is currently located.
        /// </summary>
        public int Column => _column;

        /// <summary>
        /// Location in scenario where this pedestrian is currently located.
        /// </summary>
        public Location Location => new(_row, _column);

        /// <summary>
        /// Path followed by pedestrian in scenario during simulation.
        /// </summary>
        public List<Location> Path => _path;
        
        
        /// <summary>
        /// Number of steps currently taken by this pedestrian
        /// </summary>
        public int NumberOfSteps => _numberOfSteps;

        /// <summary>
        /// Number of discrete time steps elapsed when pedestrian exited the scenario.
        /// </summary>
        public int ExitTimeSteps
        {
            get => _exitTimeSteps;
            set => _exitTimeSteps = value;
        }
        
        #endregion
        
        /// <summary>
        /// A tentative movement consists of a location (where we should move) and a desirability (the higher the
        /// desirability the higher the willingness to move to such location). We do not use the term probability because
        /// sum of all desirabilities do not have to be 1.
        /// </summary>
        /// <param name="location">Where we should move.</param>
        /// <param name="desirability">Willing to move to such location.</param>
        private record TentativeMovement(Location location, double desirability) : IComparable<TentativeMovement>
        {
            public int CompareTo(TentativeMovement other)
            {
                return this.desirability.CompareTo(other.desirability);
            }

            public Location location { get; } = location;
            public double desirability { get; } = desirability;

            public override string ToString()
            {
                return location.ToString() + ", desirability: " + desirability;
            }
        }
        
        /// <summary>
        /// Reset pedestrians identifiers count to 0.
        /// </summary>
        public static void ResetIdentifiers()
        {
            _nextIdentifier = 0;
        }

        #region Public Methods

        /// <summary>
        /// Constructs a new pedestrian.
        /// </summary>
        /// <param name="row">Row in scenario where pedestrian will be located.</param>
        /// <param name="column">Column in scenario where pedestrian will be located.</param>
        /// <param name="parameters">Parameters describing new pedestrian.</param>
        /// <param name="automaton">Automaton where this pedestrian evolves.</param>
        internal void Initialize(int row, int column, PedestrianParameters parameters, CellularAutomaton automaton)
        {
            this._identifier = _nextIdentifier++;
            this._row = row;
            this._column = column;
            this._parameters = parameters;
            this._automaton = automaton;
            this._numberOfSteps = 0;
            this._path = new List<Location>();
            this._path.Add(new Location(row, column));
        }

        /// <summary>
        /// Make pedestrian move to cell with coordinates {@code row} and {@code column}.
        /// </summary>
        /// <param name="row">Vertical coordinate of destination cell.</param>
        /// <param name="column">Horizontal coordinate of destination cell.</param>
        public void MoveTo(int row, int column)
        {
            this._row = row;
            this._column = column;
            this._numberOfSteps++;
            // this._path.Add(new Location(row, column));
            this._path.Add(new Location(row, column));
        }
        
        /// <summary>
        /// Make pedestrian move to cell with coordinates defined by {@code location}.
        /// </summary>
        /// <param name="location">Location of destination cell.</param>
        public void MoveTo(Location location) => MoveTo(location.Row, location.Column);

        /// <summary>
        /// Make the pedestrian to stay in its current cell.
        /// </summary>
        public void DoNotMove() => this._path.Add(new Location(_row, _column));
        
        /// <summary>
        /// Choose randomly pedestrian's next move from those computed by {@code computeTransitionDesirabilities}.
        /// </summary>
        /// <returns>The Location of the next movement. Null if it can't move or if Bernoulli based in pedestrians speed returns false.</returns>
        public Location ChooseMovement()
        {
            if (Statistics.Bernoulli(_parameters.velocityPercent))
            {
                // try to move at this step to respect pedestrian speed
                List<TentativeMovement> movements = ComputeTransitionDesirabilities();
                // Debug.Log("id: " + _identifier + ", Position: " + _row + ", " + _column);
                // movements.ForEach(movement => Debug.Log(movement));

                // if (movements.Count <= 0)
                if (movements.Count > 0)
                {
                    // choose one movement according to discrete distribution of desirabilities
                    TentativeMovement chosen = Statistics.Discrete(movements, m => m.desirability);
                    return chosen.location;
                    // return chosen.location != null ? chosen.location : null;
                }
            }

            // do not move at this step to respect pedestrian speed
            return null;
        }

        /// <summary>
        /// Updates pedestrian position.
        /// </summary>
        public void Paint()
        {
            Vector3 position = _automaton.Stage.GetRowColumnCell(new Vector2(_row, _column)).transform.position;
            transform.position = position + Vector3.up * transform.localScale.y / 2;
            // transform.position = new Vector3(position.x, position.y + transform.localScale.y/2, position.z);
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Computes transition desirabilities for reachable cells in the neighbourhood on this pedestrian. (the higher the
        /// desirability the higher the willingness to move to such location). We do not use the term probability because
        /// sum of all desirabilities do not have to be 1.
        /// </summary>
        /// <returns>List of tentative movements that this pedestrian can make, each one with associate desirability.</returns>
        private List<TentativeMovement> ComputeTransitionDesirabilities()
        {
            Stage scenario = _automaton.Stage;
            List<Location> neighbours = _automaton.Neighbours(_row, _column);

            // var movements = new List<TentativeMovement>(neighbours.size());
            List<TentativeMovement> movements = new List<TentativeMovement>
            {
                Capacity = neighbours.Count // this is to cap the capacity, although it is not necessary since the add is only inside a loop that iterates the neighbours list.
            };
            double minDesirability = Double.MaxValue;
            foreach (Location neighbour in neighbours) {
                if (_automaton.IsCellReachable(neighbour))
                {
                    // count reachable cells around new location
                    int numberOfReachableCellsAround = 0;
                    foreach (Location around in _automaton.Neighbours(neighbour)) {
                        if (_automaton.IsCellReachable(around))
                        {
                            numberOfReachableCellsAround++;
                        }
                    }

                    double attraction = _parameters.fieldAttractionBias * scenario.StaticFloorField.getField(neighbour);
                    // float attraction = parameters.fieldAttractionBias;
                    double repulsion = _parameters.crowdRepulsion / (1 + numberOfReachableCellsAround);
                    double desirability = Math.Exp(attraction - repulsion);
                    movements.Add(new TentativeMovement(neighbour, desirability));
                    if (desirability < minDesirability)
                        minDesirability = desirability;
                }
            }
            var gradientMovements = new List<TentativeMovement>(neighbours.Count);
            foreach (TentativeMovement m in movements)
                gradientMovements.Add(new TentativeMovement(m.location,
                    DESIRABILITY_EPSILON + m.desirability - minDesirability));

            return gradientMovements;
        }
        
        #endregion

        #region Overrided Methods

        /// <summary>
        /// A hash code for this pedestrian.
        /// </summary>
        public override int GetHashCode()
        {
            // return Integer.hashCode(identifier);
            return HashCode.Combine(_identifier);
        }
        
        /// <summary>
        /// Checks whether this pedestrian is equal to another object.
        /// </summary>
        /// <param name="obj">Another object to compare to this pedestrian.</param>
        /// <returns>If this pedestrian is equal to object obj</returns>
        public override bool Equals(System.Object obj)
        {
            Pedestrian other = obj as Pedestrian;
            if (other == null)
                return false;

            if (_identifier == other._identifier)
                return true;

            return false;
        }

        /// <summary>
        /// A textual representation of this pedestrian.
        /// </summary>
        public override string ToString()
        {
            return "Pedestrian" + "(" + Location.ToString() + ", id: " + _identifier + ")";
            // return "Pedestrian" + "(" + Location().ToString() + ", " + _identifier + ")";

        }
        
        #endregion
    }

}