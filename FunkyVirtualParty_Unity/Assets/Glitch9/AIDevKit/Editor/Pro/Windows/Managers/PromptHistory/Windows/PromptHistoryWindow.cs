using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    [InitializeOnLoad]
    public partial class PromptHistoryWindow : ExtendedTreeViewWindow
        <
            PromptHistoryWindow,
            PromptHistoryWindow.PromptHistoryTreeView,
            PromptHistoryTreeViewItem,
            PromptHistoryWindow.PromptHistoryTreeViewDetailsWindow,
            PromptRecord,
            PromptHistoryTreeViewItemFilter,
            PromptHistoryWindow.PromptHistoryContextMenuHandler
        >
    {
        static PromptHistoryWindow()
        {
            AIDevKitEditor.onShowPromptHistoryWindow += ShowWindow;
        }

        internal static class ColumnIndex
        {
            public const int DATE = 0;
            public const int MODEL = 1;
            public const int TASK_TYPE = 2;
            public const int INPUT = 3;
            public const int OUTPUT = 4;
            public const int PRICE = 6;
            public const int SENDER = 5;
        }

        private static class PrefsKey
        {
            public const string SELECTED_TYPES = "OpenAIPromptHistory.SelectedTypes";
        }

        public static PromptHistoryWindow Instance { get; set; }
        public static void ShowWindow() => Instance = InitializeWindow(AIDevKitEditor.Labels.PromptHistory);

        protected override List<TreeViewColumnData> CreateColumns()
        {
            List<TreeViewColumnData> columns = new()
            {
                new TreeViewColumnData
                {
                    Index = ColumnIndex.DATE,
                    HeaderContent = new GUIContent("Date"),
                    Width = TreeViewColumnWidth.Medium,
                    AutoResize = true,
                    CanSort = true
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.MODEL,
                    HeaderContent = new GUIContent("Model"),
                    Width = TreeViewColumnWidth.Small,
                    AutoResize = true,
                    CanSort = true
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.TASK_TYPE,
                    HeaderContent = new GUIContent("Request"),
                    Width =  TreeViewColumnWidth.Small,
                    AutoResize = true,
                    CanSort = true
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.INPUT,
                    HeaderContent = new GUIContent("Input"),
                    Width = TreeViewColumnWidth.Wide,
                    AutoResize = true,
                    CanSort = false
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.OUTPUT,
                    HeaderContent = new GUIContent("Output"),
                    Width = TreeViewColumnWidth.Wide,
                    AutoResize = true,
                    CanSort = false
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.SENDER,
                    HeaderContent = new GUIContent("Sender"),
                    Width = TreeViewColumnWidth.Tiny,
                    AutoResize = true,
                    CanSort = true
                },
                new TreeViewColumnData
                {
                    Index = ColumnIndex.PRICE,
                    HeaderContent = new GUIContent("Price"),
                    Width = TreeViewColumnWidth.Tiny,
                    AutoResize = true,
                    CanSort = true
                },
            };

            return columns;
        }

        protected override void BottomBar()
        {
            if (TreeView == null) return;
            GUILayout.BeginHorizontal(TreeViewStyles.BottomBarStyle);
            try
            {
                DrawRefreshButton();

                GUILayout.FlexibleSpace();

                GUILayout.Label($"Total Price: {TreeView.TotalPrice.ToString(AIDevKitEditor.DisplayedCurrencyCode, "0.00")}", ExStyles.labelRight, GUILayout.MinWidth(160), GUILayout.MaxWidth(160));
                CurrencyCode newCode = ExGUILayout.EnumPopupEx(AIDevKitEditor.DisplayedCurrencyCode, AIDevKitGUIUtility.SelectedCurrencyCodes, null, null, GUILayout.Width(100));
                if (newCode != AIDevKitEditor.DisplayedCurrencyCode)
                {
                    TreeView.TotalPrice.CurrencyCode = newCode;
                    AIDevKitEditor.DisplayedCurrencyCode = newCode;
                    TreeView.ReloadTreeView();
                }
                if (ExGUI.ResetButton())
                {
                    TreeView.CalcTotalPrice();
                    Repaint();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }
    }
}