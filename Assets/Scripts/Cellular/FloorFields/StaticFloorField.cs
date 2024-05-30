using System;
using StageGenerator;

namespace Cellular.FloorFields
{
    public abstract class StaticFloorField : IFloorField {
        protected readonly double[,] staticFloorField;
        protected readonly Stage scenario;

        protected StaticFloorField(double[,] staticFloorField, Stage scenario) {
            this.staticFloorField = staticFloorField;
            this.scenario = scenario;
        }

        public abstract void Initialize();

        public int GetRows() {
            return scenario.Rows;
        }

        public int GetColumns() {
            return scenario.Columns;
        }

        public double GetField(int row, int column) {
            if (row < 0 || row >= GetRows())
                throw new ArgumentException("GetField: invalid row");
            if (column < 0 || column >= GetColumns())
                throw new ArgumentException("GetField: invalid column");
            return staticFloorField[row,column];
        }

        public double GetField(Location location) {
            return GetField(location.Row, location.Column);
        }
    }
}