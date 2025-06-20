using UnityEngine;

namespace Glitch9
{
    internal static class UnityCompat
    {
        public static T FindFirstObjectByType<T>() where T : UnityEngine.Object
        {
#if UNITY_6000_0_OR_NEWER
            return GameObject.FindFirstObjectByType<T>();
#else
            return GameObject.FindObjectOfType<T>();
#endif 
        }

        public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : UnityEngine.Object
        {
#if UNITY_6000_0_OR_NEWER
            return GameObject.FindObjectsByType<T>(sortMode);
#else
            return GameObject.FindObjectsOfType<T>(true);
#endif
        }
    }
}