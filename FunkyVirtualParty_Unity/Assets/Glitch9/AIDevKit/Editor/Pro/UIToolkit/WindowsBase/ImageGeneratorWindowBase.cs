using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.Google;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;
using Glitch9.IO.Files;
using System;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System.Linq;
using Glitch9.Editor.UIToolkit;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal abstract class ImageGeneratorWindowBase<TSelf> : FileGeneratorWindowBase<TSelf, ImageGeneratorSettings<TSelf>, File<Texture2D>>
        where TSelf : ImageGeneratorWindowBase<TSelf>
    {
        protected override void OnModelChanged(Model model)
        {
            _gridViewAspectRatio = ImageModelUtil.ResolveAspectRatio(model, AspectRatio, Size);
            UpdateAspectRatio();
        }

        protected virtual ImageSize Size { get => Settings.Size.Value; set => Settings.Size.Value = value; }
        protected virtual ImageQuality Quality { get => Settings.Quality.Value; set => Settings.Quality.Value = value; }
        protected virtual ImageStyle Style => ArtStyle.HasFlag(ArtStyle.Realistic) ? ImageStyle.Vivid : ImageStyle.Natural;
        protected virtual Google.AspectRatio AspectRatio { get => Settings.AspectRatio.Value; set => Settings.AspectRatio.Value = value; }
        protected virtual PersonGeneration PersonGeneration => PersonGeneration.None;
        protected ArtStyle ArtStyle { get => Settings.ArtStyle.Value; set => Settings.ArtStyle.Value = value; }
        protected GameGenre GameGenre { get => Settings.GameGenre.Value; set => Settings.GameGenre.Value = value; }
        protected GameTheme GameTheme { get => Settings.GameTheme.Value; set => Settings.GameTheme.Value = value; }

        protected override float GridViewAspectRatio => _gridViewAspectRatio;
        private float _gridViewAspectRatio = 1f;

        private void UpdateAspectRatio()
        {
            if (gridView == null || _gridViewAspectRatio <= 0f) return;
            gridView.schedule.Execute(() =>
            {
                gridView.SetAspectRatio(_gridViewAspectRatio);
            }).ExecuteLater(1); // 1 frame 뒤에 실행
        }

        protected abstract string FormatPrompt(string prompt);
        protected override void DrawIMGUIGenerationSettings()
        {
            Model = AIDevKitGUI.IMGPopup(Model, label: GUIContents.Model, apiWidth: 80);
            DrawIMGUINSlider();
        }

        protected override void DrawIMGUIApiParameters()
        {
            if (Model == null) return;

            DrawIMGUIApiSettingsBefore();

            EditorGUI.BeginDisabledGroup(Model.Api != Api.OpenAI);
            {
                Quality = AIDevKitGUI.ImageQualityPopup(Quality, Model);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(UseProjectContext);
            {
                ArtStyle = (ArtStyle)EditorGUILayout.EnumFlagsField(GUIContents.ArtStyle, ArtStyle);
                GameGenre = (GameGenre)EditorGUILayout.EnumFlagsField(GUIContents.GameGenre, GameGenre);
                GameTheme = (GameTheme)EditorGUILayout.EnumPopup(GUIContents.GameTheme, GameTheme);
            }
            EditorGUI.EndDisabledGroup();

            bool newUseProjectContext = ExGUILayout.AnimatedToggle(GUIContents.UseProjectContext, UseProjectContext);
            if (newUseProjectContext != UseProjectContext)
            {
                UseProjectContext = newUseProjectContext;

                if (UseProjectContext)
                {
                    ArtStyle = AIDevKitSettings.ProjectContext.ArtStyle;
                    GameGenre = AIDevKitSettings.ProjectContext.Genre;
                    GameTheme = AIDevKitSettings.ProjectContext.Theme;
                }
            }

            DrawIMGUIApiSettingsAfter();
        }

        protected virtual void DrawIMGUIApiSettingsBefore() { }
        protected virtual void DrawIMGUIApiSettingsAfter() { }

        protected override void OnClickUseThisPromptButton() => throw new NotImplementedException();
        protected override async UniTask GenerateContentAsync(string prompt)
        {
            int index = 0;

            string formattedPrompt = FormatPrompt(prompt);
            GENImageTask task = prompt.GENImage(formattedPrompt)
                .SetModel(Model)
                .SetCount(N)
                .SetOutputPath(AIDevKitSettings.OutputPath)
                .SetSender(windowName);

            if (Model.Api == Api.OpenAI)
            {
                if (Model.IsDallE3())
                {
                    task.SetSize(Size)
                        .SetQuality(Quality)
                        .SetStyle(Style);
                }

                if (Model.IsDallE2())
                {
                    task.SetSize(Size);
                }

                if (Model.IsGptImage1())
                {
                    task.SetSize(Size)
                        .SetQuality(Quality);
                }
            }
            else if (Model.Api == Api.Google)
            {
                task.SetAspectRatio(AspectRatio)
                    .SetPersonGeneration(PersonGeneration);
            }

            await foreach (GeneratedImage image in task.YieldAsync())
            {
                foreach (var img in image?.ToFiles() ?? Enumerable.Empty<File<Texture2D>>())
                    outputFiles.Add(index++, img);
            }
        }


        private async void LoadImageAsync(int index, bool forceRefresh = false)
        {
            if (index < 0 || index >= outputFiles.Count)
            {
                Debug.LogWarning($"Invalid image index: {index}. Valid range is 0 to {outputFiles.Count - 1}.");
                return;
            }

            await outputFiles[index].LoadAssetAsync(forceRefresh, file => RebuildPreviewUI());
        }

        protected override VisualElement CreateGridViewItemINTERNAL(File<Texture2D> file, int i)
        {
            Texture2D tex = file.Asset;
            VisualElement box = null;

            if (tex == null)
            {
                if (file.IsError)
                {
                    string errorMessage = file.LastError;
                    if (string.IsNullOrEmpty(errorMessage)) errorMessage = $"Error loading image from path: {file.FullPath}";

                    box = new Label(errorMessage);
                    box.AddToClassList("preview-image");
                    box.AddToClassList("message-error");
                }
                else if (!file.IsLoading)
                {
                    LoadImageAsync(i);
                }

                if (box == null)
                {
                    box = new LoadingSpinner(32);
                    box.AddToClassList("preview-image");
                }

                var refreshButton = new Button(() => LoadImageAsync(i, true)) { text = "Refresh" };
                refreshButton.AddToClassList("refresh-button");
                box.Add(refreshButton);
            }
            else
            {
                box = new VisualElement();
                box.AddToClassList("preview-image");
                box.style.backgroundImage = new StyleBackground(tex);
            }

            return box;
        }
    }
}