using System.IO;
using Newtonsoft.Json;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;

namespace DataJson
{

    public static class SaveNewtonsoftJsonManager
    {
        public static void SaveScoreJson(string path, JsonSnapshotsList snapshotsList, float cellDimension)
        {
            JsonSnapshotsList battleActionsList = new JsonSnapshotsList(snapshotsList, cellDimension);
            // string json = JsonConvert.SerializeObject(battleActionsList);
            string json = JsonConvert.SerializeObject(battleActionsList, Formatting.Indented);

            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
                Debug.Log($"Trace written to file TraceJson.json successfully.");
            }
        }
    
        public static JsonSnapshotsList LoadScoreJson(string path)
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