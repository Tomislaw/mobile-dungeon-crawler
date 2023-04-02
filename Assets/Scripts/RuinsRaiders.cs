using UnityEngine;

namespace RuinsRaiders
{
    public class App
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnLoad()
        {
            SaveableData.LoadAll();
        }
    }
}
