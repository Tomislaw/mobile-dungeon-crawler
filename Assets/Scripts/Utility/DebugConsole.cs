using System.Collections.Concurrent;
using UnityEngine;

namespace RuinsRaiders
{
    // Debug class for writing all global events into TextMeshPro Text
    public class DebugConsole : MonoBehaviour
    {

        public int lines = 8;

        private FixedSizedQueue<string> logs;
        private TMPro.TMP_Text text;

        void OnEnable()
        {
            EventManager.Register(HandleLog);
        }

        void OnDisable()
        {
            EventManager.Register(HandleLog);
        }

        void HandleLog(string logString)
        {
            logs.Enqueue(logString);
            string str = "";
            foreach (var s in logs)
            {
                str += ">" + s + "\n";
            }
            text.text = str;
        }
        void Awake()
        {
            logs = new FixedSizedQueue<string>(lines);
            text = GetComponent<TMPro.TMP_Text>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }
    }
}