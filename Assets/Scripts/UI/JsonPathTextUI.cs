using System;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class JsonPathTextUI : MonoBehaviour
    {
        public TypeJsonButton type;
        private TextMeshProUGUI _folderTMP;
        private string path;

        void OnEnable() => FileExplorerEvents.OnSelectedPathForJson += OnUpdateFolderText;
        void OnDisable() => FileExplorerEvents.OnSelectedPathForJson -= OnUpdateFolderText;
        
        void Awake()
        {
            _folderTMP = GetComponent<TextMeshProUGUI>();
        }

        // La variable de savingJson no influye en este caso, ya que lo unico que quiero es modificar el texto de la UI en caso de que sea Trace o Stage
        private void OnUpdateFolderText(string path, TypeJsonButton type, bool savingJson)
        {
            OnUpdateFolderText(path, type);
        }
        
        private void OnUpdateFolderText(string path, TypeJsonButton type)
        {
            if (this.type == type)
            {
                _folderTMP.text = "Ruta: \n" + path;
                this.path = path;
            }
        }
    }
}