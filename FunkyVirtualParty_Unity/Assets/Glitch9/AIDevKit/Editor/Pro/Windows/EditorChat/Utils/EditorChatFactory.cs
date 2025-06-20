using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static class EditorChatFactory
    {

        internal static class Keys
        {
            internal const string Scene = "Scene";
            internal const string ActiveSelection = "Active Selection";
            internal const string ConsoleErrors = "Console Errors";
            internal const string AttachedTextAssets = "Attached Scripts / Text Assets";
        }

        internal static async UniTask<UserMessage> CreateUserMessageAsync(string userMessage, List<string> attachedTextAssets)
        {
            UnityEngine.Object selected = Selection.activeObject;

            string sceneName = EditorSceneManager.GetActiveScene().name;
            string activeSelection = selected != null ? new UnityObjectProfile(selected).ToString() : null;
            string formattedTextAssets = await EditorChatTextFormatter.FormatTextAssetsAsync(attachedTextAssets);

            Dictionary<string, string> metadata = new();
            if (!string.IsNullOrEmpty(sceneName)) metadata.Add(Keys.Scene, sceneName);
            if (!string.IsNullOrEmpty(activeSelection)) metadata.Add(Keys.ActiveSelection, activeSelection);
            if (!string.IsNullOrEmpty(formattedTextAssets)) metadata.Add(Keys.AttachedTextAssets, formattedTextAssets);

            return new(userMessage) { EditorSessionData = metadata };
        }

        internal static SystemMessage CreateSystemMessage(string systemMessage, string displayMessage = null)
        {
            return new(systemMessage) { DisplayMessage = displayMessage };
        }

        internal static async UniTask<ChatMessageItem> CreateChatMessageItemAsync(int index, ChatMessage msg, List<IFile> scriptFiles = null)
        {
            string content = msg.ToString();
            ChatMessageItemType type;
            Texture2D icon = null;
            Usage usage = null;
            UnixTime timestamp = msg.Timestamp;
            List<Texture2D> attachedImages = new();
            List<ChatContextItem> attachedFilesOtherThanImages = new();

            switch (msg.Role)
            {
                case ChatRole.System:
                    type = ChatMessageItemType.System;
                    if (msg is SystemMessage systemMessage)
                        content = systemMessage.DisplayMessage;
                    break;
                case ChatRole.User:
                    type = ChatMessageItemType.User;
                    //icon = AIDevKitIcons.User;
                    if (msg is UserMessage userMessage)
                    {
                        // var imageFiles = userMessage.AttachedFiles.GetList<File<Texture2D>>();
                        // foreach (var file in imageFiles)
                        // {
                        //     if (file.Exists)
                        //     {
                        //         Texture2D texture = await file.LoadAssetAsync();
                        //         if (texture != null) attachedImages.Add(texture);
                        //     }
                        // }

                        if (userMessage.AttachedFiles.IsNotNullOrEmpty())
                        {
                            foreach (var file in userMessage.AttachedFiles)
                            {
                                if (file == null) continue;

                                if (file is File<Texture2D> imageFile && imageFile.Exists)
                                {
                                    Texture2D texture = await imageFile.LoadAssetAsync();
                                    if (texture != null) attachedImages.Add(texture);
                                }
                                else
                                {
                                    var contextItem = new ChatContextItem(
                                        id: file.FullPath,
                                        name: file.Name,
                                        icon: EditorChatUtil.ResolveFileIcon(file) as Texture2D
                                    );
                                    attachedFilesOtherThanImages.Add(contextItem);
                                }
                            }
                        }

                        if (scriptFiles.IsNotNullOrEmpty())
                        {
                            foreach (var scriptFile in scriptFiles)
                            {
                                if (scriptFile == null) continue;

                                var contextItem = new ChatContextItem(
                                    id: scriptFile.FullPath,
                                    name: scriptFile.Name,
                                    icon: EditorChatUtil.ResolveFileIcon(scriptFile) as Texture2D
                                );

                                attachedFilesOtherThanImages.Add(contextItem);
                            }
                        }
                    }
                    break;
                case ChatRole.Assistant:
                    type = ChatMessageItemType.Assistant;
                    //icon = AIDevKitIcons.Assistant;
                    if (msg is ResponseMessage response)
                        usage = response.Usage;
                    break;
                default:
                    type = ChatMessageItemType.System;
                    break;
            }

            return new ChatMessageItem
            {
                Index = index,
                Type = type,
                Content = content,
                Icon = icon,
                Usage = usage,
                Timestamp = timestamp,
                AttachedImages = attachedImages,
                AttachedFiles = attachedFilesOtherThanImages,
            };
        }

        internal static ChatMessageItem CreateTempMessageItem(string msg, MessageType messageType, List<ButtonEntry> buttons = null)
        {
            return new ChatMessageItem
            {
                Index = -1, // 임시 메시지이므로 인덱스는 -1로 설정
                Content = msg,
                Type = messageType switch
                {
                    MessageType.Info => ChatMessageItemType.TempInfo,
                    MessageType.Warning => ChatMessageItemType.TempWarning,
                    MessageType.Error => ChatMessageItemType.TempError,
                    _ => ChatMessageItemType.TempInfo,
                },
                Icon = messageType switch
                {
                    MessageType.Info => EditorIcons.StatusCheck as Texture2D,
                    MessageType.Warning => EditorIcons.ConsoleWarnIcon as Texture2D,
                    MessageType.Error => EditorIcons.ConsoleErrorIcon as Texture2D,
                    _ => null
                },
                Timestamp = UnixTime.Now,
                Buttons = buttons,
            };
        }
    }
}