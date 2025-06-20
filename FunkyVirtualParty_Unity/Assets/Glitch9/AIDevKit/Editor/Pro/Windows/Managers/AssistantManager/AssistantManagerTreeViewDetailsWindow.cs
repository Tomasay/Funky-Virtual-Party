using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Assistants
{
    public partial class AssistantManagerWindow
    {
        public class AssistantManagerTreeViewDetailsWindow : ExtendedTreeViewDetailsWindow
        {
            private static class Labels
            {
                internal static readonly GUIContent Name = new(
                    "Name",
                    "The name of the assistant. The maximum length is 64 characters.");

                internal static readonly GUIContent Description = new(
                    "Description",
                    "The description of the assistant. The maximum length is 512 characters.");
                internal static readonly GUIContent Temperature = new(
                    "Temperature",
                    "What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.");

                internal static readonly GUIContent TopP = new("Top P",
                    "An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass." +
                    "So 0.1 means only the tokens comprising the top 10% probability mass are considered.");

                internal static readonly GUIContent Tools = new("Tools",
                    "A list of tool enabled on the assistant. There can be a maximum of 128 tools per assistant. Tools can be of types code_interpreter, file_search, or function.");

                internal static readonly GUIContent Metadata = new(
                    "Metadata", "Set of 16 key-value pairs that can be attached to an object. " +
                    "This can be useful for storing additional information about the object in a structured format. " +
                    "Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.");

                internal static readonly GUIContent ResponseFormat = new(
                    "Response Format",
                    "Specifies the format that the model must output. Compatible with GPT-4o, GPT-4 Turbo, and all GPT-3.5 Turbo models since gpt-3.5-turbo-1106." +
                    "Setting to { \"type\": \"json_object\" } enables JSON mode, which guarantees the message the model generates is valid JSON." +
                    "Important: when using JSON mode, you must also instruct the model to produce JSON yourself via a system or user message. Without this, " +
                    "the model may generate an unending stream of whitespace until the generation reaches the token limit, resulting in a long-running and seemingly \"stuck\" request. " +
                    "Also note that the message content may be partially cut off if finish_reason=\"length\", " +
                    "which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.");
            }

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
            private ReorderableList toolsReorderableList;
            private ReorderableList metadataReorderableList;

            protected override void DrawSubtitle()
            {
                TreeViewGUI.SubtitleLeft($"Id: {Data.Id}");
                TreeViewGUI.SubtitleLeft($"Created At: {DateString}");
            }

            protected override void DrawBody()
            {
                // Model
                EditingData.Model = AIDevKitGUI.LLMPopup(EditingData.Model, Api.OpenAI, GUIContents.Model);

                // Name
                Data.Name = EditorGUILayout.TextField(Labels.Name, Data.Name);

                // Description 
                ExGUILayout.ExpandableTextField(Labels.Description, EditingData.Description, t => EditingData.Description = t, GUILayout.Height(TEXT_AREA_HEIGHT));

                // Instructions 
                ExGUILayout.ExpandableTextField(GUIContents.Instructions, EditingData.Instructions, t => EditingData.Instructions = t, GUILayout.Height(TEXT_AREA_HEIGHT));

                GUILayout.Space(2);

                // Response Format
                TextFormat responseFormat = EditingData.ResponseFormat.ToEnum<TextFormat>();
                EditingData.ResponseFormat = (TextFormat)EditorGUILayout.EnumPopup(Labels.ResponseFormat, responseFormat);

                // Temperature
                EditingData.Temperature ??= AIDevKitConfig.TemperatureDefault;
                EditingData.Temperature = EditorGUILayout.Slider(Labels.Temperature, EditingData.Temperature.Value, 0.0f, 1.0f);

                // Top P
                EditingData.TopP ??= AIDevKitConfig.TopPDefault;
                EditingData.TopP = EditorGUILayout.Slider(Labels.TopP, EditingData.TopP.Value, 0.0f, 1.0f);

                // // Code Interpreter
                // EditingData.CodeInterpreterEnabled = EditorGUILayout.Toggle("Code Interpreter", EditingData.CodeInterpreterEnabled);
                // EditingData.FileSearchEnabled = EditorGUILayout.Toggle("File Search", EditingData.FileSearchEnabled);


                GUILayout.Space(5);

                // ToolCalls
                //EditorGUILayout.LabelField(GUIContents.kTools);
                DrawTools(EditingData.Tools);

                GUILayout.Space(5);

                // Metadata
                EditingData.Metadata ??= new();
                metadataReorderableList ??= ExGUIUtility.CreateReorderableDictionary(EditingData.Metadata, Labels.Metadata);
                metadataReorderableList.DoLayoutList();
            }

            private void DrawTools(List<ToolCall> tools)
            {
                tools ??= new();
                toolsReorderableList ??= ExGUIUtility.CreateReorderableArray(tools.ToArray(), ToolCustomDrawer, (editedArray) => EditingData.Tools = editedArray.ToList(), Labels.Tools);
                toolsReorderableList.DoLayoutList();
            }

            private ToolCall ToolCustomDrawer(Rect rect, int index, ToolCall toolCall)
            {
                if (toolCall == null) return null;

                if (toolCall is FunctionCall functionCall)
                {
                    Rect[] rects = rect.SplitHorizontally(2, 0.3f, 0.7f);
                    toolCall.Type = (ToolType)EditorGUI.EnumPopup(rects[0], toolCall.Type);

                    if (GUI.Button(rects[1], functionCall.Name, EditorStyles.miniButton))
                    {
                        FunctionToolWindow.ShowWindow(functionCall.Function);
                    }
                }
                else
                {
                    toolCall.Type = (ToolType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), toolCall.Type);
                }

                return toolCall;
            }
        }
    }
}