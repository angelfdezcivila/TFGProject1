using System;
using UnityEngine;

namespace Events
{
    public static class SimulationEvents
    {
        public static Action<float, float> OnUpdateStageParameters;
        // public static Action<int, Vector3> OnUpdateStageParameters;
        public static Action OnPlaySimulation;
    }
}