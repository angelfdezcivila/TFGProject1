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

        #region Overrided Methods

        protected override int SetNumberOfBlocks()
        {
            return _domain.obstacles.Count;
        }

        protected override void CalculateExit()
        {
            // Debug.Log("Accesses: " + _domain.accesses.Count);
            foreach (AccessEntryJson access in _domain.accesses)
            {
                // Debug.Log("ID: " + access.id + access.shape.bottomLeft);
                InstantiateBlock(access.shape, Cell.CellTypeEnum.Exit);
            }
            
        }

        protected override void CalculateObstacle()
        {
            // Debug.Log("Obstacles: " + _domain.accesses.Count);
            foreach (ObstacleEntryJson obstacle in _domain.obstacles)
            {
                InstantiateBlock(obstacle.shape, Cell.CellTypeEnum.Obstacle);
            }
        }
        
        #endregion
        
        private void InstantiateBlock(ShapeJson shape, Cell.CellTypeEnum cellType)
        {
            // if (shape.type == ShapeJson.ShapeTypeEnum.Rectangle)
            if (shape.ShapeType is RectangleJson)
            {
                int height = (int)Mathf.Ceil(NumberIndexesInAxis(shape.height));
                int width = (int)Mathf.Ceil(NumberIndexesInAxis(shape.width));
                // int height = (int)NumberIndexesInAxis(access.shape.height);
                // int width = (int)NumberIndexesInAxis(access.shape.width);
                    
                // Debug.Log($"Height:{height} ; Width:{width}");
                // for (int i = 0; i < access.shape.height; i++)
                for (int i = 0; i < height; i++)
                {
                    // for (int j = 0; j < access.shape.width; j++)
                    for (int j = 0; j < width; j++)
                    {
                        // en el json, la x en access.shape.bottomLeft es la columna y la y es la fila, por lo que hay que invertirlo
                        // float bottomLeftRow = i + access.shape.bottomLeft.y;
                        // float bottomLeftColumn = j + access.shape.bottomLeft.x;
                        float bottomLeftRow = i + (int)NumberIndexesInAxis(shape.bottomLeft.y);
                        float bottomLeftColumn = j + (int)NumberIndexesInAxis(shape.bottomLeft.x);
                        // Debug.Log($"Row: {bottomLeftRow} ; Column: {bottomLeftColumn}");
                        SetCellType( new Vector2(bottomLeftRow, bottomLeftColumn), cellType);
                    }
                }
            }
            else if (shape.ShapeType is CircleJson)
            {
                Debug.Log(cellType + " isCircle");
            }
        }
        
    }
}