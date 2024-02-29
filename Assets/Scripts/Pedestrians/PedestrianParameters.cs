using System;

namespace Pedestrians
{
    public record PedestrianParameters(float fieldAttractionBias, float crowdRepulsion, float velocityPercent) {
        public float fieldAttractionBias { get; set; } = fieldAttractionBias;
        public float crowdRepulsion { get; set; } = crowdRepulsion;
        public float velocityPercent { get; set; } = velocityPercent;
        /**
         * Class for building a pedestrian parameters by providing each one.
         */
        public class Builder {
            private float _fieldAttractionBias = 1.0f;
            private float _crowdRepulsion = 1.10f;
            private float _velocityPercent = 1.0f;

            public Builder() {
            }

            /**
             * @param fieldAttractionBias how is the pedestrian attracted to exits.
             */
            public Builder FieldAttractionBias(float fieldAttractionBias) {
                this._fieldAttractionBias = fieldAttractionBias;
                return this;
            }

            /**
             * @param crowdRepulsion pedestrian's repulsion to get stuck in a position too crowded.
             */
            public Builder CrowdRepulsion(float crowdRepulsion) {
                this._crowdRepulsion = crowdRepulsion;
                return this;
            }

            /**
             * @param velocityPercent pedestrian's velocity as percent of maximum velocity achieved by fastest pedestrian (1.0 =
             *                        100%). Maximum velocity is defined as
             *  {@link es.uma.lcc.caesium.pedestrian.evacuation.simulator.cellular.automaton.automata.CellularAutomatonParameters.BuilderWithScenarioWithTimeLimit#timePerTick(double)}
             *   /
             *  {@link es.uma.lcc.caesium.pedestrian.evacuation.simulator.cellular.automaton.automata.scenario.Scenario#getCellDimension()}.
             */
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