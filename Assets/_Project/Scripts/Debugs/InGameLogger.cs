using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Debugs
{
    public class InGameLogger : MonoBehaviour
    {
        private struct LogEntry
        {
            public string message;
            public LogType type;

            public LogEntry(string message, LogType type)
            {
                this.message = message;
                this.type = type;
            }
        }

        private static readonly List<LogEntry> logs = new List<LogEntry>();
        private Vector2 scroll;

        // Toggle visibility
        private bool visible = true;
        public KeyCode toggleKey = KeyCode.F1;

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                visible = !visible;
            }
        }

        private void OnEnable()
        {
            UnityEngine.Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            UnityEngine.Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            logs.Add(new LogEntry(logString, type));

            if (logs.Count > 200)
            {
                logs.RemoveAt(0);
            }
        }

        private void OnGUI()
        {
            if (!visible) return;

            GUI.color = Color.white;
            GUI.backgroundColor = new Color(0, 0, 0, 0.6f);

            GUILayout.BeginArea(new Rect(10, 10, 700, 350), GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);

            foreach (var entry in logs)
            {
                GUI.color = GetColor(entry.type);
                GUILayout.Label(entry.message);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUI.color = Color.white; // reset
        }

        private Color GetColor(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    return Color.red;

                case LogType.Exception:
                    return new Color(1f, 0.3f, 0.3f);

                case LogType.Warning:
                    return Color.yellow;

                case LogType.Assert:
                    return Color.magenta;

                default:
                    return Color.white;
            }
        }
    }
}
