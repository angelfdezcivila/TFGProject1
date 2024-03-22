using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DataJson
{
    [Serializable]
    public class JsonSnapshotsList
    {
        public List<JsonCrowdList> snapshots;
        public float cellDimension;
    
        public JsonSnapshotsList()
        {
            snapshots = new List<JsonCrowdList>();
            // snapshots.Add(new JsonCrowdList());
        }

        public JsonSnapshotsList(JsonSnapshotsList snapshotsList, float cellDimension)
        {
            this.snapshots = snapshotsList.snapshots;
            this.cellDimension = cellDimension;
        }

        public void AddCrowdsToList(JsonCrowdList crowdList)
        {
            snapshots.Add(crowdList);
        }
    }
}