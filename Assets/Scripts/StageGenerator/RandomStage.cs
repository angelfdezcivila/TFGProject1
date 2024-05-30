using System.Collections.Generic;
using JsonDataManager.Stage;
using JsonDataManager.Stage.ShapeType;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StageGenerator
{
    public sealed class RandomStage : Stage
    {
        
        #region Constructors

        public RandomStage(GameObject cellPrefab, Transform transformParent)
            : base(cellPrefab, transformParent, new Vector3(1f, 1f, 1f), 45, 90)
        {
        }

        public RandomStage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension)
            : base(cellPrefab, transformParent, cellsDimension, 45, 90)
        {
        }
        
        public RandomStage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, int height, int columns)
            : base(cellPrefab, transformParent, cellsDimension, height, columns)
        {
        }
        
        #endregion
        
        #region Overrided Methods

        protected override int SetNumberOfBlocks()
        {
            return Random.Range(50, 120);
        }
        
        protected override void CalculateObstacle()
        {
            int numberOfBlocksPlaced = 0;
            int maxTries = _numberOfBlocks * 3;
            while (numberOfBlocksPlaced < _numberOfBlocks && maxTries > 0) {
                // Debug.Log("Number of blocks = " + _numberOfBlocks + " : " + numberOfBlocksPlaced + " : " + maxTries);
            
                int width = Statistics.Statistics.Bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
                int height = 1 + Random.Range(0, Mathf.Max(1, _rows / (int)(2 * width)));
    
                int row = Random.Range(0, 1 + _rows - height);
                // int row = Random.Range(-_rows/2, _rows/2 - height); // Only works for odd rows
                int column = Random.Range(2, 1 + _columns - width - 2);   
            
                bool shouldBePlaced = ObstacleCanBePlaced(row, column, width, height);
    
                if (shouldBePlaced) {
                    numberOfBlocksPlaced++;
                }
                maxTries -= 1;
            }
        }

        protected override void CalculateExits()
        {
            if (Statistics.Statistics.Bernoulli(0.9f))
            {
                int row = 2;
                int column = _columns-1;
                AddAccessCornerBottomLeft(row, column, 1, 5);
                for (int i = row; i < 7; i++)
                {
                    SetExit(i, column);
                }

                // scenario.setExit(new Rectangle(2, columns - 1, 5, 1));
            }
            if (Statistics.Statistics.Bernoulli(0.9f)) {
                int row = _rows - 7;
                int column = _columns-1;
                AddAccessCornerBottomLeft(row, column, 1, 5);
                for (int i = row; i < _rows - 2; i++)
                {
                    SetExit(i, column);
                }
            }
            if (Statistics.Statistics.Bernoulli(0.9f)) {
                int row = 10;
                int column = 0;
                AddAccessCornerBottomLeft(row, column, 1, 5);
                for (int i = row; i < 15; i++)
                {
                    SetExit(i, column);
                }
            }
            if (Statistics.Statistics.Bernoulli(0.9f)) {
                int row = _rows - 15;
                int column = 0;
                AddAccessCornerBottomLeft(row, column, 1, 5);
                for (int i = row; i < _rows - 10; i++)
                {
                    SetExit(i, column);
                }
            }
            if (Statistics.Statistics.Bernoulli(0.5f)) {
                // In case the number of rows and/or columns is odd,
                // the central access will be shifted one cell to the right and/or upwards respectively
                // Another solution would be to make the acces 1x1 or 3x3.
                int startRow = _rows / 2;
                int startColumn = _columns / 2;
                AddAccessCornerBottomLeft(startRow, startColumn, 2, 2);
                for (int i = startRow; i < startRow + 2; i++)
                {
                    for (int j = startColumn; j < startColumn + 2; j++)
                    {
                        SetExit(i, j);
                    }
                }
            }
        }
        
        #endregion

        #region Private Methods

        private void SetExit(int row, int column)
        {
            SetCellType(new Vector2(row, column), Cell.CellTypeEnum.Exit);
        }

        private void AddAccessCornerBottomLeft(int row, int column, int width, int height)
        {
            float realRow = row * CellsDimension.x;
            float realColumn = column * CellsDimension.x;
            RectangleJson accessRectangle = new RectangleJson(new CoordinatesStageJson(realColumn, realRow), width*_cellsDimension.x, height*_cellsDimension.x);
            AccessEntryJson access = new AccessEntryJson
            {
                // shape = new ShapeJson(ShapeJson.ShapeTypeEnum.Rectangle, new CoordinatesStageJson(realColumn, realRow), width*_cellsDimension.x, height*_cellsDimension.x),
                shape = new ShapeJson(accessRectangle),
                id = _exitsCornerLeftDown.Count
            };
            // Debug.Log($"Access Corner: {access.shape.bottomLeft}");
            _exitsCornerLeftDown.Add(access);
        }

        private bool ObstacleCanBePlaced(int row, int column, int width, int height)
        {
            List<Vector2> obstacleCandidates = new List<Vector2>();
        
            bool shouldBePlaced = true;
            int rowBorder = row>2? row - 2 : 0; // If row = 1 or row = 0, it remains positive and does not go under the board.
            int columnBorder = column>=4? column - 2 : 2; // If column = 3 or column = 2, it still has a margin of 2 to the edges.
            // Debug.Log("(" + row + ", " + column + ") : (" + rowBorder + ", " + columnBorder + ") : (" + height + ", " + width + ")");
        
            int heightMax = row + height + 2<_rows? row + height + 2 : _rows-1; // To control that the maximum height does not exceed the maximum when checking if it can be put on.
            int widthMax = column + width + 2<_columns-2? column + width + 2 : _columns-1; // To control that the maximum width does not exceed the maximum when checking if it can be set.
        
            int rowCounter = rowBorder;
            int columnCounter = columnBorder;
        
            while (rowCounter < heightMax && shouldBePlaced)
            {
                columnCounter = columnBorder;

                while (columnCounter < widthMax && shouldBePlaced)
                {
                    Vector2 cellPosition = new Vector2(rowCounter, columnCounter);
                    // If the obstacle intersects with another object, it cannot be placed.
                    shouldBePlaced = GetCellType(cellPosition) == Cell.CellTypeEnum.Floor;

                    // Checks if the position is part of the range or part of the obstacle itself.
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
                float realRow = row * CellsDimension.x;
                float realColumn = column * CellsDimension.x;
                // comented because 33 + 10 + 2 < 45? 10 : 44 - 33
                // int realHeight = row + height + 2 < _rows? height : heightMax - row;
                int realHeight = row + height + 2 <= _rows? height : heightMax - row;
                int realWidth = width;

                RectangleJson accessRectangle = new RectangleJson(new CoordinatesStageJson(realColumn, realRow), realWidth*_cellsDimension.x, realHeight*_cellsDimension.x);

                ObstacleEntryJson obstacle = new ObstacleEntryJson
                {
                    // shape = new ShapeJson(ShapeJson.ShapeTypeEnum.Rectangle, new CoordinatesStageJson(realColumn, realRow), realWidth*_cellsDimension.x, realHeight*_cellsDimension.x)
                    // shape = new ShapeJson(accessRectangle, new CoordinatesStageJson(realColumn, realRow)),
                    shape = new ShapeJson(accessRectangle),
                };
                // Debug.Log($"Obstacle Corner: {obstacle.shape.bottomLeft}");
                _obstaclesCornerLeftDown.Add(obstacle);
                
                foreach (Vector2 candidate in obstacleCandidates)
                {
                    SetCellType(candidate, Cell.CellTypeEnum.Obstacle);
                }
            }
        
            return shouldBePlaced;
        }

        #endregion

    }
}