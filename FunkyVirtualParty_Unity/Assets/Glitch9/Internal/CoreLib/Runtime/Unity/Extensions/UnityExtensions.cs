using System;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9
{
    public static class UnityExtensions
    {
        internal static Action ToAction(this UnityEvent unityEvent)
        {
            if (unityEvent == null)
            {
                return null;
            }
            else
            {
                return () => unityEvent.Invoke();
            }
        }

        internal static Action<T> ToAction<T>(this UnityEvent<T> unityEvent)
        {
            if (unityEvent == null)
            {
                return null;
            }
            else
            {
                return arg => unityEvent.Invoke(arg);
            }
        }

        public static Sprite ToSprite(this Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }
            else
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }
}