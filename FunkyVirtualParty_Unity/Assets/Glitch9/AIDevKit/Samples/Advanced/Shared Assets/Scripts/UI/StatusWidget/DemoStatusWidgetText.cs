using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoStatusWidgetText : MonoBehaviour
    {
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        public void SetText(string text, Color? color = null)
        {
            try
            {
                _text.text = text;
                if (color != null)
                    _text.color = color.Value;
            }
            catch
            {
                // do nothing
            }

        }
    }
}