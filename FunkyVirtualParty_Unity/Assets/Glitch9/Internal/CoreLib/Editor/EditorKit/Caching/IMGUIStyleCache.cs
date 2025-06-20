using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class IMGUIStyleCache
    {
        private readonly Dictionary<string, GUIStyle> _cache = new();

        internal GUIStyle this[string key]
        {
            get => Get(key, GUIStyle.none);
            set
            {
                if (_cache.ContainsKey(key))
                {
                    _cache[key] = value;
                }
                else
                {
                    _cache.Add(key, value);
                }
            }
        }

        internal GUIStyle Get(string key, GUIStyle defaultStyle)
        {
            if (!_cache.TryGetValue(key, out GUIStyle style))
            {
                style = new GUIStyle(defaultStyle);
                _cache[key] = style;
            }
            return style;
        }

        internal void Add(string key, GUIStyle style)
        {
            if (_cache.ContainsKey(key))
            {
                _cache[key] = style;
            }
            else
            {
                _cache.Add(key, style);
            }
        }

        internal bool TryGetValue(string key, out GUIStyle style)
        {
            if (_cache.TryGetValue(key, out style))
            {
                return true;
            }
            else
            {
                style = null;
                return false;
            }
        }
    }
}