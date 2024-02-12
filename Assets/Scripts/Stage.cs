using UnityEngine;

public class Stage
{
    private GameObject _obstaclePrefab;
    private Vector2 _cellsDimension;
    private LayerMask _whatIsWall;
    private int _rows;
    private int _columns;

    private Stage(GameObject obstaclePrefab, Vector2 cellsDimension, LayerMask whatIsWall, int rows, int columns)
    {
        _obstaclePrefab = obstaclePrefab;
        _cellsDimension = cellsDimension;
        _whatIsWall = whatIsWall;
        _rows = rows;
        _columns = columns;
    }

    public class Builder {
        private int _rows = 10;
        private int _columns = 10;
        private Vector2 _cellsDimension = new Vector2(1f, 1f);
        private GameObject _obstaclePrefab;
        private LayerMask _whatIsWall;

        /// <param name="rows">Número de filas del escenario</param>
        public Builder rows(int rows) {
            this._rows = rows;
            return this;
        }
        
        /// <param name="columns">Número de columnas del escenario</param>
        public Builder columns(int columns) {
            this._columns = columns;
            return this;
        }

        /**
         * @param cellDimension Cells are rectangles. Dimension (in meters) of both sides of a grid cell in stage.
         */
        public Builder cellDimension(Vector2 cellsDimension) {
            this._cellsDimension = cellsDimension;
            return this;
        }

        /**
         * @param prefab of the obstacle
         */
        public Builder obstaclePrefab(GameObject obstaclePrefab)
        {
            this._obstaclePrefab = obstaclePrefab;
            return this;
        }

        public Builder whatIsWall(LayerMask whatIsWall)
        {
            this._whatIsWall = whatIsWall;
            return this;
        }

        public Stage build() {
            return new Stage(_obstaclePrefab, _cellsDimension, _whatIsWall, _rows, _columns);
        }
    }
}