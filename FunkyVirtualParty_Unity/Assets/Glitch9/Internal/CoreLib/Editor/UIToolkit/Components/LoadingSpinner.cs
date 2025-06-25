using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class LoadingSpinner : VisualElement
    {
        private readonly IVisualElementScheduledItem _rotationSchedule;
        private int _frame;

        public LoadingSpinner(float size = 16f)
        {
            style.width = size;
            style.height = size;
            style.backgroundImage = new StyleBackground((Texture2D)EditorGUIUtility.IconContent("WaitSpin00").image);
            /*
            style.backgroundSize = new BackgroundSize(Length.Auto(), Length.Auto());
            style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Center));
            style.backgroundPositionY = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Center));
            */

            _rotationSchedule = schedule.Execute(UpdateFrame).Every(100);

            //Debug.Log($"LoadingSpinner created with size {size}px");
        }

        private void UpdateFrame()
        {
            _frame = (_frame + 1) % 12;
            //Debug.Log($"LoadingSpinner frame updated to {_frame}");
            style.backgroundImage = new StyleBackground((Texture2D)EditorGUIUtility.IconContent($"WaitSpin{_frame:00}").image);
        }

        public void Start() => _rotationSchedule.Resume();
        public void Stop() => _rotationSchedule.Pause();
    }
}