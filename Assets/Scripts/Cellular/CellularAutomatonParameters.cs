using System;
using StageGenerator;

namespace Cellular
{
  public record CellularAutomatonParameters(
    Stage Scenario
    , Neighbourhood Neighbourhood
    , float TimeLimit
    , float TimePerTick
    , float MultiplierSpeedFactor
  ) {
  
    public Stage Scenario { get; set; } = Scenario;
    public Neighbourhood Neighbourhood { get; set; } = Neighbourhood;
    public float TimeLimit { get; set; } = TimeLimit;
    public float TimePerTick { get; set; } = TimePerTick;
    public float MultiplierSpeedFactor { get; set; } = MultiplierSpeedFactor;

    /**
   * Classes for building cellular automaton parameters by providing each one.
   */
    public class Builder {
      /**
     * @param scenario Static scenario where simulation takes place.
     */
      public BuilderWithScenario Scenario(Stage scenario) {
        BuilderWithScenario builder = new BuilderWithScenario();
        builder._stage = scenario;
        return builder;
      }
    
      public class BuilderWithScenario {
        public Stage _stage;
        public BuilderWithScenario() {
        }

        /**
       * @param timeLimit Time limit of simulation in seconds.
       */
        public BuilderWithScenarioWithTimeLimit TimeLimit(float timeLimit) {
          BuilderWithScenarioWithTimeLimit builder = new BuilderWithScenarioWithTimeLimit(this);
          builder._timeLimit = timeLimit;
          return builder;
        }
      }

      public class BuilderWithScenarioWithTimeLimit {
        private Stage _stage;
        public float _timeLimit;
        private Neighbourhood _neighbourhood;
        private float _timePerTick;
        private float _multiplierSpeedFactor;

        public BuilderWithScenarioWithTimeLimit(BuilderWithScenario builder) {
          this._stage = builder._stage;
          this._neighbourhood = VonNeumannNeighbourhood.of(_stage); // default neighbourhood
          this._timePerTick = 0.4f; // default is 0.4 secs per tick
          this._multiplierSpeedFactor = 20; // default GUI time is x20 faster
        }

        /**
       * @param buildNeighbourhood a function taking current scenario and returning neighbourhood relationship used by
       *                           automaton.
       */
        public BuilderWithScenarioWithTimeLimit Neighbourhood(Func<Stage, Neighbourhood> buildNeighbourhood)
        {
          this._neighbourhood = buildNeighbourhood.Invoke(_stage);
          return this;
        }

        /**
       * @param timePerTick Seconds of time elapsed for each tick of simulation.
       *                       Notice that definition of this parameter also implies
       *                       a redefinition of {@link BuilderWithScenarioWithTimeLimit#pedestrianReferenceVelocity(double)}
       *                       in accordance with scenario's cell dimensions.
       */
        public BuilderWithScenarioWithTimeLimit TimePerTick(float timePerTick) {
          this._timePerTick = timePerTick;
          return this;
        }

        /**
       * @param pedestrianReferenceVelocity Maximum pedestrian velocity in meters per second.
       *                       Notice that definition of this parameter also implies
       *                       a redefinition of {@link BuilderWithScenarioWithTimeLimit#timePerTick(double)}
       *                       in accordance with scenario's cell dimensions.
       */
        public BuilderWithScenarioWithTimeLimit PedestrianReferenceVelocity(float pedestrianReferenceVelocity) {
          // TODO: Aquí se está suponiendo que las celdas son cuadradas, pero en caso de poder no serlo se debería de cambiar esta operación
          // Por ahora no se va a tener en cuenta las dimensiones de las celdas para la velocidad de los agentes ya que los agentes van a tener el tamaño de las celdas
          // this._timePerTick = pedestrianReferenceVelocity * _stage.CellsDimension.x;
          this._timePerTick = pedestrianReferenceVelocity;
          return this;
        }

        /**
       * @param GUITimeFactor Acceleration for rendering animation wrt real time.
       */
        public BuilderWithScenarioWithTimeLimit MultiplierSpeedFactor(float multiplierSpeedFactor) {
          this._multiplierSpeedFactor = multiplierSpeedFactor;
          return this;
        }

        public CellularAutomatonParameters Build() {
          return new CellularAutomatonParameters(_stage, _neighbourhood, _timeLimit, _timePerTick, _multiplierSpeedFactor);
        }
      }
    
    }
  
  }
}