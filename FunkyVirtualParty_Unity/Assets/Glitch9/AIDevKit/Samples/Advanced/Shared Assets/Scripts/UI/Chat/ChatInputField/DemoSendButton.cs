using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoSendButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public UnityEngine.UI.Image image;

        public event Action onSend;
        public bool interactable;

        private void Update()
        {
            // check for enter key press
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Click();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        private void Click()
        {
            if (interactable)
            {
                onSend?.Invoke();
            }
        }
    }
}