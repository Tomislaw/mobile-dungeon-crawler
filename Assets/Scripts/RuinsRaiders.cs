using UnityEngine;

namespace RuinsRaiders
{
    public class App
    {

        #if !UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnLoad()
        {
            Debug.unityLogger.logHandler = DebugLogger.Instance;
            SaveableData.LoadAll();
        }

        #endif
    }
}
