using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SerializedList<T>
    {
        public List<T> list;
        public SerializedList(List<T> l)
        {
            list = l;
        }
    }

    public static class DataManager
    {
        private static string basePath = "Assets/Resources/";
        private static string dataPath = "Data/";
        private static string prefabPath = "Prefabs/";


        // Json 데이터 구조 틀을 만들기 위해 활용
        public static void Serialize<T>(string name, List<T> objects)
        {
            string json = JsonUtility.ToJson(new SerializedList<T>(objects));
            string fileName = typeof(T).Name + ".json";

            Debug.Log(json);
            File.WriteAllText(basePath + dataPath + fileName, json);
        }

        public static List<T> Deserialize<T>()
        {
            string fileName = typeof(T).Name + ".json";
            fileName = basePath + dataPath + fileName;
            string json = File.ReadAllText(fileName);

            SerializedList<T> list = JsonUtility.FromJson<SerializedList<T>>(json);

            return list.list;
        }
    }
}
