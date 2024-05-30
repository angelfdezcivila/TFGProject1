using System;
using System.Collections.Generic;

namespace JsonDataManager.Stage
{
    // Representation of a domain (a stage) of an enviroment
    [Serializable]
    public class DomainEntryJson
    {
        public int id;
        public int height;
        public int width;
        public List<ObstacleEntryJson> obstacles;
        public List<AccessEntryJson> accesses;
    
        public DomainEntryJson()
        {
            this.id = 1;
            this.height = 10;
            this.width = 10;
            obstacles = new List<ObstacleEntryJson>();
            accesses = new List<AccessEntryJson>();
        }

        public DomainEntryJson(int id, int height, int width, List<ObstacleEntryJson> obstacles, List<AccessEntryJson> accesses)
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
        
        public void AddAccessToList(AccessEntryJson access)
        {
            accesses.Add(access);
        }
    }
}