using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public static class FileExplorerEvents
    {
        public static Action<bool, TypeJsonButton> OnOpenFileExplorer; // Evento para abrir el explorador de archivos
        public static Action<string, TypeJsonButton, bool> OnSelectedPathForJson; // Una vez se pulsa al botón de seleccionar en el explorador de archivos
    }
}