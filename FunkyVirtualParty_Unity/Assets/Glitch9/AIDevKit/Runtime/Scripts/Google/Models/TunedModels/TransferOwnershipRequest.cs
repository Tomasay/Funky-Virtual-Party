using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Transfers ownership of the tuned model. This is the only way to change ownership of the tuned model. The current owner will be downgraded to writer role.
    /// </summary>
    public class TransferOwnershipRequest : GenerativeAIRequest
    {
        /// <summary>
        /// Required. The email address of the user to whom the tuned model is being transferred to.
        /// </summary>
        [JsonProperty("emailAddress")] public string EmailAddress { get; set; }

        /// <summary>
        /// Required. The ID of the tuned model to transfer ownership of.
        /// </summary>
        [JsonIgnore] public string TunedModelId { get; set; }
    }
}