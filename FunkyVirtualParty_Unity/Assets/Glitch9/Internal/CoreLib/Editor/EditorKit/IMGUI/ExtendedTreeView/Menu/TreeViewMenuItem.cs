using System;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public interface ITreeViewMenuEntry { }

    public class TreeViewMenuItem : ITreeViewMenuEntry
    {
        public string Name { get; set; }
        public int MenuWidth { get; set; }

        public TreeViewMenuItem(string name)
        {
            Name = name;
            MenuWidth = CalculateMenuWidth(name);
        }

        private static int CalculateMenuWidth(string menuName)
        {
            if (string.IsNullOrEmpty(menuName))
                return 0;

            GUIStyle style = GUI.skin.button;
            Vector2 size = style.CalcSize(new GUIContent(menuName));
            return Mathf.CeilToInt(size.x) + 20; // Add some padding
        }
    }

    /// <summary>
    /// Represents a toolbar item for a tree view.
    /// </summary>
    public class TreeViewMenuDropdown : TreeViewMenuItem
    {
        public Action<Rect> Action { get; set; }

        public TreeViewMenuDropdown(string name, Action<Rect> action) : base(name)
        {
            Action = action;
        }
    }

    public class TreeViewMenuToggle : TreeViewMenuItem
    {
        public Action<bool> Action { get; set; }
        public bool IsChecked { get; set; }

        public TreeViewMenuToggle(string name, Action<bool> action, bool isChecked) : base(name)
        {
            Action = action;
            IsChecked = isChecked;
        }
    }

    public class TreeViewMenuSearchField : ITreeViewMenuEntry
    {
        public bool IsAtStart { get; set; } = false;

        public TreeViewMenuSearchField(bool isAtStart = false)
        {
            IsAtStart = isAtStart;
        }
    }
}