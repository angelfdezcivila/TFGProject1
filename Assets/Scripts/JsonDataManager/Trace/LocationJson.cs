using System;

namespace JsonDataManager.Trace
{
    [Serializable]
    // Representación de la localización de un agente en el json
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