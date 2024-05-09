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
using SimpleFileBrowser;
using UI;
using Button = UnityEngine.UI.Button;

public class InitializateStage : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cellsPrefab;
    public GameObject pedestrianPrefab;

    #region private parameters

    private Vector3 _cellsDimension;
    private float _timeLimit;
    private float _pedestriansVelocity;
    private float _multiplierSpeed;

    private bool _showModal = false;
    // private const float WindowWidth = 400;
    private float WindowWidth => Screen.width * 0.21f;
    private float WindowHeight => Screen.height * 0.18f;

    #endregion
    
    // private StageWithBuilder.StageWithBuilder _stageBuilder;
    private Stage _stage;
    private CellularAutomaton _automaton;
    private Statistics _statistics;
    
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

    void Start()
    {
        // _cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        _cellsDimension = new Vector3(0.5f, 0.5f, 0.5f);
        _timeLimit = 10 * 60;
        _pedestriansVelocity = 1.3f;
        _multiplierSpeed = 8;

        SimulationEvents.OnInitializeStageParameters += InitializeParameters;
        SimulationEvents.OnPlaySimulation += StartSimulation;
        SimulationEvents.OnUpdateSimulationSpeed += UpdateMultiplierSpeed;
        SimulationEvents.OnGenerateRandomStage += RandomStage;
        
        FileExplorerEvents.OnSelectedPathForJson?.Invoke(PathsForJson.SaveTraceJson, TypeJsonButton.Trace, true);
        FileExplorerEvents.OnSelectedPathForJson?.Invoke(PathsForJson.SaveStageJson, TypeJsonButton.Stage, true);
    }

    void OnDestroy()
    {
        SimulationEvents.OnInitializeStageParameters -= InitializeParameters;
        SimulationEvents.OnPlaySimulation -= StartSimulation;
        SimulationEvents.OnUpdateSimulationSpeed -= UpdateMultiplierSpeed;
        SimulationEvents.OnGenerateRandomStage -= RandomStage;
        
    }
    
    private void OnGUI()
    {
        GUI.backgroundColor = Color.black;
        GUI.contentColor = Color.white;
        if (_showModal)
        {
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

    private void StartAndSaveSimulation()
    {
        // DestroySimulation();
        
        // Para que las posiciones reales empiezen en esta posición 
        
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        // _cellsDimension = new Vector3(0.5f, 0.5f, 0.5f);

        // RandomStage();
        if ((_automaton == null && _stage == null) || (_automaton != null && _stage != null)  //Para que solo se genere si no estaba generado antes o si estaba ya en una simulación
            || _stage.CellsDimension != _cellsDimension) //o en caso de que se haya modificado el tamaño de celdas después de generar el escenario
            SimulationEvents.OnGenerateRandomStage?.Invoke();

        InitializeAutomaton();

        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
            new PedestrianParameters.Builder()
                .FieldAttractionBias(Random.Range(0.0f, 10.0f))
                .CrowdRepulsion(Random.Range(0.1f, 0.5f))
                .VelocityPercent(Random.Range(0.3f, 1.0f))
                .Build();
        
        // int numberOfPedestrians = Random.Range(150, 600);
        int maxNumber = (_stage.Rows * _stage.Columns - (_stage.Obstacles.Count + _stage.Exits.Count))/4;
        int numberOfPedestrians = Random.Range(maxNumber/4, maxNumber);
        Debug.Log("Numero de agentes: " + numberOfPedestrians);
        _automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        StartCoroutine(nameof(RunAutomatonSimulationCoroutine));
        
        // RunAutomaton();
    }

    private void RandomStage()
    {
        _stage?.DestroyStage();
        // _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);
        _stage = new RandomStage(cellsPrefab, transform, _cellsDimension, 45, 55);
        
        _stage.InstantiateStage();
    }

    private void LoadSimulation(JsonSnapshotsList traceJson, JsonStage stageJson)
    {
        // DestroySimulation();

        Vector3 cellsDimension = new Vector3(traceJson.cellDimension, traceJson.cellDimension, traceJson.cellDimension);
        DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == 1);
        
        _stage = new StageFromJson(cellsPrefab, transform, cellsDimension, stageJson, domain);
        // _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);

        _stage.InstantiateStage();
        InitializeAutomaton();
        
        StartCoroutine(nameof(LoadingSimulationCoroutine), traceJson);
        
    }

    private void DestroySimulation()
    {
        if (_automaton != null && _stage != null)  //Para que solo se destruya si estaba ya en una simulación
        {
            StopAllCoroutines();
            _stage.DestroyStage();
            _automaton.DestroyAllAutomatons();
            Debug.Log("Simulación destruida");
        }
    }

    private void InitializeAutomaton()
    {
        // _stage.InstantiateStage();
        
        // Para que las posiciones reales empiezen en esta posición 
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
         _statistics = _automaton.computeStatistics();
        _showModal = true;
        SaveInJson();
    }
    
    private IEnumerator LoadingSimulationCoroutine(JsonSnapshotsList traceJson)
    {
        yield return _automaton.LoadingSimulationCoroutine(traceJson);
        _statistics = _automaton.computeStatistics();
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

    #region RunAutomatonWithoutCoroutines

    //Este método debería de ir en un update

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
        {
            // Si se detecta un json desde la ruta almacenada en la variable _pathToReadJson,
            // el escenario también se ha precargado y coinciden el escenario cargado y el que se quiere simular
            // (ponerle un id de escenario tanto al json del escenario como al del snapshot???) simular la traza.
            JsonSnapshotsList traceJson = SaveJsonManager.LoadTraceJson(PathsForJson.LoadTraceJson);
            JsonStage stageJson = SaveJsonManager.LoadStageJson(PathsForJson.LoadStageJson);
            // traceJson.snapshots.ForEach(list => Debug.Log("Loading timestep: " + list.timestamp));
            Debug.Log(PathsForJson.LoadTraceJson + PathsForJson.LoadStageJson);
            LoadSimulation(traceJson, stageJson);
        }
        else
        {
            StartAndSaveSimulation();
        }
    }

    private void InitializeParameters(float cellsDimensions, float pedestriansVelocity, float multiplierSpeed)
    {
        // _timeLimit = timeLimit;
        _cellsDimension = Vector3.one * cellsDimensions;
        _pedestriansVelocity = pedestriansVelocity;
        _multiplierSpeed = multiplierSpeed;

    }
    
    private void UpdateMultiplierSpeed(float multiplierSpeed)
    {
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
