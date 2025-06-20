using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.AIDevKit.Editor.Chatbots;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static partial class EditorChatUtil
    {
        internal static class File
        {
            internal static void SaveChatAsText(IReadOnlyList<ChatMessage> messages, Result<ChatSession> currentSession)
            {
                if (messages.IsNullOrEmpty()) return;

                string sessionName = currentSession.Value.Name;

                if (string.IsNullOrWhiteSpace(sessionName))
                {
                    sessionName = "unknown_session";
                }
                else
                {
                    sessionName = sessionName.ToSnakeCase();
                }

                sessionName += $"_{DateTime.Now:yyyyMMdd_HHmmss}";

                string fileName = $"{sessionName}.txt";
                string filePath = EditorUtility.SaveFilePanel("Save Chat as Text", "", fileName, "txt");
                if (string.IsNullOrEmpty(filePath)) return;

                System.IO.File.WriteAllLines(filePath, messages.Select(chat => chat.ToString()));
                AssetDatabase.Refresh();
            }

            internal static AttachedFileType ResolveFileType(string filePath)
            {
                string extension = Path.GetExtension(filePath);

                if (EditorChatConfig.ImageExtensions.Contains(extension))
                    return AttachedFileType.Image;

                if (EditorChatConfig.TextExtensions.Contains(extension))
                    return AttachedFileType.ScriptOrText;

                if (EditorChatConfig.AudioExtensions.Contains(extension))
                    return AttachedFileType.Audio;

                return AttachedFileType.NotSupported;
            }

            internal static Result<ChatSession> GetOrCreateSession(string id, bool? newSession = null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(id)) id = EditorChatSettings.CurrentSessionId;
                    newSession ??= string.IsNullOrWhiteSpace(id);
                    ChatSession session;

                    if (newSession.Value)
                    {
                        session = ChatSessionManager.CreateSession(EditorChatConfig.kChatbotStartingMessage);
                        if (session == null) throw new Exception("Failed to create a new chat session.");
                        AIDevKitDebug.Blue($"Creating new chat session with ID: {session.Id}");
                        session.AutoSave = true;
                        session.Instructions = EditorChatConfig.kChatbotInstruction;
                        session.SaveFile();
                    }
                    else
                    {
                        session = ChatSessionManager.GetSession(id);
                        session ??= ChatSessionManager.GetLastEditorSession();

                        if (session == null) throw new Exception($"Failed to load chat session with ID: {id}");
                    }

                    return Result<ChatSession>.Success(session);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return Result<ChatSession>.Fail(null, e);
                }
            }
        }
    }
}