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
            // gateways = new List<GatewayEntryJson>();
            gateways = DefaultGateways();
            domains = new List<DomainEntryJson>();
        }

        public JsonStage(List<DomainEntryJson> domains)
        {
            // this.gateways = new List<GatewayEntryJson>();
            this.gateways = DefaultGateways();
            this.domains = domains;
        }

        public JsonStage(List<GatewayEntryJson> gateways, List<DomainEntryJson> domains)
        {
            this.gateways = gateways;
            this.domains = domains;
        }
        
        private List<GatewayEntryJson> DefaultGateways()
        {
            List<GatewayEntryJson> gatewaysList = new List<GatewayEntryJson>();
            GatewayEntryJson gateway = new GatewayEntryJson();
            
            gatewaysList.Add(gateway);

            return gatewaysList;
        }

        public void AddDomain(DomainEntryJson domainJson)
        {
            domains.Add(domainJson);
        }
    }
}