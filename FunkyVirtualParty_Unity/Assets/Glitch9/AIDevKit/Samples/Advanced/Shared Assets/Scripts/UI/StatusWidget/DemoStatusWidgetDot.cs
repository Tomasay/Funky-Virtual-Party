using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoStatusWidgetDot : MonoBehaviour
    {
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetStatus(DemoStatusType statusType)
        {
            try
            {
                _image.color = DemoStatusWidgetConfig.GetColor(statusType);
            }
            catch
            {
                // do nothing
            }
        }
    }
}