using System;

namespace Pedestrian
{
    public class PedestrianFactory
    {
        private readonly CellularAutomaton automaton;

        public PedestrianFactory(CellularAutomaton automaton) {
            this.automaton = automaton;
        }

        public Pedestrian GetInstance(int row, int column, PedestrianParameters parameters) {
            if (row < 0 || row >= automaton.Rows) throw new ArgumentOutOfRangeException("getInstance: invalid row");
            if (column < 0 || column >= automaton.Columns) throw new ArgumentOutOfRangeException("getInstance: invalid column");
            return new Pedestrian(row, column, parameters, automaton);
        }

        public Pedestrian GetInstance(Location location, PedestrianParameters parameters) {
            return GetInstance(location.row, location.column, parameters);
        }
    }
}