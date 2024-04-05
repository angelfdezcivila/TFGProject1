using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace JsonDataManager.Stage
{
    [Serializable]
    //Representación de los obstáculos de un dominio de un entorno
    public class DomainEntryJson
    {
        public int id;
        public int height;
        public int width;
        public List<ObstacleEntryJson> obstacles;
        public List<ObstacleEntryJson> accesses;
    
        public DomainEntryJson()
        {
            obstacles = new List<ObstacleEntryJson>();
            accesses = new List<ObstacleEntryJson>();
        }

        public DomainEntryJson(int id, int height, int width, List<ObstacleEntryJson> obstacles, List<ObstacleEntryJson> accesses)
        {
            this.id = id;
            this.height = height;
            this.width = width;
            this.obstacles = obstacles;
            this.accesses = accesses;
        }

        public void AddObstacleToList(ObstacleEntryJson obstacle)
        {
            obstacles.Add(obstacle);
        }
        
        public void AddAccessToList(ObstacleEntryJson access)
        {
            accesses.Add(access);
        }
    }
}