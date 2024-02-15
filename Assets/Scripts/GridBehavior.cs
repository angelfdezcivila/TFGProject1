using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    public GameObject obstaclePrefab;
    
    [Header("Parameters")] 
    [SerializeField] private Vector2 _nodeSize;
    [SerializeField] private Vector2 _gridSize;
    [SerializeField] private LayerMask _whatIsWall;
    
    [Header("Grid")]
    [Tooltip("[Col, Row]")]
    private NodeBasic[,] _grid;

    public NodeBasic StartNode;
    public NodeBasic EndNode;
    public List<NodeBasic> FinalPath;
    
    private int _columns;
    private int _rows;
    
    // Para que el numero de filas y columnas sea 45x90 con unas celdas de 0.4m, el tamaño del grid debería de ser de 36x18m
    
    //Valores de prueba: 0.4x0.4m tamaño del nodo, y 30x20m el grid

    // public int Columns => _columns;
    // public int Rows => _rows;
    
    public int Columns
    {
        get { return _columns; }
        private set { _columns = value; }
    }

    public int Rows
    {
        get { return _rows; }
        private set { _rows = value; }
    }

    public GridBehavior(Vector2 nodeSize, Vector2 gridSize, LayerMask whatIsWall)
    {
        _nodeSize = nodeSize;
        _gridSize = gridSize;
        _whatIsWall = whatIsWall;
    }

    void Awake()
    {
        CreateGrid();
        CreateStage();
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
            CreateGrid();
    }

    private void CreateGrid()
    {
        _columns = Mathf.FloorToInt(_gridSize.x / _nodeSize.x);
        _rows = Mathf.FloorToInt(_gridSize.y / _nodeSize.y);
        
        Vector3 startingPosition = transform.position - new Vector3(_gridSize.x, 0f, _gridSize.y)/2f;

        _grid = new NodeBasic[_columns, _rows];

        for (int col = 0; col < _columns; col++)
        {
            for (int row = 0; row < _rows; row++)
            {
                Vector3 nodePosition = startingPosition +
                                       new Vector3(col * _nodeSize.x, 0f, row * _nodeSize.y);
                nodePosition.x += _nodeSize.x / 2f;
                nodePosition.z += _nodeSize.y / 2f;

                bool isWall = Physics.CheckBox(nodePosition, 
                    new Vector3(_nodeSize.x, 1f, _nodeSize.y) / 2, 
                    Quaternion.identity, _whatIsWall);

                NodeBasic.CellTypeEnum cellType = isWall? NodeBasic.CellTypeEnum.Obstacle : NodeBasic.CellTypeEnum.Floor;

                _grid[col, row] = new NodeBasic(col, row, nodePosition, cellType);
            }
        }
    }

    private void CreateStage()
    {
        // RandomStage.CreateRandomStage(_rows, _columns, _nodeSize);
        Stage stage = new RandomStage(obstaclePrefab, _nodeSize, _whatIsWall, _rows, _columns);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridSize.x, 2, _gridSize.y));
        
        if (_grid == null)
            return;
        
        for (int col = 0; col < _columns; col++)
        {
            for (int row = 0; row < _rows; row++)
            {
                NodeBasic node = _grid[col, row];
                
                Gizmos.color = Color.cyan;
                if (FinalPath.Contains(node))
                    Gizmos.color = Color.red;
            
                // if (node.isWall)
                if (node.CellType == NodeBasic.CellTypeEnum.Obstacle)
                    Gizmos.color = Color.black;
                
                if(StartNode == node || EndNode == node)
                    Gizmos.color = Color.green;
                
                Gizmos.DrawCube(node.NodePosition, new Vector3(_nodeSize.x, 0.5f, _nodeSize.y));
            }
        }
        
        // foreach (NodeBasic node in _grid)
        // {
        //     Gizmos.color = Color.cyan;
        //     if (FinalPath.Contains(node))
        //         Gizmos.color = Color.red;
        //     
        //     if (node.isWall)
        //         Gizmos.color = Color.black;
        //     
        //     Gizmos.DrawCube(node.NodePosition, new Vector3(_nodeSize.x, 0.5f, _nodeSize.y));
        // }
    }
}