using System.Collections.Generic;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal class VoiceCatalogueUpdateWindow : EditorWindow
    {
        internal static void ShowWindow(List<IVoiceData> newVoices, List<VoiceCatalogueEntry> deprecatedVoices)
        {
            VoiceCatalogueUpdateWindow popup = GetWindow<VoiceCatalogueUpdateWindow>(true, "Voice Updates", true);
            popup.Initialize(newVoices, deprecatedVoices);
        }

        private List<IVoiceData> _newVoices;
        private List<VoiceCatalogueEntry> _deprecatedVoices;
        private Vector2 _scrollPosition;

        private void Initialize(List<IVoiceData> newVoices, List<VoiceCatalogueEntry> deprecatedVoices)
        {
            _newVoices = newVoices;
            _deprecatedVoices = deprecatedVoices;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical(ExStyles.popupBody);
            {
                GUILayout.BeginVertical(ExStyles.darkBackground);
                {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                    {
                        if (_newVoices.Count > 0)
                        {
                            GUILayout.Label($"New Voices ({_newVoices.Count})", ExStyles.bigBoldLabel);

                            foreach (IVoiceData voice in _newVoices)
                            {
                                DrawNewVoice(voice);
                            }
                        }

                        if (_deprecatedVoices.Count > 0)
                        {
                            GUILayout.Label($"Deprecated Voices ({_deprecatedVoices.Count})", ExStyles.bigBoldLabel);

                            foreach (IVoiceData voice in _deprecatedVoices)
                            {
                                DrawDeprecatedVoice(voice);
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("Close", ExStyles.bigButton))
                {
                    Close();
                }
            }
            GUILayout.EndVertical();
        }


        private void DrawNewVoice(IVoiceData voice)
        {
            const float kHeight = 20f;

            GUILayout.BeginHorizontal(GUILayout.Height(kHeight));
            {
                // Layout: [Icon] [Name] [CreatedAt] [Status(legacy, deprecated)]
                Texture icon = EditorIcons.StatusCheck;
                string name = voice.Name;

                GUILayout.Label(icon, GUILayout.Width(20f), GUILayout.Height(kHeight));
                GUILayout.Label(name, EditorStyles.label, GUILayout.ExpandWidth(true), GUILayout.Height(kHeight));
                GUILayout.Label(voice.CreatedAt?.ToString("yyyy-MM-dd"), EditorStyles.label, GUILayout.Width(100f), GUILayout.Height(kHeight));

                if (GUILayout.Button("Add to Library", GUILayout.Width(100f), GUILayout.Height(kHeight)))
                {
                    VoiceCatalogueUtil.AddToLibrary(voice.Id);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDeprecatedVoice(IVoiceData voice)
        {
            const float kHeight = 20f;

            GUILayout.BeginHorizontal(GUILayout.Height(kHeight));
            {
                // Layout: [Icon] [Name] [CreatedAt] [Status(legacy, deprecated)]
                Texture icon = EditorIcons.StatusObsolete;
                string name = voice.Name;

                GUILayout.Label(icon, GUILayout.Width(20f), GUILayout.Height(kHeight));
                GUILayout.Label(name, EditorStyles.label, GUILayout.ExpandWidth(true), GUILayout.Height(kHeight));
                GUILayout.Label(voice.CreatedAt?.ToString("yyyy-MM-dd"), EditorStyles.label, GUILayout.Width(100f), GUILayout.Height(kHeight));

                if (GUILayout.Button("Remove from Library", GUILayout.Width(100f), GUILayout.Height(kHeight)))
                {
                    VoiceCatalogueUtil.RemoveFromLibrary(voice.Api, voice.Id);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}