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
        private GlobalEvent onTrigger;

        private Dictionary<string, UnityEvent> _eventDictionary;
        private static EventManager _eventManager;

        private static EventManager Instance
        {
            get
            {
                if (!_eventManager)
                {
                    _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!_eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        _eventManager.Init();
                    }
                }

                return _eventManager;
            }
        }

        void Init()
        {
            if (_eventDictionary == null)
            {
                _eventDictionary = new Dictionary<string, UnityEvent>();
            }
        }

        public static void Register(UnityAction<string> action)
        {
            Instance.onTrigger.AddListener(action);
        }

        public static void StartListening(string eventName, UnityAction listener)
        {
            if (Instance._eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Instance._eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction listener)
        {
            if (_eventManager == null)
                return;

            if (Instance._eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent(string eventName)
        {
            if (Instance._eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
            {
                thisEvent.Invoke();
            }
            Instance.onTrigger.Invoke(eventName);
        }

        [Serializable]
        public class GlobalEvent : UnityEvent<string> { }
    }
}