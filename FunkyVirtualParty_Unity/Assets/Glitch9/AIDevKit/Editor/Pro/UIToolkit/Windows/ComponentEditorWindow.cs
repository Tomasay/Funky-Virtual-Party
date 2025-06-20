using Glitch9.AIDevKit.Editor.Pro;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class ComponentEditorWindow : CodeGeneratorWindowBase<ComponentEditorWindow>
    {
        private MonoScript _script;
        private bool _isEdited = false;

        internal static void ShowWindow(MonoScript script)
        {
            if (script == null)
            {
                Debug.LogError("No script selected. Please select a MonoScript to edit.");
                return;
            }

            GeneratorWindowInfo info = GeneratorWindowInfo.GetInfo<ComponentEditorWindow>();

            var wnd = GetWindow<ComponentEditorWindow>(info.windowName);
            wnd.titleContent = new GUIContent(info.windowName);
            wnd.minSize = new Vector2(Config.WindowWidth, Config.WindowHeight);
            wnd.windowName = info.windowName;
            wnd.parametersLabel = info.parametersLabel;
            wnd.promptSamples = info.promptSamples;
            wnd.defaultModelId = info.defaultModelId;

            wnd._script = script;
            wnd.currentPath = AssetDatabase.GetAssetPath(script);
        }

        protected override string GetInstructionText()
        {
            return AIDevKitConfig.ComponentEditorInstruction;
        }

        protected override string FormatPrompt(string prompt)
        {
            if (string.IsNullOrEmpty(prompt)) return string.Empty;
            if (_script == null)
            {
                Debug.LogError("There was an error loading the selected script. Please re-open the window.");
                return string.Empty;
            }

            string scriptSource = MonoScriptUtil.GetScriptSourceText(_script);

            if (string.IsNullOrEmpty(scriptSource))
            {
                Debug.LogError("The selected script is empty or could not be read. Please check the script file.");
                return string.Empty;
            }

            const string kPromptFormat = "Edit the following script:\n\n{scriptSource}\n\nUsing the following prompt:\n\n{prompt}\n\n";

            string formattedPrompt = kPromptFormat
                .Replace("{scriptSource}", scriptSource.Trim())
                .Replace("{prompt}", prompt.Trim());

            return formattedPrompt;
        }

        protected override void RebuildPreviewUI()
        {
            if (outputLabel != null)
            {
                outputLabel.text = _isEdited ? "Edited Script Preview" : "Script To Edit";
            }

            base.RebuildPreviewUI();
        }
    }
}