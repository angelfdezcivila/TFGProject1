using System;
using StageGenerator;
using UnityEngine;

public record CellularAutomatonParameters(
    Stage Scenario
    , Neighbourhood Neighbourhood
    , double TimeLimit
    , double TimePerTick
    , int GUITimeFactor
) {
  
  public Stage Scenario { get; set; } = Scenario;
  public Neighbourhood Neighbourhood { get; set; } = Neighbourhood;
  public double TimeLimit { get; set; } = TimeLimit;
  public double TimePerTick { get; set; } = TimePerTick;
  public int GUITimeFactor { get; set; } = GUITimeFactor;

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
      public BuilderWithScenarioWithTimeLimit TimeLimit(double timeLimit) {
        BuilderWithScenarioWithTimeLimit builder = new BuilderWithScenarioWithTimeLimit(this);
        builder._timeLimit = timeLimit;
        return builder;
      }
    }

    public class BuilderWithScenarioWithTimeLimit {
      private Stage _stage;
      public double _timeLimit;
      private Neighbourhood _neighbourhood;
      private double _timePerTick;
      private int _GUITimeFactor;

      public BuilderWithScenarioWithTimeLimit(BuilderWithScenario builder) {
        this._stage = builder._stage;
        this._neighbourhood = VonNeumannNeighbourhood.of(_stage); // default neighbourhood
        this._timePerTick = 0.4; // default is 0.4 secs per tick
        this._GUITimeFactor = 20; // default GUI time is x20 faster
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
      public BuilderWithScenarioWithTimeLimit TimePerTick(double timePerTick) {
        this._timePerTick = timePerTick;
        return this;
      }

      /**
       * @param pedestrianReferenceVelocity Maximum pedestrian velocity in meters per second.
       *                       Notice that definition of this parameter also implies
       *                       a redefinition of {@link BuilderWithScenarioWithTimeLimit#timePerTick(double)}
       *                       in accordance with scenario's cell dimensions.
       */
      public BuilderWithScenarioWithTimeLimit PedestrianReferenceVelocity(double pedestrianReferenceVelocity) {
        this._timePerTick = pedestrianReferenceVelocity * _stage.CellsDimension.x;
        return this;
      }

      /**
       * @param GUITimeFactor Acceleration for rendering animation wrt real time.
       */
      public BuilderWithScenarioWithTimeLimit GUITimeFactor(int GUITimeFactor) {
        this._GUITimeFactor = GUITimeFactor;
        return this;
      }

      public CellularAutomatonParameters Build() {
        return new CellularAutomatonParameters(_stage, _neighbourhood, _timeLimit, _timePerTick, _GUITimeFactor);
      }
  }
    
  }
  
}