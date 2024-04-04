using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DataJson
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

        public JsonCrowdList(JsonCrowdList crowdList, float timestamp)
        {
            this.crowd = crowdList.crowd;
        }

        public void AddCrowdToList(CrowdEntryJson action)
        {
            crowd.Add(action);
        }
    }
}