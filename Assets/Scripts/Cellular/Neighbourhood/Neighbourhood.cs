using System.Collections.Generic;

public interface Neighbourhood {
    /**
     * Returns neighbourhood of a cell.
     *
     * @param row    vertical coordinate of cell.
     * @param column horizontal  coordinate of cell.
     * @return locations of all cells in neighborhood of cell.
     */
    List<Location> Neighbours(int row, int column);

    /**
     * Returns neighbourhood of a cell.
     *
     * @param location location of cell.
     * @return locations of all cells in neighborhood of cell.
     */
    List<Location> Neighbours(Location location) {
        return Neighbours(location.row, location.column);
    }
}