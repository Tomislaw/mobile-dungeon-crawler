using UnityEngine;

namespace RuinsRaiders.AI
{
    public struct ActivatorData{
        public GameObject triggeredBy;
        public GameObject triggeredFor;
    }

    public abstract class BasicAiActivatorData : ScriptableObject
    {
        public string Name;
        public abstract BasicAiActivator Create(GameObject gameObject);
    }

    public abstract class BasicAiActivator
    {
        public abstract ActivatorData? Triggered();
    }
}
