using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor
{
    internal class StyleSheetCache
    {
        private readonly Dictionary<string, StyleSheet> _cache = new();

        internal StyleSheet this[string key]
        {
            get => Get(key);
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
        private readonly string basePath;

        internal StyleSheetCache(string basePath)
        {
            if (!basePath.EndsWith("/")) basePath += "/";
            this.basePath = basePath;
        }

        internal static StyleSheetCache WithMarkerFile(string markerFileName)
        {
            string basePath = DirectoryFinder.FindDirectory(markerFileName);
            if (basePath == null) return null;
            return new StyleSheetCache(basePath);
        }

        internal StyleSheet Get(string fileName)
        {
            if (!_cache.TryGetValue(fileName, out StyleSheet style))
            {
                // Try to load the StyleSheet from the base path
                string fullPath = basePath + fileName;
                if (!fullPath.EndsWith(".uss")) fullPath += ".uss"; // Ensure the file has the correct extension
                style = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>(fullPath);
                if (style == null)
                {
                    Debug.LogWarning($"StyleSheetCache: StyleSheet '{fileName}' not found at path '{fullPath}'.");
                    return null;
                }
                _cache[fileName] = style;
            }

            return style;
        }

        internal void Add(string fileName, StyleSheet style)
        {
            if (_cache.ContainsKey(fileName))
            {
                _cache[fileName] = style;
            }
            else
            {
                _cache.Add(fileName, style);
            }
        }

        internal bool TryGetValue(string key, out StyleSheet style)
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