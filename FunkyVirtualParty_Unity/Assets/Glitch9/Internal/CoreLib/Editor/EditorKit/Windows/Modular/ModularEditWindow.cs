using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class ModularEditWindow : EditorWindow
    {
        internal static void ShowWindow<T>(
           string title,
           List<T> entries,
           bool isLocked
           ) where T : IModularEditWindowEntry
        {
            ModularEditWindow window = GetWindow<ModularEditWindow>(false);
            window.titleContent = new GUIContent(title);
            window.Initialize(entries, isLocked);
        }

        private List<IModularEditWindowEntry> _entries;
        private bool _isLocked;
        private bool _anyExpanding = false;

        internal void Initialize<T>(List<T> entries, bool isLocked)
            where T : IModularEditWindowEntry
        {
            _entries = entries.Cast<IModularEditWindowEntry>().ToList();
            _isLocked = isLocked;

            float sumMinHeight = 0f;

            for (int i = 0; i < _entries.Count; i++)
            {
                IModularEditWindowEntry entry = _entries[i];
                sumMinHeight += entry.MinHeight;
                if (entry.ExpandHeight) _anyExpanding = true;
            }

            minSize = new Vector2(300f, sumMinHeight + 100f);
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(_isLocked);
            {
                EditorGUIUtility.labelWidth = ModularEditWindowConfig.kLabelWidth;

                if (_entries.IsNotNullOrEmpty())
                {
                    for (int i = 0; i < _entries.Count; i++)
                    {
                        IModularEditWindowEntry entry = _entries[i];
                        entry.Draw();
                    }
                }

                if (!_anyExpanding) GUILayout.FlexibleSpace();

                DrawButtons();

                EditorGUIUtility.labelWidth = 0;
            }
            EditorGUI.EndDisabledGroup();
        }


        private void DrawButtons()
        {
            EditorGUILayout.BeginVertical(ExStyles.paddedArea);
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Cancel"))
                    {
                        Close();
                    }

                    if (GUILayout.Button("Save"))
                    {
                        Save();
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                IModularEditWindowEntry entry = _entries[i];
                entry.Save();
            }

            Close();
        }
    }
}