using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(WebSearch))]
    public class WebSearchEditor : UnityEditor.Editor
    {
        private SerializedProperty webSearchOptions;
        private SerializedProperty searchContextSize;
        private SerializedProperty city;
        private SerializedProperty country;
        private SerializedProperty region;
        private SerializedProperty timezone;

        private WebSearch _target;


        private void OnEnable()
        {
            webSearchOptions = serializedObject.FindProperty(nameof(webSearchOptions));

            if (webSearchOptions == null)
            {
                Debug.LogError("WebSearchOptions property not found. Please ensure it is defined in the WebSearch component.");
                return;
            }

            searchContextSize = webSearchOptions.FindPropertyRelative(nameof(WebSearchOptions.SearchContextSize));

            SerializedProperty userLocation = webSearchOptions.FindPropertyRelative(nameof(WebSearchOptions.UserLocation));
            SerializedProperty approximate = userLocation.FindPropertyRelative(nameof(UserLocation.Approximate));

            city = approximate.FindPropertyRelative(nameof(Location.City));
            country = approximate.FindPropertyRelative(nameof(Location.Country));
            region = approximate.FindPropertyRelative(nameof(Location.Region));
            timezone = approximate.FindPropertyRelative(nameof(Location.Timezone));

            _target = (WebSearch)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.WebSearch, "Web Search", "A component that performs web searches based on user-defined parameters.");
            EditorGUILayout.Space();

            ExGUILayout.BeginSection("General Settings");
            {
                _target.SearchContextSize = ExGUILayout.NullableEnumPopup(new GUIContent("Context Size", "High level guidance for the amount of context window space to use for the search."), _target.SearchContextSize);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection("User Location");
            {
                _target.Country = ExGUILayout.NullableEnumPopup(new GUIContent("Country", "The two-letter ISO country code of the user, e.g. US."), _target.Country);
                EditorGUILayout.PropertyField(city, new GUIContent("City", "Free text input for the city of the user, e.g. San Francisco."));
                EditorGUILayout.PropertyField(region, new GUIContent("Region", "Free text input for the region of the user, e.g. California."));
                _target.TimeZone = ExGUILayout.NullableEnumPopup(new GUIContent("Time-Zone", "The IANA timezone of the user, e.g. America/Los_Angeles."), _target.TimeZone);
            }
            ExGUILayout.EndSection();

            serializedObject.ApplyModifiedProperties();
        }
    }
}