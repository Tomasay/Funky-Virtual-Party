using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class Embedding : AIResponse
    {
        /// <summary>
        /// The embedding vector, which is a list of floats.
        /// The length of vector depends on the model as listed in the embedding guide.
        /// See the <see href="https://platform.openai.com/docs/guides/embeddings">embedding guide</see> for more information on the length of the vector for each model.
        /// </summary>
        [JsonProperty("embedding")] public float[] EmbeddingVector { get; set; }

        /// <summary>
        /// The index of the embedding in the list of embeddings.
        /// </summary>
        [JsonProperty("index")] public int Index { get; set; }
    }
}