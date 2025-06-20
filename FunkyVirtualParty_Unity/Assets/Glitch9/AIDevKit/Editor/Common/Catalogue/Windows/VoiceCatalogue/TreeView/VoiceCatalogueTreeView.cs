using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using Glitch9.IO.Files;
using System.Collections.Generic;
using System.IO;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class VoiceCatalogueWindow
    {
        public class VoiceCatalogueTreeView : ExtendedTreeView
        {
            public VoiceCatalogueTreeView(TreeViewState treeViewState, MultiColumnHeader multiColumnHeader) : base(treeViewState, multiColumnHeader)
            {
            }

            protected override IEnumerable<VoiceCatalogueEntry> GetSourceData()
            {
                return VoiceCatalogue.Instance.Entries;
            }



            protected override void RowGUI(RowGUIArgs args)
            {
                TreeViewItem treeViewItem = args.item;

                if (treeViewItem is VoiceCatalogueTreeViewItem voiceItem)
                {
                    Rect rowRect = args.rowRect;
                    Texture rowTex = ResolveRowTexture(voiceItem);
                    GUI.DrawTexture(rowRect, rowTex, ScaleMode.StretchToFill);
                }

                for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    CellGUI(args.GetCellRect(i), treeViewItem, i, ref args);
                }
            }

            protected override void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args)
            {
                if (item is not VoiceCatalogueTreeViewItem i) return;

                switch (columnIndex)
                {
                    case ColumnIndex.IN_LIB:
                        TreeViewGUI.CheckCircleCell(cellRect, i.InMyLibrary);
                        break;

                    case ColumnIndex.API:
                        AIDevKitGUI.TreeView.ApiColumn(cellRect, i.Api);
                        break;

                    case ColumnIndex.NAME:
                        Rect contentRect = cellRect;

                        Rect[] rectSplit = contentRect.SplitHorizontallyFixed(22f);
                        Rect playBtnRect = rectSplit[0];
                        contentRect = rectSplit[1];

                        if (GUI.Button(playBtnRect, EditorIcons.PlayButton))
                        {
                            i.Data.PlayPreviewAsync().Forget();
                        }

                        if (i.IsNew)
                        {
                            rectSplit = contentRect.SplitHorizontallyFixed(22f);
                            GUI.Label(rectSplit[0], new GUIContent(EditorIcons.NewBadge));
                            contentRect = rectSplit[1];
                        }

                        string name = i.Name;
                        if (i.IsFeatured) name += "⭐";

                        TreeViewGUI.ContentCell(contentRect, new GUIContent(name, i.Description));
                        break;

                    case ColumnIndex.TYPE:
                        TreeViewGUI.StringCell(cellRect, i.Type.GetInspectorName());
                        break;

                    case ColumnIndex.GENDER:
                        TreeViewGUI.StringCell(cellRect, i.Gender.GetInspectorName());
                        break;

                    case ColumnIndex.AGE:
                        TreeViewGUI.StringCell(cellRect, i.Age.GetInspectorName());
                        break;

                    case ColumnIndex.LANGUAGE:
                        TreeViewGUI.StringCell(cellRect, i.LanguageDisplay);
                        break;
                }
            }
        }

        private static Texture ResolveRowTexture(VoiceCatalogueTreeViewItem voiceItem)
        {
            if (voiceItem.Gender == VoiceGender.Male) return EditorTextures.transparentBlueTexture;
            if (voiceItem.Gender == VoiceGender.Female) return EditorTextures.transparentPinkTexture;
            return EditorTextures.transparentPurpleTexture;
        }
    }
}
