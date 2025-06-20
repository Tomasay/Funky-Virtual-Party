using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal partial class AIDevKitGUI
    {
        private static class Config
        {
            internal const float BigBtnHeight = 24f;
        }

        internal static void LabelField<T>(string label, T? value) where T : struct
        {
            string display = value.HasValue ? AIDevKitGUIUtility.FormatValue(value.Value) : "-";
            EditorGUILayout.LabelField(label, display, AIDevKitStyles.Label);
        }

        internal static void LabelField<T>(string label, T value)
        {
            string display = AIDevKitGUIUtility.FormatValue(value);
            EditorGUILayout.LabelField(label, display, AIDevKitStyles.Label);
        }

        private static string ResolvePerformanceLabel(int value, string unknown = "Unknown")
        {
            // 0 - Unknown, 1 - Low, 2 - Average, 3 - High, 4 - Higher, 5 - Highest
            return value switch
            {
                0 => unknown,
                1 => "Low",
                2 => "Average",
                3 => "High",
                4 => "Higher",
                5 => "Highest",
                _ => value.ToString()
            };
        }

        private static string ResolveSpeedLabel(float value, string unknown = "Unknown")
        {
            // 0 - Unknown, 1 - Slowest, 2 - Slow, 3 - Medium, 4 - Fast, 5 - Very Fast
            return value switch
            {
                0 => unknown,
                1 => "Slowest",
                2 => "Slow",
                3 => "Medium",
                4 => "Fast",
                5 => "Very Fast",
                _ => value.ToString("F2")
            };
        }

        internal static void PerformanceField(string label, int value)
        {
            string display = ResolvePerformanceLabel(value);
            EditorGUILayout.LabelField(label, display, AIDevKitStyles.Label);
        }

        internal static void SpeedField(string label, float value)
        {
            string display = ResolveSpeedLabel(value);
            EditorGUILayout.LabelField(label, display, AIDevKitStyles.Label);
        }

        internal static void PerformanceFieldWithIcons(string label, int value)
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));

                for (int i = 0; i < value; i++)
                {
                    GUILayout.Label(EditorIcons.LightBulb, AIDevKitStyles.LabelIcon);
                }

                GUILayout.Label($"({ResolvePerformanceLabel(value)})");
            }
            GUILayout.EndHorizontal();
        }

        internal static void SpeedFieldWithIcons(string label, int value)
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));

                for (int i = 0; i < value; i++)
                {
                    GUILayout.Label(EditorIcons.Lightning, AIDevKitStyles.LabelIcon);
                }

                GUILayout.Label($"({ResolveSpeedLabel(value)})");
            }
            GUILayout.EndHorizontal();
        }

        internal static void CopiableLabelField(string label, string value) => CopiableLabelField(label, new GUIContent(value));
        internal static void CopiableLabelField(string label, GUIContent value) => Render.DrawCopiableLabelField(label, value);
        internal static bool LinkButton(string label) => GUILayout.Button(label, AIDevKitStyles.LinkButton, GUILayout.Height(16));
        internal static void TokenField(string label, int? value) => Render.DrawTokenField(label, value);
        internal static void CurrencyField(string label, Currency value) => Render.DrawCurrencyField(label, value);
        internal static void OutputPathField(GUIContent label, SerializedProperty outputPath) => ExEditorGUI.PathField(label, outputPath, Application.persistentDataPath, AIDevKitSettings.OutputPath);
        internal static void OutputPathField(SerializedProperty outputPath) => ExEditorGUI.PathField(GUIContents.OutputPath, outputPath, Application.persistentDataPath, AIDevKitSettings.OutputPath);

        internal static ImageSize ImageSizePopup(ImageSize selected, Model model)
        {
            // check if the current model supports the selected image size
            if (model != null && !ImageOptionUtil.IsImageSizeSupported(selected, model))
            {
                selected = AIDevKitConfig.GetDefaultImageSizeForModel(model.Id);
            }

            return (ImageSize)EditorGUILayout.EnumPopup(
                label: GUIContents.ImageSize,
                selected: selected,
                checkEnabled: size => ImageOptionUtil.IsImageSizeSupported((ImageSize)size, model),
                includeObsolete: false
            );
        }

        internal static ImageQuality ImageQualityPopup(ImageQuality selected, Model model)
        {
            // check if the current model supports the selected image quality
            if (model != null && !ImageOptionUtil.IsImageQualitySupported(selected, model))
            {
                selected = AIDevKitConfig.GetDefaultImageQualityForModel(model.Id);
            }

            return (ImageQuality)EditorGUILayout.EnumPopup(
                label: GUIContents.ImageQuality,
                selected: selected,
                checkEnabled: quality => ImageOptionUtil.IsImageQualitySupported((ImageQuality)quality, model),
                includeObsolete: false
            );
        }

        internal static void ModelCapability(string name, Texture icon, string tooltip = null)
        {
            GUIContent content = new(name, tooltip);
            GUILayout.BeginVertical(AIDevKitStyles.CapabilityCard);
            {
                if (icon != null)
                {
                    GUILayout.Label(icon, AIDevKitStyles.CapabilityIcon);
                }
                GUILayout.Label(content, ExStyles.centeredMiniBoldLabel, GUILayout.Height(28));
            }
            GUILayout.EndVertical();
        }


        internal static bool ModelCapabilityToggle(Texture icon, string name, string tooltip, bool isEnabled)
        {
            GUI.color = isEnabled ? ExGUI.green : Color.white;

            bool clicked = false;

            // 카드 전체를 버튼으로 감싸기
            if (GUILayout.Button(new GUIContent("", tooltip), AIDevKitStyles.CapabilityCard))
            {
                clicked = true;
            }

            // 내용은 버튼 위에 덧씌우기 (Label들은 버튼 내부에서 레이아웃만 담당)
            var lastRect = GUILayoutUtility.GetLastRect();
            GUI.BeginGroup(lastRect);
            {
                float iconSize = 32f;
                float iconX = (lastRect.width - iconSize) / 2f;
                float iconY = 10f;

                if (icon != null)
                {
                    Rect iconRect = new Rect(iconX, iconY, iconSize, iconSize);
                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                }

                var labelStyle = ExStyles.centeredMiniBoldLabel;
                Rect labelRect = new(0, lastRect.height - 30, lastRect.width, 28);
                GUI.Label(labelRect, name, labelStyle);
            }
            GUI.EndGroup();


            GUI.color = Color.white;
            return clicked ? !isEnabled : isEnabled;
        }
    }
}