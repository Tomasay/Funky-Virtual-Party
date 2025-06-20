using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class VisualElementHandle<T> where T : VisualElement
    {
        public static implicit operator T(VisualElementHandle<T> handle) => handle.Value;
        private readonly VisualElement _root;
        private readonly string _name;
        private T _element;

        public VisualElementHandle(VisualElement root, string name)
        {
            _root = root;
            _name = name;
        }

        public T Value
        {
            get
            {
                _element ??= _root.Q<T>(_name);
                return _element;
            }
            set
            {
                _element?.RemoveFromHierarchy();
                _element = value;
                if (value != null) _root.Add(_element);
            }
        }

        public void RemoveFromHierarchy() => Value = null;
        public bool IsNull => Value == null;

        public void Add(VisualElement element)
        {
            if (element == null)
            {
                Debug.LogWarning("Attempted to add a null VisualElement.");
                return;
            }

            Value.Add(element);
        }

        public void Remove(VisualElement element)
        {
            if (element == null)
            {
                Debug.LogWarning("Attempted to remove a null VisualElement.");
                return;
            }

            Value.Remove(element);
        }

        public void AddToClassList(string className)
        {
            if (string.IsNullOrEmpty(className))
            {
                Debug.LogWarning("Attempted to add an empty class name.");
                return;
            }

            Value.AddToClassList(className);
        }
    }
}