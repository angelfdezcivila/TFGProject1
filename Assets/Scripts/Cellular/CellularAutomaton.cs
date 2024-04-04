using System;
using System.Collections;
using System.Collections.Generic;
using JsonDataManager.Trace;
using Pedestrians;
using StageGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cellular
{
  public class CellularAutomaton{

    #region Private variables

    /**
   * Scenario where simulation takes place.
   */
    private readonly Stage _stage;
    /**
   * Parameters describing this automaton.
   */
    private readonly CellularAutomatonParameters _parameters;
    /**
   * Neighbourhood relationship used by this automaton.
   */
    private readonly Neighbourhood _neighbourhood;
    /**
   * {@code true} if cell is occupied by a pedestrian in current discrete state.
   */
    private bool[,] _occupied;
    /**
   * {@code true} if cell will be occupied by a pedestrian in next discrete state.
   */
    private bool[,] _occupiedNextState;
    /**
   * Factory for generating pedestrians for this automaton.
   */
    private readonly PedestrianFactory _pedestrianFactory;
    /**
   * List of pedestrians currently within the scenario.
   */
    private readonly List<Pedestrian> _inScenarioPedestrians;
    /**
   * List of pedestrians that have evacuated the scenario.
   */
    private readonly List<Pedestrian> _outOfScenarioPedestrians;
    /**
   * Number of discrete time steps elapsed since the start of the simulation.
   */
    private int _timeSteps;
    /**
    * GameObject that contains all the pedestrians in stage.
    */
    private GameObject _pedestrianContainer;
    #endregion
    

    #region Properties
    
    /// <summary>
    /// Tiempo de cada tick de la animación entre cada movimiento de los agentes como grupo.
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
    
    #endregion
    
    #region Constructor
    /// <summary>
    /// Creates a new Cellular Automaton with provided parameters.
    /// </summary>
    /// <param name="parameters">parameters describing this automaton.</param>
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

    private void ClearCells(bool[,] cells) {
      for (int i = 0; i < cells.GetLength(0); i++)
      {
        for (int j = 0; j < cells.GetLength(1); j++)
        {
          cells[i, j] = false;
        }
      }
    }
    
    /// <summary>
    /// Adds a new pedestrian to this automaton.
    /// </summary>
    /// <param name="row">row index of scenario where new pedestrian should be placed.</param>
    /// <param name="column">column index of scenario where new pedestrian should be placed.</param>
    /// <param name="parameters">parameters describing new pedestrian.</param>
    /// <returns>if pedestrian could be created (location was neither blocked nor taken by another pedestrian).</returns>
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
    /// <param name="location">location indexes in scenario where new pedestrian should be placed.</param>
    /// <param name="parameters">parameters describing new pedestrian.</param>
    /// <returns>if pedestrian could be created (location was neither blocked nor taken by another pedestrian).</returns>
    public bool AddPedestrian(Location location, PedestrianParameters parameters) {
      return AddPedestrian(location.Row, location.Column, parameters);
    }
    
    /// <summary>
    /// Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
    /// </summary>
    /// <param name="numberOfPedestrians">number of new pedestrian to add.</param>
    /// <param name="pedestrianParameters">parameters describing new pedestrians.</param>
    public void AddPedestriansUniformly(int numberOfPedestrians, PedestrianParameters pedestrianParameters) {
      AddPedestriansUniformly(numberOfPedestrians, () => pedestrianParameters);
    }
    
    /// <summary>
    /// Adds a given number of new pedestrians located uniform randomly among free cells in automaton's scenario.
    /// </summary>
    /// <param name="numberOfPedestrians">number of new pedestrian to add.</param>
    /// <param name="pedestrianParameters">a supplier providing parameters describing each new pedestrians.</param>
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

    public void AddPedestriansFromJson(JsonSnapshotsList traceJson)
    {
      
      int numberOfPedestrians = traceJson.snapshots[0].crowd.Count;
      for (int i = 0; i < numberOfPedestrians; i++)
      {
        CrowdEntryJson pedestrian = traceJson.snapshots[0].crowd[i];
        CoordinatesJson coordinates = pedestrian.location.coordinates;
        AddPedestrianFromJson((int)(coordinates.X / CellsDimension), (int)(coordinates.Y / CellsDimension), traceJson);
      }
    }
    
    /// <summary>
    /// Adds a new pedestrian from a json.
    /// </summary>
    /// <param name="row">row index of scenario where new pedestrian should be placed.</param>
    /// <param name="column">column index of scenario where new pedestrian should be placed.</param>
    /// <param name="parameters">parameters describing new pedestrian.</param>
    /// <returns>if pedestrian could be created (location was neither blocked nor taken by another pedestrian).</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool AddPedestrianFromJson(int row, int column, JsonSnapshotsList traceJson) {
      if (row < 0 || row >= Rows) throw new ArgumentOutOfRangeException("AddPedestrian: invalid row");
      if (column < 0 || column >= Columns) throw new ArgumentOutOfRangeException("AddPedestrian: invalid column");
      if (IsCellReachable(row, column)) {
        Pedestrian pedestrianLoaded = _pedestrianFactory.GetInstance(row, column, null);
        
        // AddPathToPedestrian(traceJson, pedestrianLoaded);
        
        _occupied[row, column] = true;
        _inScenarioPedestrians.Add(pedestrianLoaded);
        return true;
      } else {
        return false;
      }
    }

    // Load the current timeStep for a pedestrian loaded from a json
    private void PedestrianTimeStepLoaded(JsonSnapshotsList traceJson, Pedestrian pedestrian)
    {
      JsonCrowdList snapshot = traceJson.snapshots[_timeSteps]; // snaphot.timestamp == _timeSteps
          
      CrowdEntryJson crowdEntryJson = snapshot.crowd.Find(pedestrianToFind => pedestrianToFind.id == pedestrian.Identifier);
      if (crowdEntryJson != null)
      {
        CoordinatesJson locationCoordinates = crowdEntryJson.location.coordinates;
        int row = (int)(locationCoordinates.X / CellsDimension);
        int column = (int)(locationCoordinates.Y / CellsDimension);
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

    /// <summary>
    /// Returns neighbours of a cell in this automaton (will depend on neighbourhood relationship).
    /// </summary>
    /// <param name="row">row index of cell.</param>
    /// <param name="column">column index of cell.</param>
    /// <returns>neighbours of a cell.</returns>
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
    /// <returns>neighbours of a cell.</returns>
    /// <exception cref="ArgumentException"></exception>
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
    
    
    /// <summary>
    /// Checks whether a cell is occupied by some pedestrian.
    /// </summary>
    /// <param name="row">row index of cell.</param>
    /// <param name="column">column index of cell.</param>
    /// <returns>if cell is occupied (if there is an obstacle or another pedestrian).</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsCellOccupied(int row, int column) {
      if (row < 0 || row >= Rows)
        throw new ArgumentException("IsCellOccupied: invalid row");
      if (column < 0 || column >= Columns)
        throw new ArgumentException("IsCellOccupied: invalid column");

      return _occupied[row, column];
    }

    /**
   * Checks whether a cell is occupied by some pedestrian.
   *
   * @param location location of cell to check.
   * @return {@code true} if cell is occupied by some pedestrian.
   */
    
    /// <summary>
    /// Checks whether a cell is occupied by some pedestrian.
    /// </summary>
    /// <param name="location">location with indexes of cell to check.</param>
    /// <returns>if cell is occupied (if there is an obstacle or another pedestrian).</returns>
    /// <exception cref="ArgumentException"></exception>
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
    
      return !_occupied[row,column] && !_stage.IsCellBlocked(row, column);
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
    
      return _occupiedNextState[row,column];
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
    private void TimeStepAutomaton() {
      _timeSteps++;
      
      // clear new state
      ClearCells(_occupiedNextState);

      //Para comprobar que no se repitan los agentes
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
      Debug.Log("TIMESTEP: " + _timeSteps);
      // inScenarioPedestrians.ForEach(pedestrian => Debug.Log(pedestrian + ", timestamp: " + timeSteps));
      // timeSteps++;
    }

    // TODO: los agentes no se destruye ni se detecta si han salido o no correctamente debido a que hay que cargar el escenario correspondiente
    private void TimeStepLoadedSimulation(JsonSnapshotsList traceJson)
    {
      _timeSteps++;
      //Se obtiene del json la lista en el timeStep correspondiente.
      //Como al inicio hemos añadido todos los agentes del json a la lista _inScenarioPedestrians, podemos iterar sobre esta
        List<Pedestrian> pedestriansToMove = new List<Pedestrian>(_inScenarioPedestrians);
        // List<Pedestrian> pedestriansToMove = new List<Pedestrian>(); //Esto contiene los agentes de ese timestep
        // traceJson.snapshots[_timeSteps].crowd.ForEach(crowdEntryJson => 
        //   pedestriansToMove.Add(_inScenarioPedestrians.Find(pedestrian => pedestrian.Identifier == crowdEntryJson.id)));
        
        foreach (Pedestrian pedestrian in pedestriansToMove)
        {
          // AddPathToPedestrian(traceJson, pedestrian);
          PedestrianTimeStepLoaded(traceJson, pedestrian);
          // Location location = pedestrian.GetPath()[_timeSteps];
        }
        
        Debug.Log("TIMESTEP: " + _timeSteps + "Count: " + _inScenarioPedestrians.Count);
    }
  
    /**
   * Runs a step of this automaton.
   */
    public void RunAutomatonStep() {
      TimeStepAutomaton();
      Paint();
    }
    
    
     /**
    * Runs a step of the simulation loaded.
    */
    public void LoadSimulationStep(JsonSnapshotsList traceJson) {
      TimeStepLoadedSimulation(traceJson);
      Paint();
    }

    public bool SimulationShouldContinue()
    {
      float maximalTimeSteps = _parameters.TimeLimit / _parameters.TimePerTick;
      return SimulationShouldContinue(maximalTimeSteps);
    }
  
    private bool SimulationShouldContinue(float maximalTimeSteps)
    {
      return _inScenarioPedestrians.Count > 0 && _timeSteps < maximalTimeSteps;
      // return _inScenarioPedestrians.Count > 0 && _timeSteps < 500; //Para testear
    }
  
    public IEnumerator RunAutomatonSimulationCoroutine() {
      InitializeStaticFloor();
      
      Debug.Log("Real time per tick" + RealTimePerTick);
    
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
    
    public IEnumerator LoadingSimulationCoroutine(JsonSnapshotsList traceJson) {
      Debug.Log("Real time per tick" + RealTimePerTick);
      
      // Crear y añadir a escena los agentes en su posición inicial
      AddPedestriansFromJson(traceJson);
      
      Paint();
      yield return new WaitForSeconds(1.5f); //Para ver las posiciones iniciales de cada agente

      // while (_timeSteps < traceJson.snapshots.Count)
      while (SimulationShouldContinue())
      {
        LoadSimulationStep(traceJson);
        yield return new WaitForSeconds(RealTimePerTick);
        // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

      }
      
      _timeSteps++;
      Paint();
    }

    public void InitializeStaticFloor()
    {
      _stage.StaticFloorField.initialize();
      _timeSteps = 0;
    }

    public void Paint()
    {
      foreach (Pedestrian pedestrian in _inScenarioPedestrians)
      {
        pedestrian.paint();
      }
    }

    /**
   * Returns number of evacuees (number of pedestrians that have evacuated scenario).
   *
   * @return number of evacuees.
   */
    private int NumberOfEvacuees => _outOfScenarioPedestrians.Count;

    /**
   * Returns number of non evacuees (number of pedestrians still inside scenario).
   *
   * @return number of non evacuees.
   */
    private int NumberOfNonEvacuees => _inScenarioPedestrians.Count;

    /**
   * Returns evacuation times for evacuees.
   * @return evacuation times for evacuees.
   */
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

    /**
   * Returns distances to the closest exit for each non evacuee.
   *
   * @return distances to closest exit for each non evacuee.
   */
    public float[] distancesToClosestExit() {
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
      foreach (Pedestrians.Pedestrian pedestrian in _outOfScenarioPedestrians) {
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


    private CrowdEntryJson JsonPedestrian(int id, int domain, float row, float column)
    {
      // LocationJson locationJson = new LocationJson(domain, GetVectorToPosition(row, column));
      LocationJson locationJson = new LocationJson(domain, GetCoordinatesToPosition(row, column));
      CrowdEntryJson crowdJson = new CrowdEntryJson(locationJson, id);

      return crowdJson;
    }

    // private CoordinatesJson GetVector2Position(int row, int column)
    private Vector2 GetVectorToPosition(float row, float column)
    {
      // Vector3 pos3D = parameters.Scenario.GetRowColumnPosition(new Vector2(row, column)).transform.position;
      // Vector2 position = new Vector2(pos3D.x, pos3D.z);
      // CoordinatesJson position = new CoordinatesJson(pos3D.x, pos3D.y);
      Vector2 position = new Vector2(row, column);
      
      return position;
    }
    
    private CoordinatesJson GetCoordinatesToPosition(float row, float column)
    {
      CoordinatesJson coordinates = new CoordinatesJson(row, column);
      
      return coordinates;
    }

    /**
    /**
     * Json representing traces of all pedestrians through the scenario.
     *
     * @return Json representing traces of all pedestrians through the scenario.
     */
    // public JsonSnapshotsList JsonTrace() {
    public List<JsonCrowdList> JsonTrace() {
      int domain = 1; // TODO: currently there is only a single domain
    
      // Create an empty JsonArray for the snapshots
      // JsonSnapshotsList snapshots = new JsonSnapshotsList();
      List<JsonCrowdList> snapshots = new List<JsonCrowdList>();

    
      List<Pedestrian> allPedestrians = new List<Pedestrian>();
      _inScenarioPedestrians.ForEach(pedestrian => allPedestrians.Add(pedestrian));
      _outOfScenarioPedestrians.ForEach(pedestrian => allPedestrians.Add(pedestrian));
      allPedestrians.Sort((p1, p2) => p1.Identifier.CompareTo(p2.Identifier));
      // allPedestrians.sort(Comparator.comparing(Pedestrian::getIdentifier));
      // allPedestrians.ForEach(pedestrian => Debug.Log("FINISHED " + pedestrian + ", timestamp: " + pedestrian.GetPath().Count +  ", Steps: " + pedestrian.getNumberOfSteps()));

      // Create snapshots
      for (int t = 0; t < _timeSteps; t++) {
        JsonCrowdList crowd = new JsonCrowdList();
        foreach (Pedestrian pedestrian in allPedestrians) {
          List<Location> path = pedestrian.GetPath();
          if (path.Count > t)
          {
            Location location = path[t];
            // crowd.AddCrowdToList(JsonPedestrian(pedestrian.Identifier, domain, location.Row, location.Column));
            crowd.AddCrowdToList(JsonPedestrian(pedestrian.Identifier, domain, 
              location.Row*CellsDimension, location.Column*CellsDimension));
          }
        }

        if (crowd.crowd.Count > 0) // En caso de que no haya ningún agente, es decir, que haya terminado la simulación, no hay que añadirlo al json
        {
          crowd.timestamp = t;
          // snapshots.AddCrowdsToList(crowd);
          snapshots.Add(crowd);
        }

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