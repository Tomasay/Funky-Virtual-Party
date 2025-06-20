using System.Collections.Generic;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class ModelCatalogueWindow
    {
        public class ModelCatalogueDetailsWindow : ExtendedTreeViewDetailsWindow
        {
            private enum TokenCostDisplayType
            {
                PerToken,
                Per1MTokens,
            }

            private static EPrefs<TokenCostDisplayType> _tokenCostDisplayType;
            private static TokenCostDisplayType tokenCostDisplayType
            {
                get
                {
                    _tokenCostDisplayType ??= new EPrefs<TokenCostDisplayType>(nameof(ModelCatalogueDetailsWindow), TokenCostDisplayType.PerToken);
                    return _tokenCostDisplayType.Value;
                }
                set
                {
                    _tokenCostDisplayType.Value = value;
                }
            }

            private string _dateDisplay;
            private string DateDisplay
            {
                get
                {
                    if (string.IsNullOrEmpty(_dateDisplay))
                    {
                        _dateDisplay = Data.CreatedAt?.ToString("yyyy-MM-dd");
                    }
                    return _dateDisplay;
                }
            }

            private (string, Texture)[] _capabilityContents;
            private (string, Texture)[] CapabilityContents
            {
                get
                {
                    if (_capabilityContents == null)
                    {
                        ModelFeature cap = Data.Feature;
                        List<(string, Texture)> contents = new();
                        long capValue = (long)cap;
                        foreach (var kvp in ModelCatalogueWindowUtil.CreateCapabilityMap())
                        {
                            long bit = (long)kvp.Key;
                            if ((capValue & bit) == bit)
                            {
                                contents.Add(kvp.Value);
                            }
                        }
                        _capabilityContents = contents.ToArray();
                    }
                    return _capabilityContents;
                }
            }

            private bool? _showProvider;
            private bool ShowProvider => _showProvider ??= ModelMetadataUtil.HasDiffProvider(Data.Api, Data.Provider);

            protected override float CalcTitleHeight() => 22f;
            protected override GUIContent CreateTitle()
            {
                string titleText = !string.IsNullOrEmpty(Data.Name) ? Data.Name : kFallbackTitle;
                return new GUIContent(titleText, AIDevKitGUIUtility.GetApiIcon(Data.Api));
            }

            protected override void DrawSubtitle()
            {
                TreeViewGUI.Subtitle($"ID: {Data.Id}", $"Created At: {DateDisplay}");
                //TreeViewGUI.SubtitleLeft();
            }

            protected override void DrawBody()
            {
                DrawLocalStatus();
                GUILayout.Space(5);
                DrawModelDetails();
                GUILayout.Space(5);
                DrawModelFeatures();

                if (!string.IsNullOrEmpty(Data.Description))
                {
                    GUILayout.Space(5);
                    DrawDescription();
                }

                if (Data.IsFineTuned)
                {
                    GUILayout.Space(5);
                    DrawFineTuningInfo();
                }

                GUILayout.Space(5);
                DrawTokenLimits();

                if (!Item.Pricing.IsNullOrEmpty())
                {
                    GUILayout.Space(5);
                    DrawModelPricing();
                }
            }

            private void DrawLocalStatus()
            {
                Texture statusIcon;
                string message;

                if (Item.InMyLibrary)
                {
                    statusIcon = EditorIcons.StatusCheck;
                    message = "This model is already in your library.";
                }
                else
                {
                    statusIcon = EditorIcons.StatusWarning;
                    message = "This model is not in your library.";
                }

                const float kHeight = 36f;

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    if (statusIcon != null)
                    {
                        GUILayout.Label(statusIcon, GUILayout.Width(30), GUILayout.Height(kHeight));
                    }

                    GUILayout.Label(message, ExStyles.verticallyCenteredLabel, GUILayout.Height(kHeight));

                    GUILayout.FlexibleSpace();

                    if (Item.InMyLibrary)
                    {
                        GUI.color = ExGUI.red;
                        if (GUILayout.Button(GUIContents.RemoveFromLibrary, AIDevKitStyles.WordWrappedButton, GUILayout.Width(40), GUILayout.Height(kHeight)))
                        {
                            ModelCatalogueUtil.RemoveFromLibrary(Item);
                            Repaint();
                        }
                        GUI.color = Color.white;
                    }
                    else
                    {

                        GUI.color = ExGUI.green;
                        if (GUILayout.Button(GUIContents.AddToLibrary, AIDevKitStyles.WordWrappedButton, GUILayout.Width(40), GUILayout.Height(kHeight)))
                        {
                            ModelCatalogueUtil.AddToLibrary(Item);
                            Repaint();
                        }
                        GUI.color = Color.white;
                    }
                }
                GUILayout.EndHorizontal();
            }

            private void DrawModelFeatures()
            {
                TreeViewGUI.BeginSection("Model Features");
                {
                    if (CapabilityContents.Length > 0)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            for (int i = 0; i < CapabilityContents.Length; i++)
                            {
                                var (name, icon) = CapabilityContents[i];
                                AIDevKitGUI.ModelCapability(name, icon);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.Label("No features available for this model.", EditorStyles.wordWrappedLabel);
                    }
                }
                TreeViewGUI.EndSection();
            }

            private void DrawModelDetails()
            {
                TreeViewGUI.BeginSection("Model Details");
                {
                    AIDevKitGUI.LabelField("API", Data.Api.GetInspectorName());
                    if (ShowProvider) AIDevKitGUI.LabelField("Provider", Data.Provider);

                    AIDevKitGUI.CopiableLabelField("ID", Data.Id);
                    AIDevKitGUI.CopiableLabelField("Display Name", Data.Name);

                    AIDevKitGUI.PerformanceField("Performance", Data.Performance);
                    AIDevKitGUI.SpeedField("Speed", Data.Speed);

                    AIDevKitGUI.LabelField("Input Modalities", Data.InputModality.ToString());
                    AIDevKitGUI.LabelField("Output Modalities", Data.OutputModality.ToString());

                    AIDevKitGUI.LabelField("Family", Data.Family);
                    // AIDevKitGUI.CopiableLabelField("Family Version", Data.FamilyVersion);
                    // AIDevKitGUI.CopiableLabelField("Model Version", Data.ModelVersion);
                    AIDevKitGUI.LabelField("Owned by", Data.OwnedBy);
                }
                TreeViewGUI.EndSection();
            }

            private void DrawDescription()
            {
                TreeViewGUI.BeginSection("Model Description");
                {
                    EditorGUILayout.LabelField(Data.Description, EditorStyles.wordWrappedLabel);
                }
                TreeViewGUI.EndSection();
            }

            private void DrawFineTuningInfo()
            {
                TreeViewGUI.BeginSection("Fine Tuning Information");
                {
                    //AIDevKitGUI.LabelField("Is Fine Tuned", Data.IsFineTuned ? "Yes" : "No");
                    AIDevKitGUI.CopiableLabelField("Base Model", Data.BaseId ?? "N/A");
                }
                TreeViewGUI.EndSection();
            }

            private void DrawTokenLimits()
            {
                TreeViewGUI.BeginSection("Token Limits");
                {
                    AIDevKitGUI.TokenField("Input Token Limit", Data.InputTokenLimit);
                    AIDevKitGUI.TokenField("Output Token Limit", Data.OutputTokenLimit);
                }
                TreeViewGUI.EndSection();
            }

            private void DrawModelPricing()
            {
                GUILayout.BeginHorizontal();
                {
                    TreeViewGUI.HeaderLabel("Model Pricing");
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(tokenCostDisplayType == TokenCostDisplayType.PerToken ? "Per 1M Tokens" : "Per Token", GUILayout.Width(100)))
                    {
                        tokenCostDisplayType = tokenCostDisplayType == TokenCostDisplayType.PerToken ? TokenCostDisplayType.Per1MTokens : TokenCostDisplayType.PerToken;
                    }
                }
                GUILayout.EndHorizontal();


                GUILayout.BeginVertical(ExStyles.helpBoxedSection);
                {
                    foreach (var kvp in Item.Pricing)
                    {
                        var (priceType, price) = kvp;

                        string priceText = priceType.GetInspectorName();
                        double displayPrice;

                        if (priceType.IsTokenType())
                        {
                            if (tokenCostDisplayType == TokenCostDisplayType.PerToken)
                            {
                                displayPrice = price;
                            }
                            else
                            {
                                displayPrice = price * 1000000.0; // Convert to per 1M tokens
                                priceText = priceText.Replace("(per)", "(per 1M)");
                            }
                        }
                        else
                        {
                            displayPrice = price;
                        }

                        AIDevKitGUI.CurrencyField(priceText, displayPrice);
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}