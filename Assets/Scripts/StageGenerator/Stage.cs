using System.Collections.Generic;
using UnityEngine;

namespace StageGenerator
{
    public abstract class Stage
    {
        protected GameObject _cellPrefab;
        protected int _numberOfBlocks;
        protected Vector3 _cellsDimension;
        protected int _rows;
        protected int _columns;
        protected List<List<NodeBasic>> _cellMatrix; // Matriz de casillas
        protected Transform _transformParent;
        private List<NodeBasic> _exits;
        private List<NodeBasic> _obstacles;

        public List<NodeBasic> Exits => _exits;
        public List<NodeBasic> Obstacles => _obstacles;
        public int Rows => _rows;
        public int Columns => _columns;
        public Vector2 CellsDimension
        {
            get { return new Vector2(_cellsDimension.x, _cellsDimension.z); }
        }

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
            if (cellsDimension.x < 1 || cellsDimension.y < 1 || cellsDimension.z < 1)
                _cellsDimension = cellPrefab.transform.localScale;
            else
                _cellsDimension = cellsDimension;
            
            _cellPrefab = cellPrefab;
            _transformParent = transformParent;
            
            _numberOfBlocks = SetNumberOfBlocks();
            _cellMatrix = new List<List<NodeBasic>>();
            _exits = new List<NodeBasic>();
            _obstacles = new List<NodeBasic>();
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
                _cellMatrix.Add(new List<NodeBasic>()); // Inicializa la lista de casillas de esa fila
                for (int j = 0; j < _columns; j++) // Para cada columna
                {
                    float rowAxis = i * _cellsDimension.x;
                    float columnAxis = j * _cellsDimension.y;
                    // En cellMatrix se van a ordenar primero por filas y luego por columnas,
                    // por lo que en la escena, las filas son representadas en el eje Z y las columnas en el eje X
                    GameObject cellObj = GameObject.Instantiate(_cellPrefab, new Vector3(columnAxis, 0f, rowAxis), Quaternion.identity, this._transformParent); // Instancia la casilla en una posición
                    cellObj.transform.localScale = _cellsDimension;
                    NodeBasic nodeBasic = cellObj.GetComponent<NodeBasic>();
                    nodeBasic.SetCellPosition(i, j);
                    _cellMatrix[i].Add(nodeBasic); // Añade la casilla a la lista de la fila (a la matriz) de casillas
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
        
        #region Getters and Setters

        // Cambia el tipo de una casilla del tablero en la posición 'pos'
        protected void SetCellType(Vector2 pos, NodeBasic.CellTypeEnum type)
        {
            _cellMatrix[(int)pos.x][(int)pos.y].CellType = type;
            if (type == NodeBasic.CellTypeEnum.Exit)
                _exits.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
            else if (type == NodeBasic.CellTypeEnum.Obstacle)
                _obstacles.Add(_cellMatrix[(int)pos.x][(int)pos.y]);
        }

        // Devuelve el tipo de casilla de una posición
        protected NodeBasic.CellTypeEnum GetCellType(Vector2 pos)
        {
            return _cellMatrix[(int)pos.x][(int)pos.y].CellType;
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

        #region Public Methods
        
        public bool IsCellBlocked(int row, int column) {
            return _cellMatrix[row][column].CellType == NodeBasic.CellTypeEnum.Obstacle;
        }
        
        public bool IsCellBlocked(Location location) {
            return IsCellBlocked(location.row, location.column);
        }
        
        public bool IsCellExit(int row, int column) {
            return _cellMatrix[row][column].CellType == NodeBasic.CellTypeEnum.Exit;
        }
        
        public bool IsCellExit(Location location) {
            return IsCellExit(location.row, location.column);
        }
    
        #endregion
    }
}