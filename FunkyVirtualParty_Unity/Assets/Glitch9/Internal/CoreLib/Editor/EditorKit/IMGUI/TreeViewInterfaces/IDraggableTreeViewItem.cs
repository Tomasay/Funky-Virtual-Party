namespace Glitch9.Editor.IMGUI
{
    public interface IDraggableTreeViewItem
    {
        int Index { get; set; }
        bool IsDraggable { get; }
    }

}