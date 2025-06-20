using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class EditorTextureCache<T> where T : Texture
    {
        private readonly Dictionary<string, T> _light = new();
        private readonly Dictionary<string, T> _dark = new();
        private readonly string basePath;

        internal EditorTextureCache(string basePath)
        {
            // the path has to ends with a /
            if (!basePath.EndsWith("/")) basePath += "/";
            this.basePath = basePath;
            // Debug.Log($"EditorTextureCache: {basePath}");
        }

        internal static EditorTextureCache<T> WithMarkerFile(string markerFileName)
        {
            string basePath = DirectoryFinder.FindDirectory(markerFileName);
            if (basePath == null) return null;
            return new EditorTextureCache<T>(basePath);
        }

        internal T Get(string texName)//
        {
            if (EditorGUIUtility.isProSkin)
            {
                // add d_ prefix to the icon name to get the dark version
                string darkIconName = "d_" + texName;
                T tex = GetINTERNAL(darkIconName, _dark);
                if (tex != null) return tex;
            }

            return GetINTERNAL(texName, _light);
        }

        internal T GetLight(string texName)
        {
            return GetINTERNAL(texName, _light);
        }

        internal T GetDark(string texName)
        {
            return GetINTERNAL("d_" + texName, _dark);
        }

        internal void Add(string texName, T tex)
        {
            if (texName.StartsWith("d_")) _dark[texName] = tex;
            else _light[texName] = tex;
        }

        private T GetINTERNAL(string texName, IDictionary<string, T> dictionary)
        {
            if (!dictionary.TryGetValue(texName, out T texture))
            {
                using (StringBuilderPool.Get(out StringBuilder sb))
                {
                    sb.Append(basePath);
                    sb.Append(texName);
                    string path = sb.ToString();

                    // if path doesn't have extension, add .png
                    if (texName.IndexOf('.') == -1)
                    {
                        sb.Append(".png");
                        path = sb.ToString();
                    }

                    texture = AssetDatabase.LoadAssetAtPath<T>(path);
                    //if (texture == null) Debug.LogError($"EditorTextureCache: Texture not found at path: {path}");
                    dictionary.Add(texName, texture);
                }
            }
            return texture;
        }
    }
}