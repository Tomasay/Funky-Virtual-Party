using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoButton : MonoBehaviour, IPointerClickHandler
    {
        public event Action onClick;
        public void OnPointerClick(PointerEventData eventData) => onClick?.Invoke();
    }
}