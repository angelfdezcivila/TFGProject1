using System;
using Cellular;
using UnityEngine;

namespace StageGenerator
{
    [Serializable]
    public class Cell : MonoBehaviour
    {
        // Datos del nodo
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject obstaclePrefab;
        [SerializeField] private GameObject exitPrefab;

        [SerializeField]
        private int _rowIndex;
        [SerializeField]
        private int _columnIndex;
        [SerializeField]
        private CellTypeEnum _cellType;
    
        protected internal CellTypeEnum CellType
        {
            get => _cellType;
            set => _cellType = value;
        }

        public enum CellTypeEnum
        {
            Floor, Obstacle, Exit
        }

        public void SetCellPosition(int row, int column)
        {
            _rowIndex = row;
            _columnIndex = column;
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

        public void Destroy()
        {
            
        }
    
        public float DistanceTo(int row, int column) {
            return Mathf.Sqrt(distanceSqr(row, column));
        }
        
        public float DistanceTo(Location location) {
            return DistanceTo(location.Row, location.Column);
        }
        
        public int distanceSqr(int row, int column) {
            var dh = Mathf.Max(_columnIndex - column, Mathf.Max(0, column - _columnIndex));
            var dv = Mathf.Max(_rowIndex - row, Mathf.Max(0, row - _rowIndex));
            return dh*dh + dv*dv;
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
}
