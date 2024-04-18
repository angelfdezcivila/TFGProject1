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

        private ShapeType.ShapeType shapeType;
        public ShapeType.ShapeType GetShapeType
        {
            get
            {
                switch (type.ToUpper())
                {
                    case "RECTANGLE" :
                        return new RectangleJson(bottomLeft, width, height);
                    case "CIRCLE" :
                        return new CircleJson(center, radius);
                    default:
                        return new RectangleJson();
                }
            }
            private set
            {
                shapeType = value;
                type = shapeType.NameRepresentation;
            }
        }
        
        

        // Lo hago con un método y no una propiedad ya que el json sí detecta las propiedades
        // public ShapeType.ShapeType GetShapeType() => shapeType;

        public string type;
        private string Type
        {
            get
            {
                if (shapeType is RectangleJson) return "RECTANGLE";
                else if (shapeType is CircleJson) return "CIRCLE";
                return "RECTANGLE";
            }
            set
            {
                type = value;
                if (type.ToUpper() == "RECTANGLE")
                {
                    shapeType = new RectangleJson(bottomLeft, width, height);
                }
                else if (type.ToUpper() == "CIRCLE")
                {
                    shapeType = new CircleJson(center, radius);
                }
            }
        }

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
            GetShapeType = rectangle;
            
            type = rectangle.NameRepresentation;
            width = rectangle.Width;
            height = rectangle.Height;
            bottomLeft = new CoordinatesStageJson();
            
            Debug.Log("sussy " + rectangle + " with name " + type);
        }

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
            
            this.GetShapeType = shapeType;
            Type = shapeType.NameRepresentation;

        }

    }
}