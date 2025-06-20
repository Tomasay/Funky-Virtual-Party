using System;
using Glitch9.AIDevKit.Editor.Pro;
using Glitch9.Editor.UIToolkit;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal static class GeneratorUtil
    {
        internal static string ResolveLabelText(PromptRecord record)
        {
            using (StringBuilderPool.Get(out var sb))
            {
                if (record == null) return string.Empty;

                if (record.CreatedAt != default)
                {
                    sb.Append($"{FormatUnixTime(record.CreatedAt)} | ");
                }

                if (!string.IsNullOrEmpty(record.Prompt))
                {
                    sb.Append(record.Prompt);
                }

                return sb.ToString();
            }
        }

        internal static string FormatUnixTime(UnixTime unixTime)
        {
            DateTime dateTime = unixTime.ToLocalTime();

            // 오늘이라면 "오늘 HH:mm:ss" 형식으로 표시
            if (dateTime.Date == DateTime.Today)
            {
                return $"Today {dateTime:H:mm tt}";
            }

            // 어제라면 "어제 HH:mm:ss" 형식으로 표시
            if (dateTime.Date == DateTime.Today.AddDays(-1))
            {
                return $"Yesterday {dateTime:H:mm tt}";
            }

            // 그 외의 날짜는 "yyyy-MM-dd HH:mm:ss" 형식으로 표시
            return dateTime.ToString("yyyy-MM-dd");
        }

        internal static Texture2D RemoveBackground(int index, Texture2D tex, float threashold, string[] outputContentPaths)
        {
            if (tex == null) return null;

            tex = ImageBackgroundRemover.RemoveBackground(tex, threashold);
            string originalPath = outputContentPaths[index];  // add "_bg_removed" to the original path

            string newPath = originalPath.Replace(".png", "_bg_removed.png");
            System.IO.File.WriteAllBytes(newPath, tex.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log($"Background removed and saved to {newPath}");

            return tex;
        }

        internal static void PrepareContainer(this VisualElement container)
        {
            container.Clear();
            container.style.alignContent = Align.Center;
            container.style.flexDirection = FlexDirection.Column;
            container.style.flexWrap = Wrap.Wrap;
            container.style.justifyContent = Justify.Center;
        }

        internal static void PrepareIMGUIContainer(this VisualElement container)
        {
            container.style.flexDirection = FlexDirection.Column;
            container.style.flexGrow = 1f;
            container.style.flexShrink = 0f;
        }

        internal static VisualElement CreateGridViewLoadingItem(string message, string title = null)
        {
            var spinner = new LoadingSpinner(32);
            spinner.AddToClassList("preview-image");
            if (!string.IsNullOrEmpty(title))
            {
                var titleLabel = new Label(title);
                titleLabel.AddToClassList("grid-view-box-info-title");
                spinner.Add(titleLabel);
            }
            var label = new Label(message);
            label.AddToClassList("grid-view-box-info-details");
            spinner.Add(label);
            return spinner;
        }

        internal static VisualElement CreateGridViewEmptyItem(string message, IFile file = null)
        {
            string path = null;
            if (file != null) path = $"<color=yellow>{file.FullPath}</color>";

            var column = new VisualElement();
            column.AddToClassList("preview-image");
            column.style.flexDirection = FlexDirection.Column;
            column.style.alignItems = Align.Center;
            column.style.justifyContent = Justify.Center;

            if (!string.IsNullOrEmpty(message))
            {
                var titleLabel = new Label(message);
                titleLabel.AddToClassList("grid-view-box-info-title");
                titleLabel.AddToClassList("bottom-gap");
                column.Add(titleLabel);
            }

            if (!string.IsNullOrEmpty(path))
            {
                var pathLabel = new Label(path);
                pathLabel.AddToClassList("grid-view-box-info-details");
                pathLabel.AddToClassList("bottom-gap");
                column.Add(pathLabel);
            }

            // add a button to open the directory of the file
            var openButton = new Button(() => EditorUtility.RevealInFinder(file.FullPath))
            {
                text = "Open Directory"
            };

            column.Add(openButton);
            return column;
        }

    }
}