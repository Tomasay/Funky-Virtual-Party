
using UnityEngine;

namespace Glitch9.AIDevKit
{
    public interface IErrorHandler
    {
        void OnError(string message);
        void OnReceiveErrorResponse(ErrorResponse errorResponse);
        void OnException(System.Exception exception);
    }

    public abstract class ErrorReceiver : MonoBehaviour, IErrorHandler
    {
        public abstract void OnError(string message);
        public abstract void OnReceiveErrorResponse(ErrorResponse errorResponse);
        public abstract void OnException(System.Exception exception);
    }
}