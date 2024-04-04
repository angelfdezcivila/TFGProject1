using System.Collections.Generic;
using System.IO;
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
        public static void SaveTraceJson(string path, List<JsonCrowdList> snapshotsList, float cellDimension)
        {
            JsonSnapshotsList battleActionsList = new JsonSnapshotsList(snapshotsList, cellDimension);
            // string json = JsonUtility.ToJson(battleActionsList);
            string json = JsonConvert.SerializeObject(battleActionsList, Formatting.Indented); // Clase de la librería Newtonsoft
            // string json = JsonConvert.SerializeObject(battleActionsList);

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
    }
}