using UnityEngine;

namespace RuinsRaiders
{
    public class App
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnLoad()
        {
            Debug.unityLogger.logHandler = DebugLogger.Instance;
            SaveableData.LoadAll();
        }
    }
}
