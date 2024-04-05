using System.Collections.Generic;
using System.IO;
using JsonDataManager.Stage;
using JsonDataManager.Trace;
using Newtonsoft.Json;
using UnityEngine;

namespace DataJson
{
    // Parece ser que el problema para redondear las coordenadas se debe a que el Jsonutility no se lleva bien con los valores decimales,
    // por lo que a la hora de pasar el float al json, le añade un montón de decimales.
    // Para arreglar esto, mirar UnityNewtonsoftJsonSerializer ,Newtonsoft y SimpleJson 
    // https://forum.unity.com/threads/jsonutility-serializes-floats-with-way-too-many-digits.541045/#post-5485749
    public static class SaveJsonManager
    {

        #region Trace

        public static void SaveTraceJson(string path, List<JsonCrowdList> snapshotsList, float cellDimension)
        {
            JsonSnapshotsList snapshotsListJson = new JsonSnapshotsList(snapshotsList, cellDimension);
            // string json = JsonUtility.ToJson(snapshotsListJson);
            string json = JsonConvert.SerializeObject(snapshotsListJson, Formatting.Indented); // Clase de la librería Newtonsoft
            // string json = JsonConvert.SerializeObject(snapshotsListJson);

            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
                Debug.Log($"Trace written to file TraceJson.json successfully.");
            }
        }
    
        public static JsonSnapshotsList LoadTraceJson(string path)
        {
            JsonSnapshotsList result = new JsonSnapshotsList();

            if (File.Exists(path))
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string json = streamReader.ReadToEnd();
                    result = JsonUtility.FromJson<JsonSnapshotsList>(json);
                }
            }
            else
            {
                Debug.Log("File doesn't exist");
            }
        
            return result;
        }

        #endregion

        #region Stage

        public static void SaveStageJson(string path, List<GatewayEntryJson> gateways, List<DomainEntryJson> domains)
        {
            JsonStage stageJson = new JsonStage(gateways, domains);
            // string json = JsonUtility.ToJson(stageJson);
            string json = JsonConvert.SerializeObject(stageJson, Formatting.Indented); // Clase de la librería Newtonsoft
            // string json = JsonConvert.SerializeObject(stageJson);

            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
                Debug.Log($"Trace written to file TraceJson.json successfully.");
            }
        }
    
        public static JsonStage LoadStageJson(string path)
        {
            JsonStage result = new JsonStage();

            if (File.Exists(path))
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string json = streamReader.ReadToEnd();
                    result = JsonUtility.FromJson<JsonStage>(json);
                }
            }
            else
            {
                Debug.Log("File doesn't exist");
            }
        
            return result;
        }

        #endregion
        
    }
}