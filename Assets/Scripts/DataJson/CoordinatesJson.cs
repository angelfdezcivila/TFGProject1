using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataJson
{
    [Serializable]
    // Representación de las coordenadas de un agente en el json

    public class CoordinatesJson
    {
        public float X;
        public float Y;

        public CoordinatesJson()
        {
            X = 0;
            Y = 0;
        }
    
    
        public CoordinatesJson(float x, float y)
        {
            this.X = x;
            this.Y = y;
            Round();
        }

        private void Round()
        {
            X = (float)Math.Round(X, 2);
            Y = (float)Math.Round(Y, 2);
        }
    }
}