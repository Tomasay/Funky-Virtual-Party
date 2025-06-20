using System.Collections.Generic;
using System.Linq;
using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.Editor.Chatbots;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(Chatbot)), CanEditMultipleObjects]
    public class ChatbotEditor : ChatbotEditorBaseV2
    {
        private SerializedProperty chatSessionId;
        private SerializedProperty webSearchModule;
        private bool WebSearch;

        private ChatSession _selectedSession;
        private ModelSettings ModelOptions => GetModelOptions();
        private Model CurrentModel => _selectedSession?.Model;
        private SerializedProperty streamReceiver;
        protected override bool Stream
        {
            get => _selectedSession?.Stream ?? false;
            set
            {
                if (_selectedSession != null)
                {
                    _selectedSession.Stream = value;
                    _selectedSession.SaveFile();
                }
            }
        }

        private ModelSettings GetModelOptions()
        {
            if (_selectedSession == null) return null;
            _selectedSession.ModelOptions ??= new ModelSettings();
            return _selectedSession.ModelOptions;
        }

        private string ChatSessionId => chatSessionId?.stringValue;

        protected override void OnEnable()
        {
            base.OnEnable();

            chatSessionId = serializedObject.FindProperty(nameof(chatSessionId));
            if (!string.IsNullOrWhiteSpace(ChatSessionId)) _selectedSession = ChatSession.LoadFile(ChatSessionId);

            streamReceiver = serializedObject.FindProperty(nameof(streamReceiver));
            webSearchModule = serializedObject.FindProperty(nameof(webSearchModule));
            WebSearch = webSearchModule.objectReferenceValue != null;

            features.Add(
                new()
                {
                    Icon = AIDevKitIcons.WebSearch,
                    Label = "Web\nSearch",
                    Tooltip = "Enable web search for the chatbot. This allows the chatbot to perform web searches to provide more accurate responses.",
                    GetValue = () => WebSearch,
                    SetValue = v => WebSearch = v,
                    HandleChange = (oldVal, newVal) => HandleModuleToggleChange<WebSearch>(webSearchModule, oldVal, newVal)
                }
            );
        }

        protected override void DrawStatusMessage()
        {
            if (Stream && (CurrentModel == null || !CurrentModel.HasFeature(ModelFeature.Streaming)))
            {
                ExGUILayout.HelpBoxEx(
                    "The selected chatbot model may not support streaming responses. If you're confident that it does, feel free to ignore this message. " +
                    "For accurate details, please refer to the Model Manager or the official documentation.",
                    MessageTypeEx.Warning);
            }

            if (Functions && (CurrentModel == null || !CurrentModel.HasFeature(ModelFeature.FunctionCalling)))
            {
                ExGUILayout.HelpBoxEx(
                    "The selected chatbot model may not support function calling. If you're confident that it does, feel free to ignore this message. " +
                    "For accurate details, please refer to the Model Manager or the official documentation.",
                    MessageTypeEx.Warning);
            }

            if (WebSearch && (CurrentModel == null || !CurrentModel.HasFeature(ModelFeature.Search)))
            {
                ExGUILayout.HelpBoxEx(
                    "The selected chatbot model may not support web search. If you're confident that it does, feel free to ignore this message. " +
                    "For accurate details, please refer to the Model Manager or the official documentation.",
                    MessageTypeEx.Warning);
            }
        }

        protected override void DrawUnityComponentLabel()
        {
            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.Chatbot, "Chatbot", "Integrate AI Chatbots into your game.", DrawAdvancedViewToggle);
        }

        protected override void DrawChatbotProfile()
        {
            ExGUILayout.BeginSection("Chat Session Profile");
            {
                string selectedId = chatSessionId.stringValue;
                string newSelectedId = DrawChatbotField(selectedId, new GUIContent("Selected Session", "The currently selected Chatbot session. You can create or load a session from the dropdown."));
                if (newSelectedId != selectedId) chatSessionId.stringValue = newSelectedId;


                if (_selectedSession == null)
                {
                    ExGUILayout.ErrorLabel("No session selected. Please select a session from the dropdown above.");
                }
                else
                {
                    string newName = EditorGUILayout.TextField("Session Name", _selectedSession.Name);
                    if (newName != _selectedSession.Name) { _selectedSession.Name = newName; _selectedSession.SaveFile(); }

                    int newMaxContextMessages = EditorGUILayout.IntSlider(GUIContents.MaxContextMessages, _selectedSession.MaxContextMessages, 10, 100);
                    if (newMaxContextMessages != _selectedSession.MaxContextMessages) { _selectedSession.MaxContextMessages = newMaxContextMessages; _selectedSession.SaveFile(); }

                    bool newAutoSave = EditorGUILayout.Toggle(GUIContents.ChatSessionAutoSave, _selectedSession.AutoSave);
                    if (newAutoSave != _selectedSession.AutoSave) { _selectedSession.AutoSave = newAutoSave; _selectedSession.SaveFile(); }
                }
            }
            ExGUILayout.EndSection();
        }

        protected override void DrawChatBehaviour()
        {
            if (_selectedSession == null) return;

            ExGUILayout.BeginSection("Chat Behavior");
            {
                Model newModel = AIDevKitGUI.LLMPopup(_selectedSession.Model, label: new GUIContent("Chatbot Model", tooltip: "The LLM model used for the chatbot. This should be a chat-capable model."));
                if (newModel != _selectedSession.Model) { _selectedSession.Model = newModel; _selectedSession.SaveFile(); }

                Model summaryModel = AIDevKitGUI.LLMPopup(_selectedSession.UtilityModel, label: GUIContents.UtilityModel);
                if (summaryModel != _selectedSession.UtilityModel) { _selectedSession.UtilityModel = summaryModel; _selectedSession.SaveFile(); }

                // bool newStream = EditorGUILayout.Toggle(GUIContents.Stream, _selectedSession.Stream);
                // if (newStream != _selectedSession.Stream) { _selectedSession.Stream = newStream; _selectedSession.SaveFile(); }

                ExGUILayout.ExpandableTextField(GUIContents.Instructions, _selectedSession.Instructions, (newInstruction) =>
                {
                    if (newInstruction != _selectedSession.Instructions)
                    {
                        _selectedSession.Instructions = newInstruction;
                        _selectedSession.SaveFile();
                    }
                });

                ExGUILayout.ExpandableTextField(GUIContents.StartingMessage, _selectedSession.StartingMessage, (newStartingMessage) =>
                {
                    if (newStartingMessage != _selectedSession.StartingMessage)
                    {
                        _selectedSession.StartingMessage = newStartingMessage;
                        _selectedSession.SaveFile();
                    }
                });

                if (AdvancedView) Stream = EditorGUILayout.Toggle(GUIContents.Stream, Stream);
            }
            ExGUILayout.EndSection();
        }

        protected override void DrawAdvancedOptions()
        {
            if (_selectedSession == null) return;

            ExGUILayout.BeginSection("Advanced Options");
            {
                if (ModelOptions == null)
                {
                    ExGUILayout.HelpBoxExBig("There was an error loading the Model Options. Please check the console for more details.", MessageTypeEx.Error);
                }
                else
                {
                    ReasoningEffort newReasoningEffort = ExGUILayout.EnumPopup(GUIContents.ReasoningEffort, _selectedSession.ReasoningOptions?.Effort ?? ReasoningEffort.Medium);
                    if (newReasoningEffort != _selectedSession.ReasoningOptions?.Effort)
                    {
                        if (newReasoningEffort == ReasoningEffort.Medium)
                        {
                            _selectedSession.ReasoningOptions = null;
                        }
                        else
                        {
                            _selectedSession.ReasoningOptions ??= new ReasoningOptions();
                            _selectedSession.ReasoningOptions.Effort = newReasoningEffort;
                        }
                        _selectedSession.SaveFile();
                    }

                    int? maxTokens = ExGUILayout.NullableField(GUIContents.MaxTokens, ModelOptions.MaxTokens, -1, (v) => EditorGUILayout.IntField(v));
                    if (maxTokens != ModelOptions.MaxTokens) { ModelOptions.MaxTokens = maxTokens; _selectedSession.SaveFile(); }

                    float? temperature = ExGUILayout.NullableField(GUIContents.Temperature, ModelOptions.Temperature, AIDevKitConfig.TemperatureDefault, (v) => EditorGUILayout.Slider(v, 0f, 2f));
                    if (temperature != ModelOptions.Temperature) { ModelOptions.Temperature = temperature; _selectedSession.SaveFile(); }

                    float? topP = ExGUILayout.NullableField(GUIContents.TopP, ModelOptions.TopP, AIDevKitConfig.TopPDefault, (v) => EditorGUILayout.Slider(v, 0f, 1f));
                    if (topP != ModelOptions.TopP) { ModelOptions.TopP = topP; _selectedSession.SaveFile(); }

                    float? frequencyPenalty = ExGUILayout.NullableField(GUIContents.FrequencyPenalty, ModelOptions.FrequencyPenalty, AIDevKitConfig.FrequencyPenaltyDefault, (v) => EditorGUILayout.Slider(v, -2f, 2f));
                    if (frequencyPenalty != ModelOptions.FrequencyPenalty) { ModelOptions.FrequencyPenalty = frequencyPenalty; _selectedSession.SaveFile(); }
                }
            }
            ExGUILayout.EndSection();
        }

        protected override bool HandleStreamToggleChange(bool oldValue, bool newValue) => HandleModuleToggleChange<SingleResponseStreamReceiver>(streamReceiver, oldValue, newValue);

        protected override void DrawStreamReceiver()
        {
            EditorGUILayout.PropertyField(streamReceiver, new GUIContent("Streaming Receiver", "The receiver for streaming responses. This should be set to a component that implements IChatCompletionStreamReceiver."));
        }

        internal string DrawChatbotField(string selectedId, GUIContent label)
        {
            ChatSession session = null;
            int savedIndent = EditorGUI.indentLevel;

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUI.indentLevel = 0;
                session = AIDevKitProGUI.ChatSessionPopup(selectedId, ChatSessionManager.Sessions);
            }
            finally
            {
                // restore indent level
                EditorGUI.indentLevel = savedIndent;
                GUILayout.EndHorizontal();
            }

            return session?.Id;
        }
    }
}