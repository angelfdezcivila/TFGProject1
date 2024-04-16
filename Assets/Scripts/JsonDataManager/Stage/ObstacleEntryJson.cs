using System;
using JsonDataManager.Trace;
using Newtonsoft.Json;

namespace JsonDataManager.Stage
{
    [Serializable]
    // Representación de un obstáculo en el json
    public class ObstacleEntryJson
    {
        public ShapeJson shape;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string name;         // Opcional
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string description;  // Opcional

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