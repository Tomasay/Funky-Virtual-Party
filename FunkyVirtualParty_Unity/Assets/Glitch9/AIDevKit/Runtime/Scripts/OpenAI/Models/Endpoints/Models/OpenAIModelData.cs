using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Describes an OpenAI model offering that can be used with the API.
    /// </summary>
    /// <remarks>
    /// Only used for List models and Retrieve model call.
    /// </remarks>
    public class OpenAIModelData : AIResponse, IModelData
    {
        /// <summary>
        /// The organization that owns the model.
        /// If it's a public model, it's owned by "openai".
        /// </summary>
        [JsonProperty("owned_by")] public string OwnedBy { get; set; }


        #region IModelData implementation  
        [JsonIgnore] public Api Api => Api.OpenAI;
        [JsonIgnore] public string Provider => Api.OpenAI.ToString();
        [JsonIgnore] public string Name => null;

        #endregion IModelData implementation 
    }
}
