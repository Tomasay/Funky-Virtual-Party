using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    /// <summary>
    /// A reusable vertical resize handle that modifies the height of a target VisualElement.
    /// </summary>
    public class ResizeHandle : VisualElement
    {
        public bool IsResizing { get; private set; } = false;
        private Vector2 startMouse;
        private float originalHeight;
        private readonly float minHeight;
        private readonly VisualElement target;
        private readonly bool updateParentHeight;
        private readonly Action<float> onHeightChanged;

        public ResizeHandle(string name, VisualElement targetElement, float minHeight, bool updateParentHeight = false, Action<float> onHeightChanged = null)
        {
            target = targetElement;

            this.minHeight = minHeight;
            base.name = name;
            this.updateParentHeight = updateParentHeight;
            this.onHeightChanged = onHeightChanged;

            pickingMode = PickingMode.Position;
            style.position = Position.Absolute;
            style.bottom = 2;
            style.right = 2;
            style.width = 16;
            style.height = 16;
            style.alignSelf = Align.FlexEnd;
            style.justifyContent = Justify.FlexEnd;
            style.backgroundColor = Color.clear;
            style.unityBackgroundImageTintColor = Color.white;
            style.backgroundImage = new StyleBackground(EditorTextures.ResizeHandle); // Replace with your resize handle texture

            AddToClassList(name);
            targetElement.Add(this);
            RegisterCallback<MouseDownEvent>(OnPress);
            RegisterCallback<MouseMoveEvent>(OnDrag);
            RegisterCallback<MouseUpEvent>(OnRelease);
        }

        private void OnPress(MouseDownEvent evt)
        {
            IsResizing = true;
            startMouse = evt.mousePosition;
            originalHeight = target.resolvedStyle.height;
            this.CaptureMouse();
        }

        private void OnDrag(MouseMoveEvent evt)
        {
            if (!IsResizing) return;
            float delta = evt.mousePosition.y - startMouse.y;
            float newHeight = Mathf.Max(minHeight, originalHeight + delta);
            parent.style.height = newHeight;
            target.style.height = newHeight;
            //Debug.Log($"Target height (style): {target.style.height}, resolved: {target.resolvedStyle.height}");

            if (updateParentHeight) target.parent.style.height = newHeight;
            onHeightChanged?.Invoke(newHeight);
        }

        private void OnRelease(MouseUpEvent evt)
        {
            IsResizing = false;
            this.ReleaseMouse();
        }
    }

}
