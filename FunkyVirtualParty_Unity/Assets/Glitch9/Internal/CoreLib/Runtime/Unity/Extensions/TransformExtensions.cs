using System;
using UnityEngine;

namespace Glitch9
{
    public static class TransformExtensions
    {
        public static void SetActive(this Transform transform, bool active)
        {
            if (transform == null) return;
            transform.gameObject.SetActive(active);
        }

        public static void SetLocalPosX(this Transform transform, float posX, bool relative = false)
        {
            if (relative)
            {
                transform.localPosition = new Vector3(transform.localPosition.x + posX, transform.localPosition.y, transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(posX, transform.localPosition.y, transform.localPosition.z);
            }
        }

        public static void SetLocalPosY(this Transform transform, float posY, bool relative = false)
        {
            if (relative)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + posY, transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);
            }
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            if (Application.isPlaying)
            {
                RunActionOnAllChildren(transform, (child) => UnityEngine.Object.Destroy(child.gameObject));
            }
            else
            {
                while (transform.childCount > 0)
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
        }

        public static void ShowAllChildren(this Transform transform) => RunActionOnAllChildren(transform, (child) => child.gameObject.SetActive(true));
        public static void HideAllChildren(this Transform transform) => RunActionOnAllChildren(transform, (child) => child.gameObject.SetActive(false));
        public static void RunActionOnAllChildren(this Transform transform, Action<Transform> action)
        {
            if (transform.LogIfNull()) return;
            if (transform.childCount <= 0) return;
            foreach (Transform child in transform)
            {
                action(child);
            }
        }

        public static void SafeRemoveComponent<T>(this Transform transform) where T : Component
        {
            if (transform == null) return;
            T component = transform.GetComponent<T>();
            if (component != null) transform.RemoveComponent<T>();
        }

        public static void EnsureComponent<T>(this Transform transform) where T : Component
        {
            if (transform == null) return;
            T component = transform.GetComponent<T>();
            if (component == null) transform.gameObject.AddComponent<T>();
        }

        public static void RemoveMissingComponents(this Transform transform)
        {
            if (transform == null) return;
            Component[] components = transform.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null)
                {
                    transform.RemoveComponent(component);
                }
            }
        }

        public static void RemoveComponent<T>(this Transform transform) where T : Component
        {
            if (transform == null) return;
            T component = transform.GetComponent<T>();
            if (component != null) transform.RemoveComponent(component);
        }

        public static void RemoveComponent(this Transform transform, Component component)
        {
            if (transform == null) return;
            if (component == null) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(component);
            else UnityEngine.Object.DestroyImmediate(component);
        }

        public static float GetHeight(this Transform transform)
        {
            if (transform == null) return 0;
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null) return rectTransform.rect.height;
            else return transform.localScale.y;
        }

        public static float GetWidth(this Transform transform)
        {
            if (transform == null) return 0;
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null) return rectTransform.rect.width;
            else return transform.localScale.x;
        }
    }
}