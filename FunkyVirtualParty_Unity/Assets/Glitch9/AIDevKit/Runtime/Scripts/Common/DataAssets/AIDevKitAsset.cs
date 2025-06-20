using UnityEngine;

namespace Glitch9.AIDevKit
{
    public abstract class AIDevKitAsset : ScriptableObject
    {
        public static implicit operator string(AIDevKitAsset asset) => asset == null ? null : asset.Id;

        [SerializeField] protected string id;
        [SerializeField] protected string displayName; // can't use 'name' as it's a reserved keyword for ScriptableObject (UnityEngine)
        [SerializeField] protected Api api;
        [SerializeField] protected bool deprecated;
        [SerializeField] protected bool custom;
        [SerializeField] protected bool locked;

        /// <summary>
        /// The unique identifier of the asset 
        /// If it's a model: "gpt-3.5-turbo", "gemini-pro-vision"
        /// If it's a voice: "alloy"(OpenAI), "21m00Tcm4TlvDq8ikWAM"(ElevenLabs)
        /// </summary>
        public string Id => id;

        /// <summary>
        /// The name of the asset 
        /// If it's a model: "gpt-3.5-turbo", "gemini-pro-vision"
        /// If it's a voice: "alloy"(OpenAI), "rachel"(ElevenLabs)
        /// This is a human-readable name for the model that can be displayed in the UI or logs.
        /// </summary>
        public virtual string Name => displayName ?? id;

        /// <summary>
        /// The API service this asset uses (e.g., OpenAI, Google).
        /// This information is used to route requests to the correct <see cref="AIClient"/>.
        /// </summary>
        public Api Api => api;

        /// <summary>
        /// Indicates whether the asset is user-customized or not.
        /// If it's a model, it means the model is a fine-tuned version of a base model.
        /// If it's a voice, it means the voice is a custom voice created by the user.
        /// </summary>
        public bool IsCustom => custom;

        /// <summary>
        /// Indicates whether the asset is deprecated and should not be used in production.
        /// <see cref="CRUDService"/> will throw an exception if a deprecated asset is used.
        /// </summary>
        public bool IsDeprecated => deprecated;

        internal bool IsLocked
        {
            get => locked;
            set => locked = value;
        }

        internal void Deprecate() => deprecated = true;

        public static bool operator ==(AIDevKitAsset a, AIDevKitAsset b) => Equals(a, b);
        public static bool operator !=(AIDevKitAsset a, AIDevKitAsset b) => !(a == b);

        public override string ToString() => Id ?? string.Empty;
        public bool Equals(AIDevKitAsset other) => other != null && Api == other.Api && Id == other.Id;
        public override bool Equals(object obj) => Equals(obj as AIDevKitAsset);
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Api.GetHashCodeOrDefault();
                hash = hash * 23 + Id.GetHashCodeOrDefault();
                return hash;
            }
        }
    }


    public interface IAIDevKitAssetFilter<T> where T : AIDevKitAsset
    {
        Api Api { get; }
        bool IsEmpty { get; }
        bool Matches(T data);
    }
}