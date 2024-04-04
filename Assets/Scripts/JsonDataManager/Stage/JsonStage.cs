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
        public List<JsonObstaclesList> domains;
    
        public JsonStage()
        {
            domains = new List<JsonObstaclesList>();
        }   

        public JsonStage(List<GatewayEntryJson> gateways, List<JsonObstaclesList> domains)
        {
            this.gateways = gateways;
            this.domains = domains;
        }
        
    }
}