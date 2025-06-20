using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Chatbots
{
    public partial class ChatbotManagerWindow
    {
        public class ChatbotManagerTreeView : ExtendedTreeView
        {
            public ChatbotManagerTreeView(
                TreeViewState treeViewState,
                MultiColumnHeader multiColumnHeader) :
                base(treeViewState, multiColumnHeader)
            { }


            protected override IEnumerable<ChatSession> GetSourceData()
            {
                if (ChatSessionManager.Sessions.IsNullOrEmpty()) return new List<ChatSession>();
                return ChatSessionManager.Sessions;
            }


            protected override void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args)
            {
                if (item is not ChatbotManagerTreeViewItem i)
                {
                    GUI.Label(cellRect, "Unknown item type");
                    return;
                }

                switch (columnIndex)
                {
                    case ColumnIndex.MODEL:
                        TreeViewGUI.StringCell(cellRect, i.ModelName, "Unknown");
                        break;
                    case ColumnIndex.NAME:
                        TreeViewGUI.StringCell(cellRect, i.ChatbotName, "Unknown");
                        break;
                    case ColumnIndex.CREATED_AT:
                        TreeViewGUI.UnixTimeCell(cellRect, i.CreatedAt);
                        break;
                    case ColumnIndex.UPDATED_AT:
                        TreeViewGUI.UnixTimeCell(cellRect, i.UpdatedAt);
                        break;
                    case ColumnIndex.LAST_MESSAGE:
                        TreeViewGUI.StringCell(cellRect, i.LastMessage);
                        break;
                }
            }
        }
    }
}