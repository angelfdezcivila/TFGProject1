using System;
using UnityEngine;

namespace Events
{
    public static class SimulationEvents
    {
        public static Action<float, float, float> OnInitializeStageParameters;
        // public static Action<int, Vector3> OnUpdateStageParameters;
        public static Action<bool> OnPlaySimulation;
        public static Action<float> OnUpdateSimulationSpeed;
        public static Action OnGenerateRandomStage;
    }
}