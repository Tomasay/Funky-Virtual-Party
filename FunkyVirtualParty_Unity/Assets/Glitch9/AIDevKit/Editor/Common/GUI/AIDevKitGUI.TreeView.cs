using System.Collections.Generic;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal partial class AIDevKitGUI
    {
        internal static class TreeView
        {
            internal static void ApiColumn(Rect cellRect, Api api)
            {
                string displayName = api.GetInspectorName();
                Texture2D icon = AIDevKitGUIUtility.GetApiIcon(api);

                if (icon != null)
                {
                    //EditorGUI.LabelField(cellRect, new GUIContent(displayName, AIDevKitGUIUtility.GetApiIcon(api)));
                    // only draw the icon if it exists
                    Render.DrawIconContent(0, cellRect, new GUIContent(displayName, icon));
                }
                // else
                // {
                //     EditorGUI.LabelField(cellRect, displayName);
                // }
            }

            internal static void TokenCostColumn(Rect cellRect, double value)
            {
                if (value == AIDevKitConfig.FreePriceMagicNumber) // -99
                {
                    EditorGUI.LabelField(cellRect, "Free");
                    return;
                }

                if (value < 0)
                {
                    EditorGUI.LabelField(cellRect, "-");
                    return;
                }

                string display = value.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                EditorGUI.LabelField(cellRect, display);
            }

            internal static void FeaturesColumn(Rect cellRect, ModelFeature cap)
            {
                List<GUIContent> contents = AIDevKitGUIUtility.GetFeatureContents(cap);

                if (contents.Count == 0)
                {
                    GUI.Label(cellRect, "None");
                    return;
                }

                for (int i = 0; i < contents.Count; i++)
                {
                    var content = contents[i];
                    if (content == null) continue;
                    Render.DrawIconContent(i, cellRect, content);
                }
            }

            internal static bool FilterButton() => GUILayout.Button(new GUIContent("Filter", EditorIcons.FilterByLabel), AIDevKitStyles.SearchBarButton);
            internal static bool FilterResetButton() => GUILayout.Button(new GUIContent("Reset", EditorIcons.Reset), AIDevKitStyles.SearchBarButton);
            internal static bool UpdateCatalogueButton() => GUILayout.Button(new GUIContent("Update\nCatalogue"), AIDevKitStyles.BigSearchBarButton);
            internal static bool GenerateSnippetsButton() => GUILayout.Button(new GUIContent("Generate\nSnippets"), AIDevKitStyles.BigSearchBarButton);

            internal static bool BeginFilterSection(string title, bool isEnabled)
            {
                isEnabled = EditorGUILayout.ToggleLeft(title, isEnabled);
                EditorGUI.BeginDisabledGroup(!isEnabled);
                EditorGUI.indentLevel = 1;
                return isEnabled;
            }

            internal static void EndFilterSection()
            {
                EditorGUI.indentLevel = 0;
                EditorGUI.EndDisabledGroup();
            }

            internal static void PerformanceColumn(Rect cellRect, int value)
            {
                // for (int i = 0; i < value; i++)
                // {
                //     //GUILayout.Label(EditorIcons.LightBulb, AIDevKitStyles.LabelIcon);
                //     Render.DrawIconContent(i, cellRect, new GUIContent(EditorIcons.LightBulb), iconSize: 16f);
                // }

                string display = ResolvePerformanceLabel(value, "-");
                EditorGUI.LabelField(cellRect, display);
            }

            internal static void SpeedColumn(Rect cellRect, int value)
            {
                // for (int i = 0; i < value; i++)
                // {
                //     //GUILayout.Label(EditorIcons.Lightning, AIDevKitStyles.LabelIcon);
                //     Render.DrawIconContent(i, cellRect, new GUIContent(EditorIcons.Lightning), iconSize: 16f);
                // }

                string display = ResolveSpeedLabel(value, "-");
                EditorGUI.LabelField(cellRect, display);
            }
        }
    }
}