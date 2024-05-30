using System.Collections.Generic;
using StageGenerator;

namespace Cellular.Neighbourhood
{
    public class MooreNeighbourhood : INeighbourhood
    {
        private readonly int _rows;
        private readonly int _columns;

        /// <summary>
        /// Creates a Moore neighbourhood for a scenario.
        /// </summary>
        /// <param name="rows">Number of rows in scenario.</param>
        /// <param name="columns">Number of columns in scenario.</param>
        public MooreNeighbourhood(int rows, int columns) {
            this._rows = rows;
            this._columns = columns;
        }

        /// <summary>
        /// Creates a Moore neighbourhood for given scenario.
        /// </summary>
        /// <param name="scenario">Scenario in which neighbourhood is described.</param>
        /// <returns>A Moore neighbourhood for given scenario.</returns>
        public static MooreNeighbourhood Of(Stage scenario) {
            return new MooreNeighbourhood(scenario.Rows, scenario.Columns);
        }

        public List<Location> Neighbours(int row, int column) {
            var neighbours = new List<Location>(8);
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
            // northeast
            if (row < _rows - 1 && column < _columns - 1) {
                neighbours.Add(new Location(row + 1, column + 1));
            }
            // southeast
            if (row > 0 && column < _columns - 1) {
                neighbours.Add(new Location(row - 1, column + 1));
            }
            // northwest
            if (row < _rows - 1 && column > 0) {
                neighbours.Add(new Location(row + 1, column - 1));
            }
            // southwest
            if (row > 0 && column > 0) {
                neighbours.Add(new Location(row - 1, column - 1));
            }
            return neighbours;
        }
    }
}