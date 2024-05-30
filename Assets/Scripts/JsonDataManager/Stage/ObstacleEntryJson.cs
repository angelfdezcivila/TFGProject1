using System;
using JsonDataManager.Trace;
using Newtonsoft.Json;

namespace JsonDataManager.Stage
{
    // Representation of an obstacle in json
    [Serializable]
    public class ObstacleEntryJson
    {
        public ShapeJson shape;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string name;         // Optional
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string description;  // Optional

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