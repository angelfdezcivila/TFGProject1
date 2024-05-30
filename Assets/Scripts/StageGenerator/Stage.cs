using System.Collections.Generic;
using Cellular;
using Cellular.FloorFields;
using JsonDataManager.Stage;
using UnityEngine;

namespace StageGenerator
{
    public abstract class Stage
    {
        #region Private Variables

        private protected GameObject _cellPrefab;
        private protected int _numberOfBlocks; // Number of obstacles
        private protected Vector3 _cellsDimension;
        private protected int _rows;
        private protected int _columns;
        private protected List<List<Cell>> _cellMatrix; // Matrix of cells
        private protected Transform _transformParent;
        private protected List<Cell> _exits;
        private protected List<AccessEntryJson> _exitsCornerLeftDown;
        private protected List<Cell> _obstacles;
        private protected List<ObstacleEntryJson> _obstaclesCornerLeftDown;
        private protected IFloorField _staticFloorField;
        private GameObject _cellsContainer;
        private int _height;
        private int _width;
        
        #endregion

        #region Public Properties
        
        public IFloorField StaticFloorField => _staticFloorField;
        public List<Cell> Exits => _exits;
        public List<Cell> Obstacles => _obstacles;
        public List<AccessEntryJson> AccessesCornerLeftDown => _exitsCornerLeftDown;
        public List<ObstacleEntryJson> ObstaclesCornerLeftDown => _obstaclesCornerLeftDown;

        /// <summary>
        /// Number of rows
        /// </summary>
        public int Rows => _rows;
        /// <summary>
        /// Number of columns
        /// </summary>
        public int Columns => _columns;
        /// <summary>
        /// Stage height
        /// </summary>
        public int Height => _height;
        /// <summary>
        /// Stage width
        /// </summary>
        public int Width => _width;
        /// <summary>
        /// Cell dimensions
        /// </summary>
        public Vector3 CellsDimension => _cellsDimension;
        
        #endregion

        #region Constructors

        protected Stage(GameObject cellPrefab, Transform transformParent)
        {
            InitializeConstants(cellPrefab, transformParent,new Vector3(1f, 1f, 1f), 10, 10);
        }
        
        protected Stage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension)
        {
            InitializeConstants(cellPrefab, transformParent, cellsDimension, 10, 10);
        }
        
        protected Stage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, int height, int width)
        {
            InitializeConstants(cellPrefab, transformParent, cellsDimension, height, width);
        }

        private void InitializeConstants(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension,
            int height, int width)
        {
            // This is here so that the map cannot be smaller than it was originally intended.
            // if (cellsDimension.x < 1 || cellsDimension.y < 1 || cellsDimension.z < 1)
            //     _cellsDimension = cellPrefab.transform.localScale;
            // else
            _cellsDimension = cellsDimension;

            _height = height;
            _width = width;
            
            // The ranges (clamp) are mainly to control that there is a minimum number of rows and columns and not an error.
            _rows = Mathf.Clamp((int)NumberIndexesInAxis(height), 15, 500);
            _columns = Mathf.Clamp((int)NumberIndexesInAxis(width), 15, 500);
            
            _cellPrefab = cellPrefab;
            _transformParent = transformParent;
            
            // The _numberOfBlocks variable is assigned in the InstantiateStage() since it is an overwritable method, we avoid errors by assigning it there.
            // _numberOfBlocks = SetNumberOfBlocks();
            _cellMatrix = new List<List<Cell>>();
            _exits = new List<Cell>();
            _obstacles = new List<Cell>();
            _exitsCornerLeftDown = new List<AccessEntryJson>();
            _obstaclesCornerLeftDown = new List<ObstacleEntryJson>();
            
            _staticFloorField = DijkstraStaticFloorFieldWithMooreNeighbourhood.Of(this);
            
            // InstantiateStage();  // It will be executed from outside
        }

        #endregion
        
        #region Overwritable Methods
        
        /// <summary>
        /// Sets the number of obstacles.
        /// </summary>
        /// <returns></returns>
        protected abstract int SetNumberOfBlocks();

        /// <summary>
        /// Calculate the cells for the accesses.
        /// </summary>
        protected abstract void CalculateExits();

        /// <summary>
        /// Calculate the cells for the obstacles.
        /// </summary>
        protected abstract void CalculateObstacle();

        #endregion
    
        #region Private Methods

        /// <summary>
        /// Instantiates the stage.
        /// </summary>
        public void InstantiateStage()
        {
            _numberOfBlocks = SetNumberOfBlocks();
            // InitializeConstants(cellPivotPrefab, new Vector2(1f, 1f), 10, 10);
            _cellsContainer = new GameObject();
            _cellsContainer.transform.SetParent(_transformParent);
            _cellsContainer.name = "Cells";
            InitializeBoard();
            CalculateExits();
            CalculateObstacle();
            GenerateCells();
        }
        

        private void InitializeBoard()
        {
            for (int i = 0; i < _rows; i++) // For each row
            {
                _cellMatrix.Add(new List<Cell>()); // Initialize the list of cells in that row
                for (int j = 0; j < _columns; j++) // for each column
                {
                    float rowAxis = i * _cellsDimension.x;
                    float columnAxis = j * _cellsDimension.z;
                    // In cellMatrix it is going to be sorted first by rows and then by columns,
                    // so in the scene, rows are represented on the Z-axis and columns on the X-axis.
                    // GameObject cellObj = GameObject.Instantiate(_cellPrefab, new Vector3(columnAxis, 0f, rowAxis), Quaternion.identity, this._transformParent); // Instantiates the cell in a position
                    GameObject cellObj = GameObject.Instantiate(_cellPrefab, new Vector3(columnAxis, 0f, rowAxis), Quaternion.identity, _cellsContainer.transform); // Instantiates the cell in a position
                    cellObj.transform.localScale = _cellsDimension;
                    Cell cell = cellObj.GetComponent<Cell>();
                    cell.SetCellPosition(i, j);
                    _cellMatrix[i].Add(cell); // Adds the cell to the list of the row (to the matrix) of cells
                    cellObj.name = $"Cell ({i},{j})"; // Changes the name of the cell Object.
                }
            }
        }
    
        /// <summary>
        /// Generates the content of the board cells once the cell type has been determined.
        /// </summary>
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
        
        /// <summary>
        /// Gets the index number (rows or columns) based on the cell dimensions and the real distance on that axis
        /// </summary>
        /// <param name="realPositionInAxis">Real position on an axis.</param>
        /// <returns></returns>
        public float NumberIndexesInAxis(float realPositionInAxis)
        {
            // return (int)Math.Ceiling(Mathf.Clamp(realPositionInAxis/_cellsDimension.x, 15, 500));
            // return (int)Math.Ceiling(realPositionInAxis/_cellsDimension.x);
            // return (int)(realPositionInAxis/_cellsDimension.x);
            return (realPositionInAxis/_cellsDimension.x);
        }

        /// <summary>
        /// Changes the type of the cell in a position.
        /// </summary>
        /// <param name="pos">Indexes of cell position.</param>
        /// <param name="type">Type to assign to the cell.</param>
        protected void SetCellType(Vector2 pos, Cell.CellTypeEnum type)
        {
            // _cellMatrix[(int)pos.x][(int)pos.y].CellType = type;
            GetRowColumnCell(pos).CellType = type;
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

        /// <summary>
        /// Returns the cell type of a cell in a position.
        /// </summary>
        /// <param name="pos">Indexes of cell position.</param>
        /// <returns></returns>
        protected Cell.CellTypeEnum GetCellType(Vector2 pos)
        {
            // return _cellMatrix[(int)pos.x][(int)pos.y].CellType;
            return GetRowColumnCell(pos).CellType;
        }

        #endregion

        #region Public methods

        public void DestroyStage()
        {
            // GameObject.DestroyImmediate(_cellsContainer);
            GameObject.Destroy(_cellsContainer);
        }
        
        /// <summary>
        /// Checks in the cell matrix the cell corresponding to the row and column indexes.
        /// </summary>
        /// <param name="pos">Cell position indexes. These indexes do not take cell dimensions into account.</param>
        /// <returns>The cell corresponding to the position indexes.</returns>
        public Cell GetRowColumnCell(Vector2 pos)
        {
            return _cellMatrix[(int)pos.x][(int)pos.y];
        }
        
        /// <summary>
        /// Checks whether a grid cell is blocked in this scenario.
        /// </summary>
        /// <param name="row">Vertical coordinate of cell.</param>
        /// <param name="column">Horizontal coordinate of cell.</param>
        /// <returns></returns>
        public bool IsCellObstacle(int row, int column) {
            return GetCellType(new Vector2(row, column)) == Cell.CellTypeEnum.Obstacle;
        }

        /// <summary>
        /// Checks whether a grid cell is blocked in this scenario.
        /// </summary>
        /// <param name="location">Coordinates of cell.</param>
        /// <returns></returns>
        public bool IsCellObstacle(Location location) {
            return IsCellObstacle(location.Row, location.Column);
        }
        
        /// <summary>
        /// Checks whether a grid cell is an access in this scenario.
        /// </summary>
        /// <param name="row">Vertical coordinate of cell.</param>
        /// <param name="column">Horizontal coordinate of cell.</param>
        /// <returns></returns>
        public bool IsCellExit(int row, int column)
        {
            return GetCellType(new Vector2(row, column)) == Cell.CellTypeEnum.Exit;
        }
        
        /// <summary>
        /// Checks whether a grid cell is an access in this scenario.
        /// </summary>
        /// <param name="location">Coordinates of cell.</param>
        /// <returns></returns>
        public bool IsCellExit(Location location) {
            return IsCellExit(location.Row, location.Column);
        }
    
        #endregion
    }
}