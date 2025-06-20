using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Glitch9
{
    internal static class FindExtensions
    {
        internal static T Find<T, TSelf>(this TSelf self, T defaultComponent, string chileName) where TSelf : MonoBehaviour
        {
            if (defaultComponent != null) return defaultComponent;
            self.transform.Find(chileName).TryGetComponent<T>(out T result);
            if (result == null) Debug.LogError($"{chileName} 컴포넌트를 찾을 수 없습니다.");
            return result;
        }

        internal static T Find<T, TSelf>(this TSelf self, T defaultComponent, string chileName, string path) where TSelf : MonoBehaviour
        {
            if (defaultComponent != null) return defaultComponent;
            if (!path.EndsWith("/")) path += "/"; // if path doesn't end with a slash, add it
            string fullPath = path + chileName;
            return self.Find(defaultComponent, fullPath);
        }

        internal static Transform Find<TSelf>(this TSelf self, string childName) where TSelf : MonoBehaviour
        {
            Transform result = self.transform.Find(childName);
            if (result == null) Debug.LogError($"{childName} 컴포넌트를 찾을 수 없습니다.");
            return result;
        }

        internal static GameObject Find(this GameObject gameObject, string childName)
        {
            GameObject childObj;
            try { childObj = gameObject.transform.Find(childName).gameObject; }
            catch { childObj = null; }

            return childObj;
        }

        internal static T GetOrAddComponent<T>(this Transform obj) where T : Component
        {
            return GetOrAddComponent<T>(obj.gameObject);
        }

        internal static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.Log($"{typeof(T)} 컴포넌트를 추가합니다.");
                component = obj.AddComponent<T>();
            }

            return component;
        }

        internal static GameObject GetOrAddGameObjectInChildren(this GameObject obj, string childName)
        {
            Transform child = obj.transform.Find(childName);
            if (child == null)
            {
                GameObject childObj = new(childName);
                childObj.transform.SetParent(obj.transform);
                childObj.transform.localPosition = Vector3.zero;
                childObj.transform.localScale = Vector3.one;
                child = childObj.transform;
            }

            return child.gameObject;
        }

        internal static void SafeDestroyChild(this GameObject obj, string childName)
        {
            GameObject childObj = obj.Find(childName);

            if (childObj != null)
            {
                Object.DestroyImmediate(childObj);
            }
        }

        internal static T GetOrAddComponentInChildren<T, TV>(this TV obj, string childName = null) where T : Component where TV : Component
        {
            return obj.gameObject.GetOrAddComponentInChildren<T>(childName);
        }

        internal static T GetOrAddComponentInChildren<T>(this RectTransform rect, string childName = null) where T : Component
        {
            return rect.gameObject.GetOrAddComponentInChildren<T>(childName);
        }

        internal static T GetComponentInChildrenWithName<T>(this Transform parent, params string[] names) where T : Component
        {
            return GetComponentInChildrenWithName<T>(parent, false, names);
        }

        internal static T GetComponentInChildrenWithName<T>(this Transform parent, bool includeInactive, params string[] names) where T : Component
        {
            if (names == null || names.Length == 0) return null;

            foreach (string name in names)
            {
                Transform child = parent.FindDeepChild(name, includeInactive);
                if (child == null) continue;
                if (child.TryGetComponent(out T component)) return component;
            }

            return null;
        }

        internal static Component GetComponentInChildrenWithName(this Transform parent, Type type, params string[] names)
        {
            return GetComponentInChildrenWithName(parent, type, false, names);
        }

        internal static Component GetComponentInChildrenWithName(this Transform parent, Type type, bool includeInactive, params string[] names)
        {
            if (names == null || names.Length == 0) return null;

            foreach (string name in names)
            {
                Transform child = parent.FindDeepChild(name, includeInactive);
                if (child == null) continue;
                if (child.TryGetComponent(type, out Component component)) return component;
            }

            return null;
        }

        internal static T GetOrAddComponentInChildren<T>(this GameObject gameObject, string nameToSearch) where T : Component
        {
            if (nameToSearch.LogIfNullOrEmpty()) return null;

            // Try to find a child with the specified name
            Transform childTransform = gameObject.transform.Find(nameToSearch);
            GameObject targetObj;

            if (childTransform != null)
            {
                targetObj = childTransform.gameObject;
                T component = targetObj.GetComponent<T>();
                if (component != null) return component;
                Debug.Log($"Found {nameToSearch}, but it doesn't have {typeof(T)} component. Adding one...");
                return targetObj.AddComponent<T>();
            }
            else
            {
                Debug.Log($"Couldn't find {nameToSearch}. Creating a new GameObject with {typeof(T)} component...");
                targetObj = CreateGameObject(nameToSearch, gameObject.transform);
                return targetObj.AddComponent<T>();
            }
        }

        internal static GameObject GetOrAddGameObject(this GameObject parent, string objName, string secondName = null)
        {
            Transform childTransform = parent.transform.Find(objName);
            if (childTransform == null && secondName != null) childTransform = parent.transform.Find(secondName);
            return childTransform != null ? childTransform.gameObject : CreateGameObject(objName, parent.transform);
        }

        private static GameObject CreateGameObject(string name, Transform parent)
        {
            GameObject obj = new(name);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            return obj;
        }

        internal static Transform FindDeepChild(this Transform parent, string childName, bool includeInactive = false)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    if (includeInactive == false && !child.gameObject.activeInHierarchy) continue;
                    return child;
                }
                Transform result = child.FindDeepChild(childName);
                if (result != null) return result;
            }
            return null;
        }

        internal static int GetChildIndex<T>(this T obj) where T : Component
        {
            Transform parent = obj.transform.parent;
            T[] children = parent.GetComponentsInChildren<T>(true);
            int count = 0;

            foreach (T child in children)
            {
                if (child.Equals(obj)) return count;
                count++;
            }

            Debug.LogError($"Child index not found for {obj.GetType().Name}.");
            return -1;
        }

        internal static GameObject[] FindGameObjectsWithTagInChildren(this Transform parent, string tag)
        {
            List<GameObject> taggedChildren = new();

            foreach (Transform child in parent)
            {
                if (child.gameObject.CompareTag(tag))
                {
                    taggedChildren.Add(child.gameObject);
                }

                // Optional: to search in deeper levels, uncomment the following lines
                taggedChildren.AddRange(FindGameObjectsWithTagInChildren(child, tag));
            }

            return taggedChildren.ToArray();
        }

        internal static GameObject FindGameObjectWithTagInChildren(this Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.gameObject.CompareTag(tag))
                {
                    return child.gameObject;
                }

                // Optional: to search in deeper levels, uncomment the following lines
                GameObject result = FindGameObjectWithTagInChildren(child, tag);
                if (result != null) return result;
            }

            return null;
        }

        internal static Transform FindContainer<TContainer>(this TContainer container) where TContainer : MonoBehaviour
        {
            if (container != null) return container.transform;
            TContainer containerToFind = UnityCompat.FindFirstObjectByType<TContainer>();
            if (containerToFind == null)
            {
                Debug.LogError($"Container of type {typeof(TContainer).Name} not found");
                return null;
            }
            return containerToFind.transform;
        }
    }
}