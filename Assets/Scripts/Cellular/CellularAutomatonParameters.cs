using System;
using Cellular.Neighbourhood;
using StageGenerator;

namespace Cellular
{
  public record CellularAutomatonParameters(
    Stage Scenario
    , INeighbourhood Neighbourhood
    , float TimeLimit
    , float TimePerTick
    , float MultiplierSpeedFactor
  ) {
  
    public Stage Scenario { get; private set; } = Scenario;
    public INeighbourhood Neighbourhood { get; private set; } = Neighbourhood;
    public float TimeLimit { get; private set; } = TimeLimit;
    public float TimePerTick { get; private set; } = TimePerTick;
    public float MultiplierSpeedFactor { get; set; } = MultiplierSpeedFactor;


    /// <summary>
    /// Classes for building cellular automaton parameters by providing each one.
    /// </summary>
    public class Builder {
      /// <summary>
      /// Method to set stage.
      /// </summary>
      /// <param name="scenario">Static scenario where simulation takes place.</param>
      /// <returns></returns>
      public BuilderWithScenario Scenario(Stage scenario) {
        BuilderWithScenario builder = new BuilderWithScenario();
        builder._stage = scenario;
        return builder;
      }
    
      public class BuilderWithScenario {
        internal Stage _stage;
        public BuilderWithScenario() {
        }

        /// <summary>
        /// Method to set time limit.
        /// </summary>
        /// <param name="timeLimit">Time limit of simulation in seconds.</param>
        /// <returns></returns>
        public BuilderWithScenarioWithTimeLimit TimeLimit(float timeLimit) {
          BuilderWithScenarioWithTimeLimit builder = new BuilderWithScenarioWithTimeLimit(this);
          builder._timeLimit = timeLimit;
          return builder;
        }
      }

      public class BuilderWithScenarioWithTimeLimit {
        private Stage _stage;
        public float _timeLimit;
        private INeighbourhood _neighbourhood;
        private float _timePerTick;
        private float _multiplierSpeedFactor;

        public BuilderWithScenarioWithTimeLimit(BuilderWithScenario builder) {
          this._stage = builder._stage;
          this._neighbourhood = VonNeumannNeighbourhood.Of(_stage); // default neighbourhood
          this._timePerTick = 0.4f; // default is 0.4 secs per tick
          this._multiplierSpeedFactor = 20; // default GUI time is x20 faster
        }

        /// <summary>
        /// Method to set the neighbourhood.
        /// </summary>
        /// <param name="buildNeighbourhood">A function taking current scenario and returning neighbourhood relationship used by automaton.</param>
        /// <returns></returns>
        public BuilderWithScenarioWithTimeLimit Neighbourhood(Func<Stage, INeighbourhood> buildNeighbourhood)
        {
          this._neighbourhood = buildNeighbourhood.Invoke(_stage);
          return this;
        }
        
        /// <summary>
        /// Method to set time per tick.
        /// </summary>
        /// <param name="timePerTick">Seconds of time elapsed for each tick of simulation.
        /// Notice that definition of this parameter also implies
        /// a redefinition of {@link BuilderWithScenarioWithTimeLimit#pedestrianReferenceVelocity(double)}
        /// in accordance with scenario's cell dimensions.
        /// </param>
        /// <returns></returns>
        public BuilderWithScenarioWithTimeLimit TimePerTick(float timePerTick) {
          this._timePerTick = timePerTick;
          return this;
        }

        /// <summary>
        /// Method to set the maximum pedestrian velocity.
        /// </summary>
        /// <param name="pedestrianReferenceVelocity">Maximum pedestrian velocity in meters per second.
        /// Notice that definition of this parameter also implies
        /// a redefinition of {@link BuilderWithScenarioWithTimeLimit#timePerTick(double)}
        /// in accordance with scenario's cell dimensions.</param>
        /// <returns></returns>
        public BuilderWithScenarioWithTimeLimit PedestrianReferenceVelocity(float pedestrianReferenceVelocity) {
          // TODO: Aquí se está suponiendo que las celdas son cuadradas, pero en caso de poder no serlo se debería de cambiar esta operación
          // Por ahora no se va a tener en cuenta las dimensiones de las celdas para la velocidad de los agentes ya que los agentes van a tener el tamaño de las celdas
          // this._timePerTick = pedestrianReferenceVelocity * _stage.CellsDimension.x;
          this._timePerTick = pedestrianReferenceVelocity;
          return this;
        }

        /// <summary>
        /// Method to set multiplier speed factor.
        /// </summary>
        /// <param name="multiplierSpeedFactor">Acceleration for rendering animation wrt real time.</param>
        /// <returns></returns>
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