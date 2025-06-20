using Glitch9.AIDevKit.GENTasks;
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public partial class PromptHistoryWindow
    {
        public class PromptHistoryTreeView : ExtendedTreeView
        {
            public Currency TotalPrice { get; private set; }
            public PromptHistoryTreeView(TreeViewState tv, MultiColumnHeader mch) : base(tv, mch)
                => multiColumnHeader.SetSorting(0, true);

            protected override IEnumerable<PromptRecord> GetSourceData() => PromptHistory.ToEnumerable();
            protected override void RemoveSourceData(PromptRecord data) => PromptHistory.Remove(data);

            protected override void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args)
            {
                if (item is not PromptHistoryTreeViewItem i)
                {
                    GUI.Label(cellRect, "Unknown item type");
                    return;
                }

                switch (columnIndex)
                {
                    case ColumnIndex.TASK_TYPE:
                        TreeViewGUI.StringCell(cellRect, EndpointType.GetName(i.TaskType), "-");
                        break;
                    case ColumnIndex.DATE:
                        TreeViewGUI.UnixTimeCell(cellRect, i.CreatedAt);
                        break;
                    case ColumnIndex.MODEL:
                        DrawModelCell(cellRect, i);
                        break;
                    case ColumnIndex.INPUT:
                        TreeViewGUI.StringCell(cellRect, i.InputPreview, "-");
                        break;
                    case ColumnIndex.OUTPUT:
                        TreeViewGUI.StringCell(cellRect, i.OutputPreview, "-");
                        break;
                    case ColumnIndex.SENDER:
                        TreeViewGUI.StringCell(cellRect, i.Sender, "-");
                        break;
                    case ColumnIndex.PRICE:
                        TreeViewGUI.PriceCell(cellRect, i.Price, AIDevKitEditor.DisplayedCurrencyCode, "-");
                        break;
                }
            }

            private void DrawModelCell(Rect cellRect, PromptHistoryTreeViewItem i)
            {
                if (i.Data == null) return;
                Texture icon = AIDevKitGUIUtility.GetApiIcon(i.Api);
                if (icon == null) return; // icon can't be null so just in case
                string modelName = i.ModelName;
                if (string.IsNullOrEmpty(modelName)) modelName = i.ModelId;

                float iconSize = 16;
                float iconPadding = 2;
                float iconY = cellRect.y + (cellRect.height - iconSize) / 2;
                float iconX = cellRect.x + iconPadding;
                float textX = iconX + iconSize + iconPadding;

                GUI.DrawTexture(new Rect(iconX, iconY, iconSize, iconSize), icon);
                // Calculate the text rectangle
                Rect textRect = new(textX, cellRect.y, cellRect.width - (textX - cellRect.x) - iconPadding, cellRect.height);

                GUI.Label(textRect, modelName);
            }

            internal void CalcTotalPrice()
            {
                TotalPrice = 0;

                foreach (PromptRecord data in SourceData)
                {
                    // if (data == null || data.Price == null || data.Price < 0) continue;
                    // TotalPrice += data.Price;
                    if (data?.Price is { } price && price > 0)
                    {
                        TotalPrice.PriceInUsd = TotalPrice.PriceInUsd + price.PriceInUsd;
                        //Debug.Log($"total price += {price}, now {TotalPrice}");
                    }
                    //AIDevKitDebug.Mark($"total price += {data.Price.ToString()}, now {TotalPrice.ToString()}");
                }
            }

            protected override void OnTreeViewUpdated()
            {
                CalcTotalPrice();
            }
        }
    }
}
