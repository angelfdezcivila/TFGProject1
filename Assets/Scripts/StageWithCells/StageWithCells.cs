using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StageWithCells : MonoBehaviour
{
    public GameObject cellPivotPrefab;
    // [Range(50, 120)]
    private int _numberOfBlocks;
    public int minNumberOfBlocksInclusive = 50;
    public int maxNumberOfBlocksExclusive = 120;
    
    private Vector2 _cellsDimension;
    private LayerMask _whatIsWall;
    public int rows = 45;
    public int columns = 90;
    private List<List<NodeBasic>> _cellMatrix; // Matriz de casillas

    #region Constructors

    // private StageWithCells()
    // {
    //     _rows = 10;
    //     _columns = 10;
    //     _cellsDimension = new Vector2(1f, 1f);
    // }
    //
    // protected StageWithCells(GameObject obstaclePrefab, Vector2 cellsDimension, LayerMask whatIsWall, int rows, int columns)
    // {
    //     _cellsDimension = cellsDimension;
    //     _whatIsWall = whatIsWall;
    //     this._rows = rows;
    //     this._columns = columns;
    // }

    #endregion
    
    void Start()
    {
        _numberOfBlocks = Random.Range(minNumberOfBlocksInclusive, maxNumberOfBlocksExclusive);
        Debug.Log("rows: " + rows + " ; Columns : " + columns);
        _cellMatrix = new List<List<NodeBasic>>();
        InitializeBoard();
        // CalculateExit();
        CalculateObstacle();
        GenerateCells();
    }

    #region Private Methods

    private void InitializeBoard()
    {
        for (int i = 0; i < rows; i++) // Para cada fila
        {
            _cellMatrix.Add(new List<NodeBasic>()); // Inicializa la lista de casillas de esa fila
            for (int j = 0; j < columns; j++) // Para cada columna
            {
                GameObject cellObj = Instantiate(cellPivotPrefab, new Vector3(i, 0f, j), Quaternion.identity, this.transform); // Instancia la casilla en una posición
                _cellMatrix[i].Add(cellObj.GetComponent<NodeBasic>()); // Añade la casilla a la lista de la fila (a la matriz) de casillas
                // cellObj.name = "Cell " + "(" + i + "," + j + ")"; // Cambia el nombre del objeto
                cellObj.name = $"Cell ({i},{j})"; // Cambia el nombre del objeto
            }
        }
    }
    
    // Genera el contenido de las casillas del tablero una vez determinado su tipo de casilla
    private void GenerateCells()
    {
        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
                _cellMatrix[i][j].Build();
        }
    }

    #endregion
    

    #region Overwritable Methods

    // Calcula las casillas para las salidas
    public virtual void CalculateExit()
    {
        List<Vector2> candidates = new List<Vector2>(); // inicializamos una lista de posiciones candidatas a ser la posición de inicio de la carretera
        for (int i = 0; i < rows; i++) // Para cada fila
            for (int j = 0; j < columns; j++) // Para cada columna
            {
                if (i == 0 || i == rows - 1 || j == 0 || j == columns - 1) // El inicio de la carretera solo puede ser en posiciones de los bordes del tablero
                    candidates.Add(new Vector2(i, j));
            }
    }

    // Calcula las posiciones donde poner un obstáculo
    public virtual void CalculateObstacle()
    {
        int numberOfBlocksPlaced = 0;
        int maxTries = _numberOfBlocks * 3;
        while (numberOfBlocksPlaced < _numberOfBlocks && maxTries > 0) {
            Debug.Log("Number of blocks = " + _numberOfBlocks + " : " + numberOfBlocksPlaced + " : " + maxTries);
            
            int width = bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
            // int width = bernoulli(0.5f) ? 1 + Random.Range(0, 2) : 1 + Random.Range(0, 20);
            int height = 1 + Random.Range(0, Mathf.Max(1, rows / (int)(2 * width)));

            int row = Random.Range(0, 1 + rows - height);
            // int row = Random.Range(-_rows/2, _rows/2 - height); // Solo funciona para las filas impares
            int column = Random.Range(2, 1 + columns - width - 2);   
            
            bool shouldBePlaced = CanBePlaced(row, column, height, width);

            if (shouldBePlaced) {
                numberOfBlocksPlaced++;
            }
            maxTries -= 1;
        }
    }

    #endregion

    #region Getters and Setters

    // Cambia el tipo de una casilla del tablero en la posición 'pos'
    private void SetCellType(Vector2 pos, NodeBasic.CellTypeEnum typeEnum)
    {
        _cellMatrix[(int)pos.x][(int)pos.y].CellType = typeEnum;
    }

    // Devuelve el tipo de casilla de una posición
    public NodeBasic.CellTypeEnum GetCellType(Vector2 pos)
    {
        return _cellMatrix[(int)pos.x][(int)pos.y].CellType;
    }

    #endregion

    #region Auxiliar Methods

    private bool CanBePlaced(int row, int column, int height, int width)
    {
        List<Vector2> obstacleCandidates = new List<Vector2>();
        
        bool shouldBePlaced = true;
        int rowBorder = row>2? row - 2 : 0; //Está para que si row = 1 o row = 0, siga siendo positiva y no se salga por debajo del tablero
        int columnBorder = column>=4? column - 2 : 2; //Está para que si column = 3 o column = 2, siga teniendo un margen de 2 respecto a los bordes
        // Debug.Log("(" + row + ", " + column + ") : (" + rowBorder + ", " + columnBorder + ") : (" + height + ", " + width + ")");
        
        int heightMax = row + height + 2<rows? row + height + 2 : rows-1; //Está para controlar que la altura máxima no sobrepase el máximo a la hora de comprobar si puede ser puesto
        int widthMax = column + width + 2<columns-2? column + width + 2 : columns-1; //Está para controlar que la anchura máxima no sobrepase el máximo a la hora de comprobar si puede ser puesto
        
        int rowCounter = rowBorder;
        int columnCounter = columnBorder;
        
        while (rowCounter < heightMax && shouldBePlaced)
        {
            columnCounter = columnBorder;

            while (columnCounter < widthMax && shouldBePlaced)
            {
                Vector2 cellPosition = new Vector2(rowCounter, columnCounter);
                //Si intersecta con otro objeto, no se puede poner el obstáculo
                shouldBePlaced = GetCellType(cellPosition) == NodeBasic.CellTypeEnum.Floor;

                // Comprueba si la posicion es parte del rango o es del propio obstaculo
                bool isWall = (rowCounter >= row && columnCounter >= column) &&
                              (rowCounter < row + height && columnCounter < column + width);
                // Debug.Log("counters: " + rowCounter + ", " + columnCounter + " - " + isWall);

                if (shouldBePlaced && isWall)
                {
                    // SetCellType(cellPosition, NodeBasic.CellTypeEnum.Obstacle);
                    obstacleCandidates.Add(cellPosition);
                }

                columnCounter++;
            }

            rowCounter++;
        }

        if (shouldBePlaced)
        {
            foreach (Vector2 candidate in obstacleCandidates)
            {
                SetCellType(candidate, NodeBasic.CellTypeEnum.Obstacle);
            }
        }
        
        return shouldBePlaced;
    }
    
    // Coge las casillas alrededor de otra
    private List<Vector2> GetAroundCellsWithRange(Vector2 pos, int aroundRange)
    {
        List<Vector2> cellsAround = new List<Vector2>();
        for (int i = ((int)pos.x - aroundRange); i <= (int)pos.x + aroundRange; i++)
            for (int j = ((int)pos.y - aroundRange); j <= (int)pos.y + aroundRange; j++)
                if (i >= 0 && j >= 0 && i < rows && j < columns && (i != pos.x || j != pos.y)) // Filas y columnas de alrededor sin tener en cuenta ella misma
                    cellsAround.Add(new Vector2(i, j));
        return cellsAround;
    }
    
    private bool bernoulli(float successProbability) {
        if (!(successProbability < 0.0) && !(successProbability > 1.0)) {
            return Random.value < successProbability;
        } else {
            Debug.LogError("bernoulli: probability " + successProbability + "must be in [0.0, 1.0]");
            return false;
        }
    }
    
    #endregion

}