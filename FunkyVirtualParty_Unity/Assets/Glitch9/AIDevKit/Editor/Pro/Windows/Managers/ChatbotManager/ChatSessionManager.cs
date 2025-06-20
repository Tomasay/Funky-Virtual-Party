using System;
using System.Collections.Generic;
using System.IO;
using Glitch9.AIDevKit.Advanced.Chat;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Chatbots
{
    internal static class ChatSessionManager
    {
        internal static List<ChatSession> Sessions => _sessions ??= LoadSessions();
        private static List<ChatSession> _sessions;
        internal static List<ChatSession> EditorSessions => _editorSessions ??= LoadSessions(ChatSessionScope.RuntimeOnly);
        private static List<ChatSession> _editorSessions;
        internal static int EditorChatCount => EditorSessions.Count;

        internal static ChatSession GetSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return null;

            foreach (ChatSession chatbot in Sessions)
            {
                if (chatbot.Id == sessionId) return chatbot;
            }

            return null;
        }

        internal static ChatSession GetLastEditorSession()
        {
            if (EditorSessions == null || EditorSessions.Count == 0) return null;

            // Return the last session in the editor sessions list
            return EditorSessions[EditorSessions.Count - 1];
        }

        internal static bool DeleteSession(ChatSession session)
        {
            if (session == null || string.IsNullOrEmpty(session.Id)) return false;

            if (session.DeleteFile())
            {
                Sessions.Remove(session);
                EditorSessions.Remove(session);
                Debug.Log($"Deleted Chatbot session: {session.Id}");

                return true;
            }

            Debug.LogWarning($"Failed to delete Chatbot session: {session.Id}");
            return false;
        }

        internal static void ReloadSessions()
        {
            try
            {
                _sessions = LoadSessions();
                Debug.Log($"Reloaded {Sessions.Count} Chatbot sessions.");
                _editorSessions = LoadSessions(ChatSessionScope.RuntimeOnly);
                Debug.Log($"Reloaded {EditorSessions.Count} Chatbot sessions for the editor.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to reload editor Chatbot sessions: {e.Message}\n{e.StackTrace}");
            }
        }

        internal static List<ChatSession> LoadSessions(ChatSessionScope excludeScope = ChatSessionScope.None)
        {
            try
            {
                string path = ChatSessionUtil.GetSavePath();
                if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Chatbot save directory does not exist: {path}");
                // read all files in the directory
                string[] files = Directory.GetFiles(path, "*.json");

                List<ChatSession> sessions = new();

                foreach (string file in files)
                {
                    // AIDevKitDebug.Mark($"Loading Chatbot from file: {file}");
                    ChatSession session = ChatSessionUtil.LoadSessionFromPath(file);
                    if (session == null)
                    {
                        Debug.LogWarning($"Failed to load Chatbot from file: {file}");
                        continue;
                    }

                    if (excludeScope != ChatSessionScope.None && session.Scope == excludeScope)
                    {
                        continue;
                    }

                    sessions.Add(session);
                }

                return sessions;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to reload Chatbots: {e.Message}\n{e.StackTrace}");
            }

            return null;
        }

        internal static ChatSession CreateSession(string startingMessage = null)
        {
            try
            {
                ChatSession session = ChatSession.CreateFile(startingMessage: startingMessage) ?? throw new Exception("Failed to create a new Chatbot session.");
                Sessions.Add(session);
                if (session.Scope != ChatSessionScope.RuntimeOnly) EditorSessions.Add(session);
                return session;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }
    }
}