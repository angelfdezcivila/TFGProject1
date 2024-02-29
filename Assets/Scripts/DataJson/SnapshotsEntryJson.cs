using System;

namespace DataJson
{
    [Serializable]
    public class SnapshotsEntryJson
    {
        public int Score;

        public SnapshotsEntryJson()
        {
        }
    
    
        public SnapshotsEntryJson(int score)
        {
            Score = score;
        }
    }
}