using System;
using UnityEngine;

namespace Events
{
    public static class FileExplorerEvents
    {
        public static Action<bool> OnOpenFileExplorer; // Evento para abrir el explorador de archivos
        public static Action<string> OnSelectedPathForJson; // Una vez se pulsa al botón de seleccionar en el explorador de archivos
    }
}