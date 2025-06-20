using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Files
{
    public partial class FileManagerWindow
    {
        public class FileTreeView : ExtendedTreeView
        {
            internal int TotalSizeOpenAI { get; private set; }
            internal int TotalSizeGoogle { get; private set; }
            internal string TotalSizeStringOpenAI { get; private set; }
            internal string TotalSizeStringGoogle { get; private set; }

            public FileTreeView(TreeViewState treeViewState, MultiColumnHeader multiColumnHeader) : base(treeViewState, multiColumnHeader) { }

            protected override void OnTreeViewUpdated()
            {
                TotalSizeOpenAI = 0;
                TotalSizeGoogle = 0;

                foreach (ApiFile data in SourceData)
                {
                    if (data == null) continue;

                    if (data.Api == Api.OpenAI)
                    {
                        TotalSizeOpenAI += data.ByteSize;
                    }
                    else if (data.Api == Api.Google)
                    {
                        TotalSizeGoogle += data.ByteSize;
                    }
                }

                TotalSizeStringOpenAI = FileManager.ToBytesString(TotalSizeOpenAI);
                TotalSizeStringGoogle = FileManager.ToBytesString(TotalSizeGoogle);
            }

            protected override IEnumerable<ApiFile> GetSourceData()
            {
                return FileLibrary.DB.Values;
            }

            internal async void ReloadFiles()
            {
                SourceData = await FileManager.ReloadFilesAsync();
                ReloadTreeView(true, true);
            }

            protected override void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args)
            {
                if (item is not FileTreeViewItem i)
                {
                    GUI.Label(cellRect, "Unknown item type");
                    return;
                }

                switch (columnIndex)
                {
                    case ColumnIndex.API:
                        AIDevKitGUI.TreeView.ApiColumn(cellRect, i.Api);
                        break;
                    case ColumnIndex.FILE_NAME:
                        TreeViewGUI.ContentCell(cellRect, new GUIContent(i.FileName, i.FileIcon));
                        break;
                    case ColumnIndex.BYTE_SIZE:
                        TreeViewGUI.StringCell(cellRect, i.FormattedSize);
                        break;
                    case ColumnIndex.MIME_TYPE:
                        TreeViewGUI.StringCell(cellRect, i.MimeType.ToApiValue());
                        break;
                    case ColumnIndex.CREATED_AT:
                        TreeViewGUI.UnixTimeCell(cellRect, i.CreatedAt);
                        break;
                    case ColumnIndex.EXPIRES_AT:
                        TreeViewGUI.UnixTimeCell(cellRect, i.ExpiresAt);
                        break;
                }
            }

        }
    }
}