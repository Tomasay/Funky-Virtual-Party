using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class ModelCatalogueWindow
    {
        public class ModelCatalogueTreeView : ExtendedTreeView
        {
            public ModelCatalogueTreeView(TreeViewState treeViewState, MultiColumnHeader multiColumnHeader) : base(treeViewState, multiColumnHeader)
            {
            }

            protected override IEnumerable<ModelCatalogueEntry> GetSourceData()
            {
                return ModelCatalogue.Instance.Entries;
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                TreeViewItem treeViewItem = args.item;
                bool isObsolete = false;// = treeViewItem is ModelCatalogueTreeViewItem mItem && mItem.IsObsolete;
                bool isCustom = false;// = !isObsolete && treeViewItem is ModelCatalogueTreeViewItem cItem && cItem.IsCustom;
                bool isLocal = false;

                if (treeViewItem is ModelCatalogueTreeViewItem mItem)
                {
                    isObsolete = mItem.IsObsolete;
                    isLocal = mItem.IsLocal;
                    isCustom = mItem.IsCustom;
                }

                if (isLocal)
                {
                    GUI.color = new Color(1f, 0.35f, 0.0f, 1f);
                }
                else if (isObsolete)
                {
                    GUI.color = Color.gray;
                }
                else if (isCustom)
                {
                    GUI.color = EditorColors.blue;
                }

                for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    CellGUI(args.GetCellRect(i), treeViewItem, i, ref args);
                }

                if (isLocal || isObsolete || isCustom)
                {
                    GUI.color = Color.white;
                }
            }

            protected override void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args)
            {
                if (item is not ModelCatalogueTreeViewItem i) return;

                switch (columnIndex)
                {
                    case ColumnIndex.IN_LIB:
                        TreeViewGUI.CheckCircleCell(cellRect, i.InMyLibrary);
                        break;

                    case ColumnIndex.API:
                        AIDevKitGUI.TreeView.ApiColumn(cellRect, i.Api);
                        break;

                    // case ColumnIndex.PROVIDER:
                    //     TreeViewGUI.StringCell(cellRect, i.ModelProvider);
                    //     break;

                    case ColumnIndex.NAME:
                        Rect contentRect = cellRect;
                        if (i.IsNew)
                        {
                            Rect[] rectSplit = contentRect.SplitHorizontallyFixed(22f);
                            GUI.Label(rectSplit[0], new GUIContent(EditorIcons.NewBadge));
                            contentRect = rectSplit[1];
                        }
                        TreeViewGUI.ContentCell(contentRect, new GUIContent(i.Name, i.Description));
                        break;

                    case ColumnIndex.FAMILY:
                        TreeViewGUI.StringCell(cellRect, i.FamilyDisplayName);
                        break;

                    // case ColumnIndex.FAMILY_VERSION:
                    //     TreeViewGUI.StringCell(cellRect, i.FamilyVersion);
                    //     break;

                    case ColumnIndex.FEATURES:
                        AIDevKitGUI.TreeView.FeaturesColumn(cellRect, i.Capability);
                        break;

                    case ColumnIndex.PERFORMANCE:
                        AIDevKitGUI.TreeView.PerformanceColumn(cellRect, i.Performance);
                        break;

                    case ColumnIndex.SPEED:
                        AIDevKitGUI.TreeView.SpeedColumn(cellRect, i.Speed);
                        break;

                    case ColumnIndex.PER_INPUT_TOKEN:
                        AIDevKitGUI.TreeView.TokenCostColumn(cellRect, i.Per1MInputToken);
                        break;

                    case ColumnIndex.PER_OUTPUT_TOKEN:
                        AIDevKitGUI.TreeView.TokenCostColumn(cellRect, i.Per1MOutputToken);
                        break;

                    case ColumnIndex.CREATED:
                        TreeViewGUI.UnixDateCell(cellRect, i.CreatedAt);
                        break;
                }
            }
        }
    }
}
