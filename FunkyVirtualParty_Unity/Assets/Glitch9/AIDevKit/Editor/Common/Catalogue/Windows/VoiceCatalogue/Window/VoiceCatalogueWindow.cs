using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class VoiceCatalogueWindow : ExtendedTreeViewWindow
        <
            VoiceCatalogueWindow,
            VoiceCatalogueWindow.VoiceCatalogueTreeView,
            VoiceCatalogueTreeViewItem,
            VoiceCatalogueWindow.VoiceCatalogueDetailsWindow,
            VoiceCatalogueEntry,
            VoiceCatalogueFilter,
            VoiceCatalogueWindow.VoiceCatalogueContextMenuHandler
        >
    {
        internal static class ColumnIndex
        {
            public const int IN_LIB = 0;
            public const int API = 1;
            public const int NAME = 2;
            public const int TYPE = 3;
            public const int GENDER = 4;
            public const int AGE = 5;
            public const int LANGUAGE = 6;
        }

        [MenuItem(AIDevKitEditor.Paths.VoiceCatalogue, priority = AIDevKitEditor.Priorities.VoiceCatalogue)]
        public static void ShowWindow() => InitializeWindow(AIDevKitEditor.Labels.VoiceCatalogue);

        protected override List<TreeViewColumnData> CreateColumns()
        {
            List<TreeViewColumnData> columns = new()
            {
                new TreeViewColumnData
                {
                    Index = ColumnIndex.IN_LIB,
                    HeaderContent = new GUIContent("✓"),
                    FixedWidth = 24f,
                    AutoResize = false,
                },

                new TreeViewColumnData
                {
                    Index = ColumnIndex.API,
                    HeaderContent = new GUIContent("API"),
                    //Width = TreeViewColumnWidth.Tiny,
                    FixedWidth = 24f,
                    AutoResize = false,
                },

                new TreeViewColumnData
                {
                    Index = ColumnIndex.NAME,
                    HeaderContent = new GUIContent("Name"),
                    Width = TreeViewColumnWidth.Wide,
                },

                new TreeViewColumnData
                {
                    Index = ColumnIndex.TYPE,
                    HeaderContent = new GUIContent("Type"),
                    Width = TreeViewColumnWidth.Small,
                },

                new TreeViewColumnData
                {
                    Index = ColumnIndex.GENDER,
                    HeaderContent = new GUIContent("Gender"),
                    Width = TreeViewColumnWidth.Tiny,
                },

                new TreeViewColumnData
                {
                    Index = ColumnIndex.AGE,
                    HeaderContent = new GUIContent("Age"),
                    Width = TreeViewColumnWidth.Small,
                },

                new TreeViewColumnData
                {
                    Index = ColumnIndex.LANGUAGE,
                    HeaderContent = new GUIContent("Language"),
                },
            };

            return columns;
        }
    }
}