using System;
using System.Collections.Generic;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEngine;
using OpenAIClient = Glitch9.AIDevKit.OpenAI.OpenAI;

namespace Glitch9.AIDevKit.Editor.Assistants
{
    public partial class AssistantManagerWindow
    {
        public class AssistantManagerTreeViewContextMenuHandler : TreeViewContextMenuHandler
        {
            public AssistantManagerTreeViewContextMenuHandler(AssistantManagerTreeView treeView) : base(treeView) { }

            public override IEnumerable<ITreeViewContextMenu> CreateContextMenus()
            {
                yield return new TreeViewAction(TreeViewContextMenuType.Save)
                {
                    Action = UpdateAssistants,
                    Name = "Update",
                    ButtonLabel = "Update Assistant",
                    ShowInContextMenu = false,
                    ShowInDetailsWindow = true,
                };

                yield return new TreeViewAction(TreeViewContextMenuType.Delete)
                {
                    Action = DeleteAssistants,
                    Name = "Delete",
                    ButtonLabel = "Delete Assistant",
                    ShowInContextMenu = true,
                    ShowInDetailsWindow = true,
                };
            }

            private async void UpdateAssistants(AssistantManagerTreeViewItem[] items, Action<bool> onSuccess)
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

                    Assistant data = item.Data;
                    string assistantId = data.Id;

                    AssistantRequest modifyReqBuilder = new AssistantRequest.Builder()
                        .SetName(data.Name)
                        .SetModel(data.Model)
                        .SetDescription(data.Description)
                        .SetTemperature(data.Temperature)
                        .SetTopP(data.TopP)
                        .SetTools(data.Tools)
                        .SetMetadata(data.Metadata)
                        .SetResponseFormat(data.ResponseFormat)
                        .SetFileSearchEnabled(data.FileSearchEnabled)
                        .SetCodeInterpreterEnabled(data.CodeInterpreterEnabled)
                        .Build();

                    try
                    {
                        Assistant assistant = await OpenAIClient.DefaultInstance.Beta.Assistants.UpdateAsync(assistantId, modifyReqBuilder);
                        successAtLeastOne = assistant != null;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to create assistant: {e.Message}");
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
            }

            private async void DeleteAssistants(AssistantManagerTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                if (ShowDialog.Confirm("This will delete the selected assistants from the OpenAI service.  This action cannot be undone. Are you sure you want to proceed?"))
                {
                    bool successAtLeastOne = false;

                    foreach (var item in items)
                    {
                        if (item == null || item.IsInvalid())
                        {
                            continue;
                        }

                        Assistant data = item.Data;
                        string assistantId = data.Id;

                        bool success = await OpenAIClient.DefaultInstance.Beta.Assistants.DeleteAsync(assistantId);

                        if (success)
                        {
                            AIDevKitDebug.Mark($"Assistant {assistantId} deleted successfully.");
                            AssistantManager.RemoveAssistantLocally(data.Id);
                            successAtLeastOne = true;
                        }
                    }

                    if (successAtLeastOne)
                    {
                        AssistantManager.Save();
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