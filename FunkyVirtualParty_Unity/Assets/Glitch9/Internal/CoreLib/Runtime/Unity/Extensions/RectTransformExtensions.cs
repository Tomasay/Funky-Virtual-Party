using UnityEngine;
using UnityEngine.UI;

namespace Glitch9
{
    /// <summary>
    /// RectTransform의 변수들과 함수들이 존나 헷갈려서 만든 RawName
    /// </summary>
    internal static class RectTransformExtensions
    {
        internal static void SetActive(this RectTransform rect, bool active)
        {
            if (rect == null) return;
            rect.gameObject.SetActive(active);
        }

        internal static void SetPositionToTop(this RectTransform rect, float top)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x, -top);
        }

        internal static void SetPositionToBottom(this RectTransform rect, float bottom)
        {
            rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
        }

        internal static void SetPositionToLeft(this RectTransform rect, float left)
        {
            rect.offsetMin = new Vector2(left, rect.offsetMin.y);
        }

        internal static void SetPositionToRight(this RectTransform rect, float right)
        {
            rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
        }

        internal static void SetPosY(this RectTransform rect, float y)
        {
            rect.localPosition = new Vector3(rect.localPosition.x, y, rect.localPosition.z);
        }

        internal static void SetPosX(this RectTransform rect, float x)
        {
            rect.localPosition = new Vector3(x, rect.localPosition.y, rect.localPosition.z);
        }

        internal static void SetPosZ(this RectTransform rect, float z)
        {
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, z);
        }

        internal static Transform SetParentAndReset(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null) rectTransform.Expand();
            return transform;
        }

        internal static void ResetAll(this Transform transform)
        {
            transform.position = new Vector3(0, 0, 0);
            transform.localPosition = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(1, 1, 1);
        }

        internal static void ResetPivotToDefault(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        internal static void ResetPositionToZero(this RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
        }

        internal static void Expand(this RectTransform rectTransform, RectOffset padding = null)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = padding == null ? Vector2.zero : new Vector2(padding.left, padding.bottom);
            rectTransform.offsetMax = padding == null ? Vector2.zero : new Vector2(-padding.right, -padding.top);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        internal static void ExpandTop(this RectTransform rectTransform, RectOffset padding = null)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = padding == null ? Vector2.zero : new Vector2(padding.left, -padding.top);
            rectTransform.offsetMax = padding == null ? Vector2.zero : new Vector2(-padding.right, 0);
            rectTransform.pivot = new Vector2(0.5f, 1);
        }

        internal static void ExpandBottom(this RectTransform rectTransform, RectOffset padding = null)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.offsetMin = new Vector2(padding == null ? 0 : padding.left, 0);
            rectTransform.offsetMax = new Vector2(padding == null ? 0 : -padding.right, padding == null ? 0 : padding.bottom);
            rectTransform.pivot = new Vector2(0.5f, 0);
        }

        internal static void ExpandLeft(this RectTransform rectTransform, RectOffset padding = null)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.offsetMin = new Vector2(0, padding == null ? 0 : padding.bottom);
            rectTransform.offsetMax = new Vector2(padding == null ? 0 : padding.left, padding == null ? 0 : -padding.top);
            rectTransform.pivot = new Vector2(0, 0.5f);
        }

        internal static void ExpandRight(this RectTransform rectTransform, RectOffset padding = null)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(padding == null ? 0 : -padding.right, padding == null ? 0 : padding.bottom);
            rectTransform.offsetMax = new Vector2(0, padding == null ? 0 : -padding.top);
            rectTransform.pivot = new Vector2(1, 0.5f);
        }

        internal static void ExpandHorizontally(this RectTransform rectTransform, float height = 0)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(0, height);
            rectTransform.offsetMax = new Vector2(0, -height);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        internal static void ExpandVertically(this RectTransform rectTransform, float width = 0)
        {
            if (rectTransform.parent == null) return;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(width, 0);
            rectTransform.offsetMax = new Vector2(-width, 0);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }


        internal static void SetHeight(this RectTransform rect, float height)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }

        internal static void SetWidth(this RectTransform rect, float width)
        {
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }

        internal static void SetAnchor(this RectTransform rect, TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(0, 1);
                    rect.pivot = new Vector2(0, 1);
                    break;

                case TextAnchor.UpperCenter:
                    rect.anchorMin = new Vector2(0.5f, 1);
                    rect.anchorMax = new Vector2(0.5f, 1);
                    rect.pivot = new Vector2(0.5f, 1);
                    break;

                case TextAnchor.UpperRight:
                    rect.anchorMin = new Vector2(1, 1);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(1, 1);

                    break;

                case TextAnchor.MiddleLeft:
                    rect.anchorMin = new Vector2(0, 0.5f);
                    rect.anchorMax = new Vector2(0, 0.5f);
                    rect.pivot = new Vector2(0, 0.5f);
                    break;

                case TextAnchor.MiddleCenter:
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    break;

                case TextAnchor.MiddleRight:
                    rect.anchorMin = new Vector2(1, 0.5f);
                    rect.anchorMax = new Vector2(1, 0.5f);
                    rect.pivot = new Vector2(1, 0.5f);
                    break;

                case TextAnchor.LowerLeft:
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(0, 0);
                    rect.pivot = new Vector2(0, 0);
                    break;

                case TextAnchor.LowerCenter:
                    rect.anchorMin = new Vector2(0.5f, 0);
                    rect.anchorMax = new Vector2(0.5f, 0);
                    rect.pivot = new Vector2(0.5f, 0);
                    break;

                case TextAnchor.LowerRight:
                    rect.anchorMin = new Vector2(1, 0);
                    rect.anchorMax = new Vector2(1, 0);
                    rect.pivot = new Vector2(1, 0);
                    break;
            }

            // force rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}