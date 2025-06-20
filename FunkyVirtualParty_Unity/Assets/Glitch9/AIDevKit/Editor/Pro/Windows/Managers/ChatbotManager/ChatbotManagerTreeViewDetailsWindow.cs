using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Chatbots
{
    public partial class ChatbotManagerWindow
    {
        public class ChatbotManagerTreeViewDetailsWindow : ExtendedTreeViewDetailsWindow
        {

            private const float TEXT_AREA_HEIGHT = 60f;

            private string DateString
            {
                get
                {
                    if (string.IsNullOrEmpty(dateString))
                    {
                        dateString = Data?.CreatedAt == null ? "Unknown" : Data.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    return dateString;
                }
            }
            private string dateString;
            private ModelSettings ModelOptions => GetModelOptions();

            private ModelSettings GetModelOptions()
            {
                EditingData.ModelOptions ??= new ModelSettings();
                return EditingData.ModelOptions;
            }

            protected override void DrawSubtitle()
            {
                TreeViewGUI.Subtitle($"Id: {Data.Id}", $"Created At: {DateString}");
            }

            protected override void DrawBody()
            {
                if (EditingData == null)
                {
                    ExGUILayout.HelpBoxExBig("No Chat Session selected. Please select a Chatbot from the list.", MessageTypeEx.Warning);
                    return;
                }

                var session = EditingData;

                TreeViewGUI.BeginSection("Chat Session Profile");
                {
                    // Model
                    EditingData.Model = AIDevKitGUI.LLMPopup(EditingData.Model, Api.All, GUIContents.ChatModel);
                    EditingData.UtilityModel = AIDevKitGUI.LLMPopup(EditingData.UtilityModel, Api.All, GUIContents.UtilityModel);

                    ReasoningEffort newReasoningEffort = ExGUILayout.EnumPopup(GUIContents.ReasoningEffort, session.ReasoningOptions?.Effort ?? ReasoningEffort.Medium);
                    if (newReasoningEffort != session.ReasoningOptions?.Effort)
                    {
                        if (newReasoningEffort == ReasoningEffort.Medium)
                        {
                            session.ReasoningOptions = null;
                        }
                        else
                        {
                            session.ReasoningOptions ??= new ReasoningOptions();
                            session.ReasoningOptions.Effort = newReasoningEffort;
                        }
                        session.SaveFile();
                    }

                    Data.AutoTitle = EditorGUILayout.Toggle(GUIContents.AutoTitle, Data.AutoTitle);

                    if (!Data.AutoTitle)
                    {
                        EditorGUI.indentLevel++;
                        Data.Name = EditorGUILayout.TextField(GUIContents.ChatTitle, Data.Name);
                        EditorGUI.indentLevel--;
                    }

                    // Instructions 
                    ExGUILayout.ExpandableTextField(GUIContents.Instructions, EditingData.Instructions, t => EditingData.Instructions = t, GUILayout.Height(TEXT_AREA_HEIGHT));

                    // Starting Message
                    ExGUILayout.ExpandableTextField(GUIContents.StartingMessage, EditingData.StartingMessage, t => EditingData.StartingMessage = t, GUILayout.Height(TEXT_AREA_HEIGHT));
                }
                TreeViewGUI.EndSection();

                TreeViewGUI.BeginSection("Context Settings");
                {

                    EditingData.MaxContextMessages = EditorGUILayout.IntSlider("Max Context Messages", EditingData.MaxContextMessages, 10, 100);
                }
                TreeViewGUI.EndSection();


                TreeViewGUI.BeginSection("Advanced Options");
                {
                    if (ModelOptions == null)
                    {
                        ExGUILayout.HelpBoxExBig("There was an error loading the Model Options. Please check the console for more details.", MessageTypeEx.Error);
                    }
                    else
                    {
                        ModelOptions.MaxTokens = ExGUILayout.NullableField(GUIContents.MaxTokens, ModelOptions.MaxTokens, -1, (v) => EditorGUILayout.IntField(v));
                        ModelOptions.Temperature = ExGUILayout.NullableField(GUIContents.Temperature, ModelOptions.Temperature, AIDevKitConfig.TemperatureDefault, (v) => EditorGUILayout.Slider(v, 0f, 2f));
                        ModelOptions.TopP = ExGUILayout.NullableField(GUIContents.TopP, ModelOptions.TopP, AIDevKitConfig.TopPDefault, (v) => EditorGUILayout.Slider(v, 0f, 1f));
                        ModelOptions.FrequencyPenalty = ExGUILayout.NullableField(GUIContents.FrequencyPenalty, ModelOptions.FrequencyPenalty, AIDevKitConfig.FrequencyPenaltyDefault, (v) => EditorGUILayout.Slider(v, -2f, 2f));
                    }
                }
                TreeViewGUI.EndSection();

                TreeViewGUI.BeginSection("Information");
                {
                    EditorGUILayout.LabelField("Messages Count", EditingData.Messages?.Count.ToString() ?? "0");
                    EditorGUILayout.LabelField("Created Date", EditingData.CreatedAt.ToString("f"));
                    EditorGUILayout.LabelField("Last Updated", EditingData.UpdatedAt.ToString("f"));
                    EditorGUILayout.LabelField("Last Summary Update", EditingData.LastSummaryUpdate.ToString("f"));
                }
                TreeViewGUI.EndSection();

                if (!string.IsNullOrEmpty(EditingData.LastMessage))
                {
                    TreeViewGUI.BeginSectionWithoutBox("Last Message");
                    {
                        EditorGUILayout.LabelField(EditingData.LastMessage, ExStyles.wordWrappedTextField);
                    }
                    TreeViewGUI.EndSection();
                }

                if (!string.IsNullOrEmpty(EditingData.Summary))
                {
                    TreeViewGUI.BeginSectionWithoutBox("Summary");
                    {
                        EditorGUILayout.LabelField(EditingData.Summary, ExStyles.wordWrappedTextField);
                    }
                    TreeViewGUI.EndSection();
                }
            }
        }
    }
}