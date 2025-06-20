using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    internal class PromptHistoryGUI
    {
        private static readonly IMGUIStyleCache _cache = new();

        public static GUIStyle TextContentBox => _cache.Get(nameof(TextContentBox), new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(7, 7, 5, 5),
            margin = new RectOffset(0, 0, 0, 5)
        });
    }
}