using System.Collections.Generic;
using System.IO;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal enum AttachedFileType
    {
        NotSupported,
        ScriptOrText,
        Image,
        Audio,
    }

    internal class ChatAttachmentBinder
    {
        internal Dictionary<string, IFile> AttachedFiles { get; set; }
        private readonly EditorChatWindow _window;
        private bool _callbacksRegisteredToContainer = false;

        internal ChatAttachmentBinder(EditorChatWindow window)
        {
            _window = window;
            AttachedFiles = new();
        }

        internal void Clear()
        {
            AttachedFiles?.Clear();
            _window?.RebuildAttachmentContainer();
        }

        internal void RegisterCallbacks(VisualElement dragArea)
        {
            // _defaultRootVisualElementColor = chatArea.style.backgroundColor;
            // Color defaultColor = _defaultRootVisualElementColor.value;
            // _hoverRootVisualElementColor = new StyleColor(
            //     new Color(defaultColor.r * 1.2f, defaultColor.g * 1.4f, defaultColor.b * 1.2f, defaultColor.a) // Slightly modified color for hover effect
            // );

            // Register drag and drop callbacks on the root visual element
            // to handle file drops in the Editor Chat window.
            // This allows users to drag files into the chat for context or sharing. 
            dragArea.RegisterCallback<DragEnterEvent>(e =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                //chatArea.style.backgroundColor = _hoverRootVisualElementColor;
                dragArea.style.opacity = 0.8f; // Optional: Change opacity to indicate drag area
            });

            dragArea.RegisterCallback<DragLeaveEvent>(e =>
            {
                //chatArea.style.backgroundColor = _defaultRootVisualElementColor;
                dragArea.style.opacity = 1f; // Reset opacity when drag leaves
            });

            dragArea.RegisterCallback<DragUpdatedEvent>(e =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            });

            dragArea.RegisterCallback<DragPerformEvent>(e =>
            {
                //chatArea.style.backgroundColor = _defaultRootVisualElementColor;
                dragArea.style.opacity = 1f; // Reset opacity when files are dropped
                DragAndDrop.AcceptDrag();

                foreach (var path in DragAndDrop.paths)
                {
                    //Debug.Log($"Dropped file: {path}");
                    OnFileDrop(path);
                }
            });
        }

        private void OnFileDrop(string filePath)
        {
            AttachedFileType fileType = EditorChatUtil.File.ResolveFileType(filePath);

            if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"File dropped: {filePath}");

            if (fileType == AttachedFileType.NotSupported)
            {
                Debug.LogWarning($"File type not supported: {filePath}");
                return;
            }

            if (fileType == AttachedFileType.Image)
            {
                OnImageFileAdded(filePath);
            }
            else if (fileType == AttachedFileType.Audio)
            {
                OnAudioFileAdded(filePath);
            }
            else if (fileType == AttachedFileType.ScriptOrText)
            {
                OnScriptOrTextFileAdded(filePath);
            }

            _window.RebuildAttachmentContainer();
        }

        internal void OnImageFileAdded(string filePath)
        {
            if (!CanAddFile(filePath)) return;
            AttachedFiles.Add(filePath, new File<Texture2D>(filePath));
        }

        internal void OnAudioFileAdded(string filePath)
        {
            if (!CanAddFile(filePath)) return;
            AttachedFiles.Add(filePath, new File<AudioClip>(filePath));
        }

        internal void OnScriptOrTextFileAdded(string filePath)
        {
            if (!CanAddFile(filePath)) return;
            AttachedFiles.Add(filePath, new RawFile(filePath));
        }

        private bool CanAddFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            if (AttachedFiles.ContainsKey(filePath))
            {
                if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"File already added: {filePath}");
                return false; // 이미 추가된 파일은 무시
            }
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"File does not exist: {filePath}");
                return false; // 파일이 존재하지 않으면 무시
            }

            return true;
        }

        internal void BuildUI(VisualElement container)
        {
            //container.Clear();

            if (!_callbacksRegisteredToContainer)
            {
                RegisterCallbacks(container);
                _callbacksRegisteredToContainer = true;
            }

            VisualElement itemsColumn = container.Q<VisualElement>("attachment-column-items");
            VisualElement actionsColumn = container.Q<VisualElement>("attachment-column-actions");

            if (itemsColumn == null || actionsColumn == null)
            {
                Debug.LogError("Attachment columns not found in the container.");
                return;
            }

            itemsColumn.Clear(); // Clear existing items
            actionsColumn.Clear(); // Clear existing actions

            if (AttachedFiles.IsNotNullOrEmpty())
            {
                foreach (var kvp in AttachedFiles)
                {
                    string filePath = kvp.Key;
                    IFile file = kvp.Value;
                    if (file == null) continue;

                    var contextItem = CreateContextItem(filePath, file);
                    var itemElement = new ChatContextElement(contextItem);
                    if (itemElement != null) itemsColumn.Add(itemElement);
                }

                // add remove all button
                Button clearAllBtn = new() { text = "Clear All" };
                clearAllBtn.AddToClassList("object-area-action-button");
                clearAllBtn.clicked += () =>
                {
                    if (EditorChatSettings.DebugMode) AIDevKitDebug.Info("Clearing all attachments.");
                    Clear();
                };

                actionsColumn.Add(clearAllBtn);

                //container.Add(labelRow);
                //container.Add(itemsColumn);
            }
        }

        private ChatContextItem CreateContextItem(string filePath, IFile file)
        {
            return new ChatContextItem(
                id: filePath,
                name: file.Name,
                icon: EditorChatUtil.ResolveFileIcon(file) as Texture2D,
                remove: (id) => RemoveAttachment(id)
            );
        }

        private void RemoveAttachment(string filePath)
        {
            if (AttachedFiles.ContainsKey(filePath))
            {
                AttachedFiles.Remove(filePath);
                if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"Removed file: {filePath}");
            }
            else
            {
                Debug.LogWarning($"File not found in attachments: {filePath}");
            }

            _window.RebuildAttachmentContainer();
        }
    }
}