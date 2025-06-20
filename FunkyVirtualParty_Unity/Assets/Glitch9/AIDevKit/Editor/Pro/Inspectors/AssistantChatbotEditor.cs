using System.Collections.Generic;
using System.Linq;
using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.Editor.Assistants;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(AssistantChatbot))]
    public class AssistantChatbotEditor : ChatbotEditorBaseV2
    {
        private enum HelpBoxMode { None, ChangeExists, Success, Error, }

        private class Labels
        {
            internal static readonly GUIContent AssistantModel = new("Model", "The OpenAI GPT model to use for the Assistants API.");
            internal static readonly GUIContent Assistant = new("Selected Assistant", "The assistant to use for the OpenAI Assistants API. You need at least one assistant created in the Assistant Manager.");
            internal static readonly GUIContent AssistantName = new("Assistant Name", "The name of the assistant.");
            internal static readonly GUIContent AssistantDescription = new("Description", "A brief description of the assistant's purpose.");
            internal static readonly GUIContent Instructions = new("Instructions", "Instructions for the assistant to follow.");
            internal static readonly GUIContent ResponseFormat = new("Response Format", "The format of the response from the assistant.");
            internal static readonly GUIContent RequiredActionTimeout = new("Required Action Timeout", "Timeout in seconds for required actions. Recommended to be set to 30 seconds or more. Set to 0 for no timeout.");
            internal static readonly GUIContent OnRequiredAction = new("On Required Action", "Event triggered when a Run requires you to submit tool outputs.");
            internal static readonly GUIContent AutoCancelOnRequiredAction = new("Ignore Required Actions", "Automatically cancel the run if a run status changes to RequiredAction. This means the assistant will ignore the required action and continue with the next step.");
        }

        private SerializedProperty model, assistantId, stream;
        private SerializedProperty streamingTextEventReceiver;
        private SerializedProperty assistantEventReceiver, threadEventReceiver, runEventReceiver, messageEventReceiver;
        private SerializedProperty onRequiredAction, requiredActionTimeoutSeconds, autoCancelOnRequiredAction;

        protected override bool Stream
        {
            get => stream.boolValue;
            set => stream.boolValue = value;
        }


        private HelpBoxMode _helpBoxMode = HelpBoxMode.None;

        // Temp Assistant Properties
        private Assistant _selectedAssistant;
        private string _model;
        private string _assistantName;
        private string _assistantDescription;
        private string _instructions;
        private bool _fileSearch;
        private bool _codeInterpreter;
        private TextFormat _responseFormat;
        private float _temperature;
        private float _topP;

        protected override void OnEnable()
        {
            base.OnEnable();

            model = serializedObject.FindProperty(nameof(model));
            assistantId = serializedObject.FindProperty(nameof(assistantId));
            stream = serializedObject.FindProperty(nameof(stream));

            onRequiredAction = serializedObject.FindProperty(nameof(onRequiredAction));
            requiredActionTimeoutSeconds = serializedObject.FindProperty(nameof(requiredActionTimeoutSeconds));
            autoCancelOnRequiredAction = serializedObject.FindProperty(nameof(autoCancelOnRequiredAction));

            streamingTextEventReceiver = serializedObject.FindProperty(nameof(streamingTextEventReceiver));
            assistantEventReceiver = serializedObject.FindProperty(nameof(assistantEventReceiver));
            threadEventReceiver = serializedObject.FindProperty(nameof(threadEventReceiver));
            runEventReceiver = serializedObject.FindProperty(nameof(runEventReceiver));
            messageEventReceiver = serializedObject.FindProperty(nameof(messageEventReceiver));

            _selectedAssistant = AssistantManager.GetAssistant(assistantId.stringValue);
            if (_selectedAssistant != null) UpdateAssistantProperties();


            features.Add(
                new()
                {
                    Icon = AIDevKitIcons.File,
                    Label = "File\nSearch",
                    Tooltip = "Enable file search for the chatbot. This allows the chatbot to search files in the OpenAI API project to provide more accurate responses.",
                    GetValue = () => _fileSearch,
                    SetValue = v =>
                    {
                        _fileSearch = v;
                        _helpBoxMode = HelpBoxMode.ChangeExists; // Mark as changed
                    },
                }
            );

            features.Add(
                new()
                {
                    Icon = AIDevKitIcons.Code,
                    Label = "Code\nInterpreter",
                    Tooltip = "Enable code interpreter for the chatbot. This allows the chatbot to execute code to provide more accurate responses.",
                    GetValue = () => _codeInterpreter,
                    SetValue = v =>
                    {
                        _codeInterpreter = v;
                        _helpBoxMode = HelpBoxMode.ChangeExists; // Mark as changed
                    },
                }
            );
        }

        protected override void DrawUnityComponentLabel()
        {
            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.Assistant, "Chatbot (Assistants API)", "Integrate OpenAI's Assistants API into your project.", DrawAdvancedViewToggle);
        }

        protected override void DrawChatbotProfile()
        {
            ExGUILayout.BeginSection("Assistant Profile");
            {
                string selectedId = assistantId.stringValue;
                string newSelectedId = DrawAssistantField(selectedId, Labels.Assistant);
                if (newSelectedId != selectedId)
                {
                    assistantId.stringValue = newSelectedId;
                    UpdateAssistantProperties();
                }
                EditorGUI.BeginDisabledGroup(_selectedAssistant == null);
                {
                    _assistantName = EditorGUILayout.TextField(Labels.AssistantName, _assistantName);
                    ExGUILayout.ExpandableTextField(Labels.AssistantDescription, _assistantDescription, t => _assistantDescription = t);
                }
                EditorGUI.EndDisabledGroup();
            }
            ExGUILayout.EndSection();
        }

        protected override void DrawStatusMessage()
        {
            if (_helpBoxMode != HelpBoxMode.None)
            {
                GUILayout.BeginHorizontal();
                {
                    if (_helpBoxMode == HelpBoxMode.ChangeExists)
                    {
                        ExGUILayout.HelpBoxEx("Changes in this section will only take effect after updating the assistant. Any unsaved changes will be lost.", MessageTypeEx.Warning);
                    }
                    else if (_helpBoxMode == HelpBoxMode.Success)
                    {
                        ExGUILayout.HelpBoxEx("Assistant updated successfully!", MessageTypeEx.Success);
                    }
                    else if (_helpBoxMode == HelpBoxMode.Error)
                    {
                        ExGUILayout.HelpBoxEx("Failed to update assistant. Please check the console for more details.", MessageTypeEx.Error);
                    }

                    if (GUILayout.Button("Update\nAssistant", GUILayout.Height(38), GUILayout.Width(80))) UpdateAssistant();
                }
                GUILayout.EndHorizontal();
            }
        }

        protected override void DrawChatBehaviour()
        {
            EditorGUI.BeginChangeCheck();
            {
                ExGUILayout.BeginSection("Assistant Profile");
                {
                    EditorGUI.BeginDisabledGroup(_selectedAssistant == null);
                    {
                        _model = AIDevKitGUI.LLMPopup(_model, Api.OpenAI, Labels.AssistantModel);
                        // _responseFormat = (TextFormat)EditorGUILayout.EnumPopup(Labels.ResponseFormat, _responseFormat);
                        // EditorGUILayout.PropertyField(stream, GUIContents.Stream);
                        ExGUILayout.ExpandableTextField(Labels.Instructions, _instructions, t => _instructions = t);

                        if (AdvancedView)
                        {
                            bool[] tools = ExGUILayout.ToggleGroup("Tools", btnLabels: new[] { "File Search", "Code Interpreter" }, values: new[] { _fileSearch, _codeInterpreter });
                            _fileSearch = tools[0];
                            _codeInterpreter = tools[1];

                            Stream = EditorGUILayout.Toggle(GUIContents.Stream, Stream);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                ExGUILayout.EndSection();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _helpBoxMode = HelpBoxMode.ChangeExists;
            }
        }

        protected override void DrawAdvancedOptions()
        {
            EditorGUI.BeginChangeCheck();
            {
                ExGUILayout.BeginSection("Advanced Options");
                {
                    EditorGUI.BeginDisabledGroup(_selectedAssistant == null);
                    {
                        _responseFormat = (TextFormat)EditorGUILayout.EnumPopup(Labels.ResponseFormat, _responseFormat);
                        _temperature = ExGUILayout.ResettableField(GUIContents.Temperature, _temperature, AIDevKitConfig.TemperatureDefault,
                            (v) => EditorGUILayout.Slider(v, AIDevKitConfig.TemperatureMin, AIDevKitConfig.TemperatureMax));
                        _topP = ExGUILayout.ResettableField(GUIContents.TopP, _topP, AIDevKitConfig.TopPDefault,
                            (v) => EditorGUILayout.Slider(v, AIDevKitConfig.TopPMin, AIDevKitConfig.TopPMax));
                    }
                    EditorGUI.EndDisabledGroup();
                }
                ExGUILayout.EndSection();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _helpBoxMode = HelpBoxMode.ChangeExists;
            }
        }

        protected override void DrawStreamReceiver()
        {
            EditorGUILayout.PropertyField(streamingTextEventReceiver, GUIContents.StreamEventReceiver);
        }

        protected override void DrawExtraSettings()
        {
            DrawLifeCycleReceivers();
            DrawRequiredActionSettings();
        }

        private void UpdateAssistantProperties()
        {
            if (_selectedAssistant == null) return;

            // Update properties from selected assistant
            _model = _selectedAssistant.Model;
            _assistantName = _selectedAssistant.Name;
            _assistantDescription = _selectedAssistant.Description;
            _instructions = _selectedAssistant.Instructions;
            _fileSearch = _selectedAssistant.FileSearchEnabled;
            _codeInterpreter = _selectedAssistant.CodeInterpreterEnabled;
            _responseFormat = _selectedAssistant.ResponseFormat.ToEnum<TextFormat>();
            _temperature = _selectedAssistant.Temperature ?? AIDevKitConfig.TemperatureDefault;
            _topP = _selectedAssistant.TopP ?? AIDevKitConfig.TopPDefault;
        }



        private async void UpdateAssistant()
        {
            if (_selectedAssistant != null)
            {
                string id = _selectedAssistant.Id;
                AssistantRequest req = new AssistantRequest.Builder()
                    .SetModel(_model)
                    .SetName(_assistantName)
                    .SetDescription(_assistantDescription)
                    .SetInstructions(_instructions)
                    .SetFileSearchEnabled(_fileSearch)
                    .SetCodeInterpreterEnabled(_codeInterpreter)
                    .SetResponseFormat(_responseFormat)
                    .SetTemperature(_temperature)
                    .SetTopP(_topP)
                    .Build();

                if (await AssistantManager.UpdateAssistantAsync(id, req))
                {
                    _helpBoxMode = HelpBoxMode.Success;
                }
                else
                {
                    _helpBoxMode = HelpBoxMode.Error;
                }
            }
        }

        // protected override void DrawEventReceivers()
        // {
        //     ExGUILayout.BeginSection("Event Receivers");
        //     {
        //         EditorGUILayout.PropertyField(chatEventReceiver, GUIContents.ChatEventReceiver);
        //         EditorGUILayout.PropertyField(streamingTextEventReceiver, GUIContents.StreamEventReceiver);
        //         EditorGUILayout.PropertyField(toolCallReceiver, GUIContents.ToolCallReceiver);
        //         EditorGUILayout.PropertyField(functionManager, GUIContents.FunctionManager);
        //     }
        //     ExGUILayout.EndSection();
        // }

        private void DrawLifeCycleReceivers()
        {
            ExGUILayout.BeginSection("Life Cycle Event Receivers");
            {
                EditorGUILayout.PropertyField(assistantEventReceiver);
                EditorGUILayout.PropertyField(threadEventReceiver);
                EditorGUILayout.PropertyField(runEventReceiver);
                EditorGUILayout.PropertyField(messageEventReceiver);
            }
            ExGUILayout.EndSection();
        }

        private void DrawRequiredActionSettings()
        {
            ExGUILayout.BeginSection("Required Action Handling");
            {
                EditorGUILayout.PropertyField(autoCancelOnRequiredAction, Labels.AutoCancelOnRequiredAction);
                EditorGUI.BeginDisabledGroup(autoCancelOnRequiredAction.boolValue);
                {
                    EditorGUILayout.PropertyField(requiredActionTimeoutSeconds, Labels.RequiredActionTimeout);
                    EditorGUILayout.PropertyField(onRequiredAction, Labels.OnRequiredAction);
                }
                EditorGUI.EndDisabledGroup();
            }
            ExGUILayout.EndSection();
        }

        internal string DrawAssistantField(string selectedId, GUIContent label)
        {
            Assistant assistant = null;
            int savedIndent = EditorGUI.indentLevel;

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUI.indentLevel = 0;
                assistant = DrawAssistantDropdown(selectedId);
            }
            finally
            {
                // restore indent level
                EditorGUI.indentLevel = savedIndent;
                GUILayout.EndHorizontal();
            }

            return assistant?.Id;
        }

        private Assistant DrawAssistantDropdown(string selectedId)
        {
            GUIStyle dropdownStyle = EditorStyles.popup;
            GUIStyle btnStyle = EditorStyles.miniButtonRight;
            float btnWidth = 20f;

            List<Assistant> allAssistants = AssistantManager.Assistants;

            if (allAssistants.Count == 0)
            {
                DrawNoAssistants(btnStyle, btnWidth);
                return null;
            }

            _selectedAssistant = allAssistants.FirstOrDefault(a => a.Id == selectedId);

            // Fallback to first valid model if selected is null
            if (_selectedAssistant == null || string.IsNullOrEmpty(_selectedAssistant.Id))
            {
                _selectedAssistant = allAssistants.FirstOrDefault();
            }

            // If still null after all fallback attempts
            if (_selectedAssistant == null)
            {
                DrawNoAssistants(btnStyle, btnWidth);
                return null;
            }

            List<string> displayOptions = allAssistants.Select((m, i) => $"{m.Name} ({i})").ToList();

            int selectedAssetIndex = allAssistants.IndexOf(_selectedAssistant);
            if (selectedAssetIndex < 0) selectedAssetIndex = 0;
            int newAssetIndex = EditorGUILayout.Popup(selectedAssetIndex, displayOptions.ToArray(), dropdownStyle, GUILayout.ExpandWidth(true));

            if (newAssetIndex != selectedAssetIndex) _selectedAssistant = allAssistants[newAssetIndex];

            if (GUILayout.Button(AIDevKitIcons.Assistant, btnStyle, GUILayout.Width(btnWidth)))
            {
                AssistantManagerWindow.ShowWindow();
            }

            return _selectedAssistant;
        }

        private void DrawNoAssistants(GUIStyle btnStyle, float btnWidth)
        {
            ExGUILayout.ErrorLabel($"You have no assistants. Create one in the Assistant Manager.");
            if (GUILayout.Button(AIDevKitIcons.Assistant, btnStyle, GUILayout.Width(btnWidth)))
            {
                AssistantManagerWindow.ShowWindow();
            }
        }

        // protected override bool HandleStreamToggleChange(bool oldValue, bool newValue)
        // {
        //     if (oldValue == newValue) return oldValue; // No change

        //     if (newValue)
        //     {
        //         if (streamingTextEventReceiver.objectReferenceValue == null)
        //         {
        //             streamingTextEventReceiver.objectReferenceValue = GetOrCreateModule<ChatCompletionStreamReceiver>();
        //             serializedObject.ApplyModifiedProperties();
        //         }
        //     }
        //     else
        //     {
        //         streamingTextEventReceiver.objectReferenceValue = null;
        //     }

        //     return newValue; // Return the new value after applying changes
        // }

        protected override bool HandleStreamToggleChange(bool oldValue, bool newValue) => HandleModuleToggleChange<ChatCompletionStreamReceiver>(streamingTextEventReceiver, oldValue, newValue);


    }
}