using System.Collections.Generic;
using Cellular;
using Events;
using FloorFields;
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
        private protected List<Cell> _obstacles;
        private protected FloorField _staticFloorField;
        private GameObject _cellsContainer;
        public FloorField StaticFloorField => _staticFloorField;
        public List<Cell> Exits => _exits;
        public List<Cell> Obstacles => _obstacles;
        public int Rows => _rows;
        public int Columns => _columns;
        public Vector3 CellsDimension => _cellsDimension;

        #region Constructors

        protected Stage(GameObject cellPrefab, Transform transformParent)
        {
            InitializeConstants(cellPrefab, transformParent,new Vector3(1f, 1f, 1f), 10, 10);
        }
        
        protected Stage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, int rows, int columns)
        {
            InitializeConstants(cellPrefab, transformParent, cellsDimension, rows, columns);
        }

        private void InitializeConstants(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension,
            int rows, int columns)
        {
            // Los rangos son sobretodo para controlar que haya un mínimo de filas y columnas y no de error
            _rows = Mathf.Clamp(rows, 15, 100);
            _columns = Mathf.Clamp(columns, 15, 100);
            
            //Esto está para que el mapa no pueda ser más pequeño de lo que está pensado originalmente
            // if (cellsDimension.x < 1 || cellsDimension.y < 1 || cellsDimension.z < 1)
            //     _cellsDimension = cellPrefab.transform.localScale;
            // else
                _cellsDimension = cellsDimension;
            
            _cellPrefab = cellPrefab;
            _transformParent = transformParent;
            
            _numberOfBlocks = SetNumberOfBlocks();
            _cellMatrix = new List<List<Cell>>();
            _exits = new List<Cell>();
            _obstacles = new List<Cell>();
            
            _staticFloorField = DijkstraStaticFloorFieldWithMooreNeighbourhood.of(this);
            
            Start();
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

        private void Start()
        {
            // InitializeConstants(cellPivotPrefab, new Vector2(1f, 1f), 10, 10);
            _cellsContainer = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, _transformParent);
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

        // Cambia el tipo de una casilla del tablero en la posición 'pos'
        protected void SetCellType(Vector2 pos, Cell.CellTypeEnum type)
        {
            // _cellMatrix[(int)pos.x][(int)pos.y].CellType = type;
            GetRowColumnPosition(pos).CellType = type;
            if (type == Cell.CellTypeEnum.Exit)
                _exits.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
            else if (type == Cell.CellTypeEnum.Obstacle)
                _obstacles.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
        }

        // Devuelve el tipo de casilla de una posición
        protected Cell.CellTypeEnum GetCellType(Vector2 pos)
        {
            // return _cellMatrix[(int)pos.x][(int)pos.y].CellType;
            return GetRowColumnPosition(pos).CellType;
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
        
        public Cell GetRowColumnPosition(Vector2 pos)
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