using System;

namespace DataJson
{
    [Serializable]
    public class CrowdEntryJson
    {
        public int numberOfSteps;   // Para debugear
        public LocationJson location;
        public int id;

        public CrowdEntryJson()
        {
            location = new LocationJson();
            id = 0;
        }
        
        public CrowdEntryJson(int numberOfSteps, LocationJson location, int id)
        // public CrowdEntryJson(LocationJson location, int id)
        {
            this.numberOfSteps = numberOfSteps;
            this.location = location;
            this.id = id;
        }
    }
}