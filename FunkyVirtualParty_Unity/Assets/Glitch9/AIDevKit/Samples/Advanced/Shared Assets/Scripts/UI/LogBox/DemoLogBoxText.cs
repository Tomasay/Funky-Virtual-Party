using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoLogBoxText : MonoBehaviour
    {
        private Text _text;
        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        public void SetText(string text, Color? color = null)
        {
            _text.text = text.Trim();
            if (color != null) _text.color = color.Value;
        }

        public void AddText(string text, Color? color = null)
        {
            _text.text += text;
            if (color != null) _text.color = color.Value;
        }
    }
}