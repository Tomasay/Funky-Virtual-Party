using Glitch9.AIDevKit.GENTasks;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using Glitch9.IO.Files;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public class PromptHistoryTreeViewItem : ExtendedTreeViewItem<PromptHistoryTreeViewItem, PromptRecord, PromptHistoryTreeViewItemFilter>
    {
        public Api Api { get; set; }
        public int TaskType { get; set; }
        public string Sender { get; set; }
        public UnixTime CreatedAt { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string InputPreview { get; set; }
        public string OutputPreview { get; set; }
        public Currency Price { get; set; }

        public override int CompareTo(PromptHistoryTreeViewItem anotherItem, int columnIndex, bool ascending)
        {
            return columnIndex switch
            {
                PromptHistoryWindow.ColumnIndex.SENDER => CompareByString(ascending, anotherItem, data => data.Sender),
                PromptHistoryWindow.ColumnIndex.DATE => CompareByUnixTime(!ascending, anotherItem, data => data.CreatedAt),
                PromptHistoryWindow.ColumnIndex.MODEL => CompareByString(ascending, anotherItem, data => data.ModelName),
                PromptHistoryWindow.ColumnIndex.INPUT => CompareByString(ascending, anotherItem, data => data.InputPreview),
                PromptHistoryWindow.ColumnIndex.OUTPUT => CompareByString(ascending, anotherItem, data => data.OutputPreview),
                PromptHistoryWindow.ColumnIndex.TASK_TYPE => CompareByInt(ascending, anotherItem, data => data.TaskType),
                PromptHistoryWindow.ColumnIndex.PRICE => CompareByDouble(ascending, anotherItem, data => data.Price),
                _ => 0
            };
        }

        public override bool Search(string searchString)
        {
            return EditorSearchUtil.Search(searchString, Api) ||
                   EditorSearchUtil.Search(searchString, Sender) ||
                   EditorSearchUtil.Search(searchString, ModelName) ||
                   EditorSearchUtil.Search(searchString, InputPreview) ||
                   EditorSearchUtil.Search(searchString, OutputPreview);
        }

        public PromptHistoryTreeViewItem(int id, int depth, string displayName, PromptRecord data) : base(id, depth, displayName, data)
        {
            try
            {
                Api = data.Api;
                Sender = data.Sender;
                CreatedAt = data.CreatedAt;

                ModelId = data.ModelId;
                ModelName = data.ModelName;
                TaskType = data.TaskType;

                InputPreview = GetContentDisplayText(TaskType, data.InputText, data.InputFiles, true);
                OutputPreview = GetContentDisplayText(TaskType, data.OutputText, data.OutputFiles, false);

                Price = data.Price;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while creating OpenAILogTreeViewItem: {e.Message}");
            }
        }

        private static string GetContentDisplayText(int taskType, string text, List<IFile> files, bool isInput)
        {
            if (text != null) return ProcessText(text);
            if (files == null || files.Count == 0) return string.Empty;

            bool checkImage = EndpointType.HasImageInput(taskType) && isInput ||
                                EndpointType.HasImageOutput(taskType) && !isInput;

            bool checkAudio = EndpointType.HasAudioInput(taskType) && isInput ||
                                EndpointType.HasAudioOutput(taskType) && !isInput;

            int imageCount = 0;

            foreach (IFile content in files)
            {
                if (checkImage && content is File<Texture2D>)
                {
                    imageCount++;
                    continue;
                }

                if (checkAudio && content is File<AudioClip> audioResource)
                {
                    return GenerateAudioLengthString(audioResource.MediaLength);
                }
            }

            if (checkImage)
            {
                return imageCount == 1 ? "1 image" : $"{imageCount} images";
            }

            return string.Empty;
        }

        private static string ProcessText(string text)
        {
            return text.Replace("\n", " ").Trim();
        }

        private static string GenerateAudioLengthString(float minutes)
        {
            // if less than 1 minute, return seconds
            if (minutes == 0) return "-";
            if (minutes < 1) return $"{minutes * 60} seconds";
            if (minutes < 2) return "1 minute";
            return $"{minutes} minutes";
        }
    }
}