using System;

namespace JsonDataManager.Trace
{
    [Serializable]
    // Representación de un agente en el json
    public class CrowdEntryJson
    {
        public LocationJson location;
        public int id;

        public CrowdEntryJson()
        {
            location = new LocationJson();
            id = 0;
        }
        
        public CrowdEntryJson(LocationJson location, int id)
        {
            this.location = location;
            this.id = id;
        }
    }
}