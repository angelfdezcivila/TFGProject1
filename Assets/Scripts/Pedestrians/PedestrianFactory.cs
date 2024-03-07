using System;
using Cellular;
using UnityEngine;

namespace Pedestrians
{
    public class PedestrianFactory
    {
        private readonly CellularAutomaton automaton;
        private readonly GameObject _pedestrianPrefab;

        public PedestrianFactory(CellularAutomaton automaton, GameObject pedestrianPrefab) {
            this.automaton = automaton;
            this._pedestrianPrefab = pedestrianPrefab;
        }

        public Pedestrian GetInstance(int row, int column, PedestrianParameters parameters) {
            if (row < 0 || row >= automaton.Rows) throw new ArgumentOutOfRangeException("getInstance: invalid row");
            if (column < 0 || column >= automaton.Columns) throw new ArgumentOutOfRangeException("getInstance: invalid column");
            GameObject pedestrianObject = GameObject.Instantiate(_pedestrianPrefab, automaton.PedestrianContainer.transform);
            pedestrianObject.transform.localScale = automaton.GetScenario().CellsDimension - Vector3.one*0.1f;
            Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();
            pedestrian.Initialize(row, column, parameters, automaton);
            return pedestrian;
        }

        public Pedestrian GetInstance(Location location, PedestrianParameters parameters) {
            return GetInstance(location.Row, location.Column, parameters);
        }
    }
}