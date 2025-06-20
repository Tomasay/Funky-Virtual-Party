using Glitch9.ScriptableObjects;
using UnityEngine;

namespace Glitch9.AIDevKit.Client
{
    /// <summary>
    /// Base class for AI client settings.
    /// This class is used to store API keys and other settings related to AI clients.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public abstract class AIClientSettings<TSelf> : ScriptableResource<TSelf>
        where TSelf : AIClientSettings<TSelf>
    {
        [SerializeField] protected ApiKey apiKey;

        /// <summary>
        /// Retrieves the API key.
        /// </summary>
        public string GetApiKey() => apiKey?.GetKey();

        /// <summary>
        /// Checks if the API key is set.
        /// </summary> 
        public virtual bool HasApiKey() => apiKey != null && apiKey.HasValue;
    }
}