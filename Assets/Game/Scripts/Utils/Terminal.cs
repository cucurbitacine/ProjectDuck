using System;
using System.Collections.Generic;
using CucuTools.Utils;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public class Terminal : Singleton<Terminal>
    {
        private static bool _show = false;
        private static string _commandLine;
        private static GUIStyle _logStyle;
        private static GUIStyle _warningStyle;
        private static GUIStyle _errorStyle;
        
        private static readonly List<Log> LogHistory = new List<Log>();
        private const int MaxLogsDisplay = 32; 
        
        private static void GUISetupStyles()
        {
            if (_logStyle == null)
            {
                _logStyle = new GUIStyle(GUI.skin.box);
                _logStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (_warningStyle == null)
            {
                _warningStyle = new GUIStyle(GUI.skin.box);
                _warningStyle.normal.textColor = Color.yellow;
                _warningStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (_errorStyle == null)
            {
                _errorStyle = new GUIStyle(GUI.skin.box);
                _errorStyle.normal.textColor = Color.red;
                _errorStyle.alignment = TextAnchor.MiddleLeft;
            }
        }

        private static void GUICommandLine()
        {
            GUILayout.BeginHorizontal();
            
            _commandLine = GUILayout.TextField(_commandLine);
            
            if (GUILayout.Button("Execute"))
            {
                Debug.Log(_commandLine);
            }
            
            GUILayout.EndHorizontal();
        }
        
        private static void GUILogHistory()
        {
            GUILayout.BeginVertical();

            var count = Mathf.Min(MaxLogsDisplay, LogHistory.Count);
            
            for (var i = 0; i < count; i++)
            {
                GUILog(LogHistory[LogHistory.Count - 1 - i]);
            }
                
            GUILayout.EndVertical();
        }
        
        private static void GUILog(Log log)
        {
            var style = _logStyle;

            if (log.type is LogType.Warning or LogType.Assert)
            {
                style = _warningStyle;
            }
            else if (log.type is LogType.Error or LogType.Exception)
            {
                style = _errorStyle;
            }

            GUILayout.Box($"[{log.time:T}] {log.message}", style);
        }
        
        private static void HandleLog(string condition, string stacktrace, LogType type)
        {
            LogHistory.Add(new Log(DateTime.Now, condition, stacktrace, type));
        }
        
        protected override void OnStart()
        {
            Application.logMessageReceived += HandleLog;

            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _show = !_show;
            }
        }
        
        private void OnGUI()
        {
            if (_show)
            {
                GUISetupStyles();
                
                //GUICommandLine();
                
                GUILogHistory();
            }
        }
        
        private struct Log
        {
            public DateTime time;
            public string message;
            public string stacktrace;
            public LogType type;

            public Log(DateTime time, string message, string stacktrace, LogType type)
            {
                this.time = time;
                this.message = message;
                this.stacktrace = stacktrace;
                this.type = type;
            }
        }
    }
}
