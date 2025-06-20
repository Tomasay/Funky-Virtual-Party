using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public enum TreeViewColumnWidth
    {
        /// <summary>
        /// Fixed width of 24 pixels.
        /// </summary>
        Tiny,

        /// <summary>
        /// 60 ~ 80 pixels.
        /// </summary>
        Small,

        /// <summary>
        /// 100 ~ 140 pixels.
        /// </summary>
        Medium,

        /// <summary>
        /// 160 ~ 200 pixels.
        /// </summary>
        Wide,

        /// <summary>
        /// 220 ~ 260 pixels.
        /// </summary>
        ExtraWide
    }

    public class TreeViewColumnData
    {
        public int Index { get; set; }
        public GUIContent HeaderContent { get; set; }
        public TreeViewColumnWidth Width { get; set; } = TreeViewColumnWidth.Medium;
        public TextAlignment HeaderTextAlignment { get; set; }
        public bool AutoResize { get; set; } = true;
        public bool CanSort { get; set; } = true;
        public bool AllowToggleVisibility { get; set; } = true;
        public float? FixedWidth { get; set; }
        public bool DefaultVisibility { get; set; } = true;
    }
}