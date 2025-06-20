using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class AIDevKitStyles
    {
        private static readonly IMGUIStyleCache _cache = new();

        internal static GUIStyle Label => _cache.Get(nameof(Label), new GUIStyle(EditorStyles.textArea)
        {
            fontSize = 12,
        });

        internal static GUIStyle LinkButton => _cache.Get(nameof(LinkButton), new GUIStyle(EditorStyles.linkLabel)
        {
            fontSize = 10
        });

        internal static GUIStyle PromptField => _cache.Get(nameof(PromptField), new GUIStyle(EditorStyles.textField)
        {
            padding = new RectOffset(4, 4, 4, 4),
            margin = new RectOffset(4, 4, 0, 0),
            wordWrap = true,
        });

        internal static GUIStyle PromptHistoryBox => _cache.Get(nameof(PromptHistoryBox), new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(4, 4, 4, 4),
            margin = new RectOffset(0, 0, 0, 0),
            wordWrap = true,
        });

        internal static GUIStyle PromptHistoryButton => _cache.Get(nameof(PromptHistoryButton), new GUIStyle(EditorStyles.objectField)
        {
            wordWrap = false,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.MiddleLeft,
        });

        internal static GUIStyle WordWrappedButton => _cache.Get(nameof(WordWrappedButton), new GUIStyle(GUI.skin.button)
        {
            wordWrap = true,
            padding = new RectOffset(4, 4, 4, 4),
            margin = new RectOffset(0, 0, 0, 0),
        });

        internal static GUIStyle LabelIcon => _cache.Get(nameof(LabelIcon), new GUIStyle()
        {
            fixedHeight = 16,
            fixedWidth = 16,
            alignment = TextAnchor.MiddleCenter,
        });

        internal static GUIStyle CapabilityCard => _cache.Get(nameof(CapabilityCard), new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 72,
            fixedWidth = 64,
            padding = new RectOffset(4, 4, 8, 8),
            margin = new RectOffset(4, 4, 4, 4),
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
        });

        internal static GUIStyle CapabilityIcon => _cache.Get(nameof(CapabilityIcon), new GUIStyle()
        {
            fixedHeight = 32,
            alignment = TextAnchor.MiddleCenter,
        });


        internal static GUIStyle PopupWindow => _cache.Get(nameof(PopupWindow), new GUIStyle()
        {
            padding = new RectOffset(8, 8, 8, 8),
        });

        internal static GUIStyle SearchBarStyle => _cache.Get(nameof(SearchBarStyle), new GUIStyle(TreeViewStyles.BottomBarStyle)
        {
            fixedHeight = 56f,
        });

        internal static GUIStyle BigSearchBarButton => _cache.Get(nameof(BigSearchBarButton), new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 40f,
            fixedWidth = 80f,
        });

        internal static GUIStyle SearchBarButton => _cache.Get(nameof(SearchBarButton), new GUIStyle(GUI.skin.button)
        {
            fixedWidth = 80f,
            fixedHeight = 19f,
        });

        internal static GUIStyle GetPopupDropdownStyle(PopupGUIStyle style)
        {
            return style switch
            {
                PopupGUIStyle.Toolbar => EditorStyles.toolbarDropDown,
                _ => EditorStyles.popup,
            };
        }

        internal static GUIStyle GetPopupButtonStyle(PopupGUIStyle style)
        {
            return style switch
            {
                PopupGUIStyle.Toolbar => EditorStyles.toolbarButton,
                _ => EditorStyles.miniButtonRight,
            };
        }
    }
}