using System.Collections.Generic;
using StageGenerator;

namespace Cellular.Neighbourhood
{
    public class VonNeumannNeighbourhood : INeighbourhood
    {
        private readonly int _rows;
        private readonly int _columns;

        /// <summary>
        /// Creates a Von Neumann neighbourhood for a scenario.
        /// </summary>
        /// <param name="rows">Number of rows in scenario.</param>
        /// <param name="columns">Number of columns in scenario.</param>
        public VonNeumannNeighbourhood(int rows, int columns) {
            this._rows = rows;
            this._columns = columns;
        }


        /// <summary>
        /// Creates a Von Neumann neighbourhood for given scenario.
        /// </summary>
        /// <param name="scenario">Scenario in which neighbourhood is described.</param>
        /// <returns>A Von Neumann neighbourhood for given scenario.</returns>
        public static VonNeumannNeighbourhood Of(Stage scenario) {
            return new VonNeumannNeighbourhood(scenario.Rows, scenario.Columns);
        }

        public List<Location> Neighbours(int row, int column) {
            var neighbours = new List<Location>(4);
            // north
            if (row < _rows - 1) {
                neighbours.Add(new Location(row + 1, column));
            }
            // south
            if (row > 0) {
                neighbours.Add(new Location(row - 1, column));
            }
            // east
            if (column < _columns - 1) {
                neighbours.Add(new Location(row, column + 1));
            }
            // west
            if (column > 0) {
                neighbours.Add(new Location(row, column - 1));
            }
            return neighbours;
        }
    }
}