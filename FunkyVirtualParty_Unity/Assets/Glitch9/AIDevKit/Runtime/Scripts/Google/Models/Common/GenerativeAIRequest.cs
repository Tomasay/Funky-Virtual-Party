using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class GenerativeAIRequest : RequestBody
    {
        [JsonIgnore] public Model Model { get; set; }

        public abstract class GenerativeAIRequestBuilder<TBuilder, TRequest> : RequestBodyBuilder<TBuilder, TRequest>
            where TBuilder : GenerativeAIRequestBuilder<TBuilder, TRequest>
            where TRequest : GenerativeAIRequest
        {
            public TBuilder SetModel(Model model)
            {
                _req.Model = model;
                return (TBuilder)this;
            }
        }
    }
}
