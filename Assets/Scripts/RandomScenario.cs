using System;
using UnityEngine;

public class RandomScenario : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private GridBehavior _grid;

    private int _rows, _columns;

    void Start()
    {
        CreateScenario();
    }

    private void CreateScenario()
    {
        _rows = _grid.Rows;
        _columns = _grid.Columns;
    }
}