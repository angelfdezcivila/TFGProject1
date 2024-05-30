using System;

namespace Events
{
    public static class CameraEvents
    {
        public static Action<bool> OnTogglingView; // Event to toggle between top view and “free” view
    }
}