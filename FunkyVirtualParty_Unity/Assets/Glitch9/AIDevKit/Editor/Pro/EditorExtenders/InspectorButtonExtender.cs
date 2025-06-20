using System.Linq;
using Glitch9.AIDevKit.Editor.UIToolKit;
using Glitch9.AIDevKit.Editor.UnityAssistant;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.Pro
{
    [InitializeOnLoad]
    internal static class InspectorButtonExtender
    {
        private static VisualElement _componentGenButton;
        private static VisualElement _debugScriptButton;
        static InspectorButtonExtender() => EditorApplication.delayCall += Initialize;

        private static void Initialize()
        {
            if (!AIDevKitSettings.EnableComponentGenerator && !AIDevKitSettings.EnableScriptDebugger) return;

            EditorWindow inspectorWindow = GetInspectorWindow();
            if (inspectorWindow == null)
            {
                Debug.LogWarning("Inspector window not found. 'Generate Component' button will not be added.");
                return;
            }
            GetInspectorWindow().rootVisualElement.generateVisualContent += _ => { EditorApplication.update += Update; };
            StyleColor borderColor;
            StyleColor bottomBorderColor;

            if (EditorGUIUtility.isProSkin)
            {
                borderColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                bottomBorderColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            }
            else
            {
                borderColor = new Color(.65f, .65f, .65f, 1f);
                bottomBorderColor = new Color(.85f, .85f, .85f, 1f);
            }

            if (AIDevKitSettings.EnableComponentGenerator)
            {
                _componentGenButton = new Button(OnClickGenerateComponent)
                {
                    text = "Generate Component with " + EditorChatSettings.CurrentModelName,
                    style =
                    {
                        position = Position.Absolute,
                        marginTop = 36,
                        borderBottomColor = bottomBorderColor,
                        borderTopColor = borderColor,
                        borderLeftColor = borderColor,
                        borderRightColor = borderColor,
                        width = 230,
                        height = 26,
                        alignSelf = Align.Center,
                    },
                };
            }

            if (AIDevKitSettings.EnableScriptDebugger)
            {
                _debugScriptButton = new Button(OnClickDebugScript)
                {
                    text = "Debug Script with " + EditorChatSettings.CurrentModelName,
                    style =
                    {
                        position = Position.Absolute,
                        marginTop = 36,
                        borderBottomColor = bottomBorderColor,
                        borderTopColor = borderColor,
                        borderLeftColor = borderColor,
                        borderRightColor = borderColor,
                        width = 230,
                        height = 26,
                        alignSelf = Align.Center,
                    },
                };
            }

            EditorApplication.delayCall -= Initialize;
        }

        private static EditorWindow GetInspectorWindow()
        {
            EditorWindow[] editorWindowArray = Resources.FindObjectsOfTypeAll<EditorWindow>();
            EditorWindow inspectorWindow = editorWindowArray.FirstOrDefault(window => window.titleContent.text == "Inspector");
            return inspectorWindow;
        }

        private static void Update()
        {
            EditorWindow inspectorWindow = GetInspectorWindow();
            if (inspectorWindow == null) return;

            GetAssemblyInformationLabel();
            VisualElement addComponentButton = GetAddComponentButton(inspectorWindow.rootVisualElement);
            if (addComponentButton == null) return;

            if (AIDevKitSettings.EnableComponentGenerator && IsScriptSelected())
            {
                if (!addComponentButton.Contains(_debugScriptButton))
                {
                    addComponentButton.Add(_debugScriptButton);
                }
            }
            else if (AIDevKitSettings.EnableScriptDebugger && IsComponentSelected())
            {
                if (!addComponentButton.Contains(_componentGenButton))
                {
                    addComponentButton.Add(_componentGenButton);
                }
            }

            EditorApplication.update -= Update;
        }

        private static void OnClickGenerateComponent()
        {
            ComponentGeneratorWindow.ShowWindow(Application.dataPath);
        }

        private static void OnClickDebugScript()
        {
            MonoScript script = Selection.activeObject as MonoScript;
            if (script != null) EditorChatWindow.ShowWindow(script);
        }

        private static VisualElement GetAddComponentButton(VisualElement rootVisualElement)
        {
            return rootVisualElement?.Q(className: "unity-inspector-add-component-button").GetFirstOfType<VisualElement>();
        }


        private static bool IsComponentSelected() => Selection.activeGameObject != null;
        private static bool IsScriptSelected()
        {
            if (Selection.activeObject is MonoScript script)
            {
                return script.GetClass() != null;
            }
            return false;
        }

        private static Label GetAssemblyInformationLabel()
        {
            // Get all Inspector windows
            var inspectors = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var inspector in inspectors)
            {
                if (inspector.titleContent.text == "Inspector")
                {
                    // Query all Label elements
                    var labels = inspector.rootVisualElement.Query<Label>().ToList();
                    foreach (var label in labels)
                    {
                        if (label.text == "Assembly Information")
                        {
                            return label;
                        }
                    }
                }
            }
            return null;
        }
    }
}
