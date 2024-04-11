using System;

namespace JsonDataManager.Trace
{
    [Serializable]
    // Representación de un agente en el json
    public class GatewayEntryJson
    {
        public int id;
        public int domain1;
        public int domain2;

        public GatewayEntryJson()
        {
            id = 0;
            domain1 = 1;
            domain2 = 0;
        }
        
        public GatewayEntryJson(int id, int domain1, int domain2)
        {
            this.id = id;
            this.domain1 = domain1;
            this.domain2 = domain2;
        }
    }
}