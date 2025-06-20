using System;
using UnityEngine;

namespace Glitch9.AIDevKit.Components
{
    public abstract class AIDevKitComponent : MonoBehaviour
    {
        [SerializeField] protected ErrorReceiver errorReceiver;
        public bool IsBusy { get; protected set; } = false;

        protected void OnError(string message)
        {
            if (errorReceiver != null) errorReceiver.OnError(message);
            else Debug.LogError(message);
        }

        protected void OnError(Exception error)
        {
            if (errorReceiver != null) errorReceiver.OnException(error);
            else Debug.LogError(error);
        }

        protected void OnError(ErrorResponse error)
        {
            if (errorReceiver != null) errorReceiver.OnReceiveErrorResponse(error);
            else Debug.LogError(error);
        }
    }
}