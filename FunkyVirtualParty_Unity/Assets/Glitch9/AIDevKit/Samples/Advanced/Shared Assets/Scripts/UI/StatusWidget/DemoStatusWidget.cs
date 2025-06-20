using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoStatusWidget : MonoBehaviour
    {
        private DemoStatusWidgetDot _dot;
        private DemoStatusWidgetText _text;

        private void Awake()
        {
            _dot = GetComponentInChildren<DemoStatusWidgetDot>();
            _text = GetComponentInChildren<DemoStatusWidgetText>();
        }

        public void SetStatus(DemoStatusType statusType, string text)
        {
            _dot.SetStatus(statusType);
            _text.SetText(text);
        }
    }
}