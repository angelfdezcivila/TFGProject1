using System;
using JsonDataManager.Trace;

namespace JsonDataManager.Stage.ShapeType
{
    [Serializable]
    public class RectangleJson : ShapeType
    {
        public CoordinatesStageJson BottomLeft { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectangleJson() : base("RECTANGLE")
        {
            BottomLeft = new CoordinatesStageJson();
            Width = 1;
            Height = 1;
            // this.nameRepresentation = "RECTANGLE";
        }

        public RectangleJson(CoordinatesStageJson bottomLeft, float width, float height) : base("RECTANGLE")
        {
            this.BottomLeft = bottomLeft;
            this.Width = width;
            this.Height = height;
            // this.nameRepresentation = "RECTANGLE";
        }
    }
}