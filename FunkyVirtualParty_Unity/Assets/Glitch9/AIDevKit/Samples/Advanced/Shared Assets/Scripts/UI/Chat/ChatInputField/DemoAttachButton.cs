using System;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Glitch9.AIDevKit.Demo
{
    public class DemoAttachButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public UnityEngine.UI.Image image;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        public event Action<File<Texture2D>> onAttach;
        public bool interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                image.color = value ? activeColor : inactiveColor;
            }
        }
        private bool _interactable;

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        private void Click()
        {
            if (interactable)
            {
#if UNITY_EDITOR
                string path = EditorUtility.OpenFilePanel("Select an image", "", "png,jpg,jpeg");
                if (path.Length != 0)
                {
                    Debug.Log("Selected file: " + path);
                    File<Texture2D> image = new(path);
                    onAttach?.Invoke(image);
                }
#else
                Debug.LogError("File selection is only available in the editor");
#endif
            }
        }
    }
}