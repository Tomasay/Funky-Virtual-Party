using Glitch9.AIDevKit.Editor.Pro;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class IconGeneratorWindow : ImageGeneratorWindowBase<IconGeneratorWindow>
    {
        private enum IconType
        {
            Item,
            Equipment,
            Spell,
            Character,
            Environment
        }

        protected override ImageSize Size => ImageSize._1024x1024;
        protected override Google.AspectRatio AspectRatio => Google.AspectRatio.Square;

        private EPrefs<float> _rmbgThreshold;
        private EPrefs<IconType> _type;
        private EPrefs<float> RmbgThreshold => _rmbgThreshold ??= new EPrefs<float>($"{nameof(IconGeneratorWindow)}.RmbgThreshold", 0.1f);
        private EPrefs<IconType> Type => _type ??= new EPrefs<IconType>($"{nameof(IconGeneratorWindow)}.IconType", IconType.Item);


        protected override void DrawIMGUIApiSettingsBefore()
        {
            Type.Value = (IconType)EditorGUILayout.EnumPopup("Icon Type", Type.Value);
            //RmbgThreshold.Value = EditorGUILayout.Slider(GUIContents.RmbgThreshold, RmbgThreshold.Value, 0f, 1f);
        }

        protected override void AddGridViewItemContextMenu(GenericMenu menu, int index)
        {
            // remove background menu item
            menu.AddItem(new GUIContent("Remove Background"), false, () =>
            {
                if (outputFiles.TryGetValue(index, out var file))
                {
                    var rmbgTex = ImageBackgroundRemover.RemoveBackground(file.Asset, RmbgThreshold.Value);
                    if (rmbgTex != null)
                    {
                        editedFiles.AddOrUpdate(index, new(rmbgTex));
                        RebuildUI();
                    }
                }
            });

            if (editedFiles.ContainsKey(index))
            {
                // remove the removed bg (editedFiles) from the outputFiles
                menu.AddItem(new GUIContent("Undo Edits"), false, () =>
                {
                    if (editedFiles.TryGetValue(index, out var file))
                    {
                        editedFiles.Remove(index);
                        RebuildUI();
                    }
                });

                //  save the edited file
                menu.AddItem(new GUIContent("Save Edits"), false, () =>
                {
                    if (editedFiles.TryGetValue(index, out var file))
                    {
                        outputFiles.AddOrUpdate(index, file);
                        editedFiles.Remove(index);
                        RebuildUI();
                    }
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Undo Edits"));
                menu.AddDisabledItem(new GUIContent("Save Edits"));
            }

            base.AddGridViewItemContextMenu(menu, index);
        }

        protected override void DrawPreviewFooter(VisualElement footer)
        {
            base.DrawPreviewFooter(footer);

            // add a slider for the RmbgThreshold
            var rmbgThresholdSlider = new Slider("RMBG Threshold", 0f, 1f)
            {
                value = RmbgThreshold.Value,
                name = "rmbg-threshold-slider",
                showInputField = true,
                tooltip = "Adjust the threshold for removing the background. Lower values may keep more background, while higher values will remove more."
            };

            rmbgThresholdSlider.RegisterValueChangedCallback(evt =>
            {
                RmbgThreshold.Value = evt.newValue;
            });

            rmbgThresholdSlider.AddToClassList("bottom-gap");

            footer.Add(rmbgThresholdSlider);
        }

        protected override string FormatPrompt(string prompt)
        {
            const string kPromptFormat = "Generate a {iconType} icon for a {gameTheme} {gameGenre} game in the {artStyle} style on a solid white background: {prompt}";

            return kPromptFormat
                .Replace("{iconType}", Type.Value.ToString().ToLowerInvariant())
                .Replace("{gameTheme}", GameTheme.FormatEnum())
                .Replace("{gameGenre}", GameGenre.FormatFlagsEnum())
                .Replace("{artStyle}", ArtStyle.FormatFlagsEnum())
                .Replace("{prompt}", prompt)
                .Replace("  ", " ");
        }
    }
}