using System;
using System.Collections.Generic;
using System.Linq;
using Glitch9.Editor.UIToolkit;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using Cysharp.Threading.Tasks;

using static UnityEngine.UIElements.VisualElement;
using Glitch9.AIDevKit.Editor.Pro;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal abstract class GeneratorWindowBase<TSelf, TSettings> : EditorWindow
        where TSelf : GeneratorWindowBase<TSelf, TSettings>
        where TSettings : GeneratorSettings<TSelf, TSettings>, new()
    {
        protected static class Config
        {
            internal const string StyleName = "GeneratorWindow";
            internal const float MinInputPanelWidth = 300f;
            internal const float MaxInputPanelWidth = 630f;
            internal const float InputFieldPadding = 5f;
            internal const float PromptFieldMinHeight = 64f;
            internal const float IMGUILabelWidth = 80f;
            internal const float IMGUILabelWidthParameters = 100f;
            internal const float IMGUILabelWidthRequestDetails = 60f;
            internal const float GenerateButtonTopMargin = 5f;
            internal const float WindowWidth = 800f;
            internal const float WindowHeight = 600f;
        }

        private TextField _promptField;
        private VisualElement _promptFieldContainer;
        private ResizeHandle _promptFieldResizeHandle;
        private Button _generateButton;
        protected VisualElement previewContainer;

        private bool _noDefaultModelErrorShown = false;
        private Model _model;
        protected Model Model
        {
            get
            {
                if (_model == null)
                {
                    string modelId = Settings.Model.Value;
                    if (string.IsNullOrEmpty(modelId)) modelId = defaultModelId;
                    if (string.IsNullOrEmpty(modelId)) modelId = GeneratorWindowInfo.GetDefaultModelId<TSelf>();

                    _model = modelId;

                    if (!_noDefaultModelErrorShown && _model == null)
                    {
                        _noDefaultModelErrorShown = true;
                        ShowDialog.Error($"No default model found (ID:{modelId ?? "-"}). Please set a default model in AIDevKit settings.");
                    }
                }
                return _model;
            }
            set
            {
                if (value != null && _model != value)
                {
                    _model = value;
                    Settings.Model.Value = _model.Id;
                    _noDefaultModelErrorShown = false; // Reset error state when model is set
                    OnModelChanged(_model);
                }
            }
        }

        protected int N { get => Settings.N.Value; set => Settings.N.Value = value; }
        protected float PromptHeight { get => Settings.PromptHeight.Value; set => Settings.PromptHeight.Value = value; }
        protected float PromptHistoryHeight { get => Settings.PromptHistoryHeight.Value; set => Settings.PromptHistoryHeight.Value = value; }
        protected bool UseProjectContext { get => Settings.UseProjectContext.Value; set => Settings.UseProjectContext.Value = value; }

        protected TSettings Settings => _settings ??= new();
        private TSettings _settings;


        // Initial Setup -----------------------------------------------------------------------------------------------
        protected string currentPath;
        protected string windowName;
        protected string parametersLabel;
        protected string defaultModelId;
        protected string[] promptSamples;


        // State Management -------------------------------------------------------------------------------------------- 
        internal List<PromptRecord> promptHistory = new();
        protected PromptRecord currentRecord;
        protected Label outputLabel;
        protected readonly List<int> selectedIndices = new();

        protected bool IsGenerating
        {
            get => _isGenerating;
            set
            {
                if (_isGenerating == value) return;
                _isGenerating = value;
                RebuildPreviewUI();
            }
        }
        private bool _isGenerating = false;
        private bool _draggingDivider = false;

        internal static void ShowWindow(string selectedPath)
        {
            GeneratorWindowInfo info = GeneratorWindowInfo.GetInfo<TSelf>();

            var wnd = GetWindow<TSelf>(info.windowName);
            wnd.titleContent = new GUIContent(info.windowName);
            wnd.minSize = new Vector2(Config.WindowWidth, Config.WindowHeight);
            wnd.currentPath = selectedPath;
            wnd.windowName = info.windowName;
            wnd.parametersLabel = info.parametersLabel;
            wnd.promptSamples = info.promptSamples;
            wnd.defaultModelId = info.defaultModelId;
        }

        internal static async UniTask<VisualElement> CreateViewAsync(string selectedPath)
        {
            GeneratorWindowInfo info = GeneratorWindowInfo.GetInfo<TSelf>();

            var wnd = CreateInstance<TSelf>();
            wnd.titleContent = new GUIContent(info.windowName);
            wnd.minSize = new Vector2(Config.WindowWidth, Config.WindowHeight);
            wnd.currentPath = selectedPath;
            wnd.windowName = info.windowName;
            wnd.parametersLabel = info.parametersLabel;
            wnd.promptSamples = info.promptSamples;
            wnd.defaultModelId = info.defaultModelId;

            // craeteui 하고 좀 기다렸다가 반환
            wnd.CreateGUI();
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            return wnd.rootVisualElement;
        }

        protected virtual void PrepareGeneration()
        {
            // Override this method to prepare for generation, such as validating inputs or setting up the environment
        }
        protected abstract UniTask GenerateContentAsync(string prompt);

        protected abstract void DrawIMGUIGenerationSettings();
        protected abstract void DrawIMGUIApiParameters();

        protected virtual void AddExtraPromptTitleRowButtons(VisualElement buttonsRow)
        {
            // Override this method to add extra buttons to the prompt title row if needed
        }

        protected virtual void AddExtraPreviewTitleRowButtons(VisualElement buttonsRow)
        {
            var saveButton = GeneratorUIFactory.CreateSaveButton(SaveGeneratedContents);
            buttonsRow.Add(saveButton);

            // Override this method to add extra buttons to the preview title row if needed
        }

        protected abstract void SaveGeneratedContents();

        protected virtual void DrawPreviewFooter(VisualElement footer)
        {
            // Override this method to draw additional footer content in the preview area if needed
        }

        protected abstract void RebuildPreviewUI();

        protected virtual void OnModelChanged(Model model)
        {
            // Override this method to handle model changes, such as updating the UI or settings 
        }
        protected abstract bool OnSelectPromptRecord();
        protected abstract void OnClickUseThisPromptButton();


        // UI Creation -------------------------------------------------------------------------------------------------- 
        public void CreateGUI() => rootVisualElement.schedule.Execute(RebuildUI).ExecuteLater(1); // 1프레임 뒤에 실행


        internal void RebuildUI()
        {
            UIToolkitUtil.InitializeRootElement(
                root: rootVisualElement,
                style: Config.StyleName,
                marker: GeneratorStyles.kStyleMarkerFileName,
                "GeneratorWindow_Preview",
                "GeneratorWindow_PromptHistory",
                "GeneratorWindow_RequestDetails"
            );

            var inputPanel = rootVisualElement.Q<VisualElement>("input-panel");
            var splitDivider = rootVisualElement.Q<VisualElement>("split-divider");
            outputLabel = rootVisualElement.Q<Label>("output-label");

            SetupGenerateNode();
            SetupParametersNode();
            SetupPromptInputField();
            SetupPromptHistory();
            SetupDebugInfo(inputPanel);
            SetupPreview();
            SetupRequestDetails();
            SetupSplitDivider(splitDivider, inputPanel);
        }

        private void SetupGenerateNode()
        {
            var generateNode = rootVisualElement.Q<VisualElement>("generate-node");
            var iconButtonsRow = rootVisualElement.Q<VisualElement>("generate-icon-buttons-row");
            var openPathButton = GeneratorUIFactory.CreateOpenPathButton();
            iconButtonsRow.Add(openPathButton);

            _generateButton = rootVisualElement.Q<Button>("generate-button");
            //_generateButton.enabledSelf = false;
            _generateButton.clicked += () => GenerateContentINTERNAL(_promptField.value);

            var imguiContainer = new IMGUIContainer(() =>
            {
                EditorGUIUtility.labelWidth = Config.IMGUILabelWidth;
                DrawIMGUIGenerationSettings();
                //DrawIMGUIPathField();
                GUILayout.Space(Config.GenerateButtonTopMargin);
                EditorGUIUtility.labelWidth = 0f;
            });

            generateNode.Insert(1, imguiContainer);
        }

        private void SetupParametersNode()
        {
            var parametersLabel = rootVisualElement.Q<Label>("settings-label");
            parametersLabel.text = this.parametersLabel;
            parametersLabel.tooltip = $"Settings for {this.parametersLabel} generation";

            var parametersNode = rootVisualElement.Q<VisualElement>("settings-node");
            var settingsIMGUIContainer = new IMGUIContainer(() =>
            {
                EditorGUIUtility.labelWidth = Config.IMGUILabelWidthParameters;
                DrawIMGUIApiParameters();
                EditorGUIUtility.labelWidth = 0f;
            });
            parametersNode.Add(settingsIMGUIContainer);
        }

        private void SetupPromptInputField()
        {
            var buttonsRow = rootVisualElement.Q<VisualElement>("prompt-icon-buttons-row");

            // add an icon button to the far right of the title row
            var samplePromptButton = GeneratorUIFactory.CreateSamplePromptButton(() =>
            {
                if (promptSamples.IsNullOrEmpty())
                {
                    Debug.LogWarning("No sample prompts available.");
                    return;
                }

                int randomIndex = UnityEngine.Random.Range(0, promptSamples.Length);
                _promptField.value = promptSamples[randomIndex];
            });

            buttonsRow.Add(samplePromptButton);

            AddExtraPromptTitleRowButtons(buttonsRow);

            _promptFieldContainer = rootVisualElement.Q<VisualElement>("prompt-field-container");
            _promptField = rootVisualElement.Q<TextField>("prompt-field");
            UIToolkitUtil.SetupTextArea(_promptField, PromptHeight, OnInputValueChanged);

            var input = _promptField.Q("unity-text-input");
            input.style.paddingTop = Config.InputFieldPadding;
            input.style.paddingBottom = Config.InputFieldPadding;
            input.style.paddingLeft = Config.InputFieldPadding;
            input.style.paddingRight = Config.InputFieldPadding;

            _promptFieldResizeHandle = new ResizeHandle("appui-textarea__resize-handle", _promptField, Config.PromptFieldMinHeight, true, newHeight => PromptHeight = newHeight);
        }

        private void SetupPromptHistory()
        {
            var titleRow = rootVisualElement.Q<VisualElement>("history-title-row");

            // var buttonsRow = rootVisualElement.Q<VisualElement>("history-icon-buttons-row");

            // // add an icon button to the far right of the title row
            // var openHistoryTreeViewButton = GeneratorRenderer.CreateOpenHistoryButton();
            //r archiveAllTreeViewButton = GeneratorRenderer.CreateArchiveAllButton(promptHistory, ReloadPromptHistory);
            // titleRow.Add(openHistoryTreeViewButton);
            // titleRow.Add(archiveAllTreeViewButton);

            var openHistoryButton = rootVisualElement.Q<Button>("open-history-button");
            var archiveAllButton = rootVisualElement.Q<Button>("archive-all-button");

            openHistoryButton.style.backgroundImage = EditorIcons.History as Texture2D;
            archiveAllButton.style.backgroundImage = EditorIcons.Archive as Texture2D;

            openHistoryButton.clicked += () => PromptHistoryWindow.ShowWindow();
            archiveAllButton.clicked += () =>
            {
                if (promptHistory.IsNotNullOrEmpty())
                {
                    foreach (var record in promptHistory)
                    {
                        record.Archive();
                    }
                    ReloadPromptHistory();
                }
            };

            ReloadPromptHistory();

            // var listView = new PromptHistoryListView(this);
            var container = rootVisualElement.Q<VisualElement>("prompt-history-list-view-container");
            var listView = new ListView { name = "prompt-history-list-view" };
            listView.AddToClassList("prompt-history-list-view");
            listView.selectionType = SelectionType.Single;
            listView.itemsSource = promptHistory;

            listView.makeItem = () => GeneratorUIFactory.CreatePromptHistoryItem(RebuildUI);
            listView.bindItem = (element, i) => GeneratorUIFactory.BindPromptHistoryItem(element, promptHistory[i]);
            //listView.selectionChanged += OnPromptHistorySelectionChanged;

            container.Clear();
            container.Add(listView);
            new ResizeHandle("appui-listview__resize-handle", container, 100f, false, newHeight => PromptHistoryHeight = newHeight);
        }

        private void SetupDebugInfo(VisualElement inputPanel)
        {
            var buttonsRow = rootVisualElement.Q<VisualElement>("support-icon-buttons-row");
            buttonsRow.Add(GeneratorUIFactory.CreateDiscordButton());
            buttonsRow.Add(GeneratorUIFactory.CreatePreferencesButton());
            buttonsRow.Add(GeneratorUIFactory.CreateOnlineDocButton());
            buttonsRow.Add(GeneratorUIFactory.CreateReloadUIButton(RebuildUI));
            inputPanel.style.width = GeneratorStyles.InputPanelWidth;
        }

        private void SetupPreview()
        {
            var iconButtonsRow = rootVisualElement.Q<VisualElement>("preview-icon-buttons-row");
            AddExtraPreviewTitleRowButtons(iconButtonsRow);

            previewContainer = rootVisualElement.Q<VisualElement>("preview-container");
            RebuildPreviewUI();

            var previewFooter = rootVisualElement.Q<VisualElement>("preview-footer");
            DrawPreviewFooter(previewFooter);
        }

        private void SetupRequestDetails()
        {
            var requestDetailsNode = rootVisualElement.Q<VisualElement>("request-details-node");

            if (currentRecord == null)
            {
                requestDetailsNode.RemoveFromHierarchy();
                return;
            }

            var requestDetailsContainer = rootVisualElement.Q<VisualElement>("request-details-container");
            // var requestDetailsIMGUIContainer = new IMGUIContainer(() =>
            // {
            //     EditorGUIUtility.labelWidth = Config.IMGUILabelWidthRequestDetails;
            //     DrawIMGUIRequestDetails();
            //     EditorGUIUtility.labelWidth = 0f;
            // });
            // requestDetailsContainer.Add(requestDetailsIMGUIContainer);
            GeneratorUIFactory.SetupRequestDetails(requestDetailsContainer, currentRecord);

            var useThisPromptButton = rootVisualElement.Q<Button>("apply-button");
            useThisPromptButton.clicked += OnClickUseThisPromptButton;
            useThisPromptButton.SetEnabled(currentRecord != null);
            useThisPromptButton.tooltip = "Use this prompt in the current context";
        }

        private void SetupSplitDivider(VisualElement divider, VisualElement inputPanel)
        {
            divider.RegisterCallback<MouseDownEvent>(_ =>
            {
                _draggingDivider = true;
                divider.CaptureMouse();
            });

            divider.RegisterCallback<MouseUpEvent>(_ =>
            {
                _draggingDivider = false;
                divider.ReleaseMouse();
            });

            divider.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (!_draggingDivider) return;
                float newWidth = evt.mousePosition.x;
                inputPanel.style.width = Mathf.Clamp(newWidth, Config.MinInputPanelWidth, Config.MaxInputPanelWidth);
                GeneratorStyles.InputPanelWidth = inputPanel.resolvedStyle.width;
            });

            // GeneratorStyles.InputPanelWidth 로 설정된 값을 사용하여 초기 너비를 설정
            inputPanel.style.width = Mathf.Clamp(GeneratorStyles.InputPanelWidth, Config.MinInputPanelWidth, Config.MaxInputPanelWidth);
        }

        internal void SelectPromptRecord(PromptRecord record)
        {
            _promptField.value = record?.Prompt;
            currentRecord = record;

            if (OnSelectPromptRecord())
            {
                ClearSelection();
                RebuildPreviewUI();
            }
        }

        protected bool IsSelected(int index) => selectedIndices.Contains(index);

        protected void ToggleSelection(int index)
        {
            if (selectedIndices.Contains(index)) selectedIndices.Remove(index);
            else selectedIndices.Add(index);
        }

        protected void SelectRange(int index)
        {
            int start = Mathf.Min(index, selectedIndices.Count > 0 ? selectedIndices.Last() : index);
            int end = Mathf.Max(index, selectedIndices.Count > 0 ? selectedIndices.Last() : index);
            selectedIndices.Clear();
            for (int i = start; i <= end; i++) selectedIndices.Add(i);
        }

        protected void ClearSelection() => selectedIndices.Clear();

        protected string GetSavePath() => string.IsNullOrEmpty(currentPath) ? Application.dataPath : Path.Combine(Application.dataPath, currentPath).FixDoubleAssets();

        protected void DrawIMGUINSlider()
        {
            int newN = EditorGUILayout.IntSlider(GUIContents.N, N, 1, 10);

            if (newN != N)
            {
                N = newN;
                RebuildPreviewUI();
            }
        }

        private async void GenerateContentINTERNAL(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                Debug.LogWarning("Prompt is empty. Cannot generate content.");
                return;
            }

            PrepareGeneration();
            IsGenerating = true;

            try
            {
                await GenerateContentAsync(prompt);
                RebuildUI();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ShowDialog.Error(e.Message);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private void OnInputValueChanged(ChangeEvent<string> e)
        {
            e.StopPropagation();
            _generateButton.SetEnabled(!string.IsNullOrEmpty(e.newValue));
            if (_promptFieldResizeHandle.IsResizing) AutoResizePromptField();
        }

        private void OnPromptHistorySelectionChanged(IEnumerable<object> selectedItems)
        {
            if (selectedItems is not null && selectedItems.FirstOrDefault() is PromptRecord selected)
                SelectPromptRecord(selected);
        }

        private void AutoResizePromptField()
        {
            if (_promptField.resolvedStyle.width <= 0) return;
            float width = _promptField.resolvedStyle.width - _promptField.resolvedStyle.paddingLeft - _promptField.resolvedStyle.paddingRight;
            float height = _promptField.MeasureTextSize(_promptField.value, width, MeasureMode.Exactly, 0f, MeasureMode.Undefined).y +
                           _promptField.resolvedStyle.paddingTop + _promptField.resolvedStyle.paddingBottom;
            height = Mathf.Max(60f, height);
            _promptField.style.height = height;
            _promptFieldContainer.style.height = height;
        }

        private void ReloadPromptHistory()
        {
            promptHistory = PromptHistory.GetBySender(windowName);
            if (promptHistory.IsNullOrEmpty()) return;
            promptHistory.RemoveAll(record => record.IsArchived); // remove archived records
            if (promptHistory.IsNullOrEmpty()) return;
            promptHistory.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt)); // sort by creation date, most recent first
            SelectPromptRecord(promptHistory.First());
        }
    }
}
