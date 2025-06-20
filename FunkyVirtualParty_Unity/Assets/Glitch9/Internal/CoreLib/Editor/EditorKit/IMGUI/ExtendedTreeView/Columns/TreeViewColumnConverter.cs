using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Glitch9.Editor.IMGUI
{
    internal class TreeViewColumnConverter
    {
        internal static MultiColumnHeaderState.Column[] Convert(float currentViewWidth, List<TreeViewColumnData> columnDataList)
        {
            if (columnDataList == null) return null;
            columnDataList.Sort((a, b) => a.Index.CompareTo(b.Index));

            int columnCount = columnDataList.Count;
            float[] columnWidths = new float[columnCount];
            float totalFixedWidth = 0;
            int autoResizeCount = 0;

            // Calculate initial widths and track total fixed width and number of auto-resizable columns
            for (int i = 0; i < columnCount; i++)
            {
                TreeViewColumnData data = columnDataList[i];

                if (data.FixedWidth.HasValue)
                {
                    columnWidths[i] = data.FixedWidth.Value;
                }
                else
                {
                    columnWidths[i] = GetFixedWidth(data.Width);
                }

                totalFixedWidth += columnWidths[i];

                if (data.AutoResize)
                {
                    autoResizeCount++;
                }
            }

            // Determine extra width or need for scaling down
            float extraWidth = currentViewWidth - totalFixedWidth;

            if (extraWidth < 0)
            {
                // Scale down all columns proportionally if overflows
                float scale = currentViewWidth / totalFixedWidth;
                for (int i = 0; i < columnCount; i++)
                {
                    columnWidths[i] *= scale;
                }
            }
            else if (autoResizeCount > 0)
            {
                // Distribute remaining width to auto-resize columns
                float extraWidthPerColumn = extraWidth / autoResizeCount;
                for (int i = 0; i < columnCount; i++)
                {
                    if (columnDataList[i].AutoResize)
                    {
                        columnWidths[i] += extraWidthPerColumn;
                    }
                }
            }

            // Create columns
            List<MultiColumnHeaderState.Column> columnList = new();
            for (int i = 0; i < columnCount; i++)
            {
                TreeViewColumnData columnData = columnDataList[i];
                columnList.Add(new MultiColumnHeaderState.Column
                {
                    headerContent = columnData.HeaderContent,
                    headerTextAlignment = columnData.HeaderTextAlignment,
                    width = columnWidths[i],
                    maxWidth = columnData.FixedWidth ?? 10000,
                    minWidth = columnData.FixedWidth ?? 20,
                    autoResize = columnData.AutoResize,
                    canSort = columnData.CanSort,
                    allowToggleVisibility = columnData.AllowToggleVisibility,
                });
            }

            return columnList.ToArray();
        }


        private static float GetFixedWidth(TreeViewColumnWidth width)
        {
            switch (width)
            {
                case TreeViewColumnWidth.Tiny: return 24;
                case TreeViewColumnWidth.Small: return 70F;
                case TreeViewColumnWidth.Medium: return 120F;
                case TreeViewColumnWidth.Wide: return 180F;
                case TreeViewColumnWidth.ExtraWide: return 240F;
                default: return 100; // Default width if undefined
            }
        }
    }
}