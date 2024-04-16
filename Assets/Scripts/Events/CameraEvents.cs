using System;
using UnityEngine;

namespace Events
{
    public static class CameraEvents
    {
        public static Action<bool> OnTogglingView; // Evento para alternar entre vista desde arriba y vista "libre"
    }
}