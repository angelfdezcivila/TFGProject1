using System;
using JsonDataManager.Stage.ShapeType;
using JsonDataManager.Trace;
using Newtonsoft.Json;
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
        public CoordinatesStageJson bottomLeft;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public float width;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public float height;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
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
            bottomLeft = new CoordinatesStageJson();
        }

        // public ShapeJson(ShapeType.ShapeType shapeType, Vector2 bottomLeft)
        public ShapeJson(ShapeTypeEnum shapeType, CoordinatesStageJson bottomLeft)
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

        public ShapeJson(ShapeTypeEnum type, CoordinatesStageJson bottomLeft, float width, float height)
        {
            this.type = type;
            this.bottomLeft = bottomLeft;
            this.width = width;
            this.height = height;
        }
    }
}