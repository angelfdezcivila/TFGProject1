using System;

namespace JsonDataManager.Trace
{
    [Serializable]
    // Representación de la localización de un agente en el json
    public class LocationJson
    {
        public int domain;
        public CoordinatesJson coordinates;

        public LocationJson()
        {
            domain = 0;
            coordinates = new CoordinatesJson();
        }
    
    
        public LocationJson(int domain, CoordinatesJson coordinates)
        {
            this.domain = domain;
            this.coordinates = coordinates;
        }
    }
}