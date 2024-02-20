using System.Collections.Generic;
using UnityEngine;

namespace StageGenerator
{
    public abstract class Stage
    {
        protected enum CellStatus {Blocked, Clear, Exit}
        
        protected GameObject _cellPrefab;
        protected int _numberOfBlocks;

        protected Vector3 _cellsDimension;
        protected int _rows;
        protected int _columns;
        protected List<List<NodeBasic>> _cellMatrix; // Matriz de casillas
        protected Transform _transformParent;

        public int Rows => _rows;
        public int Columns => _columns;
        public Vector2 CellsDimension
        {
            get { return new Vector2(_cellsDimension.x, _cellsDimension.z); }
        }

        protected CellStatus[,] _cellsStatus;
        
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
            
            _cellsStatus = new CellStatus[_rows,_columns];
            _cellsDimension = cellsDimension;
            
            _cellPrefab = cellPrefab;
            _transformParent = transformParent;
            Start();
        }

        #endregion
    
        #region Private Methods

        private void Start()
        {
            // InitializeConstants(cellPivotPrefab, new Vector2(1f, 1f), 10, 10);

            _numberOfBlocks = SetNumberOfBlocks();
            _cellMatrix = new List<List<NodeBasic>>();
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
                    _cellMatrix[i].Add(cellObj.GetComponent<NodeBasic>()); // Añade la casilla a la lista de la fila (a la matriz) de casillas
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
        
        #region Overwritable Methods
        
        protected abstract int SetNumberOfBlocks();

        // Calcula las casillas para las salidas
        protected abstract void CalculateExit();

        // Calcula las posiciones donde poner un obstáculo
        protected abstract void CalculateObstacle();

        #endregion

        #region Getters and Setters

        // Cambia el tipo de una casilla del tablero en la posición 'pos'
        protected void SetCellType(Vector2 pos, NodeBasic.CellTypeEnum type)
        {
            _cellMatrix[(int)pos.x][(int)pos.y].CellType = type;
        }

        // Devuelve el tipo de casilla de una posición
        protected NodeBasic.CellTypeEnum GetCellType(Vector2 pos)
        {
            return _cellMatrix[(int)pos.x][(int)pos.y].CellType;
        }

        #endregion

        #region Auxiliar Methods

        protected bool ObstacleCanBePlaced(int row, int column, int height, int width)
        {
            List<Vector2> obstacleCandidates = new List<Vector2>();
        
            bool shouldBePlaced = true;
            int rowBorder = row>2? row - 2 : 0; //Está para que si row = 1 o row = 0, siga siendo positiva y no se salga por debajo del tablero
            int columnBorder = column>=4? column - 2 : 2; //Está para que si column = 3 o column = 2, siga teniendo un margen de 2 respecto a los bordes
            // Debug.Log("(" + row + ", " + column + ") : (" + rowBorder + ", " + columnBorder + ") : (" + height + ", " + width + ")");
        
            int heightMax = row + height + 2<_rows? row + height + 2 : _rows-1; //Está para controlar que la altura máxima no sobrepase el máximo a la hora de comprobar si puede ser puesto
            int widthMax = column + width + 2<_columns-2? column + width + 2 : _columns-1; //Está para controlar que la anchura máxima no sobrepase el máximo a la hora de comprobar si puede ser puesto
        
            int rowCounter = rowBorder;
            int columnCounter = columnBorder;
        
            while (rowCounter < heightMax && shouldBePlaced)
            {
                columnCounter = columnBorder;

                while (columnCounter < widthMax && shouldBePlaced)
                {
                    Vector2 cellPosition = new Vector2(rowCounter, columnCounter);
                    //Si intersecta con otro objeto, no se puede poner el obstáculo
                    shouldBePlaced = GetCellType(cellPosition) == NodeBasic.CellTypeEnum.Floor;

                    // Comprueba si la posicion es parte del rango o es del propio obstaculo
                    bool isWall = (rowCounter >= row && columnCounter >= column) &&
                                  (rowCounter < row + height && columnCounter < column + width);
                    // Debug.Log("counters: " + rowCounter + ", " + columnCounter + " - " + isWall);

                    if (shouldBePlaced && isWall)
                    {
                        obstacleCandidates.Add(cellPosition);
                    }

                    columnCounter++;
                }

                rowCounter++;
            }

            if (shouldBePlaced)
            {
                foreach (Vector2 candidate in obstacleCandidates)
                {
                    SetCellType(candidate, NodeBasic.CellTypeEnum.Obstacle);
                }
            }
        
            return shouldBePlaced;
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
    
        protected static bool bernoulli(float successProbability) {
            if (!(successProbability < 0.0) && !(successProbability > 1.0)) {
                return Random.value < successProbability;
            } else {
                Debug.LogError("bernoulli: probability " + successProbability + "must be in [0.0, 1.0]");
                return false;
            }
        }

        public bool IsCellBlocked(int row, int column) {
            return _cellsStatus[row,column] == CellStatus.Blocked;
        }
        
        public bool IsCellBlocked(Location location) {
            return IsCellBlocked(location.row, location.column);
        }
        
        public bool IsCellExit(int row, int column) {
            return _cellsStatus[row,column] == CellStatus.Exit;
        }
    
        #endregion
    }
}