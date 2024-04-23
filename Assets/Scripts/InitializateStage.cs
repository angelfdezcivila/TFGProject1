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

    #endregion
    
    // private StageWithBuilder.StageWithBuilder _stageBuilder;
    private Stage _stage;
    private CellularAutomaton _automaton;
    public static string JsonInitialFilePath => $"{Application.persistentDataPath}/" + _traceFileName;
    public static string JsonStageInitialFilePath => $"{Application.persistentDataPath}/" + _stageFileName;
    private static string _traceFileName = "TraceJson.json"; // Es posible que se quiera cambiarla, por lo que por ahora lo he dejado como variable
    private static string _stageFileName = "StageJson.json"; // Es posible que se quiera cambiarla, por lo que por ahora lo he dejado como variable
    // private string JsonScoreFilePath => $"{Application.persistentDataPath}/" + _fileName;
    // private string _fileName = "TraceJson.json";
    private string _pathToTraceJson;
    private string _pathToStageJson;

    void Start()
    {
        _pathToTraceJson = JsonInitialFilePath;
        _pathToStageJson = JsonStageInitialFilePath + _stageFileName;
        _cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        _timeLimit = 10 * 60;
        _pedestriansVelocity = 1.3f;
        _multiplierSpeed = 8;

        FileExplorerEvents.OnOpenFileExplorer += OpenFileExplorer;
        SimulationEvents.OnUpdateStageParameters += UpdateParameters;
        SimulationEvents.OnPlaySimulation += StartSimulation;
        FileExplorerEvents.OnSelectedPathForJson += UpdatePathJson;

        
        FileExplorerEvents.OnSelectedPathForJson?.Invoke(_pathToTraceJson);
    }

    void OnDestroy()
    {
        FileExplorerEvents.OnOpenFileExplorer -= OpenFileExplorer;
        SimulationEvents.OnUpdateStageParameters -= UpdateParameters;
        SimulationEvents.OnPlaySimulation -= StartSimulation;
        FileExplorerEvents.OnSelectedPathForJson -= UpdatePathJson;

    }

    private void StartAndSaveSimulation()
    {
        DestroySimulation();
        
        // Para que las posiciones reales empiezen en esta posición 
        
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        _cellsDimension = new Vector3(0.5f, 0.5f, 0.5f);

        // _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);
        _stage = new RandomStage(cellsPrefab, transform, _cellsDimension, 45, 55);
        
        InitializeAutomaton();

        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
            new PedestrianParameters.Builder()
                .FieldAttractionBias(Random.Range(0.0f, 10.0f))
                .CrowdRepulsion(Random.Range(0.1f, 0.5f))
                .VelocityPercent(Random.Range(0.3f, 1.0f))
                .Build();
        
        int numberOfPedestrians = Random.Range(150, 600);
        _automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        StartCoroutine(nameof(RunAutomatonSimulationCoroutine));
        
        // RunAutomaton();
    }
    
    private void LoadSimulation(JsonSnapshotsList traceJson, JsonStage stageJson)
    {
        DestroySimulation();

        Vector3 cellsDimension = new Vector3(traceJson.cellDimension, traceJson.cellDimension, traceJson.cellDimension);
        DomainEntryJson domain = stageJson.domains.Find(domain => domain.id == 1);
        
        _stage = new StageFromJson(cellsPrefab, transform, cellsDimension, stageJson, domain);
        // _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);

        InitializeAutomaton();
        
        StartCoroutine(nameof(LoadingSimulationCoroutine), traceJson);
        
    }

    private void DestroySimulation()
    {
        if (_stage != null)
        {
            Debug.Log("Stage destruido");
            StopAllCoroutines();
            _stage.DestroyStage();
            _automaton.DestroyAllAutomatons();
        }
    }

    private void InitializeAutomaton()
    {
        _stage.InstantiateStage();
        
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
        Statistics statistics = _automaton.computeStatistics();
        Debug.Log(statistics);
        SaveInJson();
    }
    
    private IEnumerator LoadingSimulationCoroutine(JsonSnapshotsList traceJson)
    {
        yield return _automaton.LoadingSimulationCoroutine(traceJson);
        Statistics statistics = _automaton.computeStatistics();
        Debug.Log(statistics);
    }
    
    private void SaveInJson()
    {
        // JsonSnapshotsList list = _automaton.JsonTrace();
        // SaveJsonManager.SaveScoreJson(_pathToJson, list);
        // List<JsonCrowdList> trace = _automaton.JsonTrace();
        JsonSnapshotsList trace = _automaton.JsonTrace();
        JsonStage stage = _automaton.JsonStage();
        SaveJsonManager.SaveTraceJson(_pathToTraceJson, trace);
        SaveJsonManager.SaveStageJson(_pathToStageJson, stage);
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
        if (!savingTrace)
        {
            // Si se detecta un json desde la ruta almacenada en la variable _pathToReadJson,
            // el escenario también se ha precargado y coinciden el escenario cargado y el que se quiere simular
            // (ponerle un id de escenario tanto al json del escenario como al del snapshot???) simular la traza.
            JsonSnapshotsList traceJson = SaveJsonManager.LoadTraceJson(_pathToTraceJson);
            JsonStage stageJson = SaveJsonManager.LoadStageJson(_pathToStageJson);
            // traceJson.snapshots.ForEach(list => Debug.Log("Loading timestep: " + list.timestamp));
            LoadSimulation(traceJson, stageJson);
        }
        else
        {
            StartAndSaveSimulation();
        }
    }
    
    // TODO: hay que cambiar las variables de las rutas de los json para que sean dos rutas diferentes
    private void OpenFileExplorer(bool savingJson, Button buttonTriggered)
    // private void OpenFileExplorer(bool savingJson)
    {
        if (savingJson)
        {
            FileBrowser.ShowLoadDialog( ( paths ) =>
                {
                    _pathToTraceJson = paths[0] + "/" + _traceFileName;
                    FileExplorerEvents.OnSelectedPathForJson?.Invoke(_pathToTraceJson);
                },
                () => { Debug.Log("Canceled"); },
                FileBrowser.PickMode.Folders, false, _pathToTraceJson, null, "Select Folder", "Select" );
        }
        else
        {
            FileBrowser.ShowLoadDialog( ( paths ) =>
                {
                    _pathToTraceJson = paths[0];
                    FileExplorerEvents.OnSelectedPathForJson?.Invoke(_pathToTraceJson);
                },
                () => { Debug.Log("Canceled"); },
                FileBrowser.PickMode.Files, false, _pathToTraceJson, null, "Select File", "Select" );
        }

    }
    
    private void UpdatePathJson(string path)
    {
        _pathToTraceJson = path;
    }

    private void UpdateParameters(float pedestriansVelocity, float multiplierSpeed)
    {
        // UpdateTimeLimit(timeLimit);
        UpdatePedestriansVelocity(pedestriansVelocity);
        UpdateMultiplierSpeed(multiplierSpeed);
    }
    
    private void UpdateTimeLimit(float timeLimit)
    {
        _timeLimit = timeLimit;
    }
    
    private void UpdatePedestriansVelocity(float pedestriansVelocity)
    {
        _pedestriansVelocity = pedestriansVelocity;
    }
    
    private void UpdateMultiplierSpeed(float multiplierSpeed)
    {
        _multiplierSpeed = multiplierSpeed;
    }

    #endregion
}
