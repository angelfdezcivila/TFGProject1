using System;

namespace JsonDataManager.Stage.ShapeType
{
    [Serializable]
    public class CircleJson : ShapeType
    {
        public CoordinatesStageJson Center { get; private set; }
        public float Radius { get; private set; }

        public CircleJson() : base("CIRCLE")
        {
            Center = new CoordinatesStageJson();
            Radius = 1;
        }

        public CircleJson(CoordinatesStageJson center, float radius) : base("CIRCLE")
        {
            this.Center = center;
            this.Radius = radius;
        }
    }
}