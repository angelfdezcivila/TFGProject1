using System;

namespace Pedestrians
{
    public record PedestrianParameters(float FieldAttractionBias, float CrowdRepulsion, float VelocityPercent) {
        public float FieldAttractionBias { get; set; } = FieldAttractionBias;
        public float CrowdRepulsion { get; set; } = CrowdRepulsion;
        public float VelocityPercent { get; set; } = VelocityPercent;
        
        /// <summary>
        /// Class for building a pedestrian parameters by providing each one.
        /// </summary>
        public class Builder {
            private float _fieldAttractionBias = 1.0f;
            private float _crowdRepulsion = 1.10f;
            private float _velocityPercent = 1.0f;

            public Builder() {
            }

            /// <summary>
            /// Method to set field attraction bias.
            /// </summary>
            /// <param name="fieldAttractionBias">How is the pedestrian attracted to exits.</param>
            /// <returns></returns>
            public Builder FieldAttractionBias(float fieldAttractionBias) {
                this._fieldAttractionBias = fieldAttractionBias;
                return this;
            }

            /// <summary>
            /// Method to set crowd repulsion.
            /// </summary>
            /// <param name="crowdRepulsion">pedestrian's repulsion to get stuck in a position too crowded.</param>
            /// <returns></returns>
            public Builder CrowdRepulsion(float crowdRepulsion) {
                this._crowdRepulsion = crowdRepulsion;
                return this;
            }
            
            /// <summary>
            /// Method to set the percentage of maximum velocity as pedestrian's velocity.
            /// </summary>
            /// <param name="velocityPercent">pedestrian's velocity as percent of maximum velocity achieved by fastest pedestrian (1.0 = 100%).</param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public Builder VelocityPercent(float velocityPercent) {
                if (velocityPercent <= 0 || velocityPercent > 1) throw new Exception("PedestrianParameters.velocityPercent: velocity percent must be in (0, 1.0]");

                this._velocityPercent = velocityPercent;
                return this;
            }

            public PedestrianParameters Build() {
                return new PedestrianParameters(_fieldAttractionBias, _crowdRepulsion, _velocityPercent);
            }
        }
    }

}