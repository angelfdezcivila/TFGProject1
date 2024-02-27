using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pedestrian;
using StageGenerator;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellularAutomaton{

  #region Protected variables

  /**
   * Scenario where simulation takes place.
   */
  protected Stage scenario;
  /**
   * Parameters describing this automaton.
   */
  protected CellularAutomatonParameters parameters;
  /**
   * Neighbourhood relationship used by this automaton.
   */
  protected Neighbourhood neighbourhood;
  /**
   * {@code true} if cell is occupied by a pedestrian in current discrete state.
   */
  protected bool[,] occupied;
  /**
   * {@code true} if cell will be occupied by a pedestrian in next discrete state.
   */
  protected bool[,] occupiedNextState;
  /**
   * Factory for generating pedestrians for this automaton.
   */
  protected readonly PedestrianFactory pedestrianFactory;
  /**
   * List of pedestrians currently within the scenario.
   */
  protected readonly List<Pedestrian.Pedestrian> inScenarioPedestrians;
  /**
   * List of pedestrians that have evacuated the scenario.
   */
  protected readonly List<Pedestrian.Pedestrian> outOfScenarioPedestrians;
  /**
   * Number of discrete time steps elapsed since the start of the simulation.
   */
  protected int timeSteps;
  
  #endregion
  
  public readonly float TimePerTick;
  private GameObject _pedestrianPrefab;

  #region Properties

  public int Rows => scenario.Rows;

  public int Columns => scenario.Columns;

  #endregion
  
  private static Action<PedestrianParameters> GetPedestrianParameters;

  #region Constructor

  /**
   * Creates a new Cellular Automaton with provided parameters.
   *
   * @param parameters parameters describing this automaton.
   */
  public CellularAutomaton(CellularAutomatonParameters parameters, GameObject pedestrianPrefab) {
    this.parameters = parameters;
    this.scenario = parameters.Scenario;
    this.neighbourhood = parameters.Neighbourhood;
    this.occupied = new bool[scenario.Rows,scenario.Columns];
    this.occupiedNextState = new bool[scenario.Rows,scenario.Columns];
    this.pedestrianFactory = new PedestrianFactory(this, pedestrianPrefab);

    // this.inScenarioPedestrians = Collections.synchronizedList(new List<>());
    this.inScenarioPedestrians = new List<Pedestrian.Pedestrian>();
    this.outOfScenarioPedestrians = new List<Pedestrian.Pedestrian>();
    
    this.TimePerTick  = parameters.TimePerTick;

    this._pedestrianPrefab = pedestrianPrefab;
    Reset();
  }
  
  /**
 * Resets state of automaton. All pedestrians are removed, cells became non-occupied
 * and elapsed time steps are set to 0.
 */
  private void Reset() {
    ClearCells(occupied);
    inScenarioPedestrians.Clear();
    outOfScenarioPedestrians.Clear();
    timeSteps = 0;
  }
  
  #endregion

  private void ClearCells(bool[,] cells) {

    for (int i = 0; i < cells.GetLength(0); i++)
    {
      for (int j = 0; j < cells.GetLength(1); j++)
      {
        cells[i, j] = false;
      }
    }
  }

  /**
   * Adds a new pedestrian to this automaton.
   *
   * @param row        row of scenario where new pedestrian should be placed.
   * @param column     column of scenario where new pedestrian should be placed.
   * @param parameters parameters describing new pedestrian.
   * @return {@code true} if pedestrian could be created (location was neither blocked nor taken by another pedestrian).
   */
  public bool AddPedestrian(int row, int column, PedestrianParameters parameters) {
    if (row < 0 || row >= Rows) throw new ArgumentOutOfRangeException("AddPedestrian: invalid row");
    if (column < 0 || column >= Columns) throw new ArgumentOutOfRangeException("AddPedestrian: invalid column");
    if (IsCellReachable(row, column)) {
      Pedestrian.Pedestrian pedestrian = pedestrianFactory.GetInstance(row, column, parameters);
      occupied[row, column] = true;
      inScenarioPedestrians.Add(pedestrian);
      return true;
    } else {
      return false;
    }
  }

  /**
   * Adds a new pedestrian to this automaton.
   *
   * @param location   location in scenario where new pedestrian should be placed.
   * @param parameters parameters describing new pedestrian.
   * @return {@code true} if pedestrian could be created (location was neither blocked nor taken by another pedestrian).
   */
  public bool AddPedestrian(Location location, PedestrianParameters parameters) {
    return AddPedestrian(location.row, location.column, parameters);
  }

  /**
   * Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
   *
   * @param numberOfPedestrians number of new pedestrian to add.
   * @param parameters          parameters describing new pedestrians.
   */
  public void AddPedestriansUniformly(int numberOfPedestrians, PedestrianParameters parameters) {
    AddPedestriansUniformly(numberOfPedestrians, () => parameters);
  }

  /**
   * Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
   *
   * @param numberOfPedestrians number of new pedestrian to add.
   * @param parametersSupplier  a supplier providing parameters describing each new pedestrians.
   */
  public void AddPedestriansUniformly(int numberOfPedestrians, Func<PedestrianParameters> parametersSupplier) {
    if(numberOfPedestrians < 0)
      throw new ArgumentException("AddPedestriansUniformly: number of pedestrian cannot be negative");
    var numberOfPedestriansPlaced = 0;
    while (numberOfPedestriansPlaced < numberOfPedestrians) {
      int row = Random.Range(0, Rows);
      int column = Random.Range(0, Columns);

      if (AddPedestrian(row, column, parametersSupplier.Invoke())) {
        numberOfPedestriansPlaced++;
      }
    }
  }

  /**
   * Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
   *
   * @param row    row of cell.
   * @param column column of cell.
   * @return neighbours a cell.
   */
  public List<Location> Neighbours(int row, int column) {
    if (row < 0 || row >= Rows)
      throw new ArgumentException("Neighbours: invalid row");
    if (column < 0 || column >= Columns)
      throw new ArgumentException("Neighbours: invalid column");

    return neighbourhood.Neighbours(row, column);
  }

  /**
   * Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
   *
   * @param location location of cell.
   * @return neighbours a cell.
   */
  public List<Location> Neighbours(Location location) {
    return Neighbours(location.row, location.column);
  }

  /**
   * Checks whether a cell is occupied by some pedestrian.
   *
   * @param row    row of cell to check.
   * @param column column of cell to check.
   * @return {@code true} if cell is occupied by some pedestrian.
   */
  public bool IsCellOccupied(int row, int column) {
    if (row < 0 || row >= Rows)
      throw new ArgumentException("IsCellOccupied: invalid row");
    if (column < 0 || column >= Columns)
      throw new ArgumentException("IsCellOccupied: invalid column");

    return occupied[row, column];
  }

  /**
   * Checks whether a cell is occupied by some pedestrian.
   *
   * @param location location of cell to check.
   * @return {@code true} if cell is occupied by some pedestrian.
   */
  public bool IsCellOccupied(Location location) {
    return IsCellOccupied(location.row, location.column);
  }

  /**
   * Checks whether a cell can be reached by some pedestrian (i.e. there is no pedestrian occupying the cell and the
   * cell is not blocked in the scenario).
   *
   * @param row    row of cell to check.
   * @param column column of cell to check.
   * @return {@code true} if cell can be reached by some pedestrian.
   */
  public bool IsCellReachable(int row, int column) {
    if (row < 0 || row >= Rows)
      throw new ArgumentException("isCellReachable: invalid row");
    if (column < 0 || column >= Columns)
      throw new ArgumentException("isCellReachable: invalid column");
    
    return !occupied[row,column] && !scenario.IsCellBlocked(row, column);
  }

  /**
   * Checks whether a cell can be reached by some pedestrian (i.e. there is no pedestrian occupying the cell and the
   * cell is not blocked in the scenario).
   *
   * @param location location of cell to check.
   * @return {@code true} if cell can be reached by some pedestrian.
   */
  public bool IsCellReachable(Location location) {
    return IsCellReachable(location.row, location.column);
  }

  /**
   * Checks whether some pedestrian has decided already to move to a cell in next discrete time step of simulation.
   *
   * @param row    row of cell to check.
   * @param column column of cell to check.
   * @return {@code true} if some pedestrian has decided already to move to cell in next discrete time step of
   * simulation.
   */
  public bool WillBeOccupied(int row, int column) {
    if (row < 0 || row >= Rows)
      throw new ArgumentException("willBeOccupied: invalid row");
    if (column < 0 || column >= Columns)
      throw new ArgumentException("willBeOccupied: invalid column");
    
    return occupiedNextState[row,column];
  }

  /**
   * Checks whether some pedestrian has decided already to move to a cell in next discrete time step of simulation.
   *
   * @param location location of cell to check.
   * @return {@code true} if some pedestrian has decided already to move to cell in next discrete time step of
   * simulation.
   */
  public bool WillBeOccupied(Location location) {
    return WillBeOccupied(location.row, location.column);
  }

  /**
   * Scenario where automaton is running.
   *
   * @return scenario where automaton is running.
   */
  public Stage GetScenario() {
    return scenario;
  }

  public static class ListExtensions
  {
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(IList<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = rng.Next(n - 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }

    // Extension method to remove the current item from a list while iterating
    public static IEnumerator<T> RemoveCurrent<T>(IList<T> list, IEnumerator<T> enumerator)
    {
      var current = enumerator.Current;
      bool a = list.Remove(current);
      return list.GetEnumerator();
    }
  }
  

  
  // Como el proyecto original estaba realizado en Java, era necesario hilos que esperasen a la GUI para que se sincronizase.
  // Sin embargo, como nosotros estamos en Unity, no es necesario hacerlo usando hilos, sino con Corrutinas o Invoke, u otra alternativa
  
  /**
   * Runs one discrete time step for this automaton.
   */
  public void TimeStep() {
    // Debug.Log(TimePerTick);
    // clear new state
    ClearCells(occupiedNextState);

    // inScenarioPedestrians.ForEach(pedestrian => Debug.Log(pedestrian));
    ListExtensions.Shuffle(inScenarioPedestrians);
    // Debug.Log("SHUFFLE:");
    // inScenarioPedestrians.ForEach(pedestrian => Debug.Log(pedestrian));
    // Debug.Log("SHUFFLED");

    List<Pedestrian.Pedestrian>.Enumerator pedestriansIterator = inScenarioPedestrians.GetEnumerator();
    // List<Pedestrian.Pedestrian> pedestriansIterator = inScenarioPedestrians;
    
    while (pedestriansIterator.MoveNext())
    {
      Pedestrian.Pedestrian pedestrian = pedestriansIterator.Current;
      int row = pedestrian.GetRow();
      int column = pedestrian.GetColumn();
      Debug.Log(scenario.GetRowColumnPosition(new Vector2(row, column)));
      if (scenario.IsCellExit(row, column))
      {
        pedestrian.SetExitTimeSteps(timeSteps);
        outOfScenarioPedestrians.Add(pedestrian);
        // Remove current pedestrian from the list
        pedestriansIterator = (List<Pedestrian.Pedestrian>.Enumerator)ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestriansIterator);
        // inScenarioPedestrians.Remove(pedestriansIterator.Current);
        // pedestriansIterator = inScenarioPedestrians.GetEnumerator();
      }
      else
      {
        Location location = pedestrian.ChooseMovement();
        if (location != null)
        {
          if (WillBeOccupied(location)) {
            // new location already taken by another pedestrian. Don't move
            occupiedNextState[row,column] = true;
            pedestrian.doNotMove();
          } else {
            // move to new location
            occupiedNextState[location.row,location.column] = true;
            pedestrian.moveTo(location);
          }
        }
        else
        {
          occupiedNextState[row,column] = true;
          pedestrian.doNotMove();
        }
      }
    }

    var temp = occupied;
    occupied = occupiedNextState;
    occupiedNextState = temp;
    timeSteps++;
    Debug.Log("TIMESTEP: " + timeSteps);
  }
  
  /**
   * Runs this automaton until end conditions are met.
   */
  public void Run() {
    TimeStep();
    // StartCoroutine(nameof(RunCoroutine));
  }

  public bool SimulationShouldContinue()
  {
    Debug.Log("TimeSteps = " + timeSteps);
    timeSteps = 0;
    float maximalTimeSteps = parameters.TimeLimit / TimePerTick;

    return SimulationShouldContinue(maximalTimeSteps);
  }
  
  private bool SimulationShouldContinue(float maximalTimeSteps)
  {
    return inScenarioPedestrians.Count > 0 && timeSteps < maximalTimeSteps;
  }
  
  // private void RunCoroutine() {
  public IEnumerator RunCoroutine() {
    timeSteps = 0;
    float timePerTick = parameters.TimePerTick / parameters.GUITimeFactor;
    // float timePerTick = 1; // Para poder ver cada tick
    float maximalTimeSteps = parameters.TimeLimit / parameters.TimePerTick;
    float timer = timePerTick;
    Debug.Log(parameters.TimeLimit + "/" + parameters.TimePerTick + " = " + maximalTimeSteps);
    Debug.Log("Real time per tick" + timePerTick);
    
    Paint();
    while (SimulationShouldContinue(maximalTimeSteps))
    {
      // Debug.Log(timer);
      // timer -= Time.deltaTime;
      // if (timer <= 0)
      // {
      //   TimeStep();
      //   Paint();
      //   timer += timePerTick;
      // }
      
      TimeStep();
      Paint();
      yield return new WaitForSeconds(timePerTick);

    }
    Paint();
    Debug.Log("dadwadwa" + inScenarioPedestrians.Count + " fwadwadwa " + outOfScenarioPedestrians.Count);
  }

  private void Paint()
  {
    foreach (Pedestrian.Pedestrian pedestrian in inScenarioPedestrians)
    {
      pedestrian.paint();
    }
  }

  /**
   * Returns number of evacuees (number of pedestrians that have evacuated scenario).
   *
   * @return number of evacuees.
   */
  public int NumberOfEvacuees() {
    return outOfScenarioPedestrians.Count;
  }

  /**
   * Returns number of non evacuees (number of pedestrians still inside scenario).
   *
   * @return number of non evacuees.
   */
  public int NumberOfNonEvacuees() {
    return inScenarioPedestrians.Count;
  }

  /**
   * Returns evacuation times for evacuees.
   * @return evacuation times for evacuees.
   */
  public float[] EvacuationTimes() {
    int numberOfEvacuees = NumberOfEvacuees();
    float[] times = new float[numberOfEvacuees];

    int i = 0;
    foreach (Pedestrian.Pedestrian evacuee in outOfScenarioPedestrians) {
      times[i] = evacuee.getExitTimeSteps() * parameters.TimePerTick;
      i += 1;
    }

    return times;
  }

  /**
   * Returns distances to the closest exit for each non evacuee.
   *
   * @return distances to closest exit for each non evacuee.
   */
  public float[] distancesToClosestExit() {
    int numberOfNonEvacuees = NumberOfNonEvacuees();
    float[] shortestDistances = new float[numberOfNonEvacuees];

    int i = 0;
    foreach (Pedestrian.Pedestrian nonEvacuee in inScenarioPedestrians) {
      float shortestDistance = (float)Double.MaxValue;
      foreach(NodeBasic exit in GetScenario().Exits) {
        var distance = exit.DistanceTo(nonEvacuee.getLocation());
        if (distance < shortestDistance)
          shortestDistance = distance;
      }
      shortestDistances[i] = (float)(shortestDistance * GetScenario().CellsDimension.x);
      i += 1;
    }

    return shortestDistances;
  }

  /**
   * Computes some statistics regarding the execution of the simulation.
   *
   * @return statistics collected after running simulation.
   */
  public Statistics computeStatistics() {
    int numberOfEvacuees = NumberOfEvacuees();
    float[] evacuationTimes = EvacuationTimes();
    int[] steps = new int[numberOfEvacuees];

    int i = 0;
    foreach (Pedestrian.Pedestrian pedestrian in outOfScenarioPedestrians) {
      steps[i] = pedestrian.getNumberOfSteps();
      Debug.Log("Step: " + steps[i]);
      i += 1;
    }
    float meanSteps = Statistics.Mean(steps);
    float meanEvacuationTime = Statistics.Mean(evacuationTimes);
    float medianSteps = Statistics.Median(steps);
    float medianEvacuationTime = Statistics.Median(evacuationTimes);
    int numberOfNonEvacuees = NumberOfNonEvacuees();

    return new Statistics(meanSteps, meanEvacuationTime
        , medianSteps, medianEvacuationTime
        , numberOfEvacuees, numberOfNonEvacuees);
  }


  // private static JsonObject jsonPedestrian(int id, int domain, int row, int column) {
  //   JsonObject pedestrian = new JsonObject();
  //   pedestrian.put("id", id);
  //
  //   JsonObject location = new JsonObject();
  //   location.put("domain", domain);
  //
  //   JsonObject coordinates = new JsonObject();
  //   coordinates.put("X", column);
  //   coordinates.put("Y", row);
  //
  //   location.put("coordinates", coordinates);
  //   pedestrian.put("location", location);
  //
  //   return pedestrian;
  // }
  //
  // private static JsonObject jsonSnapshot(double timestamp, JsonArray crowd) {
  //   JsonObject snapshot = new JsonObject();
  //   snapshot.put("timestamp", timestamp);
  //   snapshot.put("crowd", crowd);
  //   return snapshot;
  // }
  //
  // /**
  //  * Json representing traces of all pedestrians through the scenario.
  //  *
  //  * @return Json representing traces of all pedestrians through the scenario.
  //  */
  // public JsonObject jsonTrace() {
  //   var domain = 0; // todo currently there is only a single domain
  //
  //   // Create an empty JsonArray for the snapshots
  //   JsonArray snapshots = new JsonArray();
  //
  //   List<Pedestrian> allPedestrians = new ArrayList<>();
  //   allPedestrians.addAll(inScenarioPedestrians);
  //   allPedestrians.addAll(outOfScenarioPedestrians);
  //   allPedestrians.sort(Comparator.comparing(Pedestrian::getIdentifier));
  //
  //   // Create snapshots
  //   for (int t = 0; t < timeSteps; t++) {
  //     JsonArray crowd = new JsonArray();
  //     for (var pedestrian : allPedestrians) {
  //       var path = pedestrian.getPath();
  //       if (path.size() > t) {
  //         var location = path.get(t);
  //         crowd.add(jsonPedestrian(pedestrian.getIdentifier()
  //             , domain
  //             , location.row()
  //             , location.column()));
  //       }
  //     }
  //     snapshots.add(jsonSnapshot(t, crowd));
  //   }
  //
  //   // Create the final JsonObject with the snapshots array
  //   JsonObject result = new JsonObject();
  //   result.put("snapshots", snapshots);
  //
  //   return result;
  // }
}