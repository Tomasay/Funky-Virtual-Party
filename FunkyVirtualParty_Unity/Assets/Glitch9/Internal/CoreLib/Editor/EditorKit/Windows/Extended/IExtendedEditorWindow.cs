using UnityEngine;

namespace Glitch9.Editor
{
    internal interface IExtendedEditorWindow
    {
        EPrefs<string> WindowName { get; set; }
        EPrefs<Vector2> MinWindowSize { get; set; }
        EPrefs<Vector2> MaxWindowSize { get; set; }
        EPrefs<Vector2> ButtonSize { get; set; }
        EPrefs<Vector2> MiniButtonSize { get; set; }
        EPrefs<Vector2> ImageSize { get; set; }
        EPrefs<Vector2> IconSize { get; set; }
    }
}