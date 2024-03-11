using System;
using Events;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SaveJsonPathText : MonoBehaviour
    {
        private TextMeshProUGUI _folderTMP;

        void Awake()
        {
            _folderTMP = GetComponent<TextMeshProUGUI>();
            FileExplorerEvents.OnSelectedPathForJson += OnUpdateFolderText;
        }
        
        void OnDestroy()
        {
            FileExplorerEvents.OnSelectedPathForJson -= OnUpdateFolderText;
        }

        private void OnUpdateFolderText(string path)
        {
            _folderTMP.text = "Ruta: \n" + path;
        }
    }
}