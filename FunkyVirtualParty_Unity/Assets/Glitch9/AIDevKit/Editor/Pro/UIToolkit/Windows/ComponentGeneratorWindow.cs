using System;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class ComponentGeneratorWindow : CodeGeneratorWindowBase<ComponentGeneratorWindow>
    {
        private static string SavePath
        {
            get => EditorPrefs.GetString("ComponentGen.SavePath", string.Empty);
            set => EditorPrefs.SetString("ComponentGen.SavePath", value);
        }

        internal static int ComponentId
        {
            get => EditorPrefs.GetInt("ComponentGen.ComponentId", 0);
            set => EditorPrefs.SetInt("ComponentGen.ComponentId", value);
        }

        private static string ClassName
        {
            get => EditorPrefs.GetString("ComponentGen.ClassName", string.Empty);
            set => EditorPrefs.SetString("ComponentGen.ClassName", value);
        }

        private static bool IsCallbackRegistered
        {
            get => EditorPrefs.GetBool("ComponentGen.IsCallbackRegistered", false);
            set => EditorPrefs.SetBool("ComponentGen.IsCallbackRegistered", value);
        }

        private void OnEnable()
        {
            GameObject selectedGO = Selection.activeGameObject;

            if (selectedGO == null)
            {
                ShowDialog.Error("No GameObject selected. Please select a GameObject to attach the generated script to.");
                Close();
                return;
            }

            ComponentId = selectedGO.GetInstanceID();
            Debug.Log($"[ComponentGen] Selected GameObject ID: {ComponentId}");
        }


        protected override string GetInstructionText()
        {
            return "Create a Unity C# script that inherits from MonoBehaviour. "
            + "If necessary, include the default Start and Update methods with private visibility and no parameters. "
            + "Do not make any title, any explanations, transliteration or extra punctuation.";
        }

        protected override void DrawPreviewFooter(VisualElement footer)
        {
            base.DrawPreviewFooter(footer);

            var attachScriptButton = new Button(SaveGeneratedContents)
            {
                text = "Attach Script",
                tooltip = "Attach the generated script to the selected GameObject."
            };

            attachScriptButton.AddToClassList("primary-button");
            attachScriptButton.AddToClassList("bottom-gap");
            attachScriptButton.SetEnabled(IsCodeAvailable);
            footer.Add(attachScriptButton);
        }

        protected override void OnScriptSaved(string path, string className)
        {
            IsCallbackRegistered = false; // Reset callback registration status

            if (!IsCodeAvailable) return;
            if (string.IsNullOrWhiteSpace(className)) throw new Exception("Class name cannot be empty.");
            if (string.IsNullOrWhiteSpace(path)) throw new Exception("Save path cannot be empty.");
            base.OnScriptSaved(path, className);

            ClassName = className;
            SavePath = path;

            WaitForCompilation();
        }

        private static void AttachScriptToComponent()
        {
            Debug.Log("<color=yellow>[ComponentGen]</color> Attempting to attach script...");

            if (string.IsNullOrWhiteSpace(ClassName))
            {
                Debug.LogError("[ComponentGen] ClassName is null or empty.");
                return;
            }

            string fullPath = SavePath;
            Debug.Log($"[ComponentGen] Full saved path: {fullPath}");

            // Convert absolute path to relative Unity path
            string relativePath = fullPath.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            Debug.Log($"[ComponentGen] Unity-relative path: {relativePath}");

            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(relativePath);
            if (script == null)
            {
                Debug.LogError($"[ComponentGen] Failed to load MonoScript at: {relativePath}");
                return;
            }

            Type scriptType = script.GetClass();
            if (scriptType == null)
            {
                Debug.LogWarning("[ComponentGen] script.GetClass() returned null. Delaying attachment.");
                EditorApplication.delayCall += AttachScriptToComponent;
                return;
            }

            UnityEngine.Object target = EditorUtility.InstanceIDToObject(ComponentId);
            if (target == null)
            {
                Debug.LogError($"[ComponentGen] InstanceID {ComponentId} returned null.");
                return;
            }

            if (target is not GameObject go)
            {
                Debug.LogError($"[ComponentGen] Target is not a GameObject: {target.GetType().Name}");
                return;
            }

            if (go.GetComponent(scriptType) != null)
            {
                Debug.Log($"[ComponentGen] Script already attached: {scriptType.Name}");
            }
            else
            {
                go.AddComponent(scriptType);
                Debug.Log($"<color=green>[ComponentGen]</color> Successfully attached {scriptType.Name} to {go.name}");
            }

            EditorUtility.ClearProgressBar();
            EditorApplication.delayCall -= AttachScriptToComponent;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void WaitForCompilation()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                //Debug.Log("[ComponentGen] Compilation or update in progress. Waiting...");
                EditorApplication.delayCall += WaitForCompilation;
                return;
            }

            if (IsCallbackRegistered) return;

            Debug.Log("<color=yellow>[ComponentGen]</color> Scripts reloaded. Proceeding with attachment.");
            IsCallbackRegistered = true;
            EditorApplication.delayCall += AttachScriptToComponent;
        }
    }
}