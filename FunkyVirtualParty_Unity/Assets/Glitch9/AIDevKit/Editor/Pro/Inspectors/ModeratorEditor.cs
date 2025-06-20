using System.Collections.Generic;
using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(Moderator))]
    public class ModeratorEditor : UnityEditor.Editor
    {
        private SerializedProperty model;
        private SerializedProperty onMessageModerated;
        private SerializedProperty onMessageFlagged;
        private SerializedProperty errorReceiver;
        private List<SafetySetting> SafetySettings => _target.SafetySettings;
        private Moderator _target;

        private readonly static HarmCategory[] _sharedCategories = HarmCategoryUtil.GetSharedCategories();
        private readonly static HarmCategory[] _openAICategories = HarmCategoryUtil.GetOpenAIOnlyCategories();
        private readonly static HarmCategory[] _googleCategories = HarmCategoryUtil.GetGoogleOnlyCategories();

        private HarmCategory[] _currentCategories;
        private Api _modelApi = Api.OpenAI;

        private void OnModelChanged()
        {
            List<HarmCategory> categories = new();
            categories.AddRange(_sharedCategories);

            if (_modelApi == Api.OpenAI)
            {
                categories.AddRange(_openAICategories);
            }
            else if (_modelApi == Api.Google)
            {
                categories.AddRange(_googleCategories);
            }

            // sort by name
            categories.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
            _currentCategories = categories.ToArray();
        }

        private SafetySetting GetSafeySetting(HarmCategory category)
        {
            foreach (SafetySetting setting in SafetySettings)
            {
                if (setting == null) continue;
                if (setting.Category == category) return setting;
            }
            return null;
        }

        private HarmBlockThreshold GetHarmBlockThreshold(HarmCategory category)
        {
            SafetySetting setting = GetSafeySetting(category);
            if (setting == null) return HarmBlockThreshold.BlockNone;

            return setting.Threshold;
        }

        private void RemoveSafetySetting(HarmCategory category)
        {
            SafetySetting setting = GetSafeySetting(category);
            if (setting == null) return;

            SafetySettings.Remove(setting);
            serializedObject.ApplyModifiedProperties();
        }

        private void AddSafetySetting(HarmCategory category, HarmBlockThreshold threshold)
        {
            SafetySetting setting = GetSafeySetting(category);
            if (setting != null) return;

            SafetySettings.Add(new SafetySetting(category, threshold));
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            model = serializedObject.FindProperty("model");
            onMessageModerated = serializedObject.FindProperty("onMessageModerated");
            onMessageFlagged = serializedObject.FindProperty("onMessageFlagged");
            errorReceiver = serializedObject.FindProperty("errorReceiver");
            _target = (Moderator)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_currentCategories == null) OnModelChanged();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.Moderation, "Moderator", "Moderate messages and content using AI models.");
            EditorGUILayout.Space();

            ExGUILayout.BeginSection("General Settings");
            {
                EditorGUI.BeginChangeCheck();
                AIDevKitGUI.MODPopup(model, label: GUIContents.Model);
                if (EditorGUI.EndChangeCheck())
                {
                    Model m = model.stringValue;
                    if (m != null)
                    {
                        _modelApi = m.Api;
                        OnModelChanged();
                    }
                }

                EditorGUILayout.PropertyField(errorReceiver, GUIContents.ErrorReceiver);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection("Safety Settings");
            {
                foreach (HarmCategory category in _currentCategories)
                {
                    DrawCategoryItem(category);
                }
            }
            ExGUILayout.EndSection();




            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onMessageModerated, new GUIContent("On Message Moderated", "Event triggered when a message is moderated."));
            EditorGUILayout.PropertyField(onMessageFlagged, new GUIContent("On Message Flagged", "Event triggered when a message is flagged as inappropriate."));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCategoryItem(HarmCategory category)
        {
            HarmBlockThreshold threshold = GetHarmBlockThreshold(category);
            GUIContent name = AIDevKitGUIUtility.HarmCategoryMap[category];
            HarmBlockThreshold newThreshold = (HarmBlockThreshold)EditorGUILayout.EnumPopup(name, threshold);

            if (newThreshold != threshold)
            {
                if (newThreshold == HarmBlockThreshold.BlockNone)
                {
                    RemoveSafetySetting(category);
                }
                else
                {
                    AddSafetySetting(category, newThreshold);
                }
            }
        }
    }
}