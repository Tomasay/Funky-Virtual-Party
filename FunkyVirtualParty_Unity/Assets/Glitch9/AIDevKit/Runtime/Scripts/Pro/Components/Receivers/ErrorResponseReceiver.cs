using System;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Error Response Receiver")]
    public class ErrorResponseReceiver : ErrorReceiver
    {
        [Tooltip("Invoked when receiving an error response from the AI, or when an exception occurs.")]
        [SerializeField] UnityEvent<ErrorResponse> onError;

        public override void OnError(string message) => onError?.Invoke(new ErrorResponse(message));
        public override void OnReceiveErrorResponse(ErrorResponse errorResponse) => onError?.Invoke(errorResponse);
        public override void OnException(Exception exception) => onError?.Invoke(new ErrorResponse(exception));
    }
}