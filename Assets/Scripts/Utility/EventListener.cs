using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace RuinsRaiders
{
    // Event lister, used for registering into global EventManager instance
    public class EventListener : MonoBehaviour
    {
        [SerializeField]
        private List<Event> events = new List<Event>();

        private void Start()
        {
            EventManager.Register(OnEvent);
        }

        private void OnEvent(string name)
        {
            foreach (var item in events)
                if (item.Name == name)
                    item.UnityEvent.Invoke();
        }

        [System.Serializable]
        public struct Event
        {
            public string Name;
            public UnityEvent UnityEvent;
        }
    }
}