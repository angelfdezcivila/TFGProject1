using System;
using JsonDataManager.Trace;

namespace JsonDataManager.Stage
{
    // Representation of an access in json
    [Serializable]
    public class AccessEntryJson
    {
        public int id;
        public ShapeJson shape;
        

        public AccessEntryJson()
        {
            id = 0;
            shape = new ShapeJson();
        }

        public AccessEntryJson(int id, ShapeJson shape)
        {
            this.id = id;
            this.shape = shape;
        }
    }
}