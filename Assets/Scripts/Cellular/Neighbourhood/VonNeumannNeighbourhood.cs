using System.Collections.Generic;
using StageGenerator;

public class VonNeumannNeighbourhood : Neighbourhood
{
    private int rows, columns;

    /**
     * Creates a Von Neumann neighbourhood for a scenario.
     *
     * @param rows    number of rows in scenario.
     * @param columns number of columns in scenario.
     */
    public VonNeumannNeighbourhood(int rows, int columns) {
        this.rows = rows;
        this.columns = columns;
    }

    /**
     * Creates a Von Neumann neighbourhood for given scenario.
     *
     * @param scenario scenario in which neighbourhood is described.
     * @return a Von Neumann neighbourhood for given scenario.
     */
    public static VonNeumannNeighbourhood of(Stage scenario) {
        return new VonNeumannNeighbourhood(scenario.Rows, scenario.Columns);
    }

    public List<Location> Neighbours(int row, int column) {
        var neighbours = new List<Location>(4);
        // north
        if (row < rows - 1) {
            neighbours.Add(new Location(row + 1, column));
        }
        // south
        if (row > 0) {
            neighbours.Add(new Location(row - 1, column));
        }
        // east
        if (column < columns - 1) {
            neighbours.Add(new Location(row, column + 1));
        }
        // west
        if (column > 0) {
            neighbours.Add(new Location(row, column - 1));
        }
        return neighbours;
    }
}