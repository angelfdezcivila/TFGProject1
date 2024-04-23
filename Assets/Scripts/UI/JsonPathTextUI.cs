using System;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class JsonPathTextUI : MonoBehaviour
    {
        private TextMeshProUGUI _folderTMP;
        private string path;

        void OnEnable() => FileExplorerEvents.OnSelectedPathForJson += OnUpdateFolderText;
        void OnDisable() => FileExplorerEvents.OnSelectedPathForJson -= OnUpdateFolderText;
        
        void Awake()
        {
            _folderTMP = GetComponent<TextMeshProUGUI>();
        }

        private void OnUpdateFolderText(string path)
        {
            _folderTMP.text = "Ruta: \n" + path;
            this.path = path;
        }
    }
}