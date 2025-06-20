using System;
using UnityEditor;
using UnityEngine;
using Glitch9.Editor;
using System.Collections.Generic;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.ElevenLabs;

namespace Glitch9.AIDevKit.Editor
{
    public partial class VoiceCatalogueWindow
    {
        private List<SystemLanguage> AvailableLanguages => VoiceCatalogueWindowUtil.GetAvailableLanguages();
        private string[] AvailableLanguagesDisplayNames => VoiceCatalogueWindowUtil.GetAvailableLanguagesDisplayNames();
        private List<Api> AvailableApis => VoiceCatalogueWindowUtil.GetAvailableApis();

        protected override void BottomBar()
        {
            if (TreeView == null) return;

            GUILayout.BeginHorizontal(AIDevKitStyles.SearchBarStyle);
            {
                DrawSearchBar();
            }
            GUILayout.EndHorizontal();
        }

        private static bool VoiceAgeCheckEnabled(Enum value)
        {
            return value switch
            {
                VoiceAge.Child => false,
                _ => true,
            };
        }

        internal void OnFilterChanged()
        {
            TreeView.ReloadTreeView(true);
        }

        private void ResetFilters()
        {
            VoiceCatalogueFilter.Api = Api.All;

            VoiceCatalogueFilter.Type = VoiceType.None;
            VoiceCatalogueFilter.Category = VoiceCategory.None;
            VoiceCatalogueFilter.Gender = VoiceGender.None;
            VoiceCatalogueFilter.Age = VoiceAge.None;
            VoiceCatalogueFilter.Language = SystemLanguage.Unknown;

            VoiceCatalogueFilter.InMyLibrary = false;
            VoiceCatalogueFilter.NotInMyLibrary = false;

            VoiceCatalogueFilter.Default = false;
            VoiceCatalogueFilter.Official = false;
            VoiceCatalogueFilter.Custom = false;
            VoiceCatalogueFilter.Featured = false;
            VoiceCatalogueFilter.Deprecated = false;

            TreeView.Filter.SearchText = string.Empty;
            TreeView.ReloadTreeView(true);
        }

        private void DrawSearchBar()
        {
            const float kMinWidth = 130f;
            const float kSpace = 10f;
            const float kToggleMinWidth = 80f;
            const float kBtnWidth = 80f;

            GUILayout.BeginVertical(GUILayout.MinWidth(kMinWidth));
            {
                EditorGUIUtility.labelWidth = 120f;

                GUILayout.BeginHorizontal();
                try
                {
                    GUILayout.Label($"Displaying {TreeView.ShowingCount}/{TreeView.TotalCount}", EditorStyles.boldLabel, GUILayout.Height(18f), GUILayout.MaxWidth(156f));

                    if (GUILayout.Button(new GUIContent(EditorIcons.Reset, "Reset Filters"), ExStyles.miniButton, GUILayout.Width(20f)))
                    {
                        ResetFilters();
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }

                EditorGUIUtility.labelWidth = 24f;

                Api api = ExGUILayout.EnumPopupEx(
                    label: "API",
                    selected: VoiceCatalogueFilter.Api,
                    displayedOptions: AvailableApis,
                    displayedNames: null,
                    style: null,
                    GUILayout.ExpandWidth(true),
                    GUILayout.MaxWidth(180f)
                );

                if (api != VoiceCatalogueFilter.Api)
                {
                    VoiceCatalogueFilter.Api = api;
                    OnFilterChanged();
                }
            }
            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 70f;
            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                VoiceType voiceType = (VoiceType)EditorGUILayout.EnumPopup("Type", VoiceCatalogueFilter.Type, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (voiceType != VoiceCatalogueFilter.Type)
                {
                    VoiceCatalogueFilter.Type = voiceType;
                    OnFilterChanged();
                }

                VoiceCategory voiceCategory = (VoiceCategory)EditorGUILayout.EnumPopup(
                    label: new GUIContent("Category"),
                    selected: VoiceCatalogueFilter.Category,
                    GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true)
                );

                if (voiceCategory != VoiceCatalogueFilter.Category)
                {
                    VoiceCatalogueFilter.Category = voiceCategory;
                    OnFilterChanged();
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                VoiceGender voiceGender = (VoiceGender)EditorGUILayout.EnumPopup("Gender", VoiceCatalogueFilter.Gender, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));

                if (voiceGender != VoiceCatalogueFilter.Gender)
                {
                    VoiceCatalogueFilter.Gender = voiceGender;
                    OnFilterChanged();
                }

                VoiceAge voiceAge = (VoiceAge)EditorGUILayout.EnumPopup(
                    label: new GUIContent("Age"),
                    selected: VoiceCatalogueFilter.Age,
                    checkEnabled: VoiceAgeCheckEnabled,
                    includeObsolete: false,
                    GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true)
                );

                if (voiceAge != VoiceCatalogueFilter.Age)
                {
                    VoiceCatalogueFilter.Age = voiceAge;
                    OnFilterChanged();
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                if (!AvailableLanguages.IsNullOrEmpty())
                {
                    SystemLanguage voiceLanguage = ExGUILayout.EnumPopupEx(
                        label: "Language",
                        selected: VoiceCatalogueFilter.Language,
                        displayedOptions: AvailableLanguages,
                        displayedNames: AvailableLanguagesDisplayNames,
                        style: null,
                        GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true)
                    );

                    if (voiceLanguage != VoiceCatalogueFilter.Language)
                    {
                        VoiceCatalogueFilter.Language = voiceLanguage;
                        OnFilterChanged();
                    }
                }

                bool showCustom = EditorGUILayout.Toggle("Shared", VoiceCatalogueFilter.ShowCustom, GUILayout.MinWidth(kToggleMinWidth), GUILayout.ExpandWidth(true));
                if (showCustom != VoiceCatalogueFilter.ShowCustom)
                {
                    VoiceCatalogueFilter.ShowCustom = showCustom;
                    OnFilterChanged();
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(kSpace);
            EditorGUIUtility.labelWidth = 0f;

            GUILayout.BeginHorizontal(GUILayout.Width(kBtnWidth * 2 + 4f));
            {
                GUILayout.BeginVertical();
                {
                    if (AIDevKitGUI.TreeView.FilterButton())
                    {
                        VoiceCatalogueFilterWindow.ShowWindow(this);
                    }

                    if (AIDevKitGUI.TreeView.FilterResetButton())
                    {
                        ResetFilters();
                    }
                }
                GUILayout.EndVertical();

                if (AIDevKitGUI.TreeView.GenerateSnippetsButton())
                {
                    GenerateSnippets();
                }

                if (AIDevKitGUI.TreeView.UpdateCatalogueButton())
                {
                    if (EditorUtility.DisplayDialog("Update Voice Catalogue", "This may take a while, do you want to continue?", "Yes", "No"))
                    {
                        VoiceCatalogue.Instance.UpdateCatalogue((success) =>
                        {
                            if (success)
                            {
                                TreeView.ReloadTreeView(true);
                            }
                        });
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void GenerateSnippets()
        {
            if (EditorUtility.DisplayDialog("Generate Snippets", "This may take a while, do you want to continue?", "Yes", "No"))
            {
                if (OpenAISettings.Instance.HasApiKey()) VoiceSnippetGenerator.Generate(Api.OpenAI);
                if (ElevenLabsSettings.Instance.HasApiKey()) VoiceSnippetGenerator.Generate(Api.ElevenLabs);
                AssetDatabase.Refresh();
            }
        }
    }
}