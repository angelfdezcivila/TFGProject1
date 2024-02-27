using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using StageGenerator;
using UnityEngine;

namespace Pedestrian
{
    public class Pedestrian
    {
        /**
         * Class counter to generate unique identifiers for pedestrians.
         */
        protected static int nextIdentifier = 0;

        /**
         * Each pedestrian has a unique identifier.
         */
        protected readonly int identifier;

        /**
         * Row in scenario where pedestrian is currently located.
         */
        protected int row;

        /**
         * Column in scenario where pedestrian is currently located.
         */
        protected int column;

        /**
         * Number of steps currently taken by pedestrian.
         */
        protected int numberOfSteps;

        /**
         * Number of discrete time steps elapsed when pedestrian exited the scenario.
         */
        protected int exitTimeSteps;

        /**
         * Parameters describing this pedestrian.
         */
        protected readonly PedestrianParameters parameters;

        /**
         * Automaton where this pedestrian is running.
         */
        protected readonly CellularAutomaton automaton;

        /**
         * Path followed by pedestrian in scenario during simulation.
         */
        protected readonly List<Location> path;

        /**
         * A tentative movement consists of a location (where we should move) and a desirability (the higher the
         * desirability the higher the willingness to move to such location). We do not use the term probability because
         * sum of all desirabilities do not have to be 1.
         *
         * @param location     where we should move.
         * @param desirability willing to move to such location
         */
        
        protected record TentativeMovement(Location location, float desirability) : IComparable<TentativeMovement>
        {

            public int CompareTo(TentativeMovement other)
            {
                return this.desirability.CompareTo(other.desirability);
            }

            public Location location { get; } = location;
            public float desirability { get; } = desirability;

        }

        /**
         * Constructs a new pedestrian.
         *
         * @param row        row in scenario where pedestrian will be located.
         * @param column     column in scenario where pedestrian will be located.
         * @param parameters parameters describing new pedestrian.
         * @param automaton  automaton where this pedestrian evolves.
         */
        public Pedestrian(int row, int column, PedestrianParameters parameters, CellularAutomaton automaton)
        {
            this.identifier = nextIdentifier++;
            this.row = row;
            this.column = column;
            this.parameters = parameters;
            this.automaton = automaton;
            this.numberOfSteps = 0;
            this.path = new List<Location>();
            this.path.Add(new Location(row, column));
        }

        /**
         * Unique identifier corresponding to this pedestrian.
         *
         * @return unique identifier corresponding to this pedestrian.
         */
        public int getIdentifier()
        {
            return identifier;
        }

        /**
         * Row in scenario where this pedestrian is currently located.
         *
         * @return row in scenario where this pedestrian is currently located.
         */
        public int GetRow()
        {
            return row;
        }

        /**
         * Column in scenario where this pedestrian is currently located.
         *
         * @return column in scenario where this pedestrian is currently located.
         */
        public int GetColumn()
        {
            return column;
        }

        /**
         * Location in scenario where this pedestrian is currently located.
         *
         * @return location in scenario where this pedestrian is currently located.
         */
        public Location getLocation()
        {
            return new Location(row, column);
        }

        /**
         * Path followed by pedestrian in scenario during simulation.
         *
         * @return path followed by pedestrian in scenario during simulation.
         */
        public List<Location> getPath()
        {
            return path;
        }

        /**
         * Make pedestrian move to cell with coordinates {@code row} and {@code column}.
         *
         * @param row    vertical coordinate of destination cell.
         * @param column horizontal coordinate of destination cell.
         */
        public void moveTo(int row, int column)
        {
            this.row = row;
            this.column = column;
            this.numberOfSteps++;
            this.path.Add(new Location(row, column));
        }

        /**
         * Make pedestrian move to cell with coordinates defined by {@code location}.
         *
         * @param location location of destination cell.
         */
        public void moveTo(Location location)
        {
            moveTo(location.row, location.column);
        }

        /**
         * Make the pedestrian to stay in its current cell.
         */
        public void doNotMove()
        {
            this.path.Add(new Location(row, column));
        }

        /**
         * Number of steps currently taken by this pedestrian
         *
         * @return number of steps currently taken by this pedestrian
         */
        public int getNumberOfSteps()
        {
            return numberOfSteps;
        }

        /**
         * Record time (as number of discrete time steps) when pedestrian exited the scenario.
         *
         * @param timeSteps number of discrete time steps elapsed when pedestrian exited the scenario.
         */
        public void SetExitTimeSteps(int timeSteps)
        {
            this.exitTimeSteps = timeSteps;
        }

        /**
         * Number of discrete time steps elapsed when pedestrian exited the scenario.
         *
         * @return number of discrete time steps elapsed when pedestrian exited the scenario.
         */
        public int getExitTimeSteps()
        {
            return exitTimeSteps;
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
            Stage scenario = automaton.GetScenario();
            List<Location> neighbours = automaton.Neighbours(row, column);

            // var movements = new List<TentativeMovement>(neighbours.size());
            var movements = new List<TentativeMovement>();
            movements.Capacity = neighbours.Count; //No se si funciona correctamente
            float minDesirability = (float)Double.MaxValue;
            foreach (Location neighbour in neighbours) {
                if (automaton.IsCellReachable(neighbour))
                {
                    // count reachable cells around new location
                    var numberOfReachableCellsAround = 0;
                    foreach (Location around in automaton.Neighbours(neighbour)) {
                        if (automaton.IsCellReachable(around))
                        {
                            numberOfReachableCellsAround++;
                        }
                    }

                    float attraction = parameters.fieldAttractionBias * scenario.StaticFloorField.getField(neighbour);
                    // float attraction = parameters.fieldAttractionBias;
                    float repulsion = parameters.crowdRepulsion / (1 + numberOfReachableCellsAround);
                    float desirability = (float)Math.Exp(attraction - repulsion);
                    movements.Add(new TentativeMovement(neighbour, desirability));
                    if (desirability < minDesirability)
                        minDesirability = desirability;
                }
            }
            var gradientMovements = new List<TentativeMovement>(neighbours.Count);
            foreach (TentativeMovement m in movements)
                gradientMovements.Add(new TentativeMovement(m.location,
                    (float)DESIRABILITY_EPSILON + m.desirability - minDesirability));

            return gradientMovements;
        }

        /**
         * Choose randomly pedestrian's next move from those computed by {@code computeTransitionDesirabilities}.
         *
         * @return {@code Optional.empty} if no move is available or {@code Optional(m)} if move {@code m} was chosen.
         */
        public Location ChooseMovement()
        {
            if (Statistics.bernoulli(parameters.velocityPercent))
            {
                // try to move at this step to respect pedestrian speed
                List<TentativeMovement> movements = computeTransitionDesirabilities();
                if (movements.Count <= 0)
                {
                    // cannot make a movement
                    return null;
                }

                // choose one movement according to discrete distribution of desirabilities
                var chosen = Statistics.Discrete(movements, m => m.desirability);
                return chosen.location;
            }
            else
            {
                // do not move at this step to respect pedestrian speed
                return null;
            }
        }

        /**
         * A hash code for this pedestrian.
         *
         * @return a hash code for this pedestrian.
         */
        public override int GetHashCode()
        {
            // return Integer.hashCode(identifier);
            return HashCode.Combine(identifier);
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

            if (identifier == other.identifier)
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
            return "Pedestrian" + "(" + getLocation().ToString() + ", " + identifier + ")";
        }
    }

}