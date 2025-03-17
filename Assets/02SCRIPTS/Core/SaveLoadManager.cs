using UnityEngine;
using System.IO;

namespace Game.Core
{
    public class SaveLoadManager
    {
        public static void SaveData<T>(string fileName, T data)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        public static T LoadData<T>(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(json);
            }
            return default;
        }
    }
}