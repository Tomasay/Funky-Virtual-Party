using UnityEditor;

namespace Glitch9.Editor
{
    public class EditorDotIndicator
    {
        private const int MAX_DOTS = 3;
        private const double UPDATE_INTERVAL = 0.25;

        private int _dotCount = 0;
        private double _lastUpdateTime = 0;

        private readonly EditorWindow _window;

        public EditorDotIndicator(EditorWindow window)
        {
            _window = window;
        }

        public void Update()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime > UPDATE_INTERVAL)
            {
                _dotCount = (_dotCount + 1) % (MAX_DOTS + 1);
                _lastUpdateTime = EditorApplication.timeSinceStartup;
                _window.Repaint(); // Repaint the window to update the dots
            }
        }

        public string GetDots()
        {
            return new string('.', _dotCount);
        }

        public void Reset()
        {
            _dotCount = 0;
            _lastUpdateTime = 0;
        }
    }
}