using Glitch9.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Base class for creating settings providers
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TSettings"></typeparam>
    public abstract class ExtendedSettingsProvider<TSelf, TSettings> : SettingsProvider
        where TSelf : ExtendedSettingsProvider<TSelf, TSettings>
        where TSettings : ScriptableObject
    {
        protected ExtendedSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        {
            _providerStyle = new GUIStyle
            {
                richText = true,
                padding = new RectOffset(10, 10, 10, 10)
            };
        }

        private readonly GUIStyle _providerStyle;
        protected SerializedObject serializedObject;

        protected abstract void InitializeSettings();

        public override void OnGUI(string searchContext)
        {
            if (serializedObject == null)
            {
                serializedObject = LoadSettingsSO<TSettings>();

                if (serializedObject == null)
                {
                    EditorGUILayout.HelpBox("Failed to load settings. Please check the console for more details.", MessageType.Error);
                    return;
                }
                else
                {
                    InitializeSettings();
                }
            }

            GUILayout.BeginVertical(_providerStyle);
            {
                serializedObject.Update();  // Update the serialized object

                DrawSettings();

                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndVertical();
        }

        protected abstract void DrawSettings();

        protected SerializedObject LoadSettingsSO<T>() where T : ScriptableObject
        {
            string objectName = typeof(T).Name;
            T settingsInstance = ScriptableObjectUtil.LoadSingleton<T>(objectName, true);

            if (settingsInstance == null)
            {
                EditorGUILayout.HelpBox($"Failed to load {objectName} (Scriptable Object). Please make sure the asset exists in the Resources folder.", MessageType.Error);
                return null;
            }

            UnityEditor.Editor settingsEditor = UnityEditor.Editor.CreateEditor(settingsInstance);

            if (settingsEditor == null)
            {
                EditorGUILayout.HelpBox($"Failed to create editor for {objectName}. Please report this issue.", MessageType.Error);
                return null;
            }

            return settingsEditor.serializedObject;
        }
    }
}