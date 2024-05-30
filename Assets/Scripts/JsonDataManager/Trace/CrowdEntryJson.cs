using System;

namespace JsonDataManager.Trace
{
    // Representation of a pedestrian in json
    [Serializable]
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