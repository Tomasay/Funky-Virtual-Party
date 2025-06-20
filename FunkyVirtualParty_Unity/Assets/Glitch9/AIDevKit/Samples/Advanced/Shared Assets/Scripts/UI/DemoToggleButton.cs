using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoToggleButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;
        public event Action<bool> onValueChanged;
        private Image _image;
        private bool _isOn;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData) => Toggle();

        public void Toggle()
        {
            _isOn = !_isOn;
            _image.color = _isOn ? onColor : offColor;
            onValueChanged?.Invoke(_isOn);
        }

        public void Select(bool triggerEvent = false)
        {
            _isOn = true;
            _image.color = onColor;

            if (triggerEvent)
            {
                onValueChanged?.Invoke(_isOn);
            }
        }

        public void Deselect(bool triggerEvent = false)
        {
            _isOn = false;
            _image.color = offColor;

            if (triggerEvent)
            {
                onValueChanged?.Invoke(_isOn);
            }
        }
    }
}