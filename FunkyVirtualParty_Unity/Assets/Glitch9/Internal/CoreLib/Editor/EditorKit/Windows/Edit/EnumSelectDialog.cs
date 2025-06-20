using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Provides a popup for selecting an enum value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class EnumSelectDialog<TWindow, TValue> : SelectDialog<TWindow, TValue>
        where TWindow : EnumSelectDialog<TWindow, TValue>
        where TValue : Enum
    {

        protected override void Initialize()
        {
            minSize = new Vector2(WINDOW_WIDTH, WINDOW_MIN_HEIGHT);
            maxSize = new Vector2(WINDOW_WIDTH, WINDOW_MIN_HEIGHT);
        }

        protected override TValue DrawContent(TValue value)
        {
            TValue newValue = (TValue)EditorGUILayout.EnumPopup(value);
            return newValue;
        }
    }
}