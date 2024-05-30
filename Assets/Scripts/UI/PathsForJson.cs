using Events;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PathsForJson : MonoBehaviour
    {
        public static string SaveTraceJson => _saveTraceJson;
        public static string LoadTraceJson => _loadTraceJson;
        public static string SaveStageJson => _saveStageJson;
        public static string LoadStageJson => _loadStageJson;

        private static string _saveTraceJson;
        private static string _loadTraceJson;
        private static string _saveStageJson;
        private static string _loadStageJson;

        private static string JsonTraceInitialFilePath => $"{Application.persistentDataPath}/" + _traceFileName;
        private static string JsonStageInitialFilePath => $"{Application.persistentDataPath}/" + _stageFileName;
        private static string _traceFileName = "TraceJson.json"; // It is possible that in the future we want to be able to change it, so for now I have left it as a variable.
        private static string _stageFileName = "StageJson.json"; // It is possible that in the future we want to be able to change it, so for now I have left it as a variable.

        private void OnEnable()
        {
            FileExplorerEvents.OnOpenFileExplorer += OpenFileExplorer;
            FileExplorerEvents.OnSelectedPathForJson += UpdatePathJson;
        }

        private void OnDisable()
        {
            FileExplorerEvents.OnOpenFileExplorer -= OpenFileExplorer;
            FileExplorerEvents.OnSelectedPathForJson -= UpdatePathJson;
        }

        private void Start()
        {
            _saveTraceJson = JsonTraceInitialFilePath;
            _loadTraceJson = JsonTraceInitialFilePath;
            _saveStageJson = JsonStageInitialFilePath;
            _loadStageJson = JsonStageInitialFilePath;
        }


        private void OpenFileExplorer(bool savingJson, TypeJsonButton type)
        {
            if (savingJson)
            {
                FileBrowser.ShowLoadDialog( ( paths ) =>
                    {
                        string savingPath = "";
                        switch (type)
                        {
                            case TypeJsonButton.Trace : 
                                _saveTraceJson = paths[0] + "/" + _traceFileName;
                                savingPath = _saveTraceJson;
                                break;
                            case TypeJsonButton.Stage :
                                _saveStageJson = paths[0] + "/" + _stageFileName;
                                savingPath = _saveStageJson;
                                break;
                        }
                        // saveTraceJson = paths[0] + "/" + _traceFileName;
                        FileExplorerEvents.OnSelectedPathForJson?.Invoke(savingPath, type, savingJson);
                    },
                    () => { Debug.Log("Canceled"); },
                    // FileBrowser.PickMode.Folders, false, saveTraceJson, null, "Select Folder", "Select" );
                    FileBrowser.PickMode.Folders, false, 
                    type==TypeJsonButton.Trace ? _saveTraceJson : _saveStageJson, null, "Select Folder", "Select" );
            }
            else
            {
                FileBrowser.ShowLoadDialog( ( paths ) =>
                    {
                        string loadingPath = "";
                        switch (type)
                        {
                            case TypeJsonButton.Trace : 
                                _loadTraceJson = paths[0];
                                loadingPath = _loadTraceJson;
                                break;
                            case TypeJsonButton.Stage :
                                _loadStageJson = paths[0];
                                loadingPath = _loadStageJson;
                                break;
                        }
                        // loadTraceJson = paths[0];
                        FileExplorerEvents.OnSelectedPathForJson?.Invoke(loadingPath, type, savingJson);
                    },
                    () => { Debug.Log("Canceled"); },
                    // FileBrowser.PickMode.Files, false, loadTraceJson, null, "Select File", "Select" );
                    FileBrowser.PickMode.Files, false,
                    type==TypeJsonButton.Trace ? _loadTraceJson : _loadStageJson, null, "Select File", "Select" );
            }

        }
    
        private void UpdatePathJson(string path, TypeJsonButton type, bool savingJson)
        {
            switch (type)
            {
                case TypeJsonButton.Trace :
                    if (savingJson)
                        _saveTraceJson = path;
                    else
                        _loadTraceJson = path;
                    break;
                case TypeJsonButton.Stage :
                    if (savingJson)
                        _saveStageJson = path;
                    else
                        _loadStageJson = path;
                    break;
            }
        }
    }
}