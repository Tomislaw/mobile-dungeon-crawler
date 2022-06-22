using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace RuinsRaiders
{
    // Global event distributor, only one instance should be created for scene
    // Objects can listen for global events like player character death, level finished and other
    public class EventManager : MonoBehaviour
    {
        [SerializeField]
        private GlobalEvent OnTrigger;

        private Dictionary<string, UnityEvent> eventDictionary;
        private static EventManager eventManager;

        private static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }

                return eventManager;
            }
        }

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, UnityEvent>();
            }
        }

        public static void Register(UnityAction<string> action)
        {
            instance.OnTrigger.AddListener(action);
        }

        public static void StartListening(string eventName, UnityAction listener)
        {
            UnityEvent thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction listener)
        {
            if (eventManager == null) return;
            UnityEvent thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent(string eventName)
        {
            UnityEvent thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke();
            }
            instance.OnTrigger.Invoke(eventName);
        }

        [System.Serializable]
        public class GlobalEvent : UnityEvent<string> { }
    }
}