using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using TreeView = UnityEditor.IMGUI.Controls.TreeView;

namespace Glitch9.Editor.IMGUI
{
    public abstract partial class ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>
    {
        public abstract class ExtendedTreeView : TreeView
        {
            #region Fields

            public TTreeViewItemFilter Filter { get; protected set; }
            public List<TTreeViewData> SourceData { get; protected set; }
            public bool RequiresRefresh { get; set; }
            public int ShowingCount => _rows?.Count ?? 0;
            public int TotalCount => SourceData?.Count ?? 0;

            private readonly EPrefs<TTreeViewItemFilter> _filterSave;
            private readonly TTreeViewContextMenuHandler _contextMenuHandler;

            private List<TreeViewItem> _cachedItems;
            private List<TreeViewItem> _rows;
            private TTreeViewDetailsWindow _editWindowInstance;

            #endregion

            #region Constructor

            protected ExtendedTreeView(TreeViewState treeViewState, MultiColumnHeader multiColumnHeader) : base(treeViewState, multiColumnHeader)
            {
                try
                {
                    string filterPrefsKey = $"{GetType().Name}.Filter";
                    _filterSave = new EPrefs<TTreeViewItemFilter>(filterPrefsKey, Activator.CreateInstance<TTreeViewItemFilter>());
                    Filter = _filterSave.Value;

                    _contextMenuHandler = CreateEventHandler();
                    SourceData = GetSourceData().ToList();

                    //if (SourceData.IsNullOrEmpty()) Debug.LogWarning("No data found for tree view.");

                    ReloadTreeView(true);
                    multiColumnHeader.sortingChanged += OnSortingChanged;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            #endregion

            #region EventHandler Setup

            private TTreeViewContextMenuHandler CreateEventHandler()
            {
                // TTreeViewContextMenuHandler eventHandler = Activator.CreateInstance<TTreeViewContextMenuHandler>();
                // 변경 사항으로 parameter에 this를 넣어야한다  
                return (TTreeViewContextMenuHandler)Activator.CreateInstance(typeof(TTreeViewContextMenuHandler), this);
            }

            #endregion

            #region TreeView Core Overrides

            protected override TreeViewItem BuildRoot()
            {
                TreeViewItem root = new() { id = 0, depth = -1, displayName = "Root" };

                if (_cachedItems != null)
                {
                    SetupParentsAndChildrenFromDepths(root, _cachedItems);
                    return root;
                }

                ReloadTreeView();
                if (_cachedItems == null) return root;

                SetupParentsAndChildrenFromDepths(root, _cachedItems);
                return root;
            }

            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                if (root.children == null) return new List<TreeViewItem>();
                if (_rows != null && !RequiresRefresh) return _rows;

                _rows = new();
                foreach (TreeViewItem treeViewItem in _cachedItems)
                {
                    if (Filter != null && !Filter.IsVisible(treeViewItem)) continue;
                    _rows.Add(treeViewItem);
                }

                if (multiColumnHeader.sortedColumnIndex != -1)
                {
                    int columnIndex = multiColumnHeader.sortedColumnIndex;
                    bool ascending = multiColumnHeader.IsSortedAscending(columnIndex);
                    _rows.Sort((x, y) => (x as TTreeViewItem)?.CompareTo(y as TTreeViewItem, columnIndex, ascending) ?? 0);
                }

                RequiresRefresh = false;
                OnTreeViewUpdated();
                return _rows;
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    CellGUI(args.GetCellRect(i), args.item, i, ref args);
                }
            }

            protected override void ContextClickedItem(int id)
            {
                IList<int> selection = GetSelection();
                List<TTreeViewItem> items = selection.Select(selectedId => FindItem(selectedId, rootItem) as TTreeViewItem).Where(x => x != null).ToList();
                OnRightClickedItem(items);
            }

            protected override void DoubleClickedItem(int id)
            {
                base.DoubleClickedItem(id);

                if (FindItem(id, rootItem) is TTreeViewItem item)
                    OnDoubleClickedItem(item);
            }

            public void OnSortingChanged(MultiColumnHeader header)
            {
                //Debug.Log("Sorting changed: " + header.sortedColumnIndex);
                var index = header.sortedColumnIndex;
                var ascending = header.IsSortedAscending(index);

                SortItems(index, ascending);
                ReloadTreeView();
            }

            public void OnSearchTextChanged(string searchText)
            {
                Filter.SearchText = searchText;
                ReloadTreeView();
            }

            public virtual void OnDestroy() { if (_filterSave != null) _filterSave.Value = Filter; }

            #endregion

            #region TreeView Logic Hooks

            /// <summary>
            /// Override this method to draw the cell GUI for each column in the tree view.
            /// </summary>
            /// <param name="cellRect"></param>
            /// <param name="item"></param>
            /// <param name="columnIndex"></param>
            /// <param name="args"></param>
            protected abstract void CellGUI(Rect cellRect, TreeViewItem item, int columnIndex, ref RowGUIArgs args);

            /// <summary>
            /// Override this method to handle right-clicking on an item in the tree view.
            /// </summary>
            /// <param name="items"></param>
            protected virtual void OnRightClickedItem(IList<TTreeViewItem> items) => _contextMenuHandler?.ShowRightClickMenu(items, OnSourceDataUpdated);

            /// <summary>
            /// Override this method to handle double clicking on a tree view item.
            /// </summary>
            /// <param name="item"></param>
            protected virtual void OnDoubleClickedItem(TTreeViewItem item) => ShowDetailsWindow(item);

            protected virtual void OnTreeViewUpdated() { }

            protected virtual void SortItems(int columnIndex, bool ascending)
            {
                if (_cachedItems == null) return;
                _cachedItems.Sort(Comparison);

                int Comparison(TreeViewItem x, TreeViewItem y)
                {
                    if (x is not TTreeViewItem itemX || y is not TTreeViewItem itemY)
                    {
                        Debug.LogWarning("Comparison failed: " + x + " - " + y);
                        return 0;
                    }
                    return itemX.CompareTo(itemY, columnIndex, ascending);
                }
            }

            public void ResetFilter() => Filter = null;

            public void ShowDetailsWindow(TTreeViewItem item)
            {
                if (item == null || item.Data == null) return;
                string windowTitle = item.Data.Id == null ? "Details" : $"Details: {item.Data.Id}";

                _editWindowInstance?.Close();

                try
                {
                    _editWindowInstance = GetWindow<TTreeViewDetailsWindow>(false, windowTitle, true);
                    _editWindowInstance.minSize = new Vector2(400, 400);
                    _editWindowInstance.maxSize = new Vector2(800, 800);
                    _editWindowInstance.SetData(item, this as TTreeView, _contextMenuHandler);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error creating window: {e.Message}");
                }
            }

            #endregion

            #region Data Source Management

            /// <summary>
            /// Override this method to get data from a source.
            /// </summary>
            /// <returns></returns>
            protected abstract IEnumerable<TTreeViewData> GetSourceData();

            protected virtual void RemoveSourceData(TTreeViewData data) => throw new NotImplementedException();

            protected virtual void OnSourceDataUpdated(bool success) { if (success) ReloadTreeView(); }

            public void ReloadTreeView(bool filterUpdated = false, bool reloadSourceData = false)
            {
                if (reloadSourceData) SourceData = GetSourceData().ToList();

                if (filterUpdated)
                {
                    if (UpdateFilter())
                    {
                        RequiresRefresh = true;
                        Reload();
                        Repaint();
                    }

                    return;
                }

                RequiresRefresh = true;
                Reload();
                Repaint();

                bool UpdateFilter()
                {
                    if (SourceData == null) return false;

                    try
                    {
                        _cachedItems = SourceData.Select((d, i) => CreateItem(i + 1000, 0, d.Name ?? $"NoName ({i + 1000})", d)).Cast<TreeViewItem>().ToList();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error updating cached items: {e.Message}");
                        return false;
                    }
                }
            }

            public void ReloadDetailsWindow(TTreeViewItem item)
            {
                if (_editWindowInstance == null)
                {
                    Debug.LogWarning("Edit window instance is null, cannot repaint.");
                    return;
                }

                _editWindowInstance.ReloadData(item);
            }

            private TTreeViewItem CreateItem(int id, int depth, string name, TTreeViewData data)
            {
                try
                {
                    return (TTreeViewItem)Activator.CreateInstance(typeof(TTreeViewItem), id, depth, name, data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"CreateInstance failed: {e.Message}");
                    throw;
                }
            }

            #endregion

            #region Source Data Updating

            public void SetData(List<TTreeViewData> data)
            {
                if (data == null) return;
                SourceData = data;
                ReloadTreeView(true);
            }

            public void AddData(TTreeViewData data)
            {
                if (data == null || string.IsNullOrEmpty(data.Id)) return;

                SourceData ??= new List<TTreeViewData>();
                SourceData.Add(data);

                TTreeViewItem item = CreateItem(SourceData.Count + 1000, 0, data.Name ?? "NoName", data);
                _cachedItems.Add(item);

                ReloadTreeView(true, true);
            }

            public void UpdateData(TTreeViewData data)
            {
                if (data == null || string.IsNullOrEmpty(data.Id)) return;

                TTreeViewData existing = SourceData.FirstOrDefault(d => d.Id == data.Id);
                if (existing != null)
                {
                    int index = SourceData.IndexOf(existing);
                    SourceData[index] = data;
                }

                ReloadTreeView(true, true);
            }

            public void AddItem(params TTreeViewItem[] items)
            {
                if (items == null || items.Length == 0) return;

                foreach (var item in items)
                {
                    _cachedItems.Add(item);
                    if (item.Data != null)
                        SourceData.Add(item.Data);
                }

                ReloadTreeView(true, true);
            }

            public void UpdateItem(params TTreeViewItem[] items)
            {
                if (items == null || items.Length == 0) return;

                foreach (var item in items)
                {
                    TreeViewItem foundItem = _cachedItems.Find(x => x.id == item.id);
                    if (foundItem != null)
                    {
                        _cachedItems.Remove(foundItem);
                        _cachedItems.Add(item);
                    }

                    if (item.Data != null && !string.IsNullOrEmpty(item.Data.Id))
                    {
                        TTreeViewData existingData = SourceData.FirstOrDefault(d => d.Id == item.Data.Id);
                        if (existingData != null)
                        {
                            int index = SourceData.IndexOf(existingData);
                            SourceData[index] = item.Data;
                        }
                    }
                }

                ReloadTreeView();
            }

            public void RemoveItem(params TTreeViewItem[] items)
            {
                if (items == null || items.Length == 0) return;

                foreach (var item in items)
                {
                    TreeViewItem foundItem = _cachedItems.Find(x => x.id == item.id);
                    if (foundItem != null)
                        _cachedItems.Remove(foundItem);

                    if (item.Data != null)
                    {
                        SourceData.Remove(item.Data);
                        RemoveSourceData(item.Data);
                    }
                }

                ReloadTreeView(true, true);
            }

            public void RemoveItems(IList<TTreeViewItem> items)
            {
                if (items == null) return;

                foreach (var item in items)
                {
                    RemoveItem(item);
                }
            }

            private void RevertChanges(params TTreeViewItem[] items)
            {
                if (_editWindowInstance == null) return;
                _editWindowInstance.RevertChanges();
            }

            private bool RevertCondition(params TTreeViewItem[] items)
            {
                return _editWindowInstance != null && _editWindowInstance.IsDirty;
            }

            #endregion
        }
    }
}
