using System.Collections.Generic;
using System.Linq;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal enum PopupGUIStyle
    {
        Default,
        Toolbar,
    }

    internal abstract class AssetPopupGUI<TAsset, TFilter>
        where TAsset : AIDevKitAsset
        where TFilter : IAIDevKitAssetFilter<TAsset>
    {
        private readonly Dictionary<TFilter, Dictionary<Api, List<TAsset>>> _cache = new();
        private static bool _forceUpdateCache = false;

        private Dictionary<Api, List<TAsset>> GetCachedAssets(TFilter filter)
        {
            if (_forceUpdateCache || !_cache.TryGetValue(filter, out var result))
            {
                result = GetFilteredAssets(filter);
                _cache[filter] = result;
                _forceUpdateCache = false;
            }
            return result;
        }

        internal static void ForceUpdateCache() => _forceUpdateCache = true;
        protected abstract Dictionary<Api, List<TAsset>> GetFilteredAssets(TFilter filter);
        protected abstract TAsset GetDefaultAssetId(TFilter filter);
        protected abstract void DrawLibraryButton(TAsset asset, GUIStyle style, float width);

        internal TAsset Draw(TAsset selected, TFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
        {
            selected ??= GetDefaultAssetId(filter);
            TAsset model = null;
            int savedIndent = EditorGUI.indentLevel;

            GUIStyle popupStyle = AIDevKitStyles.GetPopupDropdownStyle(style); //? EditorStyles.toolbarDropDown : EditorStyles.popup;
            GUIStyle btnStyle = AIDevKitStyles.GetPopupButtonStyle(style); //style == PopupGUIStyle.Toolbar ? EditorStyles.toolbarButton : EditorStyles.miniButtonRight;
            float btnWidth = style == PopupGUIStyle.Toolbar ? 32f : 20f;
            float offset = style == PopupGUIStyle.Toolbar ? 0f : 2f;

            GUILayout.BeginHorizontal();
            try
            {
                if (label != null)
                {
                    EditorGUILayout.PrefixLabel(label);
                    GUILayout.Space(offset);
                }

                EditorGUI.indentLevel = 0;
                model = DrawFieldINTERNAL(selected, filter, popupStyle, btnStyle, btnWidth, apiWidth);
            }
            finally
            {
                EditorGUI.indentLevel = savedIndent;  // restore indent level
                GUILayout.EndHorizontal();
            }

            return model;
        }

        private TAsset DrawFieldINTERNAL(TAsset selected, TFilter filter, GUIStyle style, GUIStyle btnStyle, float btnWidth, float apiWidth)
        {
            var allAssets = GetCachedAssets(filter);

            if (allAssets.Count == 0)
            {
                DrawNoAssetsAvailable();
                DrawLibraryButton(selected, btnStyle, btnWidth);
                return null;
            }

            // Step 1: Fallback to first valid model if selected is null
            if (selected == null || string.IsNullOrEmpty(selected.Id))
            {
                selected = allAssets.First().Value.FirstOrDefault();
            }

            // Step 2: Ensure selected.Api is valid
            if (selected == null || !allAssets.ContainsKey(selected.Api))
            {
                selected = allAssets.First().Value.FirstOrDefault();
            }

            // Step 3: Fallback if selected.Api exists but its list is empty
            if (selected == null || allAssets[selected.Api].IsNullOrEmpty())
            {
                selected = allAssets.FirstOrDefault(kv => kv.Value?.Count > 0).Value?.FirstOrDefault();
            }

            // If still null after all fallback attempts
            if (selected == null)
            {
                DrawNoAssetsAvailable();
                DrawLibraryButton(selected, btnStyle, btnWidth);
                return null;
            }

            List<string> displayOptions = allAssets[selected.Api].Select(m => m.Name).ToList();

            bool apiSpecified = filter.Api != Api.All;

            if (!apiSpecified)
            {
                Api newApi = ExGUILayout.EnumPopupEx(
                    selected: selected.Api,
                    displayedOptions: allAssets.Keys,
                    style: style,
                    options: GUILayout.Width(apiWidth)
                );

                if (newApi != selected.Api && allAssets[newApi].Count > 0)
                {
                    selected = allAssets[newApi][0];
                }
            }

            int selectedAssetIndex = allAssets[selected.Api].FindIndex(m => m.Id == selected.Id);
            if (selectedAssetIndex < 0) selectedAssetIndex = 0;
            int newAssetIndex = EditorGUILayout.Popup(selectedAssetIndex, displayOptions.ToArray(), style, GUILayout.ExpandWidth(true));

            if (newAssetIndex != selectedAssetIndex)
            {
                selected = allAssets[selected.Api][newAssetIndex];
            }

            // Step 4: Draw Library button
            DrawLibraryButton(selected, btnStyle, btnWidth);

            return selected;
        }

        private void DrawNoAssetsAvailable() => ExGUILayout.ErrorLabel($"Please add {typeof(TAsset).Name}s to your library");
    }
}