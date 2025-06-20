using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        private const float HEIGHT = 34f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute labelAtt = attribute as LabelAttribute;
            if (labelAtt == null) return;

            Rect titleRect = position;
            titleRect.height = HEIGHT;
            // FieldGroup 이름으로 섹션을 시작합니다.
            DrawLabel(titleRect, labelAtt.Name);

            position.y += HEIGHT;

            // 실제 필드를 그립니다.
            EditorGUI.PropertyField(position, property, label, true);
        }

        private void DrawLabel(Rect rect, string label)
        {
            GUIStyle style = new(EditorStyles.boldLabel)
            {
                fontSize = 14,
                margin = new RectOffset(0, 0, 10, 10)
            };

            rect.y = rect.y;
            EditorGUI.LabelField(rect, label, style);
            float thickness = 1.2f;
            float height = 5f;
            Rect r = new(rect.x, rect.y + rect.height - 5, rect.width, height)
            {
                height = thickness
            };
            r.width -= 4;
            EditorGUI.DrawRect(r, Color.gray);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 기본 필드 높이에 섹션 라벨 높이를 추가합니다.
            return base.GetPropertyHeight(property, label) + HEIGHT;
        }
    }
}
