using System;

namespace Glitch9.AIDevKit
{
    public class InvalidServiceStatusException : Exception
    {
        public InvalidServiceStatusException(string message) : base(message) { }
        public InvalidServiceStatusException(string message, Exception innerException) : base(message, innerException) { }
        public InvalidServiceStatusException(string serviceName, string message) : base($"[{serviceName}] {message}") { }
        public InvalidServiceStatusException(string serviceName, string message, Exception innerException) : base($"{serviceName}] {message}", innerException) { }
    }
}