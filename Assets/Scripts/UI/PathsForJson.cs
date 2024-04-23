using Events;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PathsForJson : MonoBehaviour
    {
        public static string SaveTraceJson => saveTraceJson;
        public static string LoadTraceJson => loadTraceJson;
        public static string SaveStageJson => saveStageJson;
        public static string LoadStageJson => loadStageJson;

        private static string saveTraceJson;
        private static string loadTraceJson;
        private static string saveStageJson;
        private static string loadStageJson;

        private static string JsonTraceInitialFilePath => $"{Application.persistentDataPath}/" + _traceFileName;
        private static string JsonStageInitialFilePath => $"{Application.persistentDataPath}/" + _stageFileName;
        private static string _traceFileName = "TraceJson.json"; // Es posible que se quiera cambiarla, por lo que por ahora lo he dejado como variable
        private static string _stageFileName = "StageJson.json"; // Es posible que se quiera cambiarla, por lo que por ahora lo he dejado como variable

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
            saveTraceJson = JsonTraceInitialFilePath;
            loadTraceJson = JsonTraceInitialFilePath;
            saveStageJson = JsonStageInitialFilePath;
            loadStageJson = JsonStageInitialFilePath;
        }


        // TODO: hay que cambiar las variables de las rutas de los json para que sean dos rutas diferentes
        private void OpenFileExplorer(bool savingJson, TypeJsonButton type)
            // private void OpenFileExplorer(bool savingJson)
        {
            if (savingJson)
            {
                FileBrowser.ShowLoadDialog( ( paths ) =>
                    {
                        string savingPath = "";
                        switch (type)
                        {
                            case TypeJsonButton.Trace : 
                                saveTraceJson = paths[0] + "/" + _traceFileName;
                                savingPath = saveTraceJson;
                                break;
                            case TypeJsonButton.Stage :
                                saveStageJson = paths[0] + "/" + _stageFileName;
                                savingPath = saveStageJson;
                                break;
                        }
                        // saveTraceJson = paths[0] + "/" + _traceFileName;
                        FileExplorerEvents.OnSelectedPathForJson?.Invoke(savingPath, type, savingJson);
                    },
                    () => { Debug.Log("Canceled"); },
                    // FileBrowser.PickMode.Folders, false, saveTraceJson, null, "Select Folder", "Select" );
                    FileBrowser.PickMode.Folders, false, 
                    type==TypeJsonButton.Trace ? saveTraceJson : saveStageJson, null, "Select Folder", "Select" );
            }
            else
            {
                FileBrowser.ShowLoadDialog( ( paths ) =>
                    {
                        string loadingPath = "";
                        switch (type)
                        {
                            case TypeJsonButton.Trace : 
                                loadTraceJson = paths[0];
                                loadingPath = loadTraceJson;
                                break;
                            case TypeJsonButton.Stage :
                                loadStageJson = paths[0];
                                loadingPath = loadStageJson;
                                break;
                        }
                        // loadTraceJson = paths[0];
                        FileExplorerEvents.OnSelectedPathForJson?.Invoke(loadingPath, type, savingJson);
                    },
                    () => { Debug.Log("Canceled"); },
                    // FileBrowser.PickMode.Files, false, loadTraceJson, null, "Select File", "Select" );
                    FileBrowser.PickMode.Files, false,
                    type==TypeJsonButton.Trace ? loadTraceJson : loadStageJson, null, "Select File", "Select" );
            }

        }
    
        private void UpdatePathJson(string path, TypeJsonButton type, bool savingJson)
        {
            switch (type)
            {
                case TypeJsonButton.Trace :
                    if (savingJson)
                        saveTraceJson = path;
                    else
                        loadTraceJson = path;
                    break;
                case TypeJsonButton.Stage :
                    if (savingJson)
                        saveStageJson = path;
                    else
                        loadStageJson = path;
                    break;
            }
        }
    }
}