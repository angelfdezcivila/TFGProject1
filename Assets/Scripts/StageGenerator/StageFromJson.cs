using System.Collections.Generic;
using JsonDataManager.Stage;
using JsonDataManager.Stage.ShapeType;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StageGenerator
{
    public sealed class StagefromJson : Stage
    {
        private JsonStage _jsonStage;
        private DomainEntryJson _domain;
        
        public StagefromJson(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, JsonStage stageJson)
            : base(cellPrefab, transformParent, cellsDimension)
        {
            _jsonStage = stageJson;
            DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == 1);
            _domain = domain;
            
            _rows = domain.height;
            _columns = domain.width;
        }
        
        public StagefromJson(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, JsonStage stageJson, int domainId)
            : base(cellPrefab, transformParent, cellsDimension)
        {
            _jsonStage = stageJson;
            DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == domainId);
            _domain = domain;
            
            _rows = domain.height;
            _columns = domain.width;
        }

        public StagefromJson(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, JsonStage stageJson, DomainEntryJson domain)
            : base(cellPrefab, transformParent, cellsDimension, domain.height, domain.width)
        {
            _jsonStage = stageJson;
            _domain = domain;
        }

        protected override int SetNumberOfBlocks()
        {
            return _domain.obstacles.Count;
        }

        protected override void CalculateExit()
        {
            foreach (ObstacleEntryJson access in _domain.accesses)
            {
                if (access.shape.type == ShapeJson.ShapeTypeEnum.Rectangle)
                // if (access.shape.type is RectangleJson)
                {
                    for (int i = 0; i < access.shape.height; i++)
                    {
                        for (int j = 0; j < access.shape.width; j++)
                        {
                            // en el json, la x en access.shape.bottomLeft es la columna y la y es la fila, por lo que hay que invertirlo
                            SetCellType( new Vector2(i + access.shape.bottomLeft.y, j + access.shape.bottomLeft.x), Cell.CellTypeEnum.Exit);
                        }
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
                    shouldBePlaced = GetCellType(cellPosition) == Cell.CellTypeEnum.Floor;

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
                    SetCellType(candidate, Cell.CellTypeEnum.Obstacle);
                }
            }
        
            return shouldBePlaced;
        }


    }
}