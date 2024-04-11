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
            Debug.Log("Accesses: " + _domain.accesses.Count);
            foreach (AccessEntryJson access in _domain.accesses)
            {
                Debug.Log("ID: " + access.id);
                if (access.shape.type == ShapeJson.ShapeTypeEnum.Rectangle)
                // if (access.shape.type is RectangleJson)
                {
                    int height = (int)Mathf.Ceil(NumberIndexesInAxis(access.shape.height));
                    int width = (int)Mathf.Ceil(NumberIndexesInAxis(access.shape.width));
                    
                    Debug.Log($"Height:{height} ; Width:{width}");
                    // for (int i = 0; i < access.shape.height; i++)
                    for (int i = 0; i < height; i++)
                    {
                        // for (int j = 0; j < access.shape.width; j++)
                        for (int j = 0; j < width; j++)
                        {
                            // en el json, la x en access.shape.bottomLeft es la columna y la y es la fila, por lo que hay que invertirlo
                            // float bottomLeftRow = i + access.shape.bottomLeft.y;
                            // float bottomLeftColumn = j + access.shape.bottomLeft.x;
                            float bottomLeftRow = i + NumberIndexesInAxis(access.shape.bottomLeft.y);
                            float bottomLeftColumn = j + NumberIndexesInAxis(access.shape.bottomLeft.x);
                            Debug.Log($"Row: {bottomLeftRow} ; Column: {bottomLeftColumn}");
                            SetCellType( new Vector2(bottomLeftRow, bottomLeftColumn), Cell.CellTypeEnum.Exit);
                        }
                    }
                }
            }
            
        }

        protected override void CalculateObstacle()
        {
            Debug.Log("Obstacles: " + _domain.accesses.Count);
            foreach (ObstacleEntryJson obstacle in _domain.obstacles)
            {
                Debug.Log("Name: " + obstacle.name);
                if (obstacle.shape.type == ShapeJson.ShapeTypeEnum.Rectangle)
                    // if (access.shape.type is RectangleJson)
                {
                    int height = (int)Mathf.Ceil(NumberIndexesInAxis(obstacle.shape.height));
                    int width = (int)Mathf.Ceil(NumberIndexesInAxis(obstacle.shape.width));
                    
                    Debug.Log($"Height:{height} ; Width:{width}");
                    // for (int i = 0; i < access.shape.height; i++)
                    for (int i = 0; i < height; i++)
                    {
                        // for (int j = 0; j < access.shape.width; j++)
                        for (int j = 0; j < width; j++)
                        {
                            // en el json, la x en access.shape.bottomLeft es la columna y la y es la fila, por lo que hay que invertirlo
                            // float bottomLeftRow = i + access.shape.bottomLeft.y;
                            // float bottomLeftColumn = j + access.shape.bottomLeft.x;
                            float bottomLeftRow = i + NumberIndexesInAxis(obstacle.shape.bottomLeft.y);
                            float bottomLeftColumn = j + NumberIndexesInAxis(obstacle.shape.bottomLeft.x);
                            Debug.Log($"Row: {bottomLeftRow} ; Column: {bottomLeftColumn}");
                            SetCellType( new Vector2(bottomLeftRow, bottomLeftColumn), Cell.CellTypeEnum.Obstacle);
                        }
                    }
                }
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