using System;
using System.Collections;
using System.IO;
using Pedestrian;
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
    void Start()
    {
        Vector3 cellsDimension = new Vector3(0.4f, 0.4f, 0.4f);
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        _stage = new RandomStage(cellsPrefab, transform);
        // _stage = new RandomStage(cellsPrefab, transform, new Vector3(0.9f, 0.75f, 0.9f), 40, 90);
        
        var cellularAutomatonParameters =
            new CellularAutomatonParameters.Builder()
                .Scenario(_stage) // use this scenario
                .TimeLimit(10 * 60) // 10 minutes is time limit for simulation
                .Neighbourhood(MooreNeighbourhood.of) // use Moore's Neighbourhood for automaton
                .PedestrianReferenceVelocity(1.3f) // fastest pedestrians walk at 1.3 m/s
                .GUITimeFactor(8) // perform GUI animation x8 times faster than real time
                .Build();

        pedestrianPrefab.transform.localScale = cellsDimension;
        var automaton = new CellularAutomaton(cellularAutomatonParameters, pedestrianPrefab);
        
        Func<PedestrianParameters> pedestrianParametersSupplier = () =>
        new PedestrianParameters.Builder()
            .FieldAttractionBias(Random.Range(0.0f, 10.0f))
            .CrowdRepulsion(Random.Range(0.1f, 0.5f))
            .VelocityPercent(Random.Range(0.3f, 1.0f))
            .Build();
        
        var numberOfPedestrians = Random.Range(150, 600);
        automaton.AddPedestriansUniformly(numberOfPedestrians, pedestrianParametersSupplier);
        
        // RunAutomaton(automaton);
        // while (automaton.simulationShouldContinue())
        // {
        //     // automaton.Run();
        // }
        // // automaton.Run();
        // automaton.Run();
        StartCoroutine(nameof(RunAutomatonCoroutine), automaton);
        
        // Statistics statistics = automaton.computeStatistics();
        // Debug.Log(statistics);
        
        SaveInJson(automaton);
    }

    private IEnumerator RunAutomatonCoroutine(CellularAutomaton automaton)
    {
        yield return automaton.RunCoroutine();
        Statistics statistics = automaton.computeStatistics();
        Debug.Log(statistics);
    }
    
    private void SaveInJson(CellularAutomaton automaton)
    {
        // write trace to json file
        // var jsonTrace = automaton.jsonTrace();
        // String fileName = "data/traces/trace.json";
        // try (FileWriter fileWriter = new FileWriter(fileName)) {
        //     fileWriter.write(Jsoner.prettyPrint(jsonTrace.toJson()));
        //     fileWriter.flush();
        //     System.out.printf("Trace written to file %s successfully.%n", fileName);
        // } catch (IOException e) {
        //     e.printStackTrace();
        // }
    }

    //Este método debería de ir en un update
    private void RunAutomaton(CellularAutomaton automaton)
    {
        // Debug.Log("TimeSteps = " + timeSteps);
        // timeSteps = 0;
        // float timePerTick = automaton.parameters.TimePerTick;
        // float maximalTimeSteps = automaton.parameters.TimeLimit / timePerTick;
        // float timer = timePerTick;
        float timer = 0;
        
        while (automaton.SimulationShouldContinue())
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log("a");
                automaton.Run();
                Debug.Log("b");
                // timer += timePerTick;
                timer += automaton.TimePerTick;
            }

        }
        // Debug.Log("dadwadwa" + inScenarioPedestrians.Count + " fwadwadwa " + outOfScenarioPedestrians.Count);

    }
    
}
