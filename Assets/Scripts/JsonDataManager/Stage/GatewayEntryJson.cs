using System;

namespace JsonDataManager.Trace
{
    [Serializable]
    // Representación de un agente en el json
    public class GatewayEntryJson
    {
        public int id;
        public string name;         // opcional
        public string description;  // opcional
        public int domain1;
        public int domain2;

        public GatewayEntryJson()
        {
            id = 0;
        }
        
        public GatewayEntryJson(int id, int domain1, int domain2)
        {
            this.id = id;
            this.domain1 = domain1;
            this.domain2 = domain2;
        }

        public GatewayEntryJson(int id, string name, string description, int domain1, int domain2)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.domain1 = domain1;
            this.domain2 = domain2;
        }
    }
}