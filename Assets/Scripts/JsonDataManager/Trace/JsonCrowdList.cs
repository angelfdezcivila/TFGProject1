using System;
using System.Collections.Generic;

namespace JsonDataManager.Trace
{
    // Representation of a tick of time in json
    [Serializable]
    public class JsonCrowdList
    {
        public List<CrowdEntryJson> crowd;
        public float timestamp;
    
        public JsonCrowdList()
        {
            crowd = new List<CrowdEntryJson>();
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