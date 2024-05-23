using System;
using Cellular;
using UnityEngine;

namespace Pedestrians
{
    public class PedestrianFactory
    {
        private readonly CellularAutomaton _automaton;
        private readonly GameObject _pedestrianPrefab;

        public PedestrianFactory(CellularAutomaton automaton, GameObject pedestrianPrefab) {
            this._automaton = automaton;
            this._pedestrianPrefab = pedestrianPrefab;
        }

        public Pedestrian GetInstance(int row, int column, PedestrianParameters parameters) {
            if (row < 0 || row >= _automaton.Rows) throw new ArgumentOutOfRangeException("getInstance: invalid row");
            if (column < 0 || column >= _automaton.Columns) throw new ArgumentOutOfRangeException("getInstance: invalid column");
            GameObject pedestrianObject = GameObject.Instantiate(_pedestrianPrefab, _automaton.PedestrianContainer.transform);
            // pedestrianObject.transform.localScale = automaton.Stage.CellsDimension - Vector3.one*0.1f;
            pedestrianObject.transform.localScale = _automaton.Stage.CellsDimension - _automaton.Stage.CellsDimension*0.2f; // with this substract we secure that unless CellsDimension is 0, it won't be 0
            
            
            Pedestrian pedestrian = pedestrianObject.GetComponent<Pedestrian>();
            pedestrian.Initialize(row, column, parameters, _automaton);
            return pedestrian;
        }

        public Pedestrian GetInstance(Location location, PedestrianParameters parameters) {
            return GetInstance(location.Row, location.Column, parameters);
        }
    }
}