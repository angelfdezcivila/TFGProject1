using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeBasic : MonoBehaviour
{
    // Datos del nodo
    public int ColumnIndex;
    public int RowIndex;
    public Vector3 NodePosition;
    public GameObject floorPrefab;
    public GameObject obstaclePrefab;
    public GameObject exitPrefab;

    // Parámetros
    // public bool isWall;
    public CellTypeEnum CellType { get; set; }
    public enum CellTypeEnum
    {
        Floor, Obstacle, Exit
    }

    public NodeBasic(int column, int row, Vector3 pos, CellTypeEnum cellTypeEnum)
    {
        ColumnIndex = column;
        RowIndex = row;
        NodePosition = pos;
        this.CellType = cellTypeEnum;
    }


    public void Build()
    {
        switch (CellType)
        {
            case CellTypeEnum.Floor:
                Instantiate(floorPrefab, transform.position, Quaternion.identity, this.transform);
                break;
            case CellTypeEnum.Obstacle:
                Instantiate(obstaclePrefab, transform.position, Quaternion.identity, this.transform);
                break;
            case CellTypeEnum.Exit:
                Instantiate(exitPrefab, transform.position, Quaternion.identity, this.transform);
                break;
        }
    }
    
    // public override bool Equals(System.Object obj)
    // {
    //     NodeBasic node = obj as NodeBasic;
    //     if (node == null)
    //         return false;
    //
    //     if (node.ColumnIndex == this.ColumnIndex && node.RowIndex == this.RowIndex)
    //         return true;
    //
    //     return false;
    // }
    
}
