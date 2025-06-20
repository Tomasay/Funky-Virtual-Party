using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;


namespace Glitch9.AIDevKit.Editor
{
    internal class VoiceCatalogueFilterWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        internal static void ShowWindow(VoiceCatalogueWindow window)
        {
            VoiceCatalogueFilterWindow popup = GetWindow<VoiceCatalogueFilterWindow>(true, "Voice Filter", true);
            popup._window = window;
            popup.Show();
        }

        VoiceCatalogueWindow _window;

        private void OnGUI()
        {
            GUILayout.BeginVertical(ExStyles.paddedArea);
            try
            {
                try
                {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                    EditorGUI.BeginChangeCheck();
                    {
                        DrawGUI();
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        _window.OnFilterChanged();
                    }
                }
                finally
                {
                    GUILayout.EndScrollView();
                }
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }

        private void DrawGUI()
        {
            TreeViewGUI.BeginSection("Filter by Source");
            {
                VoiceCatalogueFilter.Official = EditorGUILayout.ToggleLeft("Official Voices", VoiceCatalogueFilter.Official);
                VoiceCatalogueFilter.Custom = EditorGUILayout.ToggleLeft("Fine-tuned(Custom) Voices", VoiceCatalogueFilter.Custom);
                VoiceCatalogueFilter.Default = EditorGUILayout.ToggleLeft("AIDevKit Default Voices", VoiceCatalogueFilter.Default);
            }
            TreeViewGUI.EndSection();

            TreeViewGUI.BeginSection("Filter by Status");
            {
                VoiceCatalogueFilter.InMyLibrary = EditorGUILayout.ToggleLeft("In My Library", VoiceCatalogueFilter.InMyLibrary);
                VoiceCatalogueFilter.NotInMyLibrary = EditorGUILayout.ToggleLeft("Not In My Library", VoiceCatalogueFilter.NotInMyLibrary);
                VoiceCatalogueFilter.Featured = EditorGUILayout.ToggleLeft("Featured Voices", VoiceCatalogueFilter.Featured);
                VoiceCatalogueFilter.Deprecated = EditorGUILayout.ToggleLeft("Deprecated Voices", VoiceCatalogueFilter.Deprecated);
            }
            TreeViewGUI.EndSection();
        }
    }
}