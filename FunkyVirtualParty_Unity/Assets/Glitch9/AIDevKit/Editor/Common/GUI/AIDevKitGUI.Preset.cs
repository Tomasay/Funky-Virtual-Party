using Glitch9.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal partial class AIDevKitGUI
    {
        // Shortcut for common GUI rendering tasks in AIDevKit.
        internal static class Presets
        {
            internal static void OfficialApiDocButton(Api api, string apiName, string docUrl)
            {
                Texture2D icon = AIDevKitGUIUtility.GetApiIcon(api);
                if (GUILayout.Button(new GUIContent($"  Official {apiName} Document", icon), GUILayout.Height(30f)))
                    Application.OpenURL(docUrl);
            }

            internal static void DrawProRequiredWarning()
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                try
                {
                    GUILayout.Label(EditorIcons.ProBadge, GUILayout.Width(30), GUILayout.Height(30));
                    GUILayout.Label("This feature is only available in AI DevKit Pro.", ExStyles.statusBoxBigText, GUILayout.Height(30));
                    if (GUILayout.Button("Upgrade", GUILayout.Height(30), GUILayout.Width(100))) AIDevKitEditor.OpenProURL();
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
            }

            internal static void DrawUrlButtons(params (string, string)[] labelUrlPairs)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(15f); // for indentation
                    foreach (var (label, url) in labelUrlPairs)
                    {
                        if (GUILayout.Button(label, GUILayout.Height(Config.BigBtnHeight), GUILayout.Width(120), GUILayout.ExpandWidth(true)))
                        {
                            Application.OpenURL(url);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}