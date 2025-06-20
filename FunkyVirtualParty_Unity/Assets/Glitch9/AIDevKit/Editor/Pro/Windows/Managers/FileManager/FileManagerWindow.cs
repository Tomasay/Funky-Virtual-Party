
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Files
{
    [InitializeOnLoad]
    public partial class FileManagerWindow : ExtendedTreeViewWindow
        <
            FileManagerWindow,
            FileManagerWindow.FileTreeView,
            FileTreeViewItem,
            FileManagerWindow.FileTreeViewDetailsWindow,
            ApiFile,
            TreeViewItemFilter,
            FileManagerWindow.FileTreeViewContextMenuHandler
        >
    {
        static FileManagerWindow() => AIDevKitEditor.onShowFileManagerWindow += ShowWindow;
        public static void ShowWindow() => InitializeWindow(AIDevKitEditor.Labels.FileManager);

        internal static class ColumnIndex
        {
            internal const int API = 0;
            internal const int FILE_NAME = 1;
            internal const int BYTE_SIZE = 2;
            internal const int MIME_TYPE = 3;
            internal const int CREATED_AT = 4;
            internal const int EXPIRES_AT = 5;
        }

        protected override List<TreeViewColumnData> CreateColumns()
        {
            List<TreeViewColumnData> columns = new()
            {
                new TreeViewColumnData
                {
                    Index = ColumnIndex.API,
                    HeaderContent = new GUIContent("API"),
                    Width = TreeViewColumnWidth.Tiny,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.FILE_NAME,
                    HeaderContent = new GUIContent("File Name"),
                    Width = TreeViewColumnWidth.Wide,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.BYTE_SIZE,
                    HeaderContent = new GUIContent("File Size"),
                    Width =  TreeViewColumnWidth.Tiny,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.MIME_TYPE,
                    HeaderContent = new GUIContent("MIME Type"),
                    Width =  TreeViewColumnWidth.Tiny,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.CREATED_AT,
                    HeaderContent = new GUIContent("Created At"),
                    Width =  TreeViewColumnWidth.Small,
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.EXPIRES_AT,
                    HeaderContent = new GUIContent("Expires At"),
                    Width =  TreeViewColumnWidth.Small,
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

            GUILayout.BeginHorizontal(AIDevKitStyles.SearchBarStyle);
            try
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label($"Total Size (OpenAI): {TreeView?.TotalSizeStringOpenAI ?? "Unknown"}", GUILayout.Width(180));
                    GUILayout.Label($"Total Size (Google): {TreeView?.TotalSizeStringGoogle ?? "Unknown"}", GUILayout.Width(180));
                }
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button(new GUIContent("Upload", AIDevKitIcons.OpenAI), AIDevKitStyles.SearchBarButton))
                    {
                        FileManager.UploadFile(Api.OpenAI, () => TreeView?.ReloadTreeView(true, true));
                    }

                    if (GUILayout.Button(new GUIContent("Upload", AIDevKitIcons.Google), AIDevKitStyles.SearchBarButton))
                    {
                        FileManager.UploadFile(Api.Google, () => TreeView?.ReloadTreeView(true, true));
                    }
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("Reload\nFiles", AIDevKitStyles.BigSearchBarButton))
                {
                    TreeView.ReloadFiles();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }
    }
}