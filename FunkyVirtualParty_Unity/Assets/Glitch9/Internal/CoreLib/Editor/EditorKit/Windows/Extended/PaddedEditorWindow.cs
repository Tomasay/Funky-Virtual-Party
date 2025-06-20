using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Mostly used for smaller editor windows that don't need a lot of space
    /// </summary>
    public abstract class PaddedEditorWindow : EditorWindow
    {
        private void OnGUI()
        {
            GUILayout.BeginVertical(ExStyles.paddedArea);
            {
                DrawGUI();
            }
            GUILayout.EndVertical();
        }

        protected abstract void DrawGUI();
    }
}
