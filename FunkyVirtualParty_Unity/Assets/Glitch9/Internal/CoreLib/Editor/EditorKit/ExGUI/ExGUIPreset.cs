using System;
using System.Collections.Generic;
using Glitch9.Internal;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal static class ExGUIPreset
    {
        internal static void TitleField(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, ExStyles.title);
            ExGUIUtility.DrawTitleLine();
        }

        internal static void UnityComponentLabel(Texture icon, string title, string subtitle, Action trailing = null)
        {
            GUILayout.BeginHorizontal(ExGUI.box);
            try
            {
                GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(32));

                GUILayout.BeginVertical();
                {
                    GUILayout.Label(title, ExStyles.componentTitle);
                    GUILayout.Label(subtitle, ExStyles.componentSubtitle);
                }
                GUILayout.EndVertical();


                if (trailing != null)
                {
                    GUILayout.FlexibleSpace();
                    trailing.Invoke();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal static void ReloadWindow(string message, Action onReload)
        {
            GUILayout.FlexibleSpace(); // 화면 위쪽 여백 

            GUILayout.BeginHorizontal();
            try
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(GUILayout.Width(400)); // 원하는 너비 설정
                try
                {
                    GUILayout.Label("There was an error while loading this window.", ExStyles.bigLabel);
                    ExGUILayout.HelpBoxExBig(message, MessageTypeEx.Error);

                    GUILayout.Space(20);

                    if (GUILayout.Button("Reload Window", ExStyles.bigButton))
                    {
                        onReload?.Invoke();
                    }
                }
                finally
                {
                    GUILayout.EndVertical();
                }

                GUILayout.FlexibleSpace();
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace(); // 화면 아래쪽 여백
        }

        internal static void TroubleShootings(string docUrl, string githubUrl)
        {
            GUILayout.BeginHorizontal();
            try
            {
                if (GUILayout.Button("Documentation", GUILayout.Height(24f), GUILayout.Width(200), GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(docUrl);
                }

                if (GUILayout.Button("Discord", GUILayout.Height(24f), GUILayout.Width(200), GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(EditorConfig.DiscordUrl);
                }

                if (GUILayout.Button("Report An Issue (Github)", GUILayout.Height(24f), GUILayout.Width(200), GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(githubUrl);
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal static void ProVersion(string featureName, string storeUrl)
        {
            GUILayout.BeginHorizontal(ExStyles.helpBox);
            {
                ExGUILayout.TextureField(EditorIcons.ProBadge, Vector2.one * 32);

                GUILayout.Space(10);

                GUILayout.Label($"{featureName} is a Pro feature.\r\nPlease upgrade to the Pro version to access this feature.",
                    EditorStyles.wordWrappedLabel, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800));

                if (GUILayout.Button("Upgrade to Pro", GUILayout.Width(100), GUILayout.Height(30)))
                {
                    Application.OpenURL(storeUrl);
                }
            }
            GUILayout.EndHorizontal();
        }

        public static int SwitchGroup(int selectedIndex, List<string> displayedOptions, int maxColumns = 3, params GUILayoutOption[] options)
        {
            if (displayedOptions == null || displayedOptions.Count == 0)
            {
                EditorGUILayout.HelpBox("No list found.", MessageType.Warning);
                return -1;
            }

            int index = selectedIndex;

            float totalWidth = 0;
            GUILayout.BeginHorizontal();

            // if (label != null)GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.labelWidth)); 

            int columnCount = 0;  // New variable to track the number of columns

            for (int i = 0; i < displayedOptions.Count; i++)
            {
                float buttonWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent(displayedOptions[i])).x;
                if (totalWidth + buttonWidth > EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth || columnCount >= maxColumns)
                {
                    // Wrap to the next line if the button will exceed the width of the inspector or if the maxColumns limit is reached.
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();

                    //if (label != null) GUILayout.Space(EditorGUIUtility.labelWidth);

                    totalWidth = 0;
                    columnCount = 0;  // Reset column count for the new line
                }

                // Use toggle style buttons for the toolbar buttons
                bool isActive = GUILayout.Toggle(index == i, displayedOptions[i], EditorStyles.miniButton, options);
                if (isActive) index = i;

                totalWidth += buttonWidth;
                columnCount++;  // Increment column count
            }
            GUILayout.EndHorizontal();

            return index;
        }

    }
}