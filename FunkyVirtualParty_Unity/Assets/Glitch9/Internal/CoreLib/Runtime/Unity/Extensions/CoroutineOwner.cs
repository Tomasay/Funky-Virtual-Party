using System.Collections;
using UnityEngine;
using System;

#if UNITY_EDITOR && UNITY_EDITOR_COROUTINES
using Unity.EditorCoroutines.Editor;
#endif

namespace Glitch9
{
    public class CoroutineOwner
    {
        private const string COROUTINE_OWNER_NAME_TO_FIND = "GameManager";
        private const string COROUTINE_OWNER_NAME_TO_CREATE = "CoroutineOwner";
        private static MonoBehaviour _coroutineOwner;

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (Application.isPlaying)
            {
                if (_coroutineOwner == null)
                {
                    // Find a scene object that is not destroyed on load
                    GameObject go = GameObject.Find(COROUTINE_OWNER_NAME_TO_FIND);
                    if (go == null) go = new GameObject(COROUTINE_OWNER_NAME_TO_CREATE);
                    _coroutineOwner = go.GetComponent<MonoBehaviour>();
                }

                return _coroutineOwner.StartCoroutine(enumerator);
            }
            else
            {
#if UNITY_EDITOR && UNITY_EDITOR_COROUTINES
                EditorCoroutineUtility.StartCoroutineOwnerless(enumerator);
#endif
            }

            return null;
        }

        public static void StopAllCoroutines()
        {
            if (Application.isPlaying)
            {
                _coroutineOwner.StopAllCoroutines();
            }
            else
            {
                Debug.LogWarning("StopAllCoroutines is not supported in Editor mode");
            }
        }

        public static void TriggerAfterEndOfFrame(Action callback, Action secondCallback = null)
        {
            StartCoroutine(TriggerAfterEndOfFrameCoroutine(callback, secondCallback));
        }

        private static IEnumerator TriggerAfterEndOfFrameCoroutine(Action callback, Action secondCallback = null)
        {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
            if (secondCallback == null) yield break;
            yield return new WaitForEndOfFrame();
            secondCallback?.Invoke();
        }
    }
}
