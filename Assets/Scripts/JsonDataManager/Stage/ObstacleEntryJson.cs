using System;
using JsonDataManager.Trace;

namespace JsonDataManager.Stage
{
    [Serializable]
    // Representación de un obstáculo en el json
    public class ObstacleEntryJson
    {
        public ShapeJson shape;
        public string name;
        public string description;

        public ObstacleEntryJson()
        {
            shape = new ShapeJson();
        }

        public ObstacleEntryJson(ShapeJson shape, string name, string description)
        {
            this.shape = shape;
            this.name = name;
            this.description = description;
        }
    }
}