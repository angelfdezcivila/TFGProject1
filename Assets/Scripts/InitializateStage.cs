using System;
using System.Collections;
using Cellular;
using DataJson;
using Events;
using JsonDataManager.Stage;
using JsonDataManager.Trace;
using Pedestrians;
using StageGenerator;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using UI;

public class InitializateStage : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cellsPrefab;
    public GameObject pedestrianPrefab;

    #region Private variables

    private float _timeLimit;
    private Vector3 _cellsDimension;
    private int _pedestrianNumber;
    private float _pedestriansVelocity;
    private float _multiplierSpeed;

    private bool _showModal = false;
    // private const float WindowWidth = 400;
    
    // private StageWithBuilder.StageWithBuilder _stageBuilder;
    private Stage _stage;
    private CellularAutomaton _automaton;
    private Statistics _statistics;

    #endregion

    #region Private Properties

    private float WindowWidth => Screen.width * 0.21f;
    private float WindowHeight => Screen.height * 0.18f;
    private string StatisticsText
    {
        get
        {
            if (_statistics == null)
                return "Statistics not computed yet.";
            string text = $"Mean steps: {_statistics.meanSteps} \n" +
                          $"Mean evacuation time: {_statistics.meanEvacuationTime} \n" +
                          $"Median steps: {_statistics.medianSteps} \n" +
                          $"Median evacuation time: {_statistics.medianEvacuationTime} \n" +
                          $"Number of evacuees: {_statistics.numberOfEvacuees} \n" +
                          $"Number of non evacuees: {_statistics.numberOfNonEvacuees}";
            return text;
        }
    }
    
    #endregion

    #region Unity Events

    void Start()
    {
        _timeLimit = 10 * 60;
        // _cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        _cellsDimension = new Vector3(0.5f, 0.5f, 0.5f);
        _pedestrianNumber = 1;
        _pedestriansVelocity = 1.3f;
        _multiplierSpeed = 8;
        
        FileExplorerEvents.OnSelectedPathForJson?.Invoke(PathsForJson.SaveTraceJson, TypeJsonButton.Trace, true);
        FileExplorerEvents.OnSelectedPathForJson?.Invoke(PathsForJson.SaveStageJson, TypeJsonButton.Stage, true);
    }

    void OnEnable()
    {
        SimulationEvents.OnInitializeStageParameters += InitializeParameters;
        SimulationEvents.OnPlaySimulation += StartSimulation;
        SimulationEvents.OnGenerateRandomStage += RandomStage;
        SimulationEvents.OnUpdateSimulationSpeed += UpdateMultiplierSpeed;
    }

    void OnDisable()
    {
        SimulationEvents.OnInitializeStageParameters -= InitializeParameters;
        SimulationEvents.OnPlaySimulation -= StartSimulation;
        SimulationEvents.OnGenerateRandomStage -= RandomStage;
        SimulationEvents.OnUpdateSimulationSpeed -= UpdateMultiplierSpeed;
    }
    
    void OnGUI()
    {
        if (_showModal)
        {
            GUI.backgroundColor = Color.black;
            GUI.contentColor = Color.white;
            // Define the window position and size
            Rect windowRect = new Rect((Screen.width-WindowWidth)/2, (Screen.height-WindowHeight)/2, WindowWidth, WindowHeight);
            // Display the modal window
            windowRect = GUI.ModalWindow(
                0, // Unique ID for the window
                windowRect,
                DrawModalWindowContents,
                "Statistics"
            );
        }
    }

    // To Debug Statistics window
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         _showModal = !_showModal;
    //     }
    // }

    private void DrawModalWindowContents(int windowID)
    {
        GUILayout.Space(10);
        // Display the message inside the modal window
        GUILayout.Label(StatisticsText);
        GUILayout.Space(10);
        // Add a button to close the modal window
        if (GUILayout.Button("Close"))
        {
            _showModal = false;
        }
    }
    
    #endregion

    #region Private Methods

    private void DestroySimulation()
    {
        if (_automaton != null && _stage != null)  //Gets destroyed only if it was already in a simulation.
        {
            StopAllCoroutines();
            _stage.DestroyStage();
            _automaton.DestroyAllAutomatons();
            Debug.Log("Simulación destruida");
        }
    }

    private void StartAndSaveSimulation()
    {
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        // _cellsDimension = new Vector3(0.5f, 0.5f, 0.5f);
        
        // RandomStage();
        if ((_automaton == null && _stage == null) || (_automaton != null && _stage != null)  //To be generated only if it was not generated before or if it was already in a simulation
            || _stage.CellsDimension != _cellsDimension) //or in case the cell size has been modified after generating the scenario
            SimulationEvents.OnGenerateRandomStage?.Invoke();

        InitializeAutomaton();

        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
            new PedestrianParameters.Builder()
                .FieldAttractionBias(Random.Range(0.0f, 10.0f))
                .CrowdRepulsion(Random.Range(0.1f, 0.5f))
                .VelocityPercent(Random.Range(0.3f, 1.0f))
                .Build();
        
        int maxNumber = (_stage.Rows * _stage.Columns - (_stage.Obstacles.Count + _stage.Exits.Count))/4;
        // int numberOfPedestrians = Random.Range(maxNumber/4, maxNumber);
        int numberOfPedestrians = Mathf.Clamp(_pedestrianNumber, 1, maxNumber);
        // Debug.Log("Numero de agentes: " + numberOfPedestrians);
        _automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        StartCoroutine(nameof(RunAutomatonSimulationCoroutine));
        
        // RunAutomaton();
    }

    private void LoadSimulation()
    {
        // TODO: Leer este comentario
        // Si se detecta un json desde la ruta almacenada en la variable _pathToReadJson,
        // el escenario también se ha precargado y coinciden el escenario cargado y el que se quiere simular
        // (ponerle un id de escenario tanto al json del escenario como al del snapshot???) simular la traza.
        JsonSnapshotsList traceJson = SaveJsonManager.LoadTraceJson(PathsForJson.LoadTraceJson);
        JsonStage stageJson = SaveJsonManager.LoadStageJson(PathsForJson.LoadStageJson);

        Vector3 cellsDimension = new Vector3(traceJson.cellDimension, traceJson.cellDimension, traceJson.cellDimension);
        DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == 1);
        
        _stage = new StageFromJson(cellsPrefab, transform, cellsDimension, stageJson, domain);
        // _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);

        _stage.InstantiateStage();
        InitializeAutomaton();
        
        StartCoroutine(nameof(LoadingSimulationCoroutine), traceJson);
        
    }

    private void InitializeAutomaton()
    {
        // _stage.InstantiateStage();
        
        // For the actual positions to start in this position
        transform.position = new Vector3(_stage.CellsDimension.x, 0, _stage.CellsDimension.z) / 2;
        
        var cellularAutomatonParameters =
            new CellularAutomatonParameters.Builder()
                .Scenario(_stage) // use this scenario
                .TimeLimit(_timeLimit) // 10 minutes is time limit for simulation by default
                .Neighbourhood(MooreNeighbourhood.of) // use Moore's Neighbourhood for automaton
                .PedestrianReferenceVelocity(_pedestriansVelocity) // fastest pedestrians walk at 1.3 m/s by default
                .MultiplierSpeedFactor(_multiplierSpeed) // perform animation x8 times faster than real time by default
                .Build();

        // pedestrianPrefab.transform.localScale = cellsDimension;
        _automaton = new CellularAutomaton(cellularAutomatonParameters, pedestrianPrefab);
    }

    private IEnumerator RunAutomatonSimulationCoroutine()
    {
        yield return _automaton.RunAutomatonSimulationCoroutine();
         _statistics = _automaton.ComputeStatistics();
        _showModal = true;
        SaveInJson();
    }
    
    private IEnumerator LoadingSimulationCoroutine(JsonSnapshotsList traceJson)
    {
        yield return _automaton.LoadingSimulationCoroutine(traceJson);
        _statistics = _automaton.ComputeStatistics();
        _showModal = true;
    }
    
    private void SaveInJson()
    {
        // JsonSnapshotsList list = _automaton.JsonTrace();
        // SaveJsonManager.SaveScoreJson(_pathToJson, list);
        // List<JsonCrowdList> trace = _automaton.JsonTrace();
        JsonSnapshotsList trace = _automaton.JsonTrace();
        JsonStage stage = _automaton.JsonStage();
        SaveJsonManager.SaveTraceJson(PathsForJson.SaveTraceJson, trace);
        SaveJsonManager.SaveStageJson(PathsForJson.SaveStageJson, stage);
    }
    
    #endregion

    #region RunAutomatonWithoutCoroutines

    //this void should be in an Update

    // private void RunAutomaton()
    // {
    //     InitializeFloor();
    //     float timer = _automaton.RealTimePerTick;
    //     while (_automaton.SimulationShouldContinue())
    //     {
    //         Debug.Log(timer);
    //         timer -= Time.deltaTime;
    //         if (timer <= 0)
    //         {
    //             _automaton.RunStep();
    //             timer += _automaton.RealTimePerTick;
    //         }
    //     }
    //     
    //     // InvokeRepeating(nameof(RunStep), _automaton.RealTimePerTick, _automaton.RealTimePerTick);
    //
    //     
    //     _automaton.Paint();
    // }
    //
    // private void InitializeFloor()
    // {
    //     _automaton.InitializeStaticFloor();
    //   
    //     Debug.Log("Real time per tick" + _automaton.RealTimePerTick);
    //
    //     _automaton.Paint();
    // }
    //
    // private void RunStep()
    // {
    //     _automaton.RunStep();
    // }

    #endregion

    #region Callbacks
    
    private void StartSimulation(bool savingTrace)
    {
        DestroySimulation();
        
        if (!savingTrace)
            LoadSimulation();
        else
            StartAndSaveSimulation();
    }

    private void InitializeParameters(float cellsDimensions, int pedestrianNumber, float pedestriansVelocity, float multiplierSpeed)
    {
        // _timeLimit = timeLimit;
        _cellsDimension = Vector3.one * cellsDimensions;
        _pedestrianNumber = pedestrianNumber;
        _pedestriansVelocity = pedestriansVelocity;
        _multiplierSpeed = multiplierSpeed;
    }
    
    private void RandomStage()
    {
        _stage?.DestroyStage();
        // _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);
        _stage = new RandomStage(cellsPrefab, transform, _cellsDimension, 45, 55);
        
        _stage.InstantiateStage();
    }
    
    private void UpdateMultiplierSpeed(float multiplierSpeed)
    {
        //TODO: Comprobar estos comentarios
        // El problema de esta es que a la hora de actualizar el multiplicador, se está en mitad de una de las iteraciones,
        // por lo que si se hace un cambio muy brusco, se puede llegar a notar
        #region Automaton Parameter Implementation

        if(_automaton != null)
            _automaton.UpdateMultiplierSpeed(multiplierSpeed);

        #endregion

        // El problema de esta es que habría que eliminar el multiplicador o asegurarse de que siempre sea 1,
        // ya que la velocidad de la simulación como tal dependería de un factor externo al automata como es el Time.TimeScale.
        #region TimeScale Implementation

        // if (multiplierSpeed >= 0)
        // {
        //     Time.timeScale = multiplierSpeed;
        //     // Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        // }

        #endregion
    }

    #endregion
}
