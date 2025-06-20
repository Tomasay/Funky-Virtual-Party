using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class ChatSelectionBinder
    {
        private EditorChatWindow _window;
        private UnityObjectProfile _selectedObject;
        internal UnityObjectProfile SelectedObject => _selectedObject;
        private UnityEngine.Object _lastSelectedObject;

        internal ChatSelectionBinder(EditorChatWindow window)
        {
            _window = window;
            _lastSelectedObject = Selection.activeObject;
            _selectedObject = _lastSelectedObject != null ? new UnityObjectProfile(_lastSelectedObject) : null;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            var newSelected = Selection.activeObject;

            if (newSelected != _lastSelectedObject)
            {
                _lastSelectedObject = newSelected;
                _selectedObject = newSelected != null ? new UnityObjectProfile(newSelected) : null;
                _window.RebuildSelectionContainer();
            }
        }

        internal void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        internal void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            _window = null;
            _selectedObject = null;
            _lastSelectedObject = null;
        }

        internal void BuildUI(VisualElement container, bool attachmentsExists)
        {
            if (container == null)
            {
                if (EditorChatSettings.DebugMode)
                    Debug.LogError("EditorChatSelectionManager: BuildUI called with null container.");
                return;
            }

            VisualElement itemsColumn = container.Q<VisualElement>("selection-column-items");
            //VisualElement actionsColumn = container.Q<VisualElement>("selection-column-actions");  

            if (itemsColumn == null)
            {
                if (EditorChatSettings.DebugMode)
                    Debug.LogWarning("EditorChatSelectionManager: itemsColumn not found in container.");
                return;
            }

            itemsColumn.Clear(); // Clear existing items

            if (!attachmentsExists && SelectedObject?.IsNull == false)
            {
                var contextItem = CreateContextItem(itemsColumn);
                var itemElement = new ChatContextElement(contextItem);

                itemsColumn.Add(itemElement);
            }
        }

        private ChatContextItem CreateContextItem(VisualElement itemsColumn)
        {
            return new ChatContextItem(
                id: SelectedObject.InstanceID.ToString(),
                name: SelectedObject.Name,
                icon: EditorGUIUtility.ObjectContent(_lastSelectedObject, SelectedObject.GetType()).image as Texture2D,
                remove: (id) => itemsColumn.Clear()
            );
        }
    }
}