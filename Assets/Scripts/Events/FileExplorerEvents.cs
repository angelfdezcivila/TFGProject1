using System;
using UI;

namespace Events
{
    public static class FileExplorerEvents
    {
        public static Action<bool, TypeJsonButton> OnOpenFileExplorer; // Event to open the file explorer
        public static Action<string, TypeJsonButton, bool> OnSelectedPathForJson; // Once the select button is clicked in the file explorer
    }
}