using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Glitch9.AIDevKit.Editor.Chatbots
{
    [InitializeOnLoad]
    public partial class ChatbotManagerWindow : ExtendedTreeViewWindow
        <
            ChatbotManagerWindow,
            ChatbotManagerWindow.ChatbotManagerTreeView,
            ChatbotManagerTreeViewItem,
            ChatbotManagerWindow.ChatbotManagerTreeViewDetailsWindow,
            ChatSession,
            ChatbotManagerTreeViewItemFilter,
            ChatbotManagerWindow.ChatbotManagerTreeViewContextMenuHandler
        >
    {
        static ChatbotManagerWindow() => AIDevKitEditor.onShowChatbotLibraryWindow += ShowWindow;
        public static void ShowWindow() => InitializeWindow(AIDevKitEditor.Labels.ChatSessionLibrary, true);

        internal static class ColumnIndex
        {
            internal const int MODEL = 0;
            internal const int NAME = 1;
            internal const int LAST_MESSAGE = 2;
            internal const int CREATED_AT = 3;
            internal const int UPDATED_AT = 4;
        }

        protected override List<TreeViewColumnData> CreateColumns()
        {
            List<TreeViewColumnData> columns = new()
            {
                new TreeViewColumnData
                {
                    Index = ColumnIndex.LAST_MESSAGE,
                    HeaderContent = new GUIContent("Last Message"),
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
                    Width =  TreeViewColumnWidth.Small,
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
                    Index = ColumnIndex.UPDATED_AT,
                    HeaderContent = new GUIContent("Updated At"),
                    Width =  TreeViewColumnWidth.Medium,
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

                if (GUILayout.Button("Create New Chatbot"))
                {
                    ChatSessionManager.CreateSession();
                    TreeView.Reload();
                }

                if (GUILayout.Button("Reload Chatbots"))
                {
                    ChatSessionManager.LoadSessions();
                    TreeView.Reload();
                }

                if (GUILayout.Button("Open Save Folder"))
                {
                    string path = ChatSessionUtil.GetSavePath();
                    if (!string.IsNullOrEmpty(path) && System.IO.Directory.Exists(path))
                    {
                        EditorUtility.RevealInFinder(path);
                    }
                    else
                    {
                        Debug.LogWarning("Chatbot save folder does not exist.");
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}