using System.Collections.Generic;
using Cellular;
using FloorFields;
using JsonDataManager.Stage;
using UnityEngine;

namespace StageGenerator
{
    public abstract class Stage
    {
        private protected GameObject _cellPrefab;
        private protected int _numberOfBlocks;
        private protected Vector3 _cellsDimension;
        private protected int _rows;
        private protected int _columns;
        private protected List<List<Cell>> _cellMatrix; // Matriz de casillas
        private protected Transform _transformParent;
        private protected List<Cell> _exits;
        private protected List<AccessEntryJson> _exitsCornerLeftDown;
        private protected List<Cell> _obstacles;
        private protected List<ObstacleEntryJson> _obstaclesCornerLeftDown;
        private protected FloorField _staticFloorField;
        private GameObject _cellsContainer;
        public FloorField StaticFloorField => _staticFloorField;
        public List<Cell> Exits => _exits;
        public List<Cell> Obstacles => _obstacles;
        public List<AccessEntryJson> AccessesCornerLeftDown => _exitsCornerLeftDown;
        public List<ObstacleEntryJson> ObstaclesCornerLeftDown => _obstaclesCornerLeftDown;

        /// Número de filas
        public int Rows => _rows;
        /// Número de columnas
        public int Columns => _columns;
        public Vector3 CellsDimension => _cellsDimension;

        #region Constructors

        protected Stage(GameObject cellPrefab, Transform transformParent)
        {
            InitializeConstants(cellPrefab, transformParent,new Vector3(1f, 1f, 1f), 10, 10);
        }
        
        protected Stage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension)
        {
            InitializeConstants(cellPrefab, transformParent, cellsDimension, 10, 10);
        }
        
        protected Stage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, int rows, int columns)
        {
            InitializeConstants(cellPrefab, transformParent, cellsDimension, rows, columns);
        }

        private void InitializeConstants(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension,
            int rows, int columns)
        {
            //Esto está para que el mapa no pueda ser más pequeño de lo que está pensado originalmente
            // if (cellsDimension.x < 1 || cellsDimension.y < 1 || cellsDimension.z < 1)
            //     _cellsDimension = cellPrefab.transform.localScale;
            // else
            _cellsDimension = cellsDimension;
            
            // Los rangos (clamp) son sobretodo para controlar que haya un mínimo de filas y columnas y no de error
            // El redondeo (ceiling) es para que siempre coja el valor siguiente en caso de que el resultado sea decimal (PE si 7.4 -> Se escoge 8)
            _rows = Mathf.Clamp((int)NumberIndexesInAxis(rows), 15, 500);
            _columns = Mathf.Clamp((int)NumberIndexesInAxis(columns), 15, 500);
            
            _cellPrefab = cellPrefab;
            _transformParent = transformParent;
            
            // La variable _numberOfBlocks se asigna en el InstantiateStage() ya que al ser un método sobreescribible, evitamos errores asignándolo ahí
            // _numberOfBlocks = SetNumberOfBlocks();
            _cellMatrix = new List<List<Cell>>();
            _exits = new List<Cell>();
            _obstacles = new List<Cell>();
            _exitsCornerLeftDown = new List<AccessEntryJson>();
            _obstaclesCornerLeftDown = new List<ObstacleEntryJson>();
            
            _staticFloorField = DijkstraStaticFloorFieldWithMooreNeighbourhood.of(this);
            
            // InstantiateStage();  //Se va a instanciar desde fuera
        }
        

        #endregion
        
        #region Overwritable Methods
        
        protected abstract int SetNumberOfBlocks();

        // Calcula las casillas para las salidas
        protected abstract void CalculateExit();

        // Calcula las posiciones donde poner un obstáculo
        protected abstract void CalculateObstacle();

        #endregion
    
        #region Private Methods

        public void InstantiateStage()
        {
            _numberOfBlocks = SetNumberOfBlocks();
            // InitializeConstants(cellPivotPrefab, new Vector2(1f, 1f), 10, 10);
            // Si se hace de esta manera, se crea un objecto vacío al hacer new GameObject además del _cellsContainer
            // _cellsContainer = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, _transformParent);
            _cellsContainer = new GameObject();
            _cellsContainer.transform.SetParent(_transformParent);
            _cellsContainer.name = "Cells";
            InitializeBoard();
            CalculateExit();
            CalculateObstacle();
            GenerateCells();
        }
        
         /**
         * Checks whether a grid cell is blocked in this scenario.
         *
         * @param row    vertical coordinate of cell.
         * @param column horizontal coordinate of cell.
         * @return {@code true} if grid cell is blocked in this scenario.
         */

        private void InitializeBoard()
        {
            for (int i = 0; i < _rows; i++) // Para cada fila
            {
                _cellMatrix.Add(new List<Cell>()); // Inicializa la lista de casillas de esa fila
                for (int j = 0; j < _columns; j++) // Para cada columna
                {
                    float rowAxis = i * _cellsDimension.x;
                    float columnAxis = j * _cellsDimension.z;
                    // En cellMatrix se van a ordenar primero por filas y luego por columnas,
                    // por lo que en la escena, las filas son representadas en el eje Z y las columnas en el eje X
                    // GameObject cellObj = GameObject.Instantiate(_cellPrefab, new Vector3(columnAxis, 0f, rowAxis), Quaternion.identity, this._transformParent); // Instancia la casilla en una posición
                    GameObject cellObj = GameObject.Instantiate(_cellPrefab, new Vector3(columnAxis, 0f, rowAxis), Quaternion.identity, _cellsContainer.transform); // Instancia la casilla en una posición
                    cellObj.transform.localScale = _cellsDimension;
                    Cell cell = cellObj.GetComponent<Cell>();
                    cell.SetCellPosition(i, j);
                    _cellMatrix[i].Add(cell); // Añade la casilla a la lista de la fila (a la matriz) de casillas
                    // cellObj.name = "Cell " + "(" + i + "," + j + ")"; // Cambia el nombre del objeto
                    cellObj.name = $"Cell ({i},{j})"; // Cambia el nombre del objeto
                }
            }
        }
    
        // Genera el contenido de las casillas del tablero una vez determinado su tipo de casilla
        private void GenerateCells()
        {
            for (int i = 0; i < _rows; i++)
            {
                for(int j = 0; j < _columns; j++)
                    _cellMatrix[i][j].Build();
            }
        }

        #endregion
        
        #region Protected Getters and Setters
        
        /// Obtiene el número del índice (de filas o columnas) basado en las dimensiones de las celdas y la distancia real en ese eje
        public float NumberIndexesInAxis(float realPositionInAxis)
        {
            // return (int)Math.Ceiling(Mathf.Clamp(realPositionInAxis/_cellsDimension.x, 15, 500));
            // return (int)Math.Ceiling(realPositionInAxis/_cellsDimension.x);
            // return (int)(realPositionInAxis/_cellsDimension.x);
            return (realPositionInAxis/_cellsDimension.x);
        }

        /// Cambia el tipo de una casilla del tablero en los índices 'pos'
        protected void SetCellType(Vector2 pos, Cell.CellTypeEnum type)
        {
            // _cellMatrix[(int)pos.x][(int)pos.y].CellType = type;
            GetRowColumnCell(pos).CellType = type;
            // if (type == Cell.CellTypeEnum.Exit)
            //     _exits.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
            // else if (type == Cell.CellTypeEnum.Obstacle)
            //     _obstacles.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
            switch (type)
            {
                case Cell.CellTypeEnum.Exit : 
                    _exits.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
                    break;
                case Cell.CellTypeEnum.Obstacle :
                    _obstacles.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
                    break;
            }
        }

        // Devuelve el tipo de casilla de una casilla en los índices pasados como argumento
        protected Cell.CellTypeEnum GetCellType(Vector2 pos)
        {
            // return _cellMatrix[(int)pos.x][(int)pos.y].CellType;
            return GetRowColumnCell(pos).CellType;
        }
        
        // Coge las casillas alrededor de otra
        protected List<Vector2> GetAroundCellsWithRange(Vector2 pos, int aroundRange)
        {
            List<Vector2> cellsAround = new List<Vector2>();
            for (int i = ((int)pos.x - aroundRange); i <= (int)pos.x + aroundRange; i++)
            for (int j = ((int)pos.y - aroundRange); j <= (int)pos.y + aroundRange; j++)
                if (i >= 0 && j >= 0 && i < _rows && j < _columns && (i != pos.x || j != pos.y)) // Filas y columnas de alrededor sin tener en cuenta ella misma
                    cellsAround.Add(new Vector2(i, j));
            return cellsAround;
        }

        #endregion

        #region Public methods

        public void DestroyStage()
        {
            // GameObject.DestroyImmediate(_cellsContainer);
            GameObject.Destroy(_cellsContainer);
        }
        
        /// <summary>
        /// Consulta en la matriz de celdas la celda correspondiente a los indices de filas y columna
        /// </summary>
        /// <param name="pos">Vector2 con los índices de la fila y columna. Estos índices no tienen en cuenta las dimensiones de las celdas</param>
        /// <returns>El objeto tipo Cell que hay en la fila y columna indicada</returns>
        public Cell GetRowColumnCell(Vector2 pos)
        {
            return _cellMatrix[(int)pos.x][(int)pos.y];
        }
        
        public bool IsCellBlocked(int row, int column) {
            return GetCellType(new Vector2(row, column)) == Cell.CellTypeEnum.Obstacle;
        }
        
        public bool IsCellBlocked(Location location) {
            return IsCellBlocked(location.Row, location.Column);
        }
        
        public bool IsCellExit(int row, int column)
        {
            return GetCellType(new Vector2(row, column)) == Cell.CellTypeEnum.Exit;
        }
        
        public bool IsCellExit(Location location) {
            return IsCellExit(location.Row, location.Column);
        }
    
        #endregion
    }
}