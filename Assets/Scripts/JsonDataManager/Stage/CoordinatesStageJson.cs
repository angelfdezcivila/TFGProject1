using System;

namespace JsonDataManager.Stage
{
    // Representation of coordinates of an access or an obstacle in json
    [Serializable]
    public class CoordinatesStageJson
    {
        public float x;
        public float y;

        public CoordinatesStageJson()
        {
            x = 1;
            y = 1;
        }
    
    
        public CoordinatesStageJson(float x, float y)
        {
            this.x = x;
            this.y = y;
            // Round();
        }

        private void Round()
        {
            x = (float)Math.Round(x, 2);
            y = (float)Math.Round(y, 2);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}