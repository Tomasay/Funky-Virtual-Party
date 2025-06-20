using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor.IMGUI;
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Assistants
{
    public partial class AssistantManagerWindow
    {
        public class AssistantManagerTreeView : ExtendedTreeView
        {
            public AssistantManagerTreeView(
                TreeViewState treeViewState,
                MultiColumnHeader multiColumnHeader) :
                base(treeViewState, multiColumnHeader)
            { }


            protected override IEnumerable<Assistant> GetSourceData()
            {
                if (AssistantManager.Assistants.IsNullOrEmpty()) return new List<Assistant>();
                return AssistantManager.Assistants;
            }

            protected override void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args)
            {
                if (item is not AssistantManagerTreeViewItem i)
                {
                    GUI.Label(cellRect, "Unknown item type");
                    return;
                }

                switch (columnIndex)
                {
                    case ColumnIndex.NAME:
                        TreeViewGUI.StringCell(cellRect, i.Data?.Name, "Unknown");
                        break;
                    case ColumnIndex.MODEL:
                        TreeViewGUI.StringCell(cellRect, i.ModelName, "Unknown");
                        break;
                    case ColumnIndex.CREATED_AT:
                        TreeViewGUI.UnixTimeCell(cellRect, i.Data?.CreatedAt);
                        break;
                    case ColumnIndex.RESPONSE_FORMAT:
                        TreeViewGUI.StringCell(cellRect, i.Data?.ResponseFormat.ToString());
                        break;
                    case ColumnIndex.DESCRIPTION:
                        TreeViewGUI.StringCell(cellRect, i.Data?.Description, "No description");
                        break;
                }
            }
        }
    }
}