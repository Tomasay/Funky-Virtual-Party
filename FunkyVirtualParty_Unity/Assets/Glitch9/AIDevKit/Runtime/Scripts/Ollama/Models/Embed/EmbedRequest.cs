using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    /// <summary>
    /// /embed
    /// 텍스트(또는 이미지)를 벡터로 변환하는 API
    /// 모델 이름, 입력, 옵션 등을 지정하여 임베딩 수행
    /// </summary>
    public class EmbedRequest : OllamaRequest
    {
        /// <summary>
        /// Required. The input to embed. Can be a string or list of strings.
        /// </summary>
        [JsonProperty("input")] public StringOr<string> Input { get; set; }

        /// <summary>
        /// Optional. Whether to truncate input tokens that exceed the context length.
        /// </summary>
        [JsonProperty("truncate")] public bool? Truncate { get; set; }
    }
}
