using System;
using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using Glitch9.AIDevKit.Google;
using UnityEngine;
using System.Collections.Generic;
using Glitch9.IO.Files;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class VideoGeneratorWindow : FileGeneratorWindowBase<VideoGeneratorWindow, VideoGeneratorSettings<VideoGeneratorWindow>, RawFile>
    {
        protected Google.AspectRatio AspectRatio { get => Settings.AspectRatio.Value; set => Settings.AspectRatio.Value = value; }
        protected PersonGeneration PersonGeneration { get => Settings.PersonGeneration.Value; set => Settings.PersonGeneration.Value = value; }
        private readonly Dictionary<int, RawFile> outputVideos = new();
        internal float aspectRatio = 1f;

        protected override void DrawIMGUIGenerationSettings()
        {
            Model = AIDevKitGUI.VIDPopup(Model, label: GUIContents.Model, apiWidth: 80);
            DrawIMGUINSlider();
        }

        protected override void DrawIMGUIApiParameters()
        {
            //ExGUILayout.IconLabel("Google Veo Settings", AIDevKitIcons.Google, 5, GeneratorStyles.NodeTitle);
            var newAspectRatio = ExGUILayout.EnumPopup("Aspect Ratio", AspectRatio);
            if (newAspectRatio != AspectRatio)
            {
                AspectRatio = newAspectRatio;
                gridView?.SetAspectRatio(aspectRatio);
            }
            PersonGeneration = ExGUILayout.EnumSwitch("Person Gen.", PersonGeneration);
        }

        protected override async UniTask GenerateContentAsync(string prompt)
        {
            if (string.IsNullOrEmpty(prompt)) return;

            IsGenerating = true;
            try
            {
                await prompt.GENVideo()
                    .SetModel(Model)
                    .SetSender(windowName)
                    .SetOutputPath(AIDevKitSettings.OutputPath)
                    .SetAspectRatio(AspectRatio)
                    .SetPersonGeneration(PersonGeneration)
                    .ExecuteAsync();

                RebuildUI();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ShowDialog.Error(e.Message);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        protected override void OnClickUseThisPromptButton() => throw new NotImplementedException();

        protected override VisualElement CreateGridViewItemINTERNAL(RawFile file, int i)
        {
            string filePath = file.FullPath;

            VisualElement box = new();
            box.AddToClassList("preview-image");

            // add url label
            var urlLabel = new Label(file.Url ?? "No URL");
            urlLabel.AddToClassList("url-label");
            box.Add(urlLabel);

            // add file path
            var filePathLabel = new Label(filePath);
            filePathLabel.AddToClassList("file-path-label");
            box.Add(filePathLabel);

            // add open folder button and play video button
            var openFolderButton = new Button(() =>
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    ShowDialog.Error("File path is empty.");
                    return;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    ShowDialog.Error("File does not exist.");
                    return;
                }

                string folderPath = System.IO.Path.GetDirectoryName(filePath);
                if (System.IO.Directory.Exists(folderPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(folderPath) { UseShellExecute = true });
                }
                else
                {
                    ShowDialog.Error("Folder not found.");
                }
            })
            {
                text = "Open Folder"
            };

            var playVideoButton = new Button(() =>
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    ShowDialog.Error("File path is empty.");
                    return;
                }

                if (!System.IO.File.Exists(filePath))
                {
                    ShowDialog.Error("File does not exist.");
                    return;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                else
                {
                    ShowDialog.Error("File not found.");
                }
            })
            {
                text = "Play Video"
            };

            box.Add(openFolderButton);
            box.Add(playVideoButton);

            return box;
        }
    }
}