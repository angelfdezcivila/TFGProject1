using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataJson
{
    [Serializable]
    public class LocationJson
    {
        public int domain;
        public Vector2 coordinates; // Esto funciona, pero si quisiésemos indicarlo en el json con numeros enteros, tendríamos que hacer otra clase con dos integer X e Y.
        //Otro punto por el que no se utiliza Vector2 es que a la hora de guardar el Json, se añaden decimales de más

        public LocationJson()
        {
            domain = 0;
            coordinates = new Vector2(0, 0);
        }
    
    
        public LocationJson(int domain, Vector2 coordinates)
        {
            this.domain = domain;
            this.coordinates = coordinates;
            // this.coordinates = new Vector2((float)Math.Round(coordinates.x, 2), (float)Math.Round(coordinates.y, 2));
        }
    }
}