using System.Collections;
using TMPro;
using UnityEngine;

namespace Glitch9.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LogMessageFader : MonoBehaviour
    {
        [SerializeField] private float _timeToFade = 1f;
        [SerializeField] private Color _logColor;
        
        private TextMeshProUGUI _text;
        private float _timer = 0f;
        private float _delay = 2f;
        private bool _playing = false;

        public void SetLog(string text)
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
            _text.color = _logColor;
            _text.text = text;
            _timer = 0f;
            _delay = 2f;

            if (!_playing)
            {
                StartCoroutine(FadeText());
            }
        }

        private IEnumerator FadeText()
        {
            _playing = true;

            while (_delay > 0)
            {
                _delay -= Time.deltaTime;
                yield return null;
            }

            while (_timeToFade > _timer)
            {
                _timer += Time.deltaTime;
                yield return null;
            }
            _playing = false;

            transform.parent.gameObject.SetActive(false);
        }
    }
}