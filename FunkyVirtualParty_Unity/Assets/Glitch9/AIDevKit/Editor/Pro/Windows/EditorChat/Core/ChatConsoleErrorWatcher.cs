using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    [InitializeOnLoad]
    internal class ChatConsoleErrorWatcher
    {
        static ChatConsoleErrorWatcher()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        internal class ScriptWithError
        {
            public string path;
            public int lineNumber;
            public int columnNumber;
            public string errorMessage;
        }

        internal class ConsoleErrorEntry
        {
            public string message;
            public string stackTrace;
            public DateTime time;
            public int count;
            public ScriptWithError targetScript;
        }

        private static ChatConsoleErrorWatcher _instance;
        internal static Dictionary<string, ConsoleErrorEntry> UnresolvedErrors => _unresolvedErrors;
        private readonly static Dictionary<string, ConsoleErrorEntry> _unresolvedErrors = new();
        internal static int Count
        {
            get
            {
                int count = 0;
                foreach (var entry in _unresolvedErrors.Values)
                {
                    count += entry.count;
                }
                return count;
            }
        }
        internal static string Message { get; private set; }

        private readonly EditorChatWindow _window;
        internal ChatConsoleErrorWatcher(EditorChatWindow window)
        {
            _window = window;
            _instance = this;
        }

        internal bool HasScriptErrors(out string scriptPath)
        {
            scriptPath = string.Empty;
            foreach (var entry in _unresolvedErrors.Values)
            {
                if (entry.targetScript != null)
                {
                    scriptPath = entry.targetScript.path;
                    return true;
                }
            }
            return false;
        }

        internal string GetJoinedMessage()
        {
            if (Count == 0) return "No unresolved errors.";

            using (StringBuilderPool.Get(out var sb))
            {
                sb.AppendLine("Unresolved Errors:");
                foreach (var entry in _unresolvedErrors.Values)
                {
                    //if (entry.resolved) continue;
                    // if both entry.message and entry.stackTrace are null or empty, skip this entry
                    if (string.IsNullOrWhiteSpace(entry.message) && string.IsNullOrWhiteSpace(entry.stackTrace))
                        continue;
                    sb.AppendLine("----------------------------------------");
                    if (!string.IsNullOrWhiteSpace(entry.message))
                        sb.AppendLine($"## Message: {entry.message}");

                    if (entry.targetScript != null)
                    {
                        // load the script code with TextAsset loading / AssetDatabase
                        try
                        {
                            var scriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(entry.targetScript.path);
                            if (scriptAsset == null)
                            {
                                Debug.LogWarning($"Script not found: {entry.targetScript.path}");
                            }
                            else
                            {
                                var scriptText = scriptAsset.text;
                                if (string.IsNullOrWhiteSpace(scriptText))
                                {
                                    Debug.LogWarning($"Script is empty: {entry.targetScript.path}");
                                }
                                else
                                {
                                    sb.AppendLine("## Script Information:");
                                    sb.AppendLine($"Error: {entry.targetScript.errorMessage}");
                                    sb.AppendLine($"Line: {entry.targetScript.lineNumber}, Column: {entry.targetScript.columnNumber}");

                                    // The problematic part of the script
                                    if (entry.targetScript.lineNumber > 0 && entry.targetScript.lineNumber <= scriptText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length)
                                    {
                                        sb.AppendLine($"Code: {scriptText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[entry.targetScript.lineNumber - 1].Trim()}");
                                    }

                                    // Full script text
                                    sb.AppendLine($"Full Script Text:\n{scriptText}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine($"Error loading script text: {ex.Message}");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(entry.stackTrace))
                    {
                        string stackTrace = entry.stackTrace;
                        if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Length > EditorChatSettings.MaxStackTraceLength)
                        {
                            stackTrace = stackTrace.Substring(0, EditorChatSettings.MaxStackTraceLength) + "\n... [truncated]";
                        }
                        sb.AppendLine($"## Stack Trace: {stackTrace}");
                    }

                    sb.AppendLine("----------------------------------------");
                }
                return sb.ToString();
            }
        }

        private static void OnLogMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
        {
            if (type != UnityEngine.LogType.Error && type != UnityEngine.LogType.Exception)
            {
                NotifyIfErrorExists();
                return;
            }

            string key = condition.Trim();

            if (_unresolvedErrors.TryGetValue(key, out ConsoleErrorEntry entry))
            {
                entry.count++;
                entry.time = DateTime.Now;
                Message = condition;
            }
            else
            {
                // see if message or stack trace contains a Script information 
                // Change of plans: no stack trace parsing. it can lead to wrong scripts like loggers, debuggers, etc.
                // Example of a condition that can be parsed:
                // e.g. Assets\Glitch9\AIDevKit\Editor\Pro\Windows\EditorChat\Windows\EditorChatWindow.cs(562,26): error CS1002: ; expected
                ScriptWithError scriptWithError = null;
                var match = Regex.Match(condition, @"^(.*)\((\d+),(\d+)\):\s*(error|warning)\s*(.*)$", RegexOptions.Multiline);

                if (match.Success)
                {
                    string path = match.Groups[1].Value.Trim();
                    string reletiveProjectPath = AIDevKitSettings.ProjectPath.ToRelativePath();

                    // path 가 유저의 프로젝트 경로에 있는지 확인
                    if (path.StartsWith(reletiveProjectPath) && File.Exists(path))
                    {
                        Message = match.Groups[5].Value.Trim();

                        scriptWithError = new ScriptWithError
                        {
                            path = path,
                            lineNumber = int.Parse(match.Groups[2].Value),
                            columnNumber = int.Parse(match.Groups[3].Value),
                            errorMessage = Message
                        };
                    }
                }

                _unresolvedErrors[key] = new ConsoleErrorEntry
                {
                    message = condition,
                    stackTrace = stackTrace,
                    time = DateTime.Now,
                    count = 1,
                    targetScript = scriptWithError
                };
            }

            NotifyIfErrorExists();
        }

        internal static void NotifyIfErrorExists()
        {
            if (_instance != null && Count > 0)
                _instance._window.OnConsoleErrorReceived(Count, Message ?? "Unknown error");
        }

        public void ClearAll()
        {
            _unresolvedErrors.Clear();
        }

        public void MarkAsResolved(string condition)
        {
            _unresolvedErrors.Remove(condition.Trim());
        }

        internal void ClearErrors()
        {
            // foreach (var entry in _unresolvedErrors.Values)
            // {
            //     entry.resolved = true;
            // }
            _unresolvedErrors.Clear();
        }
    }
}