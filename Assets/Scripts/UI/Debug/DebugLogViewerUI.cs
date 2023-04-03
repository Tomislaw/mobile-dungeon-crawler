using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RuinsRaiders
{
    public class DebugLogViewerUI : MonoBehaviour
    {
        public TMPro.TMP_Text debug;
        public TMPro.TMP_Text warn;
        public TMPro.TMP_Text error;

        private int _lastLogSize = 0;

        private List<TMPro.TMP_Text> _logs = new();

        void Start()
        {
            debug.gameObject.SetActive(false);
            warn.gameObject.SetActive(false);
            error.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(DebugLogger.TotalLogs != _lastLogSize)
            {
                var diff = DebugLogger.TotalLogs - _lastLogSize;

                var logs = DebugLogger.logs.TakeLast(diff);

                foreach(var log in logs)
                {
                    if(_logs.Count > 0 && _logs.Last().text == log.Item2)
                    {
                        continue;
                    }

                    TMPro.TMP_Text newLog = log.Item1 switch
                    {
                        LogType.Exception or LogType.Error or LogType.Assert => Instantiate(error),
                        LogType.Warning => Instantiate(warn),
                        _ => Instantiate(debug),
                    };

                    newLog.gameObject.transform.SetParent(debug.transform.parent, false);
                    newLog.gameObject.SetActive(true);
                    newLog.transform.localScale = Vector3.one;
                    newLog.text = log.Item2;
                    _logs.Add(newLog);
                }

                _lastLogSize = DebugLogger.TotalLogs;
                while(_logs.Count >= DebugLogger.MaxLogs)
                {
                    Destroy(_logs[0]);
                    _logs.RemoveAt(0);
                }

            }
        }
    }

    public class DebugLogger : ILogHandler
    {
        public static int MaxLogs = 9999;

        public static int TotalLogs { get; private set; }

        public static List<(LogType, string)> logs = new();


        private static DebugLogger _instance;
        public static DebugLogger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DebugLogger();

                return _instance;
            }
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (logs.Count >= MaxLogs)
                logs.RemoveAt(0);

            string log;
            try
            {
                log =  "[" + context == null ? "null" : context.name + "] " + String.Format(format, args);
            } catch (Exception e)
            {
                log = "[" + context == null ? "null" : context.name + "] " + format;
            }


            logs.Add((logType, log));
            TotalLogs++;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            if (logs.Count >= MaxLogs)
                logs.RemoveAt(0);

            var log = "[" + context.name + "] " + exception.Message;
            logs.Add((LogType.Exception, log));
            TotalLogs++;
        }
    }
}