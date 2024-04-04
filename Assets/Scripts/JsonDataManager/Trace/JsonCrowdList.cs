using System;
using System.Collections.Generic;

namespace JsonDataManager.Trace
{
    [Serializable]
    //Representación de un tick de tiempo en el json
    public class JsonCrowdList
    {
        // public List<SnapshotsEntryJson> battleActionList;
        public List<CrowdEntryJson> crowd;
        public float timestamp;
    
        public JsonCrowdList()
        {
            crowd = new List<CrowdEntryJson>();
            // crowd.Add(new CrowdEntryJson());
        }

        public JsonCrowdList(List<CrowdEntryJson> crowdList, float timestamp)
        {
            this.crowd = crowdList;
            this.timestamp = timestamp;
        }

        public void AddCrowdToList(CrowdEntryJson crowdEntry)
        {
            this.crowd.Add(crowdEntry);
        }
    }
}