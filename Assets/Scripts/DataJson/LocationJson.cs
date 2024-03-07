using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataJson
{
    [Serializable]
    public class LocationJson
    {
        public int domain;
        public Vector2 coordinates; // Esto funciona, pero si quisiésemos indicarlo en el json con numeros enteros, tendríamos que hacer otra clas con dos integer X e Y

        public LocationJson()
        {
            domain = 0;
            coordinates = new Vector2(0, 0);
        }
    
    
        public LocationJson(int domain, Vector2 coordinates)
        {
            this.domain = domain;
            this.coordinates = coordinates;
        }
    }
}