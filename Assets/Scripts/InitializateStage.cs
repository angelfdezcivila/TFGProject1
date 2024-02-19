using StageGenerators;
using StageWithBuilder;
using UnityEngine;

public class InitializateStage : MonoBehaviour
{
    public GameObject cellsPrefab;
    // private StageWithBuilder.StageWithBuilder _stageBuilder;
    private StageGenerators.Stage _stage;
    void Start()
    {
        // _stageBuilder = RandomStageWithBuilder.getRandomStage(cellsPrefab, transform);
        _stage = new RandomStage(cellsPrefab, transform);
    }
}
