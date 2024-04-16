using System;

namespace JsonDataManager.Trace
{
    // Representación de las coordenadas de un agente en el json
    [Serializable]
    public class CoordinatesTraceJson
    {
        public float X;
        public float Y;

        public CoordinatesTraceJson()
        {
            X = 1;
            Y = 1;
        }
    
    
        public CoordinatesTraceJson(float x, float y)
        {
            this.X = x;
            this.Y = y;
            // Round();
        }

        private void Round()
        {
            X = (float)Math.Round(X, 2);
            Y = (float)Math.Round(Y, 2);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}