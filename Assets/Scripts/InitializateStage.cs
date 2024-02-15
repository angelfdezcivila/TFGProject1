using System.Collections;
using System.Collections.Generic;
using StageWithCells;
using UnityEngine;

public class InitializateStage : MonoBehaviour
{
    public GameObject cellsPrefab;
    private StageWithCells.StageWithCells _stage;
    void Start()
    {
        // _stage = new RandomStageWithCells(cellsPrefab);
        _stage = RandomStageWithCells.getRandomStage(cellsPrefab, transform);
    }
}
