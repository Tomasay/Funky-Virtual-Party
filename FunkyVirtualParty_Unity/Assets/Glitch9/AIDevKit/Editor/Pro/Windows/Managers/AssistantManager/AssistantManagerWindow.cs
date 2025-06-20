using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Glitch9.AIDevKit.Editor.Assistants
{
    [InitializeOnLoad]
    public partial class AssistantManagerWindow : ExtendedTreeViewWindow
        <
            AssistantManagerWindow,
            AssistantManagerWindow.AssistantManagerTreeView,
            AssistantManagerTreeViewItem,
            AssistantManagerWindow.AssistantManagerTreeViewDetailsWindow,
            Assistant,
            AssistantManagerTreeViewItemFilter,
            AssistantManagerWindow.AssistantManagerTreeViewContextMenuHandler
        >
    {
        static AssistantManagerWindow() => AIDevKitEditor.onShowOpenAIAssistantManagerWindow += ShowWindow;
        public static void ShowWindow() => InitializeWindow(AIDevKitEditor.Labels.OpenAIAssistants, true);

        internal static class ColumnIndex
        {
            //public const int ID = 0;

            internal const int MODEL = 0;
            internal const int NAME = 1;
            internal const int DESCRIPTION = 2;
            internal const int RESPONSE_FORMAT = 3;
            internal const int CREATED_AT = 4;
        }

        protected override List<TreeViewColumnData> CreateColumns()
        {
            List<TreeViewColumnData> columns = new()
            {
                new TreeViewColumnData
                {
                    Index = ColumnIndex.DESCRIPTION,
                    HeaderContent = new GUIContent("Description"),
                    Width = TreeViewColumnWidth.ExtraWide,
                    CanSort = false,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.NAME,
                    HeaderContent = new GUIContent("Name"),
                    Width = TreeViewColumnWidth.Medium,
                    CanSort = true,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.MODEL,
                    HeaderContent = new GUIContent("Model"),
                    Width =  TreeViewColumnWidth.Tiny,
                    CanSort = true,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.CREATED_AT,
                    HeaderContent = new GUIContent("Created At"),
                    Width =  TreeViewColumnWidth.Medium,
                    CanSort = true,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.RESPONSE_FORMAT,
                    HeaderContent = new GUIContent("Response Format"),
                    Width =  TreeViewColumnWidth.Tiny,
                    CanSort = true,
                },
            };

            return columns;
        }

        protected override IEnumerable<ITreeViewMenuEntry> CreateMenuEntries()
        {
            yield return new TreeViewMenuSearchField();
        }

        protected override void BottomBar()
        {
            if (TreeView == null) return;

            GUILayout.BeginHorizontal(TreeViewStyles.BottomBarStyle);
            {
                DrawRefreshButton();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create New Assistant"))
                {
                    CreateAssistant();
                }

                if (GUILayout.Button("Reload Assistants"))
                {
                    ReloadAssistants();
                }
            }
            GUILayout.EndHorizontal();
        }

        private async void CreateAssistant()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Creating Assistant", "Please wait...", 0.5f);
                Assistant assistant = await AssistantManager.CreateAssistantAsync() ?? throw new Exception("Failed to create assistant.");
                TreeView.AddData(assistant);
                TreeView.Reload();
            }
            catch (Exception e)
            {
                ShowDialog.Error($"Failed to create assistant: {e.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private async void ReloadAssistants()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Reloading Assistants", "Please wait...", 0.5f);
                List<Assistant> assistants = await AssistantManager.ReloadAssistantsAsync() ?? throw new Exception("Failed to reload assistants.");
                TreeView.SetData(assistants);
                TreeView.Reload();
            }
            catch (Exception e)
            {
                ShowDialog.Error($"Failed to reload assistants: {e.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}