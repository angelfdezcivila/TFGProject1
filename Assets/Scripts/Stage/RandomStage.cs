using UnityEngine;
using Random = UnityEngine.Random;

namespace StageGenerators
{
    public sealed class RandomStage : Stage
    {
        
        public RandomStage(GameObject cellPrefab, Transform transformParent) : base(cellPrefab, new Vector2(0.4f, 0.4f), transformParent, 45, 90)
        {
        }

        public RandomStage(GameObject cellPrefab, Vector2 cellsDimension, Transform transformParent, int rows, int columns) : base(cellPrefab, cellsDimension, transformParent, rows, columns)
        {
            
        }

        protected override int SetNumberOfBlocks()
        {
            return Random.Range(50, 120);
        }

        protected override void CalculateExit()
        {
            if (bernoulli(0.9f)) {
                for (int i = 2; i < 7; i++)
                {
                    SetCellType(new Vector2(i, _columns-1), NodeBasic.CellTypeEnum.Exit);
                }
                // scenario.setExit(new Rectangle(2, columns - 1, 5, 1));
            }
            if (bernoulli(0.9f)) {
                for (int i = _rows - 7; i < _rows - 2; i++)
                {
                    SetCellType(new Vector2(i, _columns-1), NodeBasic.CellTypeEnum.Exit);
                }
            }
            if (bernoulli(0.9f)) {
                for (int i = 10; i < 15; i++)
                {
                    SetCellType(new Vector2(i, 0), NodeBasic.CellTypeEnum.Exit);
                }
            }
            if (bernoulli(0.9f)) {
                for (int i = _rows - 15; i < _rows - 10; i++)
                {
                    SetCellType(new Vector2(i, 0), NodeBasic.CellTypeEnum.Exit);
                }
            }
            if (bernoulli(0.5f)) {
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
            
                int width = bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
                // int width = bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
                int height = 1 + Random.Range(0, Mathf.Max(1, _rows / (int)(2 * width)));
    
                int row = Random.Range(0, 1 + _rows - height);
                // int row = Random.Range(-_rows/2, _rows/2 - height); // Solo funciona para las filas impares
                int column = Random.Range(2, 1 + _columns - width - 2);   
            
                bool shouldBePlaced = CanBePlaced(row, column, height, width);
    
                if (shouldBePlaced) {
                    numberOfBlocksPlaced++;
                }
                maxTries -= 1;
            }
        }


    }
}