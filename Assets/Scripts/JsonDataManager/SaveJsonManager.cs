using System.Collections.Generic;
using System.IO;
using JsonDataManager.Stage;
using JsonDataManager.Stage.ShapeType;
using JsonDataManager.Trace;
using Newtonsoft.Json;
using UnityEngine;

namespace JsonDataManager
{
    // TODO: tener en cuenta este comentario para la memoria
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
            SaveJson(path, snapshotsListJson);
        }

        public static void SaveTraceJson(string path, JsonSnapshotsList snapshotsList)
        {
            SaveJson(path, snapshotsList);
        }
    
        public static JsonSnapshotsList LoadTraceJson(string path)
        {
            JsonSnapshotsList result = new JsonSnapshotsList();
            return LoadJson(path, result);
        }

        #endregion

        #region Stage

        public static void SaveStageJson(string path, List<GatewayEntryJson> gateways, List<DomainEntryJson> domains)
        {
            JsonStage stageJson = new JsonStage(gateways, domains);
            SaveJson(path, stageJson);
        }
        
        public static void SaveStageJson(string path, JsonStage stageJson)
        {
            SaveJson(path, stageJson);
        }
    
        public static JsonStage LoadStageJson(string path)
        {
            // JsonStage jsonStage = null; // It doesn't matter if it is null, this constructor will still be called
            JsonStage jsonStage = new JsonStage();
            jsonStage = LoadJson(path, jsonStage);
            foreach (DomainEntryJson domain in jsonStage.domains)
            {
                foreach (ObstacleEntryJson obstacle in domain.obstacles)
                {
                    // Debug.Log("Shape before: " + obstacle.shape.ShapeType.NameRepresentation + " ;Nombre:" + obstacle.name);
                    obstacle.shape.ShapeType = UpdateShapeType(obstacle.shape.type, obstacle.shape);
                    // Debug.Log("Shape after: " + obstacle.shape.ShapeType.NameRepresentation + " ;Nombre:" + obstacle.name);
                }
                foreach (AccessEntryJson access in domain.accesses)
                {
                    access.shape.ShapeType = UpdateShapeType(access.shape.type, access.shape);
                }
            }
            
            return jsonStage;
        }

        #endregion

        private static ShapeType UpdateShapeType(string type, ShapeJson shape)
        {
            ShapeType shapeType = new RectangleJson();
            CoordinatesStageJson bottomLeft = shape.bottomLeft;
            float width = shape.height;
            float height = shape.width;

            CoordinatesStageJson center = shape.center;
            float radius = shape.radius;

            switch (type.ToUpper())
            {
                case "RECTANGLE" : 
                    shapeType = new RectangleJson(bottomLeft, width, height);
                    break;
                // case "CIRCLE" :
                //     shapeType = new CircleJson(center, radius);
                //     break;
            }

            return shapeType;
        }
        
        private static void SaveJson<T>(string path, T jsonObject)
        {
            // string json = JsonUtility.ToJson(jsonObject);
            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented, new JsonSerializerSettings()  // Class of the library Newtonsoft
            { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            // string json = JsonConvert.SerializeObject(jsonObject); // Class of the library Newtonsoft

            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
                string[] words = path.Split(' ');
                Debug.Log($"Trace written to file {words[words.Length-1]} successfully.");
            }
        }
        
        private static T LoadJson<T>(string path, T result)
        {
            if (File.Exists(path))
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string json = streamReader.ReadToEnd();
                    result = JsonUtility.FromJson<T>(json);
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