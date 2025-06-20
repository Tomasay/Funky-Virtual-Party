using System;
using System.Collections.Generic;
using System.Linq;
using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal abstract class FileGeneratorWindowBase<TSelf, TSettings, TFile> : GeneratorWindowBase<TSelf, TSettings>
        where TSelf : FileGeneratorWindowBase<TSelf, TSettings, TFile>
        where TSettings : GeneratorSettings<TSelf, TSettings>, new()
        where TFile : class, IFile
    {
        protected Dictionary<int, TFile> outputFiles = new();
        protected Dictionary<int, TFile> editedFiles = new();
        protected GridView<int> gridView;
        protected virtual float GridViewAspectRatio => 1f;
        private readonly Dictionary<int, VisualElement> _loadingSpinners = new();

        protected override void PrepareGeneration()
        {
            outputFiles.Clear();
            editedFiles.Clear();
            _loadingSpinners.Clear();
        }

        protected override void SaveGeneratedContents()
        {
            if (selectedIndices.IsNullOrEmpty())
            {
                ShowDialog.Warning("No items selected. Please select at least one item to save.");
                return;
            }

            if (outputFiles.IsNullOrEmpty())
            {
                Debug.Log("No items to save.");
                return;
            }

            foreach (var index in selectedIndices)
            {
                if (outputFiles.TryGetValue(index, out var file))
                {
                    string path = EditorUtility.SaveFilePanel("Save Generated Content", GetSavePath(), file.Name, file.Extension);

                    if (!string.IsNullOrEmpty(path))
                    {
                        try
                        {
                            file.CopyTo(path);
                            Debug.Log($"Generated content saved to: {path}");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to save generated content: {e.Message}");
                            ShowDialog.Error($"Failed to save generated content: {e.Message}");
                        }
                    }
                }
            }
        }

        protected override bool OnSelectPromptRecord()
        {
            var files = currentRecord?.GetOutputFiles<TFile>();
            if (files.IsNullOrEmpty()) return false;

            outputFiles.Clear();
            for (int i = 0; i < files.Count; i++)
                outputFiles.Add(i, files[i]);

            return true;
        }

        protected override void RebuildPreviewUI()
        {
            previewContainer?.Clear();
            if (previewContainer == null) return;

            gridView = new GridView<int>(
                makeItem: i => CreateGridViewItem(i),
                bindItem: (ve, i) => { },
                spacing: 8f,
                aspectRatio: GridViewAspectRatio
            );

            int count;

            if (IsGenerating)
            {
                count = N;
            }
            else
            {
                count = outputFiles?.Count ?? 0;
            }

            gridView.SetItems(Enumerable.Range(0, count).ToList());
            previewContainer.Add(gridView);
        }

        protected VisualElement CreateLoadingSpinner(int index, string message = "Generating...", string title = null)
        {
            if (!_loadingSpinners.TryGetValue(index, out var spinner))
            {
                spinner = GeneratorUtil.CreateGridViewLoadingItem(message, title);
                _loadingSpinners[index] = spinner;
            }

            return spinner;
        }

        protected TFile GetOutputFile(int index)
        {
            if (editedFiles != null && editedFiles.TryGetValue(index, out var editedFile))
            {
                return editedFile;
            }

            if (outputFiles != null && outputFiles.TryGetValue(index, out var file))
            {
                return file;
            }

            return null;
        }

        private VisualElement CreateGridViewItem(int i)
        {
            TFile file = GetOutputFile(i);

            if (file == null || !file.HasData)
            {
                if (IsGenerating) return CreateLoadingSpinner(i);
                return GeneratorUtil.CreateGridViewEmptyItem("No content available at:", file);
            }

            VisualElement box = CreateGridViewItemINTERNAL(file, i);

            bool selected = IsSelected(i);
            if (selected) box.AddToClassList("selected");

            int capturedIndex = i;
            box.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    bool holdingShift = evt.shiftKey;
                    bool holdingCtrl = evt.ctrlKey || evt.commandKey;

                    if (holdingShift)
                    {
                        SelectRange(capturedIndex);
                    }
                    else if (holdingCtrl)
                    {
                        ToggleSelection(capturedIndex);
                    }
                    else
                    {
                        ToggleSelection(capturedIndex);
                    }

                    RebuildPreviewUI();
                }
                else
                {
                    if (selected) ShowGridViewItemContextMenu(capturedIndex);
                }
            });

            return box;
        }

        protected abstract VisualElement CreateGridViewItemINTERNAL(TFile file, int index);

        private void ShowGridViewItemContextMenu(int index)
        {
            var menu = new GenericMenu();
            AddGridViewItemContextMenu(menu, index);
            menu.ShowAsContext();
        }

        protected virtual void AddGridViewItemContextMenu(GenericMenu menu, int index)
        {
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                if (index < 0 || outputFiles.IsNullOrEmpty()) return;

                outputFiles.Remove(index);
                currentRecord?.RemoveOutputFileAt<TFile>(index);
                ClearSelection();
                RebuildPreviewUI();
            });
        }
    }
}