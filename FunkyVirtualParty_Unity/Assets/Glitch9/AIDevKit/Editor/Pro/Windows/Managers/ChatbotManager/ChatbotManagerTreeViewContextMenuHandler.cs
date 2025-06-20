using System;
using System.Collections.Generic;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Chatbots
{
    public partial class ChatbotManagerWindow
    {
        public class ChatbotManagerTreeViewContextMenuHandler : TreeViewContextMenuHandler
        {
            public ChatbotManagerTreeViewContextMenuHandler(ChatbotManagerTreeView treeView) : base(treeView) { }

            public override IEnumerable<ITreeViewContextMenu> CreateContextMenus()
            {
                yield return new TreeViewAction(TreeViewContextMenuType.Custom)
                {
                    Action = (items, onSuccess) =>
                    {
                        ChatbotMessagesWindow.ShowWindow(
                            items.IsNullOrEmpty() ? null : items[0] is ChatbotManagerTreeViewItem item ? item.Data.Messages : null);
                    },
                    Name = "View Messages",
                    ButtonLabel = "View Messages",
                    ShowInContextMenu = true,
                    ShowInDetailsWindow = true,
                };
                yield return new TreeViewAction(TreeViewContextMenuType.Save)
                {
                    Action = SaveChatbots,
                    Name = "Save",
                    ButtonLabel = "Save Chatbot",
                    ShowInContextMenu = false,
                    ShowInDetailsWindow = true,
                };

                yield return new TreeViewAction(TreeViewContextMenuType.Delete)
                {
                    Action = DeleteChatbots,
                    Name = "Delete",
                    ButtonLabel = "Delete Chatbot",
                    ShowInContextMenu = true,
                    ShowInDetailsWindow = true,
                };
            }

            private void SaveChatbots(ChatbotManagerTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                bool successAtLeastOne = false;

                foreach (var item in items)
                {
                    if (item == null || item.IsInvalid())
                    {
                        continue;
                    }

                    try
                    {
                        item.Data.SaveFile();
                        successAtLeastOne = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to create Chatbot: {e.Message}");
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
            }

            private void DeleteChatbots(ChatbotManagerTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                if (ShowDialog.Confirm("This will delete the selected Chatbots from the OpenAI service.  This action cannot be undone. Are you sure you want to proceed?"))
                {
                    bool successAtLeastOne = false;

                    foreach (var item in items)
                    {
                        if (item == null || item.IsInvalid())
                        {
                            continue;
                        }

                        try
                        {
                            //item.Data.DeleteFile();
                            //AIDevKitDebug.Mark($"Chatbot {item.Data.Id} deleted successfully.");
                            if (ChatSessionManager.DeleteSession(item.Data))
                            {
                                successAtLeastOne = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to delete Chatbot file: {e.Message}");
                            continue;
                        }
                    }

                    if (successAtLeastOne)
                    {
                        _treeView.ReloadTreeView(true, true);
                    }

                    onSuccess?.Invoke(successAtLeastOne);
                }
                else
                {
                    onSuccess?.Invoke(false);
                }
            }
        }
    }
}