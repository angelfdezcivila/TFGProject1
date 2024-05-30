namespace Cellular.FloorFields
{
 public interface IFloorField {
    
    /// <returns>Number of rows in this floor field.</returns>
    int GetRows();
    
    /// <returns>Number of columns in this floor field.</returns>
    int GetColumns();
    
    /// <summary>
    /// Initializes this floor field.
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Gets field of cell located at given row and column.
    /// </summary>
    /// <param name="row">vertical coordinate of cell.</param>
    /// <param name="column">horizontal coordinate of cell.</param>
    /// <returns>Field of cell located at {@code row} and {@code column}.</returns>
    double GetField(int row, int column);
    
    /// <summary>
    /// Gets field of cell located at given location.
    /// </summary>
    /// <param name="location">Location location of cell.</param>
    /// <returns>Field of cell located at {@code location}.</returns>
    double GetField(Location location);
    
 }
}