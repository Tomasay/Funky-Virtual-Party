using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Glitch9.Editor.IMGUI
{
    public abstract class ReorderableTreeView<TItem> : TreeView where TItem : TreeViewItem, IDraggableTreeViewItem
    {
        protected TItem draggingItem;

        protected ReorderableTreeView(TreeViewState state, MultiColumnHeader header)
            : base(state, header) { }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            var item = FindItem(args.draggedItemIDs[0], rootItem) as TItem;
            return item != null && item.IsDraggable;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();

            IList<int> draggedIds = args.draggedItemIDs;
            if (draggedIds.Count != 1) return;

            TItem item = FindItem(draggedIds[0], rootItem) as TItem;
            if (item == null || item.Index < 0) return;

            draggingItem = item;
            DragAndDrop.SetGenericData("TreeViewItemDrag", draggedIds);
            DragAndDrop.StartDrag("Dragging TreeView Item");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems)
            {
                if (args.performDrop)
                {
                    int targetIndex = args.insertAtIndex - 1;
                    if (DragAndDrop.GetGenericData("TreeViewItemDrag") is List<int> draggedIds)
                    {
                        MoveItem(draggingItem, targetIndex);
                        OnItemMoved();
                    }
                    return DragAndDropVisualMode.Move;
                }
                return DragAndDropVisualMode.Move;
            }

            return DragAndDropVisualMode.None;
        }

        protected virtual void MoveItem(TItem item, int newIndex)
        {
            if (item == null || newIndex < 0) return;

            int oldIndex = item.Index;
            if (oldIndex == newIndex) return;
            if (oldIndex > newIndex) newIndex--;

            item.Index = newIndex;
            draggingItem = null;
        }

        public virtual void RebuildItems()
        {
            //UpdateCachedItems();
            Reload();
        }

        private void OnItemMoved()
        {
            UpdateAllIndexes();
            RebuildItems();
        }

        private void UpdateAllIndexes()
        {
            for (int i = 0; i < rootItem.children.Count; i++)
            {
                if (rootItem.children[i] is TItem item)
                {
                    item.Index = i;
                }
            }
        }
    }
}