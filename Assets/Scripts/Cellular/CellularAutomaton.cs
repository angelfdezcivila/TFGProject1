using System;
using System.Collections;
using System.Collections.Generic;
using JsonDataManager.Stage;
using JsonDataManager.Trace;
using Pedestrians;
using StageGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cellular
{
  public class CellularAutomaton{

    #region Private variables

    /// <summary>
    /// Scenario where simulation takes place.
    /// </summary>
    private readonly Stage _stage;
    /// <summary>
    /// Parameters describing this automaton.
    /// </summary>
    private readonly CellularAutomatonParameters _parameters;
    /// <summary>
    /// Neighbourhood relationship used by this automaton.
    /// </summary>
    private readonly Neighbourhood _neighbourhood;
    /// <summary>
    /// {@code true} if cell is occupied by a pedestrian in current discrete state.
    /// </summary>
    private bool[,] _occupied;
    /// <summary>
    /// {@code true} if cell will be occupied by a pedestrian in next discrete state.
    /// </summary>
    private bool[,] _occupiedNextState;
    /// <summary>
    /// Factory for generating pedestrians for this automaton.
    /// </summary>
    private readonly PedestrianFactory _pedestrianFactory;
    /// <summary>
    /// List of pedestrians currently within the scenario.
    /// </summary>
    private readonly List<Pedestrian> _inScenarioPedestrians;
    /// <summary>
    /// List of pedestrians that have evacuated the scenario.
    /// </summary>
    private readonly List<Pedestrian> _outOfScenarioPedestrians;
    /// <summary>
    /// Number of discrete time steps elapsed since the start of the simulation.
    /// </summary>
    private int _timeSteps;
    /// <summary>
    /// GameObject that contains all the pedestrians in stage.
    /// </summary>
    private GameObject _pedestrianContainer;
    /// <summary>
    /// Integer that contains the domain number of the automaton
    /// </summary>
    private int domain = 1; // TODO: currently there is only a single domain

    #endregion
    
    #region Properties
    
    /// <summary>
    /// Time of each tick of the simulation between each movement of the agents as a group.
    /// </summary>
    public float RealTimePerTick => _parameters.TimePerTick / _parameters.MultiplierSpeedFactor;
    // public float RealTimePerTick => parameters.TimePerTick;
    // public float RealTimePerTick => 1;  // Para poder ver cada tick
    // parameters.TimePerTick = 1.3 * 0.4 = 0.52;
    // public float RealTimePerTick => 0.52f/8; // tick visible en la simulacion de pepe
    private float CellsDimension => _stage.CellsDimension.x;
    
    public Stage Stage => _stage;
    
    public GameObject PedestrianContainer => _pedestrianContainer;

    public int Rows => _stage.Rows;

    public int Columns => _stage.Columns;
    
    /// <summary>
    /// Returns number of evacuees (number of pedestrians that have evacuated scenario).
    /// </summary>
    private int NumberOfEvacuees => _outOfScenarioPedestrians.Count;

    /// <summary>
    /// Returns number of non evacuees (number of pedestrians still inside scenario).
    /// </summary>
    private int NumberOfNonEvacuees => _inScenarioPedestrians.Count;
    
    #endregion
    
    #region Constructor
    /// <summary>
    /// Creates a new Cellular Automaton with provided parameters.
    /// </summary>
    /// <param name="parameters">Parameters describing this automaton.</param>
    /// <param name="pedestrianPrefab">Prefab of pedestrians</param>
    public CellularAutomaton(CellularAutomatonParameters parameters, GameObject pedestrianPrefab) {
      this._parameters = parameters;
      this._stage = parameters.Scenario;
      this._neighbourhood = parameters.Neighbourhood;
      this._occupied = new bool[_stage.Rows,_stage.Columns];
      this._occupiedNextState = new bool[_stage.Rows,_stage.Columns];
      this._pedestrianFactory = new PedestrianFactory(this, pedestrianPrefab);

      this._inScenarioPedestrians = new List<Pedestrian>();
      this._outOfScenarioPedestrians = new List<Pedestrian>();
    
      Reset();
    }
  
    /// <summary>
    /// Resets state of automaton. All pedestrians are removed, cells became non-occupied and elapsed time steps are set to 0.
    /// </summary>
    private void Reset() {
      // _pedestrianContainer = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
      _pedestrianContainer = new GameObject();
      _pedestrianContainer.name = "Pedestrians";
      ClearCells(_occupied);
      _inScenarioPedestrians.Clear();
      _outOfScenarioPedestrians.Clear();
      _timeSteps = 0;
    }
  
    #endregion
    
    #region Cell state
    
    /// <summary>
    /// Checks whether a cell is occupied by some pedestrian.
    /// </summary>
    /// <param name="row">Row index of cell to check.</param>
    /// <param name="column">Column index of cell to check.</param>
    /// <returns>If cell is occupied (if there is an obstacle or another pedestrian).</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsCellOccupied(int row, int column) {
      if (row < 0 || row >= Rows)
        throw new ArgumentException("IsCellOccupied: invalid row");
      if (column < 0 || column >= Columns)
        throw new ArgumentException("IsCellOccupied: invalid column");

      return _occupied[row, column];
    }
    
    /// <summary>
    /// Checks whether a cell is occupied by some pedestrian.
    /// </summary>
    /// <param name="location">Location with indexes of cell to check.</param>
    /// <returns>If cell is occupied (if there is an obstacle or another pedestrian).</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsCellOccupied(Location location) {
      return IsCellOccupied(location.Row, location.Column);
    }
    
    /// <summary>
    /// Checks whether a cell can be reached by some pedestrian
    /// (i.e. there is no pedestrian occupying the cell and the cell is not blocked in the scenario).
    /// </summary>
    /// <param name="row">Row index of cell to check.</param>
    /// <param name="column">Column index of cell to check.</param>
    /// <returns>If cell can be reached by some pedestrian.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsCellReachable(int row, int column) {
      if (row < 0 || row >= Rows)
        throw new ArgumentException("isCellReachable: invalid row");
      if (column < 0 || column >= Columns)
        throw new ArgumentException("isCellReachable: invalid column");
    
      return !_occupied[row,column] && !_stage.IsCellObstacle(row, column);
    }

    /// <summary>
    /// Checks whether a cell can be reached by some pedestrian (i.e. there is no pedestrian occupying the cell and the
    /// cell is not blocked in the scenario).
    /// </summary>
    /// <param name="location">Location with indexes of cell to check.</param>
    /// <returns>If cell can be reached by some pedestrian.</returns>
    public bool IsCellReachable(Location location) {
      return IsCellReachable(location.Row, location.Column);
    }

    /// <summary>
    /// Checks whether some pedestrian has decided already to move to a cell in next discrete time step of simulation.
    /// </summary>
    /// <param name="row">Row index of cell to check.</param>
    /// <param name="column">Column index of cell to check.</param>
    /// <returns>If some pedestrian has decided already to move to cell in next discrete time step of simulation.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool WillBeOccupied(int row, int column) {
      if (row < 0 || row >= Rows)
        throw new ArgumentException("willBeOccupied: invalid row");
      if (column < 0 || column >= Columns)
        throw new ArgumentException("willBeOccupied: invalid column");
    
      return _occupiedNextState[row,column];
    }
    
    /// <summary>
    /// Checks whether some pedestrian has decided already to move to a cell in next discrete time step of simulation.
    /// </summary>
    /// <param name="location">Location with indexes of cell to check.</param>
    /// <returns>If some pedestrian has decided already to move to cell in next discrete time step of simulation.</returns>
    public bool WillBeOccupied(Location location) {
      return WillBeOccupied(location.Row, location.Column);
    }
    
    #endregion
    
    #region Run Simulation

    #region Adding pedestrians

        /// <summary>
    /// Adds a new pedestrian to this automaton.
    /// </summary>
    /// <param name="row">Row index of scenario where new pedestrian should be placed.</param>
    /// <param name="column">Column index of scenario where new pedestrian should be placed.</param>
    /// <param name="parameters">Parameters describing new pedestrian.</param>
    /// <returns>If pedestrian could be created (location was neither blocked nor taken by another pedestrian).</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool AddPedestrian(int row, int column, PedestrianParameters parameters) {
      if (row < 0 || row >= Rows) throw new ArgumentOutOfRangeException("AddPedestrian: invalid row");
      if (column < 0 || column >= Columns) throw new ArgumentOutOfRangeException("AddPedestrian: invalid column");
      if (IsCellReachable(row, column)) {
        Pedestrian pedestrian = _pedestrianFactory.GetInstance(row, column, parameters);
        _occupied[row, column] = true;
        _inScenarioPedestrians.Add(pedestrian);
        return true;
      } else {
        return false;
      }
    }
    
    /// <summary>
    /// Adds a new pedestrian to this automaton.
    /// </summary>
    /// <param name="location">Location indexes in scenario where new pedestrian should be placed.</param>
    /// <param name="parameters">Parameters describing new pedestrian.</param>
    /// <returns>If pedestrian could be created (location was neither blocked nor taken by another pedestrian).</returns>
    public bool AddPedestrian(Location location, PedestrianParameters parameters) {
      return AddPedestrian(location.Row, location.Column, parameters);
    }
    
    /// <summary>
    /// Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
    /// </summary>
    /// <param name="numberOfPedestrians">Number of new pedestrian to add.</param>
    /// <param name="pedestrianParameters">Parameters describing new pedestrians.</param>
    public void AddPedestriansUniformly(int numberOfPedestrians, PedestrianParameters pedestrianParameters) {
      AddPedestriansUniformly(numberOfPedestrians, () => pedestrianParameters);
    }
    
    /// <summary>
    /// Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
    /// </summary>
    /// <param name="numberOfPedestrians">Number of new pedestrian to add.</param>
    /// <param name="pedestrianParameters">A supplier providing parameters describing each new pedestrians.</param>
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

    #endregion
    
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

      /// <summary>
      /// Extension method to remove the current item from a list while iterating
      /// </summary>
      /// <param name="list"></param>
      /// <param name="current"></param>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      public static IEnumerator<T> RemoveCurrent<T>(IList<T> list, T current)
      {
        list.Remove(current);
        return list.GetEnumerator();
      }
    }
    
    /// <summary>
    /// Runs one discrete time step for this automaton.
    /// </summary>
    private void TimeStepAutomaton() {
      _timeSteps++;
      
      // clear new state
      ClearCells(_occupiedNextState);
      
      // to check pedestrians are not repeating
      // var trueForAll = inScenarioPedestrians.TrueForAll(pedestrian =>
      // {
      //   return inScenarioPedestrians.FindAll(pedestrian1 => pedestrian1.Identifier == pedestrian.Identifier).Count == 1;
      // });
      // Debug.Log(trueForAll);

      List<Pedestrian> inStageAux = ListExtensions.Shuffle(_inScenarioPedestrians);

      List<Pedestrian>.Enumerator pedestriansIterator = inStageAux.GetEnumerator();
      // List<Pedestrian.Pedestrian> pedestriansIterator = inScenarioPedestrians;
    
      while (pedestriansIterator.MoveNext())
      {
        Pedestrian pedestrian = pedestriansIterator.Current;
        int row = pedestrian.GetRow();
        int column = pedestrian.GetColumn();
        if (_stage.IsCellExit(row, column))
        {
          pedestrian.SetExitTimeSteps(_timeSteps);
          _outOfScenarioPedestrians.Add(pedestrian);
          // Remove current pedestrian from the list
          // pedestriansIterator = (List<Pedestrian>.Enumerator)ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          // ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          _inScenarioPedestrians.Remove(pedestrian);
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
              _occupiedNextState[row,column] = true;
              pedestrian.doNotMove();
            } else {
              // move to new location
              _occupiedNextState[location.Row,location.Column] = true;
              pedestrian.moveTo(location);

              // Vector3 position = stage.GetRowColumnCell(new Vector2(location.Row, location.Column)).transform.position;
              // pedestrian.moveTo(new Location(position.x, position.z));
            }
          }
          else
          {
            _occupiedNextState[row,column] = true;
            pedestrian.doNotMove();
          }
        }
      }

      var temp = _occupied;
      _occupied = _occupiedNextState;
      _occupiedNextState = temp;
      // Debug.Log("TIMESTEP: " + _timeSteps);
      // inScenarioPedestrians.ForEach(pedestrian => Debug.Log(pedestrian + ", timestamp: " + timeSteps));
      // timeSteps++;
    }
    
    /// <summary>
    /// Runs a step of this automaton.
    /// </summary>
    public void RunAutomatonStep() {
      TimeStepAutomaton();
      Paint();
    }
    
    public void InitializeStaticFloor()
    {
      _stage.StaticFloorField.initialize();
      _timeSteps = 0;
    }
  
    public IEnumerator RunAutomatonSimulationCoroutine() {
      InitializeStaticFloor();
      
      // Debug.Log("Real time per tick" + RealTimePerTick);
    
      Paint();        
      yield return new WaitForSeconds(1.5f); //Para ver las posiciones iniciales de cada agente

      while (SimulationShouldContinue())
      {
        RunAutomatonStep();
        yield return new WaitForSeconds(RealTimePerTick);
        // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

      }

      _timeSteps++;
      Paint();
    }
    
    #endregion

    #region Load Simulation

    #region Adding pedestrians

    public void AddPedestriansFromJson(JsonSnapshotsList traceJson)
    {
      // Debug.Log($"Rows: {Rows} ; Columns: {Columns}");
      JsonCrowdList initialCrowd = traceJson.snapshots[0];
      int numberOfPedestrians = initialCrowd.crowd.Count;
      Pedestrian.ResetIdentifiers();
      for (int i = 0; i < numberOfPedestrians; i++)
      {
        CrowdEntryJson pedestrian = initialCrowd.crowd[i];
        CoordinatesTraceJson coordinates = pedestrian.location.coordinates;
        // Debug.Log($"Rows: {coordinates.Y} ; Columns: {coordinates.X}");
        
        int row = (int)_stage.NumberIndexesInAxis(coordinates.Y);
        int column = (int)_stage.NumberIndexesInAxis(coordinates.X);
        
        // AddPedestrianFromJson((int)(coordinates.Y / CellsDimension), (int)(coordinates.X / CellsDimension));
        AddPedestrianFromJson(row, column);
      }
    }
    
    /// <summary>
    /// Adds a new pedestrian from a json.
    /// </summary>
    /// <param name="row">Row index of scenario where new pedestrian should be placed.</param>
    /// <param name="column">Column index of scenario where new pedestrian should be placed.</param>
    /// <returns>If pedestrian could be created (location was neither blocked nor taken by another pedestrian).</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    // public bool AddPedestrianFromJson(int row, int column, JsonSnapshotsList traceJson) {
    public bool AddPedestrianFromJson(int row, int column) {
      if (row < 0 || row >= Rows) throw new ArgumentOutOfRangeException("AddPedestrian: invalid row" + Rows);
      if (column < 0 || column >= Columns) throw new ArgumentOutOfRangeException("AddPedestrian: invalid column");
      if (IsCellReachable(row, column)) {
        Pedestrian pedestrianLoaded = _pedestrianFactory.GetInstance(row, column, null);
        
        if (_stage.IsCellExit(row, column))
        {
          pedestrianLoaded.SetExitTimeSteps(_timeSteps);
          _outOfScenarioPedestrians.Add(pedestrianLoaded);
          // Remove current pedestrian from the list
          // pedestriansIterator = (List<Pedestrian>.Enumerator)ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          // ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          _inScenarioPedestrians.Remove(pedestrianLoaded);
          // pedestriansIterator = inScenarioPedestrians.GetEnumerator();
        
          GameObject.Destroy(pedestrianLoaded.gameObject);
        }
        else
        {
          _occupied[row, column] = true;
          _inScenarioPedestrians.Add(pedestrianLoaded);
        }
        return true;
      } else {
        return false;
      }
    }
    
    #endregion
    
    /// <summary>
    /// Load the current timeStep for a pedestrian loaded from a json
    /// </summary>
    /// <param name="traceJson"></param>
    /// <param name="pedestrian"></param>
    private void PedestrianTimeStepLoaded(JsonSnapshotsList traceJson, Pedestrian pedestrian)
    {
      JsonCrowdList snapshot = traceJson.snapshots[_timeSteps]; // snaphot.timestamp == _timeSteps
          
      CrowdEntryJson crowdEntryJson = snapshot.crowd.Find(pedestrianToFind => pedestrianToFind.id == pedestrian.Identifier);
      if (crowdEntryJson != null)
      {
        CoordinatesTraceJson locationCoordinates = crowdEntryJson.location.coordinates;
        int row = (int)_stage.NumberIndexesInAxis(locationCoordinates.Y);
        int column = (int)_stage.NumberIndexesInAxis(locationCoordinates.X);
        
        if (_stage.IsCellExit(row, column))
        {
          pedestrian.SetExitTimeSteps(_timeSteps);
          _outOfScenarioPedestrians.Add(pedestrian);
          // Remove current pedestrian from the list
          // pedestriansIterator = (List<Pedestrian>.Enumerator)ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          // ListExtensions.RemoveCurrent(inScenarioPedestrians, pedestrian);
          _inScenarioPedestrians.Remove(pedestrian);
          // pedestriansIterator = inScenarioPedestrians.GetEnumerator();
        
          GameObject.Destroy(pedestrian.gameObject);
        }
        else
        {
          pedestrian.moveTo(row, column);
        }
      }
      
    }
    
    private void TimeStepLoadedSimulation(JsonSnapshotsList traceJson)
    {
      _timeSteps++;
      //The list in the corresponding timeStep is obtained from the json.
      //As at the beginning we have added all the agents from the json to _inScenarioPedestrians, we can iterate over this list.
      List<Pedestrian> pedestriansToMove = new List<Pedestrian>(_inScenarioPedestrians);
        
      foreach (Pedestrian pedestrian in pedestriansToMove)
      {
        // Debug.Log("Pedestrian id: " + pedestrian.Identifier);
        PedestrianTimeStepLoaded(traceJson, pedestrian);
      }
        
      // Debug.Log("TIMESTEP: " + _timeSteps + "Count: " + _inScenarioPedestrians.Count);
    }
    
    /// <summary>
    /// Runs a step of the simulation loaded.
    /// </summary>
    /// <param name="traceJson"></param>
    public void LoadSimulationStep(JsonSnapshotsList traceJson) {
      TimeStepLoadedSimulation(traceJson);
      Paint();
    }
    public IEnumerator LoadingSimulationCoroutine(JsonSnapshotsList traceJson) {
      // Debug.Log("Real time per tick" + RealTimePerTick);
      
      // Create and add to the scene the agents in their initial position
      AddPedestriansFromJson(traceJson);
      
      Paint();
      yield return new WaitForSeconds(1.5f); //To be able to view the initial positions of each agent

      while (SimulationShouldContinue())
      {
        LoadSimulationStep(traceJson);
        yield return new WaitForSeconds(RealTimePerTick);
      }
      
      _timeSteps++;
      Paint();
    }
    
    #endregion

    #region Statistics
    
    /// <summary>
    /// Calculates evacuation times for evacuees.
    /// </summary>
    /// <returns>Evacuation times for evacuees.</returns>
    private float[] EvacuationTimes() {
      int numberOfEvacuees = NumberOfEvacuees;
      float[] times = new float[numberOfEvacuees];

      for (int i = 0; i < _outOfScenarioPedestrians.Count; i++)
      {
        Pedestrians.Pedestrian evacuee = _outOfScenarioPedestrians[i];
        times[i] = evacuee.getExitTimeSteps() * _parameters.TimePerTick;
      }
      // int i = 0;
      // foreach (Pedestrian.Pedestrian evacuee in outOfScenarioPedestrians) {
      //   times[i] = evacuee.getExitTimeSteps() * parameters.TimePerTick;
      //   i += 1;
      // }

      return times;
    }
    
    /// <summary>
    /// Calculates distances to the closest exit for each non evacuee.
    /// </summary>
    /// <returns>Distances to closest exit for each non evacuee.</returns>
    public float[] DistancesToClosestExit() {
      int numberOfNonEvacuees = NumberOfNonEvacuees;
      float[] shortestDistances = new float[numberOfNonEvacuees];

      int i = 0;
      foreach (Pedestrians.Pedestrian nonEvacuee in _inScenarioPedestrians) {
        float shortestDistance = (float)Double.MaxValue;
        foreach(Cell exit in _stage.Exits) {
          var distance = exit.DistanceTo(nonEvacuee.getLocation());
          if (distance < shortestDistance)
            shortestDistance = distance;
        }
        // TODO: comprobar si afecta que las celdas sean cuadradas o no
        shortestDistances[i] = (float)(shortestDistance * CellsDimension);
        i += 1;
      }

      return shortestDistances;
    }
    
    /// <summary>
    /// Computes some statistics regarding the execution of the simulation.
    /// </summary>
    /// <returns>Statistics collected after running simulation.</returns>
    public Statistics ComputeStatistics() {
      int numberOfEvacuees = NumberOfEvacuees;
      float[] evacuationTimes = EvacuationTimes();
      int[] steps = new int[numberOfEvacuees];

      int i = 0;
      foreach (Pedestrian pedestrian in _outOfScenarioPedestrians) {
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
    
    #endregion

    #region Save simulation

    private CrowdEntryJson JsonPedestrian(int id, int domain, float row, float column)
    {
      LocationJson locationJson = new LocationJson(domain, GetCoordinatesToPosition(row, column));
      CrowdEntryJson crowdJson = new CrowdEntryJson(locationJson, id);

      return crowdJson;
    }
    
    private CoordinatesTraceJson GetCoordinatesToPosition(float row, float column)
    {
      // CoordinatesJson coordinates = new CoordinatesJson(row, column);
      CoordinatesTraceJson coordinates = new CoordinatesTraceJson(column, row);
      
      return coordinates;
    }

    /// <summary>
    /// Creates a JsonSnapshotsList object with the simulation data.
    /// </summary>
    /// <returns>Json representing traces of all pedestrians through the scenario.</returns>
    public JsonSnapshotsList JsonTrace() {
    // public List<JsonCrowdList> JsonTrace() {
    
      // Create an empty JsonArray for the snapshots
      JsonSnapshotsList snapshots = new JsonSnapshotsList();
      // List<JsonCrowdList> snapshots = new List<JsonCrowdList>();

    
      List<Pedestrian> allPedestrians = new List<Pedestrian>();
      _inScenarioPedestrians.ForEach(pedestrian => allPedestrians.Add(pedestrian));
      _outOfScenarioPedestrians.ForEach(pedestrian => allPedestrians.Add(pedestrian));
      allPedestrians.Sort((p1, p2) => p1.Identifier.CompareTo(p2.Identifier));

      // Create snapshots
      for (int t = 0; t < _timeSteps; t++) {
        JsonCrowdList crowd = new JsonCrowdList();
        foreach (Pedestrian pedestrian in allPedestrians) {
          List<Location> path = pedestrian.GetPath();
          if (path.Count > t)
          {
            Location location = path[t];
            crowd.AddCrowdToList(JsonPedestrian(pedestrian.Identifier, domain, 
              // location.Row*CellsDimension, location.Column*CellsDimension));
              location.Row*CellsDimension + CellsDimension/2, location.Column*CellsDimension + CellsDimension/2));
          }
        }

        if (crowd.crowd.Count > 0) // In case there is no pedestrian left, which means the simulation has finished, we should not add it to the json
        {
          crowd.timestamp = t;
          snapshots.AddCrowdsToList(crowd);
          // snapshots.Add(crowd);
        }

      }

      snapshots.cellDimension = CellsDimension;
      return snapshots;
    }

    public JsonStage JsonStage()
    {
      JsonStage jsonStage = new JsonStage();

      // DomainEntryJson domainJson = new DomainEntryJson(domain, Rows, Columns, _stage.ObstaclesCornerLeftDown, _stage.AccessesCornerLeftDown);
      DomainEntryJson domainJson = new DomainEntryJson(domain, _stage.Height, _stage.Width, _stage.ObstaclesCornerLeftDown, _stage.AccessesCornerLeftDown);

      jsonStage.AddDomain(domainJson);
      
      return jsonStage;
    }
    
    #endregion
    
    #region Private Methods
    private void ClearCells(bool[,] cells) {
      for (int i = 0; i < cells.GetLength(0); i++)
      {
        for (int j = 0; j < cells.GetLength(1); j++)
        {
          cells[i, j] = false;
        }
      }
    }
    
    private bool SimulationShouldContinue()
    {
      float maximalTimeSteps = _parameters.TimeLimit / _parameters.TimePerTick;
      return SimulationShouldContinue(maximalTimeSteps);
    }
  
    private bool SimulationShouldContinue(float maximalTimeSteps)
    {
      return _inScenarioPedestrians.Count > 0 && _timeSteps < maximalTimeSteps;
      // return _inScenarioPedestrians.Count > 0 && _timeSteps < 500; // To test
    }

    private void Paint()
    {
      foreach (Pedestrian pedestrian in _inScenarioPedestrians)
      {
        pedestrian.paint();
      }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
    /// </summary>
    /// <param name="row">Row index of cell.</param>
    /// <param name="column">Column index of cell.</param>
    /// <returns>Neighbours of a cell.</returns>
    /// <exception cref="ArgumentException"></exception>
    public List<Location> Neighbours(int row, int column) {
      if (row < 0 || row >= Rows)
        throw new ArgumentException("Neighbours: invalid row");
      if (column < 0 || column >= Columns)
        throw new ArgumentException("Neighbours: invalid column");

      return _neighbourhood.Neighbours(row, column);
    }
    
    /// <summary>
    /// Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
    /// </summary>
    /// <param name="location">Position with indexes of a cell</param>
    /// <returns>Neighbours of a cell.</returns>
    /// <exception cref="ArgumentException"></exception>
    public List<Location> Neighbours(Location location) {
      return Neighbours(location.Row, location.Column);
    }
    
    public void DestroyAllAutomatons()
    {
        GameObject.Destroy(_pedestrianContainer);
        Pedestrian.ResetIdentifiers();
        // foreach (Pedestrian pedestrian in inScenarioPedestrians)
        // {
        //     GameObject.Destroy(pedestrian);
        //     // inScenarioPedestrians.Remove(pedestrian);
        // }
    }

    public void UpdateMultiplierSpeed(float multiplierSpeed)
    {
      _parameters.MultiplierSpeedFactor = multiplierSpeed;
    }
    
    #endregion
  }
}