using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public class EditorLoadingSpinner
    {
        private readonly EditorWindow _editorWindow;
        private readonly Texture2D _texture;
        private float _angle = 0.0f;

        public EditorLoadingSpinner(EditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
            _texture = EditorTextures.LoadingSpinner;
        }

        public void Draw(Rect rect)
        {
            float size = Mathf.Min(rect.width, rect.height);  // 1:1 비율 유지

            Rect squareRect = new(
                rect.x + (rect.width - size) / 2f,
                rect.y + (rect.height - size) / 2f,
                size,
                size
            );

            Matrix4x4 oldMatrix = GUI.matrix;
            const float kSpeed = 0.5f;

            Vector2 pivot = squareRect.center;

            _angle = (_angle + kSpeed) % 360f;
            GUIUtility.RotateAroundPivot(_angle, pivot);

            GUI.DrawTexture(squareRect, _texture);
            GUI.matrix = oldMatrix;

            _editorWindow.Repaint();
        }

        public void DoLayout(string text, int size = 32)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        const float kYOffset = 50f;
                        GUILayout.Space(kYOffset);
                        EditorGUILayout.LabelField(text, ExStyles.centeredBoldLabel, GUILayout.ExpandWidth(true));
                        Draw(GUILayoutUtility.GetLastRect().SetSize(size, size).MoveY(-kYOffset * 0.7f));
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }
    }
}