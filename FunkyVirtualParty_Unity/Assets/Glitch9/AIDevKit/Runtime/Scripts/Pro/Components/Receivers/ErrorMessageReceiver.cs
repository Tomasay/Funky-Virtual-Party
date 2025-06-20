using System;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Error Message Receiver (Text)")]
    public class ErrorMessageReceiver : ErrorReceiver
    {
        [Tooltip("Invoked when receiving an error response from the AI, or when an exception occurs.")]
        [SerializeField] UnityEvent<string> onError;

        public override void OnError(string message) => onError?.Invoke(message);
        public override void OnReceiveErrorResponse(ErrorResponse errorResponse) => onError?.Invoke(errorResponse.ToString());
        public override void OnException(Exception exception) => onError?.Invoke(exception.Message);
    }
}