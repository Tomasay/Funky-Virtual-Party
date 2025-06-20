using UnityEngine;
using UnityEditor;

namespace Glitch9.Editor
{
    public class AnimatedToggle
    {
        private bool value;
        private float animationProgress = 0f;
        private readonly float animationSpeed = 1f;

        private Texture2D OnTexture => EditorTextures.AnimatedToggleOn;
        private Texture2D OffTexture => EditorTextures.AnimatedToggleOff;
        private Texture2D HandleTexture => EditorTextures.AnimatedToggleHandle;

        public AnimatedToggle(bool initialValue = false)
        {
            value = initialValue;
            animationProgress = initialValue ? 1f : 0f;
        }

        public bool DrawGUI(GUIContent label, GUIStyle labelStyle, params GUILayoutOption[] options)
        {
            // draw prefix label and toggle button
            GUILayout.BeginHorizontal();
            try
            {
                labelStyle ??= EditorStyles.label;
                GUILayout.Label(label, labelStyle);
                GUILayout.FlexibleSpace();
                value = DrawToggle(GUILayoutUtility.GetRect(28f, 20f, options));
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
            return value;
        }

        public bool DrawGUILeft(GUIContent label, GUIStyle labelStyle, params GUILayoutOption[] options)
        {
            // draw toggle button and suffix label
            GUILayout.BeginHorizontal();
            try
            {
                labelStyle ??= EditorStyles.label;
                value = DrawToggle(GUILayoutUtility.GetRect(28f, 20f, GUILayout.Width(28f)));
                GUILayout.Label(label, labelStyle, options);
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
            return value;
        }

        private bool DrawToggle(Rect rect)
        {
            float target = value ? 1f : 0f;
            animationProgress = Mathf.Lerp(animationProgress, target, Time.deltaTime * animationSpeed);

            // 마우스 클릭 처리
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                Event.current.Use();
            }

            rect.width = 28f;
            rect.height = 20f;

            // === 반투명 처리 ===
            Color originalColor = GUI.color;
            bool isDisabled = !GUI.enabled; // 현재 비활성 상태인지 체크
            GUI.color = isDisabled ? new Color(1f, 1f, 1f, 0.4f) : originalColor;

            // 배경 텍스처
            GUI.DrawTexture(rect, value ? OffTexture : OnTexture, ScaleMode.ScaleToFit);

            // 핸들 위치 계산 및 렌더링
            float handleSize = rect.height * 0.8f;
            float padding = (rect.height - handleSize) / 2f;
            float handleX = Mathf.Lerp(rect.x + padding, rect.xMax - handleSize - padding, animationProgress);
            Rect handleRect = new Rect(handleX, rect.y + padding, handleSize, handleSize);

            GUI.DrawTexture(handleRect, HandleTexture, ScaleMode.ScaleToFit);

            GUI.color = originalColor; // GUI 색상 복원

            // 애니메이션 중이면 강제 Repaint
            if (!Mathf.Approximately(animationProgress, target) && EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.Repaint();
            }

            return value;
        }
    }
}
