using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Provides an input field for editing a string
    /// </summary>
    public class InputFieldDialog : SelectDialog<InputFieldDialog, string>
    {
        const float WIDTH = 340f;
        const float HEIGHT = 86f;
        protected override void Initialize()
        {
            minSize = new Vector2(WIDTH, HEIGHT);
            maxSize = new Vector2(WIDTH, HEIGHT);
        }

        protected override string DrawContent(string value)
        {
            string result = EditorGUILayout.TextArea(value, GUILayout.ExpandHeight(true));
            GUILayout.Space(2);
            return result;
        }
    }

    public class NumberInputDialog : SelectDialog<NumberInputDialog, int>
    {
        const float WIDTH = 340f;
        const float HEIGHT = 86f;
        protected override void Initialize()
        {
            minSize = new Vector2(WIDTH, HEIGHT);
            maxSize = new Vector2(WIDTH, HEIGHT);
        }

        protected override int DrawContent(int value)
        {
            int result = EditorGUILayout.IntField(value);
            GUILayout.Space(2);
            return result;
        }
    }
}