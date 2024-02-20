using StageGenerator;
using StageWithBuilder;
using UnityEngine;

public class InitializateStage : MonoBehaviour
{
    public GameObject cellsPrefab;
    // private StageWithBuilder.StageWithBuilder _stageBuilder;
    private StageGenerator.Stage _stage;
    void Start()
    {
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        _stage = new RandomStage(cellsPrefab, transform);
        // _stage = new RandomStage(cellsPrefab, transform, new Vector3(0.9f, 0.75f, 0.9f), 40, 90);
        
        var cellularAutomatonParameters =
            new CellularAutomatonParameters.Builder()
                .Scenario(_stage) // use this scenario
                .TimeLimit(10 * 60) // 10 minutes is time limit for simulation
                .Neighbourhood(MooreNeighbourhood.of) // use Moore's Neighbourhood for automaton
                .PedestrianReferenceVelocity(1.3) // fastest pedestrians walk at 1.3 m/s
                .GUITimeFactor(8) // perform GUI animation x8 times faster than real time
                .Build();
        
        // var automaton = new CellularAutomaton(cellularAutomatonParameters);

    }
}
