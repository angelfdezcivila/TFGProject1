using System;
using JsonDataManager.Stage.ShapeType;
using Newtonsoft.Json;
using UnityEngine;

namespace JsonDataManager.Stage
{
    // Representation of shape of an access or an obstacle in json
    [Serializable]
    public class ShapeJson
    {

        private ShapeType.ShapeType _shapeType;
        
        [JsonIgnore]
        public ShapeType.ShapeType ShapeType
        {
            get => _shapeType;
            set => _shapeType = value;
        }

        public string type;

        #region Rectangle variables

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CoordinatesStageJson bottomLeft;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public float width;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public float height;

        #endregion

        #region Circle variables

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CoordinatesStageJson center;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public float radius;

        #endregion
        
        public ShapeJson()
        {
            // RectangleJson rectangleJson = new RectangleJson();
            // type = rectangleJson;
            // width = rectangleJson.width;
            // height = rectangleJson.height;

            RectangleJson rectangle = new RectangleJson();
            _shapeType = rectangle;

            width = rectangle.Width;
            height = rectangle.Height;
            bottomLeft = new CoordinatesStageJson();

            type = rectangle.NameRepresentation;
        }

        // When loading the json, only the constructor with no arguments is used.
        public ShapeJson(ShapeType.ShapeType shapeType)
        // public ShapeJson(ShapeType.ShapeType shapeType, CoordinatesStageJson bottomLeft)
        // public ShapeJson(ShapeTypeEnum shapeType, CoordinatesStageJson bottomLeft)
        {
            if (shapeType is RectangleJson)
            {
                // RectangleJson rectangle = type as RectangleJson;
                RectangleJson rectangle = (RectangleJson) shapeType;
                bottomLeft = rectangle.BottomLeft;
                width = rectangle.Width;
                height = rectangle.Height;
            }
            else if (shapeType is CircleJson)
            {
                CircleJson circle = (CircleJson) shapeType;
                Debug.Log("Circuito en " + circle.Center + " y radio " + circle.Center);
                center = circle.Center;
                radius = circle.Radius;
            }
            
            this._shapeType = shapeType;
            type = shapeType.NameRepresentation;
        }

    }
}