using System;
using System.Collections;
using System.IO;
using Cellular;
using DataJson;
using Pedestrians;
using StageGenerator;
using TestingStageWithBuilder;
using UnityEngine;
using Random = UnityEngine.Random;

public class InitializateStage : MonoBehaviour
{
    public GameObject cellsPrefab;
    public GameObject pedestrianPrefab;
    // private StageWithBuilder.StageWithBuilder _stageBuilder;
    private StageGenerator.Stage _stage;
    private CellularAutomaton _automaton;
    private string JsonScoreFilePath => $"{Application.persistentDataPath}/TraceJson.json";
    void Start()
    {
        Vector3 cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        // _stage = new RandomStage(cellsPrefab, transform, new Vector3(0.9f, 0.75f, 0.9f), 40, 90);
        _stage = new RandomStage(cellsPrefab, transform);

        var cellularAutomatonParameters =
            new CellularAutomatonParameters.Builder()
                .Scenario(_stage) // use this scenario
                .TimeLimit(10 * 60) // 10 minutes is time limit for simulation
                .Neighbourhood(MooreNeighbourhood.of) // use Moore's Neighbourhood for automaton
                .PedestrianReferenceVelocity(1.3f) // fastest pedestrians walk at 1.3 m/s
                .GUITimeFactor(8) // perform GUI animation x8 times faster than real time
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
    
    private void Update()
    {
        // if(!_automaton.SimulationShouldContinue())
            // CancelInvoke();
    }
    

    private IEnumerator RunAutomatonCoroutine()
    {
        yield return _automaton.RunCoroutine();
        Statistics statistics = _automaton.computeStatistics();
        Debug.Log(statistics);
        SaveInJson();
    }
    
    private void SaveInJson()
    {
        JsonSnapshotsList list = new JsonSnapshotsList();
        SaveJsonManager.SaveScoreJson(JsonScoreFilePath, list);
    }

    //Este método debería de ir en un update

    private void RunAutomaton()
    {
        InitializeFloor();
        float timer = _automaton.RealTimePerTick;
        while (_automaton.SimulationShouldContinue())
        {
            Debug.Log(timer);
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                _automaton.RunStep();
                timer += _automaton.RealTimePerTick;
            }
        }
        
        // InvokeRepeating(nameof(RunStep), _automaton.RealTimePerTick, _automaton.RealTimePerTick);

        
        _automaton.Paint();
    }

    private void InitializeFloor()
    {
        _automaton.InitializeStaticFloor();
      
        Debug.Log("Real time per tick" + _automaton.RealTimePerTick);
    
        _automaton.Paint();
    }

    private void RunStep()
    {
        _automaton.RunStep();
    }
}
