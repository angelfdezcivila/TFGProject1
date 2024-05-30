using System.Collections.Generic;

namespace Cellular.Neighbourhood
{
    public interface INeighbourhood {
        
        /// <summary>
        /// Returns neighbourhood of a cell.
        /// </summary>
        /// <param name="row">vertical coordinate of cell.</param>
        /// <param name="column">horizontal coordinate of cell.</param>
        /// <returns>Locations of all cells in neighborhood of cell.</returns>
        List<Location> Neighbours(int row, int column);
        
        /// <summary>
        /// Returns neighbourhood of a cell.
        /// </summary>
        /// <param name="location">Location of cell.</param>
        /// <returns>locations of all cells in neighborhood of cell.</returns>
        List<Location> Neighbours(Location location) {
            return Neighbours(location.Row, location.Column);
        }
    }
}