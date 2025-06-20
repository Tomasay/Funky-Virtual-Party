using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class TypingIndicator : Label
    {
        private int _dotCount = 0;

        public TypingIndicator()
        {
            pickingMode = PickingMode.Ignore;
            style.whiteSpace = WhiteSpace.Normal;
            schedule.Execute(UpdateDots).Every(500); // 0.5초마다 점 추가
        }

        private void UpdateDots()
        {
            _dotCount = (_dotCount + 1) % 4;
            text = new string('.', _dotCount == 0 ? 1 : _dotCount); // 최소 1개 점 유지
        }
    }
}