using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public class EditTextWindow : EditorEditWindow<EditTextWindow, string>
    {
        protected override string DrawGUI(string value)
        {
            return EditorGUILayout.TextArea(value, ExStyles.paddedTextField, GUILayout.MinHeight(18f), GUILayout.ExpandHeight(true));
        }
    }
}