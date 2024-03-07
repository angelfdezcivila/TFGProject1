using System.IO;
using UnityEngine;

namespace DataJson
{
    public static class SaveJsonManager
    {
        public static void SaveScoreJson(string path, JsonSnapshotsList snapshotsList)
        {
            JsonSnapshotsList battleActionsList = new JsonSnapshotsList(snapshotsList);
            string json = JsonUtility.ToJson(battleActionsList);

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
        
            return result;
        }
    }
}