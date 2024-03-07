using System;
using System.Collections;
using System.IO;
using Cellular;
using DataJson;
using Events;
using Pedestrians;
using StageGenerator;
using TestingStageWithBuilder;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private StageGenerator.Stage _stage;
    private CellularAutomaton _automaton;
    private string JsonScoreFilePath => $"{Application.persistentDataPath}/TraceJson.json";
    private string _pathToReadJson = "C:/Users/Usuario/Desktop/TraceJson.json";
    
    void Start()
    {
        _cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        _timeLimit = 10 * 60;
        _pedestriansVelocity = 1.3f;
        _multiplierSpeed = 8;
        
        ParametersEvents.OnUpdateStageParameters += UpdateParameters;
        ParametersEvents.OnPlaySimulation += StartSimulation;
    }
    
    void OnDestroy()
    {
        ParametersEvents.OnUpdateStageParameters -= UpdateParameters;
        ParametersEvents.OnPlaySimulation -= StartSimulation;
    }
    
    private void StartSimulation()
    {
        if (_stage != null)
        {
            Debug.Log("Destruiiiir");
            StopAllCoroutines();
            _stage.DestroyStage();
            _automaton.DestroyAutomatons();
        }
        
        Debug.Log(_pedestriansVelocity);
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        // _stage = new RandomStage(cellsPrefab, transform, new Vector3(0.9f, 0.75f, 0.9f), 40, 90);
        // _stage = new RandomStage(cellsPrefab, transform);
        _stage = new RandomStage(cellsPrefab, transform, _cellsDimension);

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
        
        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
            new PedestrianParameters.Builder()
                .FieldAttractionBias(Random.Range(0.0f, 10.0f))
                .CrowdRepulsion(Random.Range(0.1f, 0.5f))
                .VelocityPercent(Random.Range(0.3f, 1.0f))
                .Build();
        
        var numberOfPedestrians = Random.Range(150, 600);
        _automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        StartCoroutine(nameof(RunAutomatonCoroutine));
        
        // RunAutomaton();
    }

    private IEnumerator RunAutomatonCoroutine()
    {
        // Si se detecta un json desde la ruta almacenada en la variable _pathToReadJson,
        // el escenario también se ha precargado y coinciden el escenario cargado y el que se quiere simular
        // (ponerle un id de escenario tanto al json del escenario como al del snapshot???) simular la traza.
        JsonSnapshotsList a = SaveJsonManager.LoadScoreJson(_pathToReadJson);
        a.snapshots.ForEach(list => Debug.Log("Tete" + list.timestamp));
        
        yield return _automaton.RunCoroutine();
        Statistics statistics = _automaton.computeStatistics();
        Debug.Log(statistics);
        SaveInJson();
    }
    
    private void SaveInJson()
    {
        JsonSnapshotsList list = _automaton.JsonTrace();
        // JsonSnapshotsList list = new JsonSnapshotsList();
        SaveJsonManager.SaveScoreJson(JsonScoreFilePath, list);
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

    private void UpdateParameters(float pedestriansVelocity, float multiplierSpeed)
    {
        // UpdateTimeLimit(timeLimit);
        UpdatePedestriansVelocity(pedestriansVelocity);
        UpdateMultiplierSpeed(multiplierSpeed);
        _pedestriansVelocity = pedestriansVelocity;
        _multiplierSpeed = multiplierSpeed;
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
