using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace JsonDataManager.Stage
{
    [Serializable]
    //Representación de los obstáculos de un dominio de un entorno
    public class JsonObstaclesList
    {
        public int id;
        public int height;
        public int width;
        public List<ObstacleEntryJson> obstacles;
    
        public JsonObstaclesList()
        {
            obstacles = new List<ObstacleEntryJson>();
        }

        public JsonObstaclesList(int id, int height, int width, List<ObstacleEntryJson> obstacles)
        {
            this.id = id;
            this.height = height;
            this.width = width;
            this.obstacles = obstacles;
        }

        public void AddObstacleToList(ObstacleEntryJson obstacle)
        {
            obstacles.Add(obstacle);
        }
    }
}