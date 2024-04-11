using System;
using System.Collections.Generic;
using JsonDataManager.Trace;

namespace JsonDataManager.Stage
{
    [Serializable]
    //Representación del entorno (con sus dominios) entero
    public class JsonStage
    {
        public List<GatewayEntryJson> gateways;
        public List<DomainEntryJson> domains;
    
        public JsonStage()
        {
            gateways = new List<GatewayEntryJson>();
            domains = new List<DomainEntryJson>();
        }   

        public JsonStage(List<GatewayEntryJson> gateways, List<DomainEntryJson> domains)
        {
            this.gateways = gateways;
            this.domains = domains;
        }
        
    }
}