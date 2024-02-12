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

                _grid[col, row] = new NodeBasic(col, row, nodePosition, isWall);
            }
        }
    }

    private void CreateStage()
    {
        // RandomStage.CreateRandomStage(_rows, _columns, _nodeSize);
        Stage stage = new RandomStage(obstaclePrefab, _nodeSize, _whatIsWall, _rows, _columns);
    }

    public NodeBasic GetNodeFromPosition(Vector3 pos)
    {
        if (_grid == null)
        {
            Debug.LogWarning("[GridBehavior.cs]:: Grid has null reference when getting a Node from position");
            return null;
        }

        int nodeColumnIndex = Mathf.FloorToInt((_gridSize.x/2f + pos.x) / _nodeSize.x);
        int nodeRowIndex = Mathf.FloorToInt((_gridSize.y/2f + pos.z) / _nodeSize.y);

        nodeColumnIndex = Mathf.Clamp(nodeColumnIndex, 0, _columns - 1);
        nodeRowIndex = Mathf.Clamp(nodeRowIndex, 0, _rows - 1);

        return _grid[nodeColumnIndex, nodeRowIndex];
    }

    public List<NodeBasic> GetNeighboursOfNode(NodeBasic node)
    {
        List<NodeBasic> _neighbours = new List<NodeBasic>();

        if (node.ColumnIndex + 1 < _columns && !_grid[node.ColumnIndex + 1, node.RowIndex].isWall) // Derecha
            _neighbours.Add(_grid[node.ColumnIndex + 1, node.RowIndex]);
        
        if (node.ColumnIndex - 1 > 0 && !_grid[node.ColumnIndex - 1, node.RowIndex].isWall) // Izquierda
            _neighbours.Add(_grid[node.ColumnIndex - 1, node.RowIndex]);

        if (node.RowIndex + 1 < _rows && !_grid[node.ColumnIndex, node.RowIndex + 1].isWall) // Arriba
            _neighbours.Add(_grid[node.ColumnIndex, node.RowIndex + 1]);

        if (node.RowIndex - 1 > 0 && !_grid[node.ColumnIndex, node.RowIndex - 1].isWall) // Abajo
            _neighbours.Add(_grid[node.ColumnIndex, node.RowIndex - 1]);


        return _neighbours;
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
            
                if (node.isWall)
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