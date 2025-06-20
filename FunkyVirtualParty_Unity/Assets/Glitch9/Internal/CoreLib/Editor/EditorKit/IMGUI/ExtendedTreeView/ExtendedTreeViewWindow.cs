using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    /// <summary>
    /// Base class for all tree view windows. 
    /// This class is responsible for creating the tree view, handling events, and drawing the UI.
    /// </summary>
    /// <typeparam name="TTreeViewWindow">The type of the tree view window.</typeparam>
    /// <typeparam name="TTreeView">The type of the tree view.</typeparam>
    /// <typeparam name="TTreeViewItem">The type of the tree view item.</typeparam>
    /// <typeparam name="TTreeViewDetailsWindow">The type of the tree view details window. (shows when an item is double clicked)</typeparam>
    /// <typeparam name="TTreeViewData">The type of the data associated with the tree view items.</typeparam>
    /// <typeparam name="TTreeViewItemFilter">The type of the data filter for the tree view. (mainly used for search bar)</typeparam>
    /// <typeparam name="TTreeViewContextMenuHandler">The type of the event handler for the tree view.</typeparam>
    public abstract partial class ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler> : EditorWindow
        where TTreeViewWindow : ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>
        where TTreeView : ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>.ExtendedTreeView
        where TTreeViewItem : ExtendedTreeViewItem<TTreeViewItem, TTreeViewData, TTreeViewItemFilter>
        where TTreeViewDetailsWindow : ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>.ExtendedTreeViewDetailsWindow
        where TTreeViewData : class, IData
        where TTreeViewItemFilter : TreeViewItemFilter, new()
        where TTreeViewContextMenuHandler : ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>.TreeViewContextMenuHandler
    {
        private const int MAX_INIT_COUNT = 3;
        private const float BOTTOM_BAR_BTN_WIDTH = 100;

        protected MultiColumnHeader MultiColumnHeader;
        protected TTreeView TreeView;
        protected TreeViewState TreeViewState;

        /// <summary>
        /// Often accessed from ExtendedTreeView
        /// </summary>
        public TreeViewMenu Menu { get; private set; }

        private bool _isInitialized = false;
        private int _initCount = 0;

        protected static TTreeViewWindow InitializeWindow(string name = null, bool centerWindow = false)
        {
            name ??= typeof(TTreeViewWindow).Name;
            TTreeViewWindow window = (TTreeViewWindow)GetWindow(typeof(TTreeViewWindow), false, name);
            window.Show();
            window.autoRepaintOnSceneChange = true;
            if (centerWindow)
            {
                window.position = new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400);
            }
            return window;
        }

        protected abstract List<TreeViewColumnData> CreateColumns();
        protected abstract IEnumerable<ITreeViewMenuEntry> CreateMenuEntries();
        private TreeViewMenu CreateMenu() => new(CreateMenuEntries(), TreeView.OnSearchTextChanged);
        protected virtual bool IgnoreMenu => false;

        private void Initialize()
        {
            if (_isInitialized || _initCount >= MAX_INIT_COUNT) return;
            _isInitialized = true;
            _initCount++;
            TreeView = CreateTreeView();
            if (!IgnoreMenu) Menu = CreateMenu();
        }

        protected virtual void OnDestroy()
        {
            TreeView.OnDestroy();
        }

        protected virtual void OnGUI()
        {
            try
            {
                Initialize();
                DrawMenu();
                DrawTreeView();
                DrawBottomBar();
            }
            catch (Exception e)
            {
                _isInitialized = false;
                ExGUIPreset.ReloadWindow(e.Message, () => { _initCount = 0; });
            }
        }

        private void DrawMenu()
        {
            if (IgnoreMenu) return;
            Menu ??= CreateMenu();
            Menu.Draw(position);
        }

        private void DrawTreeView()
        {
            TreeView ??= CreateTreeView();
            if (TreeView == null) return;

            Rect reservedRect = GUILayoutUtility.GetRect(600f, 400f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            TreeView.OnGUI(reservedRect);
        }

        private TTreeView CreateTreeView()
        {
            TreeViewState = new TreeViewState();

            if (MultiColumnHeader == null)
            {
                float currentViewWidth = position.width;
                MultiColumnHeaderState.Column[] columns = TreeViewColumnConverter.Convert(currentViewWidth, CreateColumns());
                if (columns == null) return null;

                MultiColumnHeaderState headerState = new(columns);
                MultiColumnHeader = new MultiColumnHeader(headerState);

                if (MultiColumnHeaderState.CanOverwriteSerializedFields(MultiColumnHeader.state, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(MultiColumnHeader.state, headerState);
            }

            return Activator.CreateInstance(typeof(TTreeView), TreeViewState, MultiColumnHeader) as TTreeView;
        }

        protected bool NullCheckItem(TTreeViewItem item)
        {
            if (item == null) return false;
            if (item.Data == null) return false;
            if (string.IsNullOrEmpty(item.Data.Id)) return false;
            return true;
        }

        private void DrawBottomBar()
        {
            if (TreeView == null) return;
            BottomBar();
        }

        protected void DrawRefreshButton()
        {
            if (GUILayout.Button(EditorIcons.Refresh))
            {
                TreeView.ReloadTreeView(true, true);
                Repaint();
            }
            GUILayout.Label($"Showing {TreeView.ShowingCount} of {TreeView.TotalCount} items.");
        }

        /// <summary>
        /// Override this method to add custom bottom bar
        /// </summary>
        protected virtual void BottomBar()
        {
            GUILayout.BeginHorizontal(TreeViewStyles.BottomBarStyle);
            try
            {
                DrawRefreshButton();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reset Filter", GUILayout.Width(BOTTOM_BAR_BTN_WIDTH)))
                {
                    TreeView.ResetFilter();
                }

                if (GUILayout.Button("Reload Data", GUILayout.Width(BOTTOM_BAR_BTN_WIDTH)))
                {
                    TreeView.ReloadTreeView();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }
    }
}