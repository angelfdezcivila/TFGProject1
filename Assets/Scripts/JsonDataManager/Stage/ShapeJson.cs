using System;
using JsonDataManager.Stage.ShapeType;
using UnityEngine;

namespace JsonDataManager.Stage
{
    [Serializable]
    // Representación de la localización de un agente en el json
    public class ShapeJson
    {
        public ShapeType.ShapeType type;
        public Vector2 bottomLeft;

        public ShapeJson()
        {
            type = new RectangleJson();
            bottomLeft = new Vector2(3, 3);
        }

        public ShapeJson(ShapeType.ShapeType shapeType, Vector2 bottomLeft)
        {
            this.type = shapeType;
            this.bottomLeft = bottomLeft;
        }
    }
}