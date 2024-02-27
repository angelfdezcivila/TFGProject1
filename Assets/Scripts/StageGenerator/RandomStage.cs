using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StageGenerator
{
    public sealed class RandomStage : Stage
    {
        
        public RandomStage(GameObject cellPrefab, Transform transformParent)
            : base(cellPrefab, transformParent, new Vector3(0.4f, 0.4f, 0.4f), 45, 90)
        {
        }

        public RandomStage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension)
            : base(cellPrefab, transformParent, cellsDimension, 45, 90)
        {
        }
        
        public RandomStage(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, int rows, int columns)
            : base(cellPrefab, transformParent, cellsDimension, rows, columns)
        {
            
        }

        protected override int SetNumberOfBlocks()
        {
            return Random.Range(50, 120);
        }

        protected override void CalculateExit()
        {
            if (Statistics.bernoulli(0.9f)) {
                for (int i = 2; i < 7; i++)
                {
                    SetCellType(new Vector2(i, _columns-1), NodeBasic.CellTypeEnum.Exit);
                }
                // scenario.setExit(new Rectangle(2, columns - 1, 5, 1));
            }
            if (Statistics.bernoulli(0.9f)) {
                for (int i = _rows - 7; i < _rows - 2; i++)
                {
                    SetCellType(new Vector2(i, _columns-1), NodeBasic.CellTypeEnum.Exit);
                }
            }
            if (Statistics.bernoulli(0.9f)) {
                for (int i = 10; i < 15; i++)
                {
                    SetCellType(new Vector2(i, 0), NodeBasic.CellTypeEnum.Exit);
                }
            }
            if (Statistics.bernoulli(0.9f)) {
                for (int i = _rows - 15; i < _rows - 10; i++)
                {
                    SetCellType(new Vector2(i, 0), NodeBasic.CellTypeEnum.Exit);
                }
            }
            if (Statistics.bernoulli(0.5f)) {
                // En el caso de que el numero de filas y/o columnas sea impar,
                // la salida central estará desplazada una casilla hacia la derecha y/o hacia arriba respectivamente
                // Otra solución sería hacer la salida de 1x1 o 3x3
                int startRow = _rows / 2;
                int startColumn = _columns / 2;
                for (int i = startRow; i < startRow + 2; i++)
                {
                    for (int j = startColumn; j < startColumn + 2; j++)
                    {
                        SetCellType(new Vector2(i, j), NodeBasic.CellTypeEnum.Exit);
                    }
                }
            }
        }

        protected override void CalculateObstacle()
        {
            int numberOfBlocksPlaced = 0;
            int maxTries = _numberOfBlocks * 3;
            while (numberOfBlocksPlaced < _numberOfBlocks && maxTries > 0) {
                // Debug.Log("Number of blocks = " + _numberOfBlocks + " : " + numberOfBlocksPlaced + " : " + maxTries);
            
                int width = Statistics.bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
                // int width = bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
                int height = 1 + Random.Range(0, Mathf.Max(1, _rows / (int)(2 * width)));
    
                int row = Random.Range(0, 1 + _rows - height);
                // int row = Random.Range(-_rows/2, _rows/2 - height); // Solo funciona para las filas impares
                int column = Random.Range(2, 1 + _columns - width - 2);   
            
                bool shouldBePlaced = ObstacleCanBePlaced(row, column, height, width);
    
                if (shouldBePlaced) {
                    numberOfBlocksPlaced++;
                }
                maxTries -= 1;
            }
        }

        private bool ObstacleCanBePlaced(int row, int column, int height, int width)
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


    }
}