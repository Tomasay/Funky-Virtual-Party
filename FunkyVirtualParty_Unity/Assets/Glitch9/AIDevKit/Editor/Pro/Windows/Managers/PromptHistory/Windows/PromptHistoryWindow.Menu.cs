using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public partial class PromptHistoryWindow
    {
        protected override IEnumerable<ITreeViewMenuEntry> CreateMenuEntries()
        {
            yield return new TreeViewMenuDropdown("File", DrawFileMenu);
            yield return new TreeViewMenuDropdown("Edit", DrawEditMenu);
            yield return new TreeViewMenuDropdown("View", DrawViewMenu);
            yield return new TreeViewMenuSearchField();
        }

        private void DrawFileMenu(Rect rect)
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent("Save Records to a JSON File"), false, BackupLogsToJsonFile);
            menu.AddItem(new GUIContent("Load Records from a JSON File"), false, RestoreLogsFromJsonFile);

            menu.DropDown(rect);
        }

        private void DrawEditMenu(Rect rect)
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent("Clear All"), false, () =>
            {
                if (ShowDialog.Confirm("Are you sure you want to clear all records? This action cannot be undone."))
                {
                    PromptHistory.Clear();
                    TreeView?.ReloadTreeView(true, true);
                }
            });


            menu.DropDown(rect);
        }

        private void DrawViewMenu(Rect rect)
        {
            if (TreeView?.Filter == null) return;

            GenericMenu menu = new();

            menu.AddItem(new GUIContent("Show OpenAI History"), PromptHistoryTreeViewSettings.ShowOpenAI, () =>
            {
                PromptHistoryTreeViewSettings.ShowOpenAI = !PromptHistoryTreeViewSettings.ShowOpenAI;
                TreeView.ReloadTreeView(true);
            });

            menu.AddItem(new GUIContent("Show Google History"), PromptHistoryTreeViewSettings.ShowGoogle, () =>
            {
                PromptHistoryTreeViewSettings.ShowGoogle = !PromptHistoryTreeViewSettings.ShowGoogle;
                TreeView.ReloadTreeView(true);
            });

            menu.AddItem(new GUIContent("Show ElevenLabs History"), PromptHistoryTreeViewSettings.ShowElevenLabs, () =>
            {
                PromptHistoryTreeViewSettings.ShowElevenLabs = !PromptHistoryTreeViewSettings.ShowElevenLabs;
                TreeView.ReloadTreeView(true);
            });

            menu.AddItem(new GUIContent("Show Ollama History"), PromptHistoryTreeViewSettings.ShowOllama, () =>
            {
                PromptHistoryTreeViewSettings.ShowOllama = !PromptHistoryTreeViewSettings.ShowOllama;
                TreeView.ReloadTreeView(true);
            });

            menu.AddItem(new GUIContent("Show OpenRouter History"), PromptHistoryTreeViewSettings.ShowOpenRouter, () =>
            {
                PromptHistoryTreeViewSettings.ShowOpenRouter = !PromptHistoryTreeViewSettings.ShowOpenRouter;
                TreeView.ReloadTreeView(true);
            });

            menu.AddSeparator(string.Empty);

            menu.AddItem(new GUIContent("Show All"), false, () =>
            {
                PromptHistoryTreeViewSettings.ShowOpenAI = true;
                PromptHistoryTreeViewSettings.ShowGoogle = true;
                PromptHistoryTreeViewSettings.ShowElevenLabs = true;
                PromptHistoryTreeViewSettings.ShowOllama = true;
                PromptHistoryTreeViewSettings.ShowOpenRouter = true;
                TreeView.ReloadTreeView(true);
            });

            menu.DropDown(rect);
        }

        private void BackupLogsToJsonFile()
        {
            // open file dialog
            string fileNameWithDate = $"openai_logs_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
            string path = EditorUtility.SaveFilePanel("Save Records to a JSON File", "", fileNameWithDate, "json");
            if (string.IsNullOrEmpty(path)) return;
            PromptHistory.BackupToJsonFile(path);
        }

        private void RestoreLogsFromJsonFile()
        {
            // open file dialog
            string path = EditorUtility.OpenFilePanel("Restore Records from a JSON File", "", "json");
            if (string.IsNullOrEmpty(path)) return;
            PromptHistory.RestoreFromJsonFile(path);
        }
    }
}