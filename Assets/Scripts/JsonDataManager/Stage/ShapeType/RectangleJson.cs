namespace JsonDataManager.Stage.ShapeType
{
    public class RectangleJson : ShapeType
    {
        public int width;
        public int height;

        public RectangleJson() : base()
        {
            width = 1;
            height = 1;
            this.nameRepresentation = "RECTANGLE";
        }

        public RectangleJson(int width, int height) : base()
        {
            this.width = width;
            this.height = height;
            this.nameRepresentation = "RECTANGLE";
        }
    }
}