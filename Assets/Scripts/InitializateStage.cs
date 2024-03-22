using System;
using System.Collections;
using Cellular;
using DataJson;
using Events;
using Pedestrians;
using StageGenerator;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using SimpleFileBrowser;

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
    public static string JsonSaveFilePath => $"{Application.persistentDataPath}/" + _fileName;
    private static string _fileName = "TraceJson.json"; // Es posible que se quiera cambiarla, por lo que por ahora lo he dejado como variable
    // private string JsonScoreFilePath => $"{Application.persistentDataPath}/" + _fileName;
    // private string _fileName = "TraceJson.json";
    private string _pathToReadJson;

    void Start()
    {
        _pathToReadJson = JsonSaveFilePath;
        _cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        _timeLimit = 10 * 60;
        _pedestriansVelocity = 1.3f;
        _multiplierSpeed = 8;

        FileExplorerEvents.OnOpenFileExplorer += OpenFileExplorer;
        SimulationEvents.OnUpdateStageParameters += UpdateParameters;
        SimulationEvents.OnPlaySimulation += StartSimulation;
        
        FileExplorerEvents.OnSelectedPathForJson?.Invoke(_pathToReadJson);
    }
    
    void OnDestroy()
    {
        FileExplorerEvents.OnOpenFileExplorer -= OpenFileExplorer;
        SimulationEvents.OnUpdateStageParameters -= UpdateParameters;
        SimulationEvents.OnPlaySimulation -= StartSimulation;
    }

    private void StartAndSaveSimulation()
    {
        DestroySimulation();
        
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        // _stage = new RandomStage(cellsPrefab, transform, new Vector3(0.9f, 0.75f, 0.9f), 40, 90);
        // _stage = new RandomStage(cellsPrefab, transform);
        _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);

        InitializeAutomaton();

        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
            new PedestrianParameters.Builder()
                .FieldAttractionBias(Random.Range(0.0f, 10.0f))
                .CrowdRepulsion(Random.Range(0.1f, 0.5f))
                .VelocityPercent(Random.Range(0.3f, 1.0f))
                .Build();
        
        int numberOfPedestrians = Random.Range(150, 600);
        _automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        StartCoroutine(nameof(RunAutomatonCoroutine), true);
        
        // RunAutomaton();
    }
    
    private void LoadSimulation(JsonSnapshotsList traceJson)
    {
        DestroySimulation();
        
        _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);

        InitializeAutomaton();

        // Esto no hace falta ya que se usa para calcular los movimientos
        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
            new PedestrianParameters.Builder()
                .FieldAttractionBias(Random.Range(0.0f, 10.0f))
                .CrowdRepulsion(Random.Range(0.1f, 0.5f))
                .VelocityPercent(Random.Range(0.3f, 1.0f))
                .Build();

        int numberOfPedestrians = traceJson.snapshots[0].crowd.Count;
        _automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        StartCoroutine(nameof(RunAutomatonCoroutine), false);
        
    }

    private void DestroySimulation()
    {
        if (_stage != null)
        {
            Debug.Log("Stage destruido");
            StopAllCoroutines();
            _stage.DestroyStage();
            _automaton.DestroyAutomatons();
        }
    }

    private void InitializeAutomaton()
    {
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

    private IEnumerator RunAutomatonCoroutine(bool savingTrace)
    {
        if(savingTrace)
            yield return _automaton.RunAutomatonSimulationCoroutine();
        else
            yield return _automaton.LoadingSimulationCoroutine();
        Statistics statistics = _automaton.computeStatistics();
        Debug.Log(statistics);
        SaveInJson();
    }
    
    private void SaveInJson()
    {
        JsonSnapshotsList list = _automaton.JsonTrace();
        // SaveJsonManager.SaveScoreJson(JsonScoreFilePath, list);
        SaveJsonManager.SaveScoreJson(_pathToReadJson, list, _cellsDimension.x);
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
        Debug.Log(savingTrace);
        if (!savingTrace)
        {
            // Si se detecta un json desde la ruta almacenada en la variable _pathToReadJson,
            // el escenario también se ha precargado y coinciden el escenario cargado y el que se quiere simular
            // (ponerle un id de escenario tanto al json del escenario como al del snapshot???) simular la traza.
            JsonSnapshotsList traceJson = SaveJsonManager.LoadScoreJson(_pathToReadJson);
            traceJson.snapshots.ForEach(list => Debug.Log("Tete" + list.timestamp));
            LoadSimulation(traceJson);
        }
        else
        {
            StartAndSaveSimulation();
        }
    }
    
    private void OpenFileExplorer(bool savingTrace)
    {
        if (savingTrace)
        {
            FileBrowser.ShowLoadDialog( ( paths ) =>
                {
                    _pathToReadJson = paths[0] + "/" + _fileName;
                    FileExplorerEvents.OnSelectedPathForJson?.Invoke(_pathToReadJson);
                },
                () => { Debug.Log( "Canceled" ); },
                FileBrowser.PickMode.Folders, false, _pathToReadJson, null, "Select Folder", "Select" );
        }
        else
        {
            FileBrowser.ShowLoadDialog( ( paths ) =>
                {
                    _pathToReadJson = paths[0];
                    FileExplorerEvents.OnSelectedPathForJson?.Invoke(_pathToReadJson);
                },
                () => { Debug.Log( "Canceled" ); },
                FileBrowser.PickMode.Files, false, _pathToReadJson, null, "Select File", "Select" );
        }

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
