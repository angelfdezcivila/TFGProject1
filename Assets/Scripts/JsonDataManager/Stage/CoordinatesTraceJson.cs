using System;
using UnityEngine.Serialization;

namespace JsonDataManager.Trace
{
    // Representación de las coordenadas de un agente en el json
    [Serializable]
    public class CoordinatesStageJson
    {
        public float x;
        public float y;

        public CoordinatesStageJson()
        {
            x = 1;
            y = 1;
        }
    
    
        public CoordinatesStageJson(float x, float y)
        {
            this.x = x;
            this.y = y;
            // Round();
        }

        private void Round()
        {
            x = (float)Math.Round(x, 2);
            y = (float)Math.Round(y, 2);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}