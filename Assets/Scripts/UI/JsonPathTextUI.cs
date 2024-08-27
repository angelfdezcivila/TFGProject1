using Events;
using TMPro;
using UnityEngine;

namespace UI
{
    public class JsonPathTextUI : MonoBehaviour
    {
        public TypeJsonButton type;
        private TextMeshProUGUI _folderTMP;
        private string _path;

        void OnEnable() => FileExplorerEvents.OnSelectedPathForJson += OnUpdateFolderText;
        void OnDisable() => FileExplorerEvents.OnSelectedPathForJson -= OnUpdateFolderText;
        
        void Awake()
        {
            _folderTMP = GetComponent<TextMeshProUGUI>();
        }

        // The 'savingJson' variable has no influence in this case, since the only thing I want is to modify the UI text in case it is Trace or Stage.
        private void OnUpdateFolderText(string path, TypeJsonButton type, bool savingJson)
        {
            OnUpdateFolderText(path, type);
        }
        
        private void OnUpdateFolderText(string path, TypeJsonButton type)
        {
            if (this.type == type)
            {
                _folderTMP.text = "Path: \n" + path;
                this._path = path;
            }
        }
    }
}