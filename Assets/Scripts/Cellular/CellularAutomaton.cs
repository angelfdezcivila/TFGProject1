using System;
using System.Collections;
using System.Collections.Generic;
using DataJson;
using Pedestrians;
using StageGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cellular
{
  public class CellularAutomaton{

    #region Protected variables

    /**
   * Scenario where simulation takes place.
   */
    protected Stage stage;
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
    protected readonly List<Pedestrian> inScenarioPedestrians;
    /**
   * List of pedestrians that have evacuated the scenario.
   */
    protected readonly List<Pedestrian> outOfScenarioPedestrians;
    /**
   * Number of discrete time steps elapsed since the start of the simulation.
   */
    protected int timeSteps;
  
    #endregion
    
    
    private GameObject _pedestrianContainer;

    #region Properties
    
    public GameObject PedestrianContainer => _pedestrianContainer;

    public int Rows => stage.Rows;

    public int Columns => stage.Columns;
    
    /// <summary>
    /// Tiempo de cada tick de la animación entre cada movimiento de los agentes como grupo.
    /// </summary>
    public float RealTimePerTick => parameters.TimePerTick / parameters.MultiplierSpeedFactor;
    // public float RealTimePerTick => parameters.TimePerTick;
    // public float RealTimePerTick => 1;  // Para poder ver cada tick
    // parameters.TimePerTick = 1.3 * 0.4 = 0.52;
    // public float RealTimePerTick => 0.52f/8; // tick visible en la simulacion de pepe
    
    #endregion
  
    // private static Action<PedestrianParameters> GetPedestrianParameters;
    public CellularAutomatonParameters CellularAutomatonParameters => parameters;

    #region Constructor

    /**
   * Creates a new Cellular Automaton with provided parameters.
   *
   * @param parameters parameters describing this automaton.
   */
    public CellularAutomaton(CellularAutomatonParameters parameters, GameObject pedestrianPrefab) {
      this.parameters = parameters;
      this.stage = parameters.Scenario;
      this.neighbourhood = parameters.Neighbourhood;
      this.occupied = new bool[stage.Rows,stage.Columns];
      this.occupiedNextState = new bool[stage.Rows,stage.Columns];
      this.pedestrianFactory = new PedestrianFactory(this, pedestrianPrefab);

      // this.inScenarioPedestrians = Collections.synchronizedList(new List<>());
      this.inScenarioPedestrians = new List<Pedestrian>();
      this.outOfScenarioPedestrians = new List<Pedestrian>();
    
      Reset();
    }
  
    /**
 * Resets state of automaton. All pedestrians are removed, cells became non-occupied
 * and elapsed time steps are set to 0.
 */
    private void Reset() {
      // _pedestrianContainer = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
      _pedestrianContainer = new GameObject();
      _pedestrianContainer.name = "Pedestrians";
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
        Pedestrian pedestrian = pedestrianFactory.GetInstance(row, column, parameters);
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
      return AddPedestrian(location.Row, location.Column, parameters);
    }

    /**
   * Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
   *
   * @param numberOfPedestrians number of new pedestrian to add.
   * @param parameters          parameters describing new pedestrians.
   */
    public void AddPedestriansUniformly(int numberOfPedestrians, PedestrianParameters pedestrianParameters) {
      AddPedestriansUniformly(numberOfPedestrians, () => pedestrianParameters);
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
      return Neighbours(location.Row, location.Column);
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
      return IsCellOccupied(location.Row, location.Column);
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
    
      return !occupied[row,column] && !stage.IsCellBlocked(row, column);
    }

    /**
   * Checks whether a cell can be reached by some pedestrian (i.e. there is no pedestrian occupying the cell and the
   * cell is not blocked in the scenario).
   *
   * @param location location of cell to check.
   * @return {@code true} if cell can be reached by some pedestrian.
   */
    public bool IsCellReachable(Location location) {
      return IsCellReachable(location.Row, location.Column);
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
      return WillBeOccupied(location.Row, location.Column);
    }

    /**
   * Scenario where automaton is running.
   *
   * @return scenario where automaton is running.
   */
    public Stage GetScenario() {
      return stage;
    }

    public static class ListExtensions
    {
      private static System.Random rng = new System.Random();

      public static List<T> Shuffle<T>(IList<T> list)
      {
        List<T> listAux = new List<T>(list);
        int n = listAux.Count;
        while (n > 1)
        {
          n--;
          int k = rng.Next(n - 1);
          T value = listAux[k];
          listAux[k] = listAux[n];
          listAux[n] = value;
        }

        return listAux;
      }

      // Extension method to remove the current item from a list while iterating
      public static IEnumerator<T> RemoveCurrent<T>(IList<T> list, T current)
      {
        list.Remove(current);
        return list.GetEnumerator();
      }
    }
  

  
    // Como el proyecto original estaba realizado en Java, era necesario hilos que esperasen a la GUI para que se sincronizase.
    // Sin embargo, como nosotros estamos en Unity, no es necesario hacerlo usando hilos, sino con Corrutinas o Invoke, u otra alternativa
  
    /**
   * Runs one discrete time step for this automaton.
   */
    public void TimeStep() {
      // clear new state
      ClearCells(occupiedNextState);

      //Para comprobar que no se repitan los agentes
      // var trueForAll = inScenarioPedestrians.TrueForAll(pedestrian =>
      // {
      //   return inScenarioPedestrians.FindAll(pedestrian1 => pedestrian1.Identifier == pedestrian.Identifier).Count == 1;
      // });
      // Debug.Log(trueForAll);

      List<Pedestrian> inStageAux = ListExtensions.Shuffle(inScenarioPedestrians);

      List<Pedestrian>.Enumerator pedestriansIterator = inStageAux.GetEnumerator();
      // List<Pedestrian.Pedestrian> pedestriansIterator = inScenarioPedestrians;
    
      while (pedestriansIterator.MoveNext())
      {
        Pedestrian pedestrian = pedestriansIterator.Current;
        int row = pedestrian.GetRow();
        int column = pedestrian.GetColumn();
        if (stage.IsCellExit(row, column))
        {
          pedestrian.SetExitTimeSteps(timeSteps);
          outOfScenarioPedestrians.Add(pedestrian);
          // Remove current pedestrian from the list
          // pedestriansIterator = (List<Pedestrian>.Enumerator)ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          // ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          inScenarioPedestrians.Remove(pedestrian);
          // pedestriansIterator = inScenarioPedestrians.GetEnumerator();
        
          GameObject.Destroy(pedestrian.gameObject);
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
              occupiedNextState[location.Row,location.Column] = true;
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
      Debug.Log("TIMESTEP: " + timeSteps);
      // inScenarioPedestrians.ForEach(pedestrian => Debug.Log(pedestrian + ", timestamp: " + timeSteps));
      timeSteps++;
    }
  
    /**
   * Runs this automaton until end conditions are met.
   */
    public void RunStep() {
      TimeStep();
      Paint();
    }

    public bool SimulationShouldContinue()
    {
      float maximalTimeSteps = parameters.TimeLimit / parameters.TimePerTick;
      return SimulationShouldContinue(maximalTimeSteps);
    }
  
    private bool SimulationShouldContinue(float maximalTimeSteps)
    {
      return inScenarioPedestrians.Count > 0 && timeSteps < maximalTimeSteps;
      // return inScenarioPedestrians.Count > 0 && timeSteps < 5;
    }
  
    public IEnumerator RunCoroutine() {
      InitializeStaticFloor();
      
      Debug.Log("Real time per tick" + RealTimePerTick);
    
      Paint();
      while (SimulationShouldContinue())
      {
        RunStep();
        yield return new WaitForSeconds(RealTimePerTick);
        // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

      }
      Paint();
    }

    public void InitializeStaticFloor()
    {
      stage.StaticFloorField.initialize();
      timeSteps = 0;
    }

    public void Paint()
    {
      foreach (Pedestrian pedestrian in inScenarioPedestrians)
      {
        pedestrian.paint();
      }
    }

    /**
   * Returns number of evacuees (number of pedestrians that have evacuated scenario).
   *
   * @return number of evacuees.
   */
    private int NumberOfEvacuees => outOfScenarioPedestrians.Count;

    /**
   * Returns number of non evacuees (number of pedestrians still inside scenario).
   *
   * @return number of non evacuees.
   */
    private int NumberOfNonEvacuees => inScenarioPedestrians.Count;

    /**
   * Returns evacuation times for evacuees.
   * @return evacuation times for evacuees.
   */
    private float[] EvacuationTimes() {
      int numberOfEvacuees = NumberOfEvacuees;
      float[] times = new float[numberOfEvacuees];

      for (int i = 0; i < outOfScenarioPedestrians.Count; i++)
      {
        Pedestrians.Pedestrian evacuee = outOfScenarioPedestrians[i];
        times[i] = evacuee.getExitTimeSteps() * parameters.TimePerTick;
      }
      // int i = 0;
      // foreach (Pedestrian.Pedestrian evacuee in outOfScenarioPedestrians) {
      //   times[i] = evacuee.getExitTimeSteps() * parameters.TimePerTick;
      //   i += 1;
      // }

      return times;
    }

    /**
   * Returns distances to the closest exit for each non evacuee.
   *
   * @return distances to closest exit for each non evacuee.
   */
    public float[] distancesToClosestExit() {
      int numberOfNonEvacuees = NumberOfNonEvacuees;
      float[] shortestDistances = new float[numberOfNonEvacuees];

      int i = 0;
      foreach (Pedestrians.Pedestrian nonEvacuee in inScenarioPedestrians) {
        float shortestDistance = (float)Double.MaxValue;
        foreach(Cell exit in GetScenario().Exits) {
          var distance = exit.DistanceTo(nonEvacuee.getLocation());
          if (distance < shortestDistance)
            shortestDistance = distance;
        }
        // TODO: comprobar si afecta que las celdas sean cuadradas o no
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
      int numberOfEvacuees = NumberOfEvacuees;
      float[] evacuationTimes = EvacuationTimes();
      int[] steps = new int[numberOfEvacuees];

      int i = 0;
      foreach (Pedestrians.Pedestrian pedestrian in outOfScenarioPedestrians) {
        steps[i] = pedestrian.getNumberOfSteps();
        i += 1;
      }
      float meanSteps = Statistics.Mean(steps);
      float meanEvacuationTime = Statistics.Mean(evacuationTimes);
      float medianSteps = Statistics.Median(steps);
      float medianEvacuationTime = Statistics.Median(evacuationTimes);
      int numberOfNonEvacuees = NumberOfNonEvacuees;

      return new Statistics(meanSteps, meanEvacuationTime
        , medianSteps, medianEvacuationTime
        , numberOfEvacuees, numberOfNonEvacuees);
    }


    private static CrowdEntryJson JsonPedestrian(int numberOfSteps, int id, int domain, int row, int column)
    {
      LocationJson locationJson = new LocationJson(domain, new Vector2(row, column));
      // CrowdEntryJson crowdJson = new CrowdEntryJson(locationJson, id);
      CrowdEntryJson crowdJson = new CrowdEntryJson(numberOfSteps, locationJson, id);
    
      return crowdJson;
    }
    
    /**
    /**
     * Json representing traces of all pedestrians through the scenario.
     *
     * @return Json representing traces of all pedestrians through the scenario.
     */
    public JsonSnapshotsList JsonTrace() {
      int domain = 0; // todo currently there is only a single domain
    
      // Create an empty JsonArray for the snapshots
      JsonSnapshotsList snapshots = new JsonSnapshotsList();
    
      List<Pedestrian> allPedestrians = new List<Pedestrian>();
      inScenarioPedestrians.ForEach(pedestrian => allPedestrians.Add(pedestrian));
      outOfScenarioPedestrians.ForEach(pedestrian => allPedestrians.Add(pedestrian));
      allPedestrians.Sort((p1, p2) => p1.Identifier.CompareTo(p2.Identifier));
      allPedestrians.ForEach(pedestrian => Debug.Log("FINISHED " + pedestrian + ", timestamp: " + pedestrian.GetPath().Count +  ", Steps: " + pedestrian.getNumberOfSteps()));
      // allPedestrians.sort(Comparator.comparing(Pedestrian::getIdentifier));

      // Create snapshots
      for (int t = 0; t < timeSteps; t++) {
        JsonCrowdList crowd = new JsonCrowdList();
        foreach (Pedestrian pedestrian in allPedestrians) {
          List<Location> path = pedestrian.GetPath();
          if (path.Count > t)
          {
            Location location = path[t];
            // crowd.AddCrowdToList(JsonPedestrian(pedestrian.Identifier, domain, location.Row, location.Column));
            crowd.AddCrowdToList(JsonPedestrian(pedestrian.getNumberOfSteps(),pedestrian.Identifier, domain, location.Row, location.Column));
          }
        }
        crowd.timestamp = t;
        snapshots.AddCrowdsToList(crowd);
      }

      return snapshots;
    }
    public void DestroyAutomatons()
    {
        GameObject.Destroy(_pedestrianContainer);
        // foreach (Pedestrian pedestrian in inScenarioPedestrians)
        // {
        //     GameObject.Destroy(pedestrian);
        //     // inScenarioPedestrians.Remove(pedestrian);
        // }
    }
  }
}