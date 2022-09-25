using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Data
{
    [Serializable]
    public class SerializableList<T>
    {
        public List<T> list;
        public SerializableList(List<T> l)
        {
            list = l;
        }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in this)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            for (int i = 0; i != keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }


    public static class DataManager
    {
        private static string basePath = "Assets/Resources/";
        private static string dataPath = "Data/";
        private static string prefabPath = "Prefabs/";

        public static string DataPath { get { return basePath + dataPath; } }
        public static string PrefabPath { get { return basePath + prefabPath; } }
        public static string SoundPath { get { return "Sounds/Data/"; } }

        // Json 데이터 구조 틀을 만들기 위해 활용
        public static void Serialize<T>(List<T> objects)
        {
            string json = JsonUtility.ToJson(new SerializableList<T>(objects));
            string fileName = typeof(T).Name + ".json";

            Debug.Log(json);
            File.WriteAllText(DataPath + fileName, json);
        }

        public static List<T> Deserialize<T>()
        {
            string fileName = typeof(T).Name + ".json";
            fileName = DataPath + fileName;
            string json = File.ReadAllText(fileName);

            SerializableList<T> list = JsonUtility.FromJson<SerializableList<T>>(json);

            return list.list;
        }

        public static GameObject GetPrefab<T>(string name)
        {
            string typeName = typeof(T).Name + "/";

            return Resources.Load<GameObject>(prefabPath + typeName + name);
        }

        public static List<T> GetObjects<T>(string filePath) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(filePath).ToList();
        }
    }
}
