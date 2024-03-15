using System;
using Cellular;
using StageGenerator;

namespace FloorFields
{
    public abstract class StaticFloorField : FloorField {
        protected readonly double[,] staticFloorField;
        protected readonly Stage scenario;

        protected StaticFloorField(double[,] staticFloorField, Stage scenario) {
            this.staticFloorField = staticFloorField;
            this.scenario = scenario;
        }

        public abstract void initialize();

        public int getRows() {
            return scenario.Rows;
        }

        public int getColumns() {
            return scenario.Columns;
        }

        public double getField(int row, int column) {
            if (row < 0 || row >= getRows())
                throw new ArgumentException("getField: invalid row");
            if (column < 0 || column >= getColumns())
                throw new ArgumentException("getField: invalid column");
            return staticFloorField[row,column];
        }

        public double getField(Location location) {
            return getField(location.Row, location.Column);
        }
    }
}