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

        // Lo hago con un método y no una propiedad ya que el json sí detecta las propiedades a la hora de guardar
        // public ShapeType.ShapeType GetShapeType() => shapeType;
        [JsonIgnore]
        public ShapeType.ShapeType ShapeType
        {
            get => shapeType;
            set => shapeType = value;
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
            shapeType = rectangle;

            width = rectangle.Width;
            height = rectangle.Height;
            bottomLeft = new CoordinatesStageJson();

            type = rectangle.NameRepresentation;
        }

        // Al cargar el json, solo se usa el constructor sin argumentos
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
            
            this.shapeType = shapeType;
            type = shapeType.NameRepresentation;
        }

    }
}