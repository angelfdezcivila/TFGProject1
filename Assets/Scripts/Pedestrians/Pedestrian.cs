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
        /**
         * Class counter to generate unique identifiers for pedestrians.
         */
        private static int _nextIdentifier = 0;

        /**
         * Each pedestrian has a unique identifier.
         */
        private int _identifier;

        /**
         * Row in scenario where pedestrian is currently located.
         */
        private int _row;

        /**
         * Column in scenario where pedestrian is currently located.
         */
        private int _column;

        /**
         * Number of steps currently taken by pedestrian.
         */
        private int _numberOfSteps;

        /**
         * Number of discrete time steps elapsed when pedestrian exited the scenario.
         */
        private int _exitTimeSteps;

        /**
         * Parameters describing this pedestrian.
         */
        private PedestrianParameters _parameters;

        /**
         * Automaton where this pedestrian is running.
         */
        private CellularAutomaton _automaton;

        /**
         * Path followed by pedestrian in scenario during simulation.
         */
        // private List<Location> _path;
        public List<Location> _path;
        
        /**
         * A tentative movement consists of a location (where we should move) and a desirability (the higher the
         * desirability the higher the willingness to move to such location). We do not use the term probability because
         * sum of all desirabilities do not have to be 1.
         *
         * @param location     where we should move.
         * @param desirability willing to move to such location
         */
        
        protected record TentativeMovement(Location location, double desirability) : IComparable<TentativeMovement>
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

        /**
         * Constructs a new pedestrian.
         *
         * @param row        row in scenario where pedestrian will be located.
         * @param column     column in scenario where pedestrian will be located.
         * @param parameters parameters describing new pedestrian.
         * @param automaton  automaton where this pedestrian evolves.
         */
        protected Pedestrian(int row, int column, PedestrianParameters parameters, CellularAutomaton automaton)
        {
            Initialize(row, column, parameters, automaton);
        }

        public void Initialize(int row, int column, PedestrianParameters parameters, CellularAutomaton automaton)
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

        /**
         * Unique identifier corresponding to this pedestrian.
         *
         * @return unique identifier corresponding to this pedestrian.
         */
        public int Identifier => _identifier;

        /**
         * Row in scenario where this pedestrian is currently located.
         *
         * @return row in scenario where this pedestrian is currently located.
         */
        public int GetRow()
        {
            return _row;
        }

        /**
         * Column in scenario where this pedestrian is currently located.
         *
         * @return column in scenario where this pedestrian is currently located.
         */
        public int GetColumn()
        {
            return _column;
        }

        /**
         * Location in scenario where this pedestrian is currently located.
         *
         * @return location in scenario where this pedestrian is currently located.
         */
        public Location getLocation()
        {
            return new Location(_row, _column);
        }

        /**
         * Path followed by pedestrian in scenario during simulation.
         *
         * @return path followed by pedestrian in scenario during simulation.
         */
        public List<Location> GetPath()
        {
            return _path;
        }

        /**
         * Make pedestrian move to cell with coordinates {@code row} and {@code column}.
         *
         * @param row    vertical coordinate of destination cell.
         * @param column horizontal coordinate of destination cell.
         */
        public void moveTo(int row, int column)
        {
            this._row = row;
            this._column = column;
            this._numberOfSteps++;
            // this._path.Add(new Location(row, column));
            this._path.Add(new Location(row, column));
        }

        /**
         * Make pedestrian move to cell with coordinates defined by {@code location}.
         *
         * @param location location of destination cell.
         */
        public void moveTo(Location location)
        {
            moveTo(location.Row, location.Column);
        }

        /**
         * Make the pedestrian to stay in its current cell.
         */
        public void doNotMove()
        {
            this._path.Add(new Location(_row, _column));
        }

        /**
         * Number of steps currently taken by this pedestrian
         *
         * @return number of steps currently taken by this pedestrian
         */
        public int getNumberOfSteps()
        {
            return _numberOfSteps;
        }

        /**
         * Record time (as number of discrete time steps) when pedestrian exited the scenario.
         *
         * @param timeSteps number of discrete time steps elapsed when pedestrian exited the scenario.
         */
        public void SetExitTimeSteps(int timeSteps)
        {
            this._exitTimeSteps = timeSteps;
        }

        /**
         * Number of discrete time steps elapsed when pedestrian exited the scenario.
         *
         * @return number of discrete time steps elapsed when pedestrian exited the scenario.
         */
        public int getExitTimeSteps()
        {
            return _exitTimeSteps;
        }


        /**
         * Minimum desirability of a cell so that it is never 0.
         */
        private static readonly double DESIRABILITY_EPSILON = 0.00001;

        /**
         * Computes transition desirabilities for reachable cells in the neighbourhood on this pedestrian. (the higher the
         * desirability the higher the willingness to move to such location). We do not use the term probability because
         * sum of all desirabilities do not have to be 1.
         *
         * @return List of tentative movements that this pedestrian can make, each one with associate desirability.
         */
        private List<TentativeMovement> computeTransitionDesirabilities()
        {
            Stage scenario = _automaton.Stage;
            List<Location> neighbours = _automaton.Neighbours(_row, _column);

            // var movements = new List<TentativeMovement>(neighbours.size());
            List<TentativeMovement> movements = new List<TentativeMovement>
            {
                Capacity = neighbours.Count // esto es para capar la capacidad, aunque no es necesario ya que el add solo está dentro de un for que itera la lista neighbours
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

        /**
         * Choose randomly pedestrian's next move from those computed by {@code computeTransitionDesirabilities}.
         *
         * @return {@code Optional.empty} if no move is available or {@code Optional(m)} if move {@code m} was chosen.
         */
        public Location ChooseMovement()
        {
            if (Statistics.bernoulli(_parameters.velocityPercent))
            {
                // try to move at this step to respect pedestrian speed
                List<TentativeMovement> movements = computeTransitionDesirabilities();
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

        public void paint()
        {
            Vector3 position = _automaton.Stage.GetRowColumnCell(new Vector2(_row, _column)).transform.position;
            transform.position = position + Vector3.up * transform.localScale.y / 2;
            // transform.position = new Vector3(position.x, position.y + transform.localScale.y/2, position.z);
        }

        /**
         * A hash code for this pedestrian.
         *
         * @return a hash code for this pedestrian.
         */
        public override int GetHashCode()
        {
            // return Integer.hashCode(identifier);
            return HashCode.Combine(_identifier);
        }

        /**
         * Checks whether this pedestrian is equal to another object.
         *
         * @param o another object to compare to this pedestrian.
         * @return {@code true} this pedestrian is equal to object {@code o}.
         */
        public override bool Equals(System.Object obj)
        {
            Pedestrian other = obj as Pedestrian;
            if (other == null)
                return false;

            if (_identifier == other._identifier)
                return true;

            return false;
        }

        /**
         * A textual representation of this pedestrian.
         *
         * @return a textual representation of this pedestrian.
         */
        public override string ToString()
        {
            return "Pedestrian" + "(" + getLocation().ToString() + ", id: " + _identifier + ")";
            // return "Pedestrian" + "(" + getLocation().ToString() + ", " + _identifier + ")";

        }
    }

}