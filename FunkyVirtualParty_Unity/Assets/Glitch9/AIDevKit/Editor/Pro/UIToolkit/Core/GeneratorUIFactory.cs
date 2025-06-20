using System;
using System.Collections.Generic;
using System.Linq;
using Glitch9.AIDevKit.Editor.Pro;
using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using Glitch9.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    // GUI Renderer for the Generator Window.
    internal static class GeneratorUIFactory
    {
        // Icon Buttons -----------------------------------------------------------
        internal static Button CreateSaveButton(Action onClick) => CreateIconButton("save-button", "Save selected generated content(s) into project", EditorIcons.Import as Texture2D, onClick);
        internal static Button CreateSamplePromptButton(Action onClick) => CreateIconButton("sample-prompt-button", "Enter Sample Prompt", AIDevKitIcons.Gemini, onClick);
        internal static Button CreateOpenPathButton() => CreateIconButton("open-path-button", "Open Output Path", EditorIcons.Folder as Texture2D, () => EditorUtility.RevealInFinder(AIDevKitSettings.OutputPath + "/"));

        private static Button CreateIconButton(string name, string tooltip, Texture2D icon, Action onClick)
        {
            var button = new Button()
            {
                name = name,
                tooltip = tooltip,
                style = { backgroundImage = icon }
            };
            button.AddToClassList("icon-button");
            button.clicked += onClick;
            return button;
        }

        // Big Icon Buttons -----------------------------------------------------------
        internal static Button CreateDiscordButton() => CreateBigIconButton("discord-button", "Join Discord", EditorTextures.Discord, () => Application.OpenURL(EditorConfig.DiscordUrl));
        internal static Button CreatePreferencesButton() => CreateBigIconButton("preferences-button", "Open Preferences", EditorTextures.Preferences, AIDevKitEditor.ShowPreferencesWindow);
        internal static Button CreateOnlineDocButton() => CreateBigIconButton("online-doc-button", "Open Online Documentation", EditorTextures.Info, () => Application.OpenURL(AIDevKitEditor.OnlineDocUrl));
        internal static Button CreateReloadUIButton(Action rebuildUI) => CreateBigIconButton("reload-ui-button", "Reload UI", EditorTextures.Reload, rebuildUI);

        private static Button CreateBigIconButton(string name, string tooltip, Texture2D icon, Action onClick)
        {
            var button = new Button()
            {
                name = name,
                tooltip = tooltip,
                style = { backgroundImage = icon }
            };
            button.AddToClassList("big-icon-button");
            button.clicked += onClick;
            return button;
        }

        internal static void SetupRequestDetails(VisualElement container, PromptRecord currentRecord)
        {
            container.Clear();
            if (currentRecord == null)
            {
                container.Add(new Label("No record selected."));
                return;
            }

            Dictionary<string, string> details = new();

            string modelName = currentRecord.ModelName;
            if (!string.IsNullOrEmpty(modelName) && modelName != "Unknown")
            {
                details.Add(GUILabels.Model, $"{modelName} ({currentRecord.Api.GetInspectorName()}, {currentRecord.ModelId})");
            }

            if (!string.IsNullOrEmpty(currentRecord.InputText))
            {
                details.Add(GUILabels.Prompt, currentRecord.InputText);
            }

            if (currentRecord.RequestOptions.IsNotNullOrEmpty())
            {
                string options = string.Join(", ", currentRecord.RequestOptions.Select(x => $"{x.Value} ({x.Key})") ?? Array.Empty<string>());
                details.Add(GUILabels.Options, options);
            }

            if (currentRecord.Price != null)
            {
                details.Add(GUILabels.Price, currentRecord.Price?.ToString());
            }

            if (details.Count == 0)
            {
                container.Add(new Label("No details available."));
                return;
            }

            foreach (var detail in details)
            {
                var infoLabel = new InfoLabel(
                    key: detail.Key,
                    value: detail.Value,
                    tooltip: detail.Value,
                    keyClass: "request-details-info-label-key",
                    valueClass: "request-details-info-label-value"
                );
                container.Add(infoLabel);
            }
        }

        // IMGUI Request Details -----------------------------------------------------------
        internal static void DrawRequestDetails(PromptRecord currentRecord)
        {
            GUILayout.BeginVertical();
            try
            {
                if (currentRecord == null)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No record selected.");
                    GUILayout.FlexibleSpace();
                    return;
                }

                GUILayout.Space(3f);
                string modelName = currentRecord.ModelName;

                if (!string.IsNullOrEmpty(modelName) && modelName != "Unknown")
                {
                    AIDevKitGUI.CopiableLabelField(GUILabels.Model, $"{modelName} ({currentRecord.Api.GetInspectorName()}, {currentRecord.ModelId})");
                }

                if (!string.IsNullOrEmpty(currentRecord.InputText))
                {
                    AIDevKitGUI.CopiableLabelField(GUILabels.Prompt, currentRecord.InputText);
                }

                if (currentRecord.RequestOptions.IsNotNullOrEmpty())
                {
                    string options = string.Join(", ", currentRecord.RequestOptions.Select(x => $"{x.Value} ({x.Key})") ?? Array.Empty<string>());
                    AIDevKitGUI.CopiableLabelField(GUILabels.Options, options);
                }

                if (currentRecord.Price != null)
                {
                    AIDevKitGUI.CopiableLabelField(GUILabels.Price, currentRecord.Price?.ToString());
                }
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }

        internal static Label CreatePromptHistoryItem(Action rebuildUI)
        {
            var label = new Label();

            label.RegisterCallback<ContextClickEvent>(evt =>
            {
                label.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    evt.menu.AppendAction("Archive History", action =>
                    {
                        if (label.userData is not PromptRecord record)
                        {
                            Debug.LogWarning("No record found for the selected label.");
                            return;
                        }

                        record.Archive();
                        rebuildUI?.Invoke();
                    });

                    evt.menu.AppendAction("Delete History", action =>
                    {
                        if (label.userData is not PromptRecord record)
                        {
                            Debug.LogWarning("No record found for the selected label.");
                            return;
                        }

                        if (PromptHistory.Remove(record))
                        {
                            rebuildUI?.Invoke();
                            AIDevKitDebug.Blue($"Deleted prompt record: {record.InputText}");
                        }
                    });
                }));

                evt.StopPropagation();
            });

            label.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    if (label.userData is not PromptRecord record)
                    {
                        Debug.LogWarning("No record found for the selected label.");
                        return;
                    }

                    try
                    {
                        string windowTitle = $"Prompt History - {GeneratorUtil.FormatUnixTime(record.CreatedAt)}";
                        var window = EditorWindow.GetWindow<PromptHistoryWindow.PromptHistoryTreeViewDetailsWindow>(false, windowTitle, true);
                        window.minSize = new Vector2(400, 400);
                        window.maxSize = new Vector2(800, 800);
                        window.SetData(record);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error creating window: {e.Message}");
                    }
                    //Debug.Log("Double click detected!"); 
                }
            });

            return label;
        }

        internal static void BindPromptHistoryItem(VisualElement element, PromptRecord record)
        {
            if (element is Label label)
            {
                label.text = GeneratorUtil.ResolveLabelText(record);
                label.tooltip = record.InputText;
                label.userData = record;
                label.AddToClassList("prompt-history-item");
            }
        }
    }
}