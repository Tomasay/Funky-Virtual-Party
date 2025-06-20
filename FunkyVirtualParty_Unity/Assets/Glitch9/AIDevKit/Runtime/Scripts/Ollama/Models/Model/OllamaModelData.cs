using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Ollama
{
    /// <summary>
    /// Information about a single locally available model.
    /// </summary>
    public class OllamaModelData : IModelData
    {
        /// <summary>
        /// Required. The name of the model.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Required. The model identifier.
        /// </summary>
        [JsonProperty("model")] public string Id { get; set; }

        /// <summary>
        /// Required. The model file size in bytes.
        /// </summary>
        [JsonProperty("size")] public long Size { get; set; }

        /// <summary>
        /// Required. The digest of the model.
        /// 모델 파일의 해시 값
        /// 보통 SHA-256 같은 해시 알고리즘으로 생성된 문자열로
        /// 해당 모델의 정합성과 고유성을 확인
        /// </summary>
        [JsonProperty("digest")] public string Digest { get; set; }

        /// <summary>
        /// Required. Detailed information about the model.
        /// </summary>
        [JsonProperty("details")] public ModelDetails Details { get; set; }

        /// <summary>
        /// Required. The expiration time of the model in Zulu time.
        /// </summary>
        [JsonProperty("expires_at")] public ZuluTime ExpiresAt { get; set; }

        /// <summary>
        /// Required. The VRAM size required by the model.
        /// </summary>
        [JsonProperty("size_vram")] public long SizeVram { get; set; }


        // --- IModelData Implementation ---
        [JsonIgnore] public Api Api => Api.Ollama;
        [JsonIgnore] public string OwnedBy => "Local";
        [JsonIgnore] public string Family => Details?.Family; // TODO: Check if this is correct. It seems to be the main family of the model.  
        [JsonIgnore] public string BaseId => Details?.BaseId;
        [JsonIgnore] public bool? IsFineTuned => BaseId != null;
        [JsonIgnore] public bool? IsTrainable => false; // there is no training(fine-tuning) in Ollama 
    }

    /// <summary>
    /// Detailed metadata about a model.
    /// </summary>
    public class ModelDetails
    {
        /// <summary>
        /// Required. The parent model if this model is derived from another.
        /// </summary>
        [JsonProperty("parent_model")] public string BaseId { get; set; }

        /// <summary>
        /// Required. The format of the model (e.g., gguf).
        /// </summary>
        [JsonProperty("format")] public string Format { get; set; }

        /// <summary>
        /// Required. The main family of the model.
        /// </summary>
        [JsonProperty("family")] public string Family { get; set; }

        /// <summary>
        /// Required. List of families this model belongs to.
        /// </summary>
        [JsonProperty("families")] public List<string> Families { get; set; }

        /// <summary>
        /// Required. The parameter size of the model (e.g., "7.2B").
        /// </summary>
        [JsonProperty("parameter_size")] public string ParameterSize { get; set; }

        /// <summary>
        /// Required. The quantization level used by the model.
        /// </summary>
        [JsonProperty("quantization_level")] public string QuantizationLevel { get; set; }
    }
}
