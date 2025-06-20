using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoLogBox : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private DemoLogBoxText logboxTextPrefab;
        private Transform _container;
        private DemoLogBoxText _currentText;

        private void Awake()
        {
            if (_container == null)
            {
                _container = transform;
            }
        }

        // param text is the completed text
        public void OnText(string text) => SetText(text);
        public void OnText(string text, Color color) => SetText(text, color);

        // param text is the part of the text that is currently being typed or streamed
        public void OnTextDelta(string text) => AddText(text);
        public void OnTextDelta(string text, Color color) => AddText(text, color);

        // param text is the completed text of the text delta
        public void OnTextDone(string text) => FinishText(text);
        public void OnTextDone(string text, Color color) => FinishText(text, color);

        private void SetText(string text, Color? color = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            DemoLogBoxText textObj = GetCurrentText();
            textObj.SetText(text, color);
            _currentText = null;
            StartCoroutine(ScrollDownToBottomRoutine()); // Scroll to bottom
        }

        private IEnumerator ScrollDownToBottomRoutine()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void AddText(string text, Color? color = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            DemoLogBoxText textObj = GetCurrentText();
            textObj.AddText(text, color);
        }

        private void FinishText(string text, Color? color = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (_currentText == null) return;
            _currentText.SetText(text, color);
            _currentText = null;
            StartCoroutine(ScrollDownToBottomRoutine()); // Scroll to bottom
        }

        private DemoLogBoxText GetCurrentText()
        {
            if (_currentText == null) _currentText = Instantiate(logboxTextPrefab, _container);
            return _currentText;
        }
    }
}