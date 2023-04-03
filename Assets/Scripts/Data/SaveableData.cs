using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    public interface SaveableData
    {
        public string GetFileName();

        public string AsSerializedString()
        {
            return JsonUtility.ToJson(this, true);
        }

        public void FromSerializedString(string data)
        {
            if (!string.IsNullOrEmpty(data))
                JsonConvert.PopulateObject(data, this);
        }

#if (UNITY_EDITOR)
        public void Load() { }
        public void Save() { }
        public void Delete() { }
        public static void DeleteAll() { }

#elif (UNITY_STANDALONE)

        public virtual void Load()
        {
            string path = GetFilePath(GetFileName());
            string result = "";
            if (File.Exists(path))
                result = File.ReadAllText(path);

            FromSerializedString(result);
        }

        public void Save()
        {
            var path = GetFilePath(GetFileName());
            File.WriteAllText(path, AsSerializedString());
        }

        public void Delete()
        {
            var path = GetFilePath(GetFileName());
            if (File.Exists(path))
                File.Delete(path);
        }

        public static void DeleteAll()
        {
            var saveables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<SaveableData>();
            foreach (var saveable in saveables)
                saveable.Delete();
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

#else
        public void Load()
        {
            FromSerializedString(PlayerPrefs.GetString(GetFileName(), ""));
        }

        public void Save()
        {
            PlayerPrefs.SetString(GetFileName(), AsSerializedString());
            PlayerPrefs.Save();
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(GetFileName());
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

#endif

        public static void SaveAll()
        {
            var saveables = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<SaveableData>();
            foreach (var saveable in saveables)
                saveable.Save();
        }

        public static List<SaveableData> GetAll()
        {
            return Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<SaveableData>().ToList();
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
