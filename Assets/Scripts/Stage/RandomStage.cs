using System;
using UnityEngine;

public sealed class RandomStage : Stage
{
    private Vector2 _nodeSize = new Vector2(0.4f, 0.4f);
    private int _columns = 45;
    private int _rows = 90;

    public RandomStage(GameObject obstaclePrefab, Vector2 cellsDimension, LayerMask whatIsWall, int rows, int columns)
        : base(obstaclePrefab, cellsDimension, whatIsWall, rows, columns)
    {
        CreateStage();
    }
    
    // public override void CreateStage()
    // {
        // Instantiate(_obstaclePrefab, new Vector3(1, 1, 1), Quaternion.identity);
    // }

}