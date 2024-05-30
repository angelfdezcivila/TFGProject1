using System;

namespace JsonDataManager.Trace
{
    // Representation of a pedestrian's location in json
    [Serializable]
    public class LocationJson
    {
        public int domain;
        public CoordinatesTraceJson coordinates;

        public LocationJson()
        {
            domain = 0;
            coordinates = new CoordinatesTraceJson();
        }
    
    
        public LocationJson(int domain, CoordinatesTraceJson coordinates)
        {
            this.domain = domain;
            this.coordinates = coordinates;
        }
    }
}