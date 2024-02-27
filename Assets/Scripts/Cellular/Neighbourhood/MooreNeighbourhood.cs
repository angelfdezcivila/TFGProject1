using System.Collections.Generic;
using StageGenerator;
using UnityEngine;

public class MooreNeighbourhood : Neighbourhood
{
    private int rows, columns;

    /**
     * Creates a Von Neumann neighbourhood for a scenario.
     *
     * @param rows    number of rows in scenario.
     * @param columns number of columns in scenario.
     */
    public MooreNeighbourhood(int rows, int columns) {
        this.rows = rows;
        this.columns = columns;
    }

    /**
     * Creates a Von Neumann neighbourhood for given scenario.
     *
     * @param scenario scenario in which neighbourhood is described.
     * @return a Von Neumann neighbourhood for given scenario.
     */
    public static MooreNeighbourhood of(Stage scenario) {
        return new MooreNeighbourhood(scenario.Rows, scenario.Columns);
    }

    public List<Location> Neighbours(int row, int column) {
        var neighbours = new List<Location>(8);
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
        // northeast
        if (row < rows - 1 && column < columns - 1) {
            neighbours.Add(new Location(row + 1, column + 1));
        }
        // southeast
        if (row > 0 && column < columns - 1) {
            neighbours.Add(new Location(row - 1, column + 1));
        }
        // northwest
        if (row < rows - 1 && column > 0) {
            neighbours.Add(new Location(row + 1, column - 1));
        }
        // southwest
        if (row > 0 && column > 0) {
            neighbours.Add(new Location(row - 1, column - 1));
        }
        return neighbours;
    }
}