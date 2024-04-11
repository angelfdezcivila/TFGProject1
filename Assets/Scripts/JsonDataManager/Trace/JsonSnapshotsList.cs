using System;
using System.Collections.Generic;

namespace JsonDataManager.Trace
{
    [Serializable]
    //Representación de la traza entera en el json
    public class JsonSnapshotsList
    {
        public List<JsonCrowdList> snapshots;
        public float cellDimension;
    
        public JsonSnapshotsList()
        {
            snapshots = new List<JsonCrowdList>();
            this.cellDimension = 1;
            // snapshots.Add(new JsonCrowdList());
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