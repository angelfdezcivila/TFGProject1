using System;
using JsonDataManager.Stage.ShapeType;
using UnityEngine;

namespace JsonDataManager.Stage
{
    [Serializable]
    // Representación de la localización de un agente en el json
    public class ShapeJson
    {
        [Serializable]
        public enum ShapeTypeEnum
        {
            Rectangle
        }
        
        // public ShapeType.ShapeType type;
        public ShapeTypeEnum type;
        public Vector2 bottomLeft;
        
        public float width;
        public float height;
        public float radius;

        public ShapeJson()
        {
            // RectangleJson rectangleJson = new RectangleJson();
            // type = rectangleJson;
            // width = rectangleJson.width;
            // height = rectangleJson.height;

            type = ShapeTypeEnum.Rectangle;
            width = 5;
            height = 5;
            bottomLeft = new Vector2(3, 3);
        }

        // public ShapeJson(ShapeType.ShapeType shapeType, Vector2 bottomLeft)
        public ShapeJson(ShapeTypeEnum shapeType, Vector2 bottomLeft)
        {
            this.type = shapeType;
            this.bottomLeft = bottomLeft;
            
            // if (type is RectangleJson)
            // {
            //     RectangleJson json = type as RectangleJson;
            //     width = json.width;
            //     height = json.height;
            // }
        }
    }
}