using Cellular;

namespace FloorFields
{
 public interface FloorField {
  /**
     * Gets number of rows in this floor field.
     *
     * @return number of rows in this floor field.
     */
  int getRows();

  /**
     * Gets number of columns in this floor field.
     *
     * @return number of columns in this floor field.
     */
  int getColumns();

  /**
     * Initializes this floor field.
     */
  void initialize();

  /**
     * Gets field of cell located at given row and column.
     *
     * @param row    vertical coordinate of cell.
     * @param column horizontal coordinate of cell.
     * @return field of cell located at {@code row} and {@code column}.
     */
  double getField(int row, int column);

  /**
     * Gets field of cell located at given location.
     *
     * @param location location of cell.
     * @return field of cell located at {@code location}.
     */
  double getField(Location location);
    
 }
}