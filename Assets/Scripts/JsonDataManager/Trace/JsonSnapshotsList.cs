using System;
using System.Collections.Generic;

namespace JsonDataManager.Trace
{
    // Representation of the trace in json
    [Serializable]
    public class JsonSnapshotsList
    {
        public List<JsonCrowdList> snapshots;
        public float cellDimension;
    
        public JsonSnapshotsList()
        {
            snapshots = new List<JsonCrowdList>();
            this.cellDimension = 1;
        }

        public JsonSnapshotsList(List<JsonCrowdList> snapshotsList, float cellDimension)
        {
            this.snapshots = snapshotsList;
            this.cellDimension = cellDimension;
        }

        public void AddCrowdsToList(JsonCrowdList crowdList)
        {
            snapshots.Add(crowdList);
        }
    }
}