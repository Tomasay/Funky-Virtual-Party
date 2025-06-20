using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Editor.Generation;
using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal abstract class CodeGeneratorWindowBase<TSelf> : GeneratorWindowBase<TSelf, CodeGeneratorSettings<TSelf>>
        where TSelf : CodeGeneratorWindowBase<TSelf>
    {
        protected virtual string Language => "csharp";
        protected virtual string Extension => "cs";
        protected virtual string DefaultFileName => "MyScript";

        protected string Namespace { get => Settings.Namespace.Value; set => Settings.Namespace.Value = value; }
        protected GameGenre Genre { get => Settings.GameGenre.Value; set => Settings.GameGenre.Value = value; }
        protected bool Is2D { get => Settings.Is2D.Value; set => Settings.Is2D.Value = value; }
        protected bool IsMultiplayer { get => Settings.IsMultiplayer.Value; set => Settings.IsMultiplayer.Value = value; }
        protected bool IsVR { get => Settings.IsVR.Value; set => Settings.IsVR.Value = value; }
        protected bool IsCodeAvailable => !string.IsNullOrEmpty(_rawCode);
        private string _rawCode;
        private string _highlightedCode;
        private Vector2 _scrollPos;
        private bool _displayingRawCode = false;
        private string DisplayedCode => _displayingRawCode ? _rawCode : _highlightedCode;

        protected override void DrawIMGUIGenerationSettings()
        {
            Model = AIDevKitGUI.LLMPopup(Model, label: GUIContents.Model, apiWidth: 80);
            Namespace = EditorGUILayout.TextField(new GUIContent("Namespace"), Namespace);
        }

        protected override void DrawIMGUIApiParameters()
        {
            EditorGUI.BeginDisabledGroup(UseProjectContext);
            {
                Genre = (GameGenre)EditorGUILayout.EnumFlagsField(GUIContents.GameGenre, Genre);
                Is2D = ExGUILayout.AnimatedToggle(GUIContents.Is2D, Is2D);
                IsVR = ExGUILayout.AnimatedToggle(GUIContents.IsVR, IsVR);
                IsMultiplayer = ExGUILayout.AnimatedToggle(GUIContents.IsMultiplayer, IsMultiplayer);
            }
            EditorGUI.EndDisabledGroup();

            bool newUseProjectContext = ExGUILayout.AnimatedToggle(GUIContents.UseProjectContext, UseProjectContext);
            if (newUseProjectContext != UseProjectContext)
            {
                UseProjectContext = newUseProjectContext;

                if (UseProjectContext)
                {
                    Genre = AIDevKitSettings.ProjectContext.Genre;
                    Is2D = AIDevKitSettings.ProjectContext.Is2D;
                    IsMultiplayer = AIDevKitSettings.ProjectContext.IsMultiplayer;
                    IsVR = AIDevKitSettings.ProjectContext.IsVR;
                }
            }
        }

        protected override void AddExtraPromptTitleRowButtons(VisualElement titleRow)
        {
            var promptGuideButton = new Button()
            {
                name = "prompt-guide-button",
                tooltip = "Open Prompt Guide",
                style = {
                    backgroundImage = EditorGUIUtility.IconContent("_Help").image as Texture2D
                }
            };
            promptGuideButton.AddToClassList("icon-button");
            promptGuideButton.clicked += () =>
            {
                CodeGenPromptGuideWindow.ShowWindow();
            };
            titleRow.Add(promptGuideButton);
        }

        protected override void AddExtraPreviewTitleRowButtons(VisualElement buttonsRow)
        {
            var copyButton = new Button()
            {
                name = "copy-button",
                tooltip = "Copy generated code to clipboard",
                style = {
                    backgroundImage = EditorIcons.Copy as Texture2D
                }
            };

            copyButton.AddToClassList("icon-button");
            copyButton.clicked += () =>
            {
                if (string.IsNullOrEmpty(_rawCode))
                {
                    Debug.Log("No code to copy.");
                    return;
                }

                GUIUtility.systemCopyBuffer = _rawCode;
                Debug.Log("Code copied to clipboard.");
            };

            buttonsRow.Add(copyButton);

            base.AddExtraPreviewTitleRowButtons(buttonsRow);
        }

        protected override async void SaveGeneratedContents()
        {
            if (string.IsNullOrEmpty(_rawCode))
            {
                Debug.Log("No code to save.");
                return;
            }

            string fileName = ScriptExporter.ExtractClassName(_rawCode);
            if (string.IsNullOrEmpty(fileName)) fileName = DefaultFileName;
            string path = EditorUtility.SaveFilePanel("Save Code", GetSavePath(), $"{fileName}.{Extension}", Extension);

            if (!string.IsNullOrEmpty(path))
            {
                await ScriptExporter.SaveAsFileAsync(_rawCode, path, fileName);
                OnScriptSaved(path, fileName);
            }
        }

        protected virtual void OnScriptSaved(string path, string className)
        {
            // Optionally, you can implement additional logic after saving the script.
            Debug.Log($"{className} script saved successfully: {path}");
        }

        protected abstract string GetInstructionText();

        protected virtual string FormatPrompt(string prompt)
        {
            if (string.IsNullOrEmpty(prompt)) return string.Empty;

            const string kPromptFormat = "Generate code for a {genre} game ({2Dor3D}, {singleOrMultiplayer}) using the following prompt:\n\n{prompt}\n\n";

            string formattedPrompt = kPromptFormat
                .Replace("{genre}", Genre.FormatFlagsEnum())
                .Replace("{2Dor3D}", Is2D ? "2D" : "3D")
                .Replace("{singleOrMultiplayer}", IsMultiplayer ? "multiplayer" : "singleplayer")
                .Replace("{prompt}", prompt.Trim());

            if (!string.IsNullOrEmpty(Namespace))
            {
                formattedPrompt += $"\nThis code should be in the namespace '{Namespace}'.";
            }

            return formattedPrompt;
        }

        protected override async UniTask GenerateContentAsync(string prompt)
        {
            string formattedPrompt = FormatPrompt(prompt);

            GENCodeTask task = prompt.GENCode(formattedPrompt)
                .SetModel(Model)
                .SetSender(windowName);

            string instruction = GetInstructionText();

            if (!string.IsNullOrEmpty(instruction))
            {
                task.SetInstruction(instruction);
            }

            await task.ExecuteAsync();
        }

        protected override void OnClickUseThisPromptButton() => throw new NotImplementedException();

        protected override bool OnSelectPromptRecord()
        {
            string firstOutputText = currentRecord?.FirstOutputText();
            string code = firstOutputText?.Trim();
            if (_rawCode == code) return false;

            if (code.StartsWith("xml")) code = code.Substring(3).Trim();

            _rawCode = code;
            _highlightedCode = SyntaxHighlighter.Highlight(Language, code);

            return true;
        }

        protected override void RebuildPreviewUI()
        {
            if (previewContainer == null) return;

            previewContainer.PrepareContainer();
            previewContainer.AddToClassList("code-block");

            if (IsGenerating)
            {
                previewContainer.style.justifyContent = Justify.Center; // 수직 정렬
                previewContainer.style.alignItems = Align.Center;       // 수평 정렬 
                previewContainer.style.flexDirection = FlexDirection.Column;

                VisualElement spinner = new LoadingSpinner();
                previewContainer.Add(spinner);

                const string loadingTextFormat = "{api}'s {modelName} is generating code... Please wait.";
                string loadingText = loadingTextFormat
                    .Replace("{api}", Model.SafeGetApiName())
                    .Replace("{modelName}", Model.SafeGetName());

                VisualElement loadingLabel = new Label(loadingText);
                loadingLabel.AddToClassList("code-loading-label");

                previewContainer.Add(loadingLabel);
            }
            else
            {
                VisualElement codeBlock = new IMGUIContainer(() =>
                {
                    if (string.IsNullOrEmpty(DisplayedCode))
                    {
                        EditorGUILayout.SelectableLabel("No code generated yet.", ExStyles.centeredBoldLabel);
                    }
                    else
                    {
                        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        EditorGUILayout.TextArea(DisplayedCode, GeneratorStyles.CodeStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        EditorGUILayout.EndScrollView();
                    }
                });

                codeBlock.AddToClassList("code-imgui-container");
                previewContainer.contentContainer.Add(codeBlock);

                // add a toggle button to switch between raw and formatted code
                var toggleButton = new Button(() =>
                {
                    _displayingRawCode = !_displayingRawCode;
                    RebuildPreviewUI();
                })
                {
                    name = "toggle-code-view-button",
                    text = _displayingRawCode ? "Color" : "Raw",
                    tooltip = _displayingRawCode ? "Switch to syntax highlighted code view" : "Switch to raw code view",
                };

                toggleButton.AddToClassList("toggle-code-view-button");
                previewContainer.contentContainer.Add(toggleButton);
            }
        }
    }
}