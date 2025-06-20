using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public abstract partial class ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>
    {
        /// <summary>
        /// Base class for child windows of the TreeViewWindow
        /// </summary>
        public abstract class ExtendedTreeViewDetailsWindow : PaddedEditorWindow
        {
            private static class GUIContents
            {
                internal static readonly GUIContent kToolMenu = new(EditorIcons.Menu, "Open the tool menu");
            }

            protected const string kFallbackTitle = "Unknown Item";
            protected const float MIN_TEXT_FIELD_HEIGHT = 20;
            public TTreeViewItem Item { get; set; }
            public TTreeView TreeView { get; set; }
            public TTreeViewContextMenuHandler EventHandler { get; set; }

            public TTreeViewData Data => Item?.Data ?? _data;
            public TTreeViewData EditingData { get; set; }
            public bool IsDirty => !Data.Equals(EditingData);
            public GUIContent Title { get; set; }

            private bool _isInitialized = false;
            private Vector2 _scrollPosition;
            private TTreeViewData _data;

            public void SetData(TTreeViewItem item, TTreeView treeView, TTreeViewContextMenuHandler eventHandler)
            {
                Item = item;
                TreeView = treeView;
                EventHandler = eventHandler;
                Initialize();
            }

            public void SetData(TTreeViewData data)
            {
                _data = data;
                Initialize();
            }

            protected virtual void Initialize()
            {
                if (_isInitialized) return;
                _isInitialized = true;
                Title = CreateTitle();
                EditingData = Data;
            }

            protected virtual GUIContent CreateTitle() => new(!string.IsNullOrEmpty(Data.Name) ? Data.Name : kFallbackTitle);
            protected abstract void DrawSubtitle();
            protected abstract void DrawBody();
            protected virtual float CalcTitleHeight() => TreeViewStyles.DetailsWindowTitle.CalcHeight(Title, position.width);

            internal void ReloadData(TTreeViewItem item)
            {
                // return if id is not the same
                if (Item != null && Item.displayName != item.displayName) return;
                Debug.Log($"Reloading data for item: {item.displayName}");

                Item = item;
                _data = item.Data;
                EditingData = Data;
                Title = CreateTitle();
                Repaint();
            }

            protected override void DrawGUI()
            {
                Initialize();

                if (Data == null)
                {
                    EditorGUILayout.LabelField("Data is null");
                    return;
                }

                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                {
                    GUILayout.Label(Title, TreeViewStyles.DetailsWindowTitle, GUILayout.Height(CalcTitleHeight()), GUILayout.MaxWidth(position.width));
                    GUILayout.Space(5);

                    DrawSubtitle();

                    GUILayout.BeginVertical(TreeViewStyles.EditWindowBody);
                    {
                        DrawBody();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    DrawContextMenuButtons();
                }
                GUILayout.EndScrollView();
            }

            public void RevertChanges()
            {
                EditingData = Data;
            }

            private void RepaintWindow(bool success)
            {
                if (success)
                {
                    Repaint();
                    TreeView.UpdateData(Item.Data);
                }
            }

            private void DrawContextMenuButtons()
            {
                if (EventHandler == null || EventHandler.ContextMenus.IsNullOrEmpty()) return;

                foreach (var contextMenu in EventHandler.ContextMenus)
                {
                    if (contextMenu is TreeViewContextMenuHandler.TreeViewAction action)
                    {
                        if (action.IsEmpty) continue;

                        if (action.ShowInDetailsWindow && action.IsEnabled(Item))
                        {
                            if (TreeViewGUI.ContextButton(
                                action.ButtonLabel ?? action.Name,
                                action.ButtonIcon,
                                action.Type))
                            {
                                action.Execute(RepaintWindow, Item);
                            }
                        }
                    }
                }
            }
        }
    }
}