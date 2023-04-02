using Newtonsoft.Json;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    public interface SaveableData
    {
        protected string GetFileName();

#if (UNITY_EDITOR)
        public void Load() { }
        public void Save() { }

#elif (UNITY_STANDALONE)

        public virtual void Load()
        {
            string path = GetFilePath(GetFileName());
            string result = "";
            if (File.Exists(path))
            {
                result = File.ReadAllText(path);
            }

            if (!string.IsNullOrEmpty(result))
            {
                JsonConvert.PopulateObject(result, this);
            }
        }


        public virtual void Save()
        {
            var json = JsonUtility.ToJson(this);
            var path = GetFilePath(GetFileName());
            File.WriteAllText(path, json);
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

#else

        public void Load()
        {
            var json = PlayerPrefs.GetString(GetFileName(), "");
            if (!string.IsNullOrEmpty(json))
                JsonConvert.PopulateObject(json, this);
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(GetFileName(), json);
        }

#endif

        public static void SaveAll()
        {
            var saveables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<SaveableData>();
            foreach (var saveable in saveables)
                saveable.Save();
        }

        public static void LoadAll()
        {
            var saveables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<SaveableData>();
            foreach (var saveable in saveables)
                saveable.Load();
        }

        public static void SaveAllOfType<T>() where T : SaveableData
        {
            var saveables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<T>();
            foreach (var saveable in saveables)
                saveable.Save();
        }

        public static void LoadAllOfType<T>() where T : SaveableData
        {
            var saveables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<T>();
            foreach (var saveable in saveables)
                saveable.Load();
        }
    }
}
