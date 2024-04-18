using System.Collections.Generic;
using JsonDataManager.Stage;
using JsonDataManager.Stage.ShapeType;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StageGenerator
{
    public sealed class StageFromJson : Stage
    {
        private JsonStage _jsonStage;
        private DomainEntryJson _domain;
        
        public StageFromJson(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, JsonStage stageJson)
            : base(cellPrefab, transformParent, cellsDimension)
        {
            _jsonStage = stageJson;
            DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == 1);
            _domain = domain;
            
            _rows = domain.height;
            _columns = domain.width;
        }
        
        public StageFromJson(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, JsonStage stageJson, int domainId)
            : base(cellPrefab, transformParent, cellsDimension)
        {
            _jsonStage = stageJson;
            DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == domainId);
            _domain = domain;
            
            _rows = domain.height;
            _columns = domain.width;
        }

        public StageFromJson(GameObject cellPrefab, Transform transformParent, Vector3 cellsDimension, JsonStage stageJson, DomainEntryJson domain)
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
                Debug.Log("ID: " + access.id + access.shape.bottomLeft);
                // Hacer que la variable type se represente en el json como string, la solución facil es cambiar la variable a tipo string
                // if (access.shape.type == ShapeJson.ShapeTypeEnum.Rectangle)
                if (access.shape.ShapeType is RectangleJson)
                {
                    int height = (int)Mathf.Ceil(NumberIndexesInAxis(access.shape.height));
                    int width = (int)Mathf.Ceil(NumberIndexesInAxis(access.shape.width));
                    // int height = (int)NumberIndexesInAxis(access.shape.height);
                    // int width = (int)NumberIndexesInAxis(access.shape.width);
                    
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
                            float bottomLeftRow = i + (int)NumberIndexesInAxis(access.shape.bottomLeft.y);
                            float bottomLeftColumn = j + (int)NumberIndexesInAxis(access.shape.bottomLeft.x);
                            Debug.Log($"Row: {bottomLeftRow} ; Column: {bottomLeftColumn}");
                            SetCellType( new Vector2(bottomLeftRow, bottomLeftColumn), Cell.CellTypeEnum.Exit);
                        }
                    }
                }
                else if (access.shape.ShapeType is CircleJson)
                {
                    Debug.Log("Obstacle isCircle");
                }
            }
            
        }

        protected override void CalculateObstacle()
        {
            Debug.Log("Obstacles: " + _domain.accesses.Count);
            foreach (ObstacleEntryJson obstacle in _domain.obstacles)
            {
                // if (obstacle.shape.type == ShapeJson.ShapeTypeEnum.Rectangle)
                if (obstacle.shape.ShapeType is RectangleJson)
                {
                    Debug.Log("Name: " + obstacle.shape.bottomLeft + obstacle.shape.height + " , " + obstacle.shape.width + " : TIPO " + obstacle.shape.type + obstacle.shape.radius);
                    Debug.Log("Name: " + obstacle.shape.bottomLeft + obstacle.shape.height + " , " + obstacle.shape.width + " : TIPO " + obstacle.shape.ShapeType.NameRepresentation);

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
                            float bottomLeftRow = i + (int)NumberIndexesInAxis(obstacle.shape.bottomLeft.y);
                            float bottomLeftColumn = j + (int)NumberIndexesInAxis(obstacle.shape.bottomLeft.x);
                            Debug.Log($"Row: {bottomLeftRow} ; Column: {bottomLeftColumn}");
                            SetCellType( new Vector2(bottomLeftRow, bottomLeftColumn), Cell.CellTypeEnum.Obstacle);
                        }
                    }
                }
                else if (obstacle.shape.ShapeType is CircleJson)
                {
                    Debug.Log("Obstacle isCircle");
                }
            }
        }
        
    }
}