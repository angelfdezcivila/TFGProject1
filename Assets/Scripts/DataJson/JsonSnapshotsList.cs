using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DataJson
{
    [Serializable]
    public class JsonSnapshotsList
    {
        public List<JsonCrowdList> snapshots;
    
        public JsonSnapshotsList()
        {
            snapshots = new List<JsonCrowdList>();
            // snapshots.Add(new JsonCrowdList());
        }

        public JsonSnapshotsList(JsonSnapshotsList snapshotsList)
        {
            this.snapshots = snapshotsList.snapshots;
        }

        public void AddActionToList(JsonCrowdList crowdList)
        {
            snapshots.Add(crowdList);
        }
    }
}