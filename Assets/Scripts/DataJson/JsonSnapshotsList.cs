using System;
using System.Collections.Generic;

namespace DataJson
{
    [Serializable]
    public class JsonSnapshotsList
    {
        public List<SnapshotsEntryJson> battleActionList;
    
        public JsonSnapshotsList()
        {
            battleActionList = new List<SnapshotsEntryJson>();
            battleActionList.Add(new SnapshotsEntryJson(3));
        }

        public JsonSnapshotsList(JsonSnapshotsList battleActionList)
        {
            this.battleActionList = battleActionList.battleActionList;
        }

        public void AddActionToList(SnapshotsEntryJson action)
        {
            battleActionList.Add(action);
        }
    }
}