using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    public List<Event> events = new List<Event>();

    private void Start()
    {
        EventManager.instance.OnTrigger.AddListener(OnEvent);
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
