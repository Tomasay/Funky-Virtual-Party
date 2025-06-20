using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum ImageFormat
    {
        /// <summary>
        /// Dall-E
        /// </summary>
        [ApiEnum("url")] Url,

        /// <summary>
        /// Dall-E, Gemini, Ollama
        /// </summary>
        [ApiEnum("b64_json")] Base64Json,

        /// <summary>
        /// Imagen
        /// </summary>
        [ApiEnum("bytes")] Bytes,
    }
}