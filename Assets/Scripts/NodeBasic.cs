using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeBasic
{
    // Datos del nodo
    public int ColumnIndex;
    public int RowIndex;
    public Vector3 NodePosition;

    // Datos de A*
    public int ig; // la i viene de internal
    public int ih;

    public int iF
    {
        get { return ig + ih; }
    }

    public NodeBasic ParentNode;

    // Parámetros
    public bool isWall;

    public NodeBasic(int column, int row, Vector3 pos, bool isWall)
    {
        ColumnIndex = column;
        RowIndex = row;
        NodePosition = pos;
        this.isWall = isWall;
    }
    
    public override bool Equals(System.Object obj)
    {
        NodeBasic node = obj as NodeBasic;
        if (node == null)
            return false;
    
        if (node.ColumnIndex == this.ColumnIndex && node.RowIndex == this.RowIndex)
            return true;
    
        return false;
    }
    
}
