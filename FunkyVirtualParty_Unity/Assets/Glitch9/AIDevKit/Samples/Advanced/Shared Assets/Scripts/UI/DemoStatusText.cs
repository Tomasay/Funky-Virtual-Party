using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public enum DemoStatus
    {
        Loading,
        Success,
        Warning,
        Error
    }

    public class DemoStatusText : MonoBehaviour
    {
        [SerializeField] private Sprite loadingIcon;
        [SerializeField] private Sprite checkIcon;
        [SerializeField] private Sprite warningIcon;
        [SerializeField] private Sprite errorIcon;

        [SerializeField] private Image icon;
        [SerializeField] private Text statusText;

        [SerializeField] private float spinningValue = -2f;

        private Transform IconTransform => _iconTransform ? _iconTransform : (_iconTransform = icon.transform);
        private Transform _iconTransform;

        public void SetStatus(DemoStatus status, string message)
        {
            switch (status)
            {
                case DemoStatus.Loading:
                    icon.sprite = loadingIcon;
                    StartSpinningIcon();
                    break;
                case DemoStatus.Success:
                    icon.sprite = checkIcon;
                    StopSpinningIcon();
                    break;
                case DemoStatus.Warning:
                    icon.sprite = warningIcon;
                    StopSpinningIcon();
                    break;
                case DemoStatus.Error:
                    icon.sprite = errorIcon;
                    StopSpinningIcon();
                    break;
            }

            statusText.text = message;
        }

        private void StartSpinningIcon()
        {
            StartCoroutine(SpinIcon());
        }

        private void StopSpinningIcon()
        {
            StopAllCoroutines();
            IconTransform.rotation = Quaternion.identity;
        }

        private IEnumerator SpinIcon()
        {
            while (true)
            {
                IconTransform.Rotate(0, 0, spinningValue);
                yield return null;
            }
        }

        private void OnDisable()
        {
            StopSpinningIcon();
        }
    }
}