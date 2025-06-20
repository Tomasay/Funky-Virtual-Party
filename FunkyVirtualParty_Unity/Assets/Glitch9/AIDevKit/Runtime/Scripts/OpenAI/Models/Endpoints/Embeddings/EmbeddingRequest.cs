using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Returns a list of embedding objects.
    /// </summary>
    public class EmbeddingRequest : AIRequest
    {
        /// <summary>
        /// [Required] Input Text to embed, encoded as a string or array of tokens.
        /// To embed multiple inputs in a single request, pass an array of strings or array of token arrays.
        /// The input must not exceed the max input tokens for the model (8192 tokens for Text-embedding-ada-002),
        /// cannot be an empty string, and any array must be 2048 dimensions or less.
        /// Example Python code for counting tokens.
        /// </summary>
        [JsonProperty("input")] public StringOr<string> Input { get; set; }

        /// <summary>
        /// The format to return the embeddings in. Can be either float or base64.
        /// </summary>
        [JsonProperty("encoding_format")] public EncodingFormat? EncodingFormat { get; set; }

        /// <summary>
        /// The number of dimensions the resulting Output embeddings should have.
        /// Only supported in Text-embedding-3 and later models.
        /// </summary>
        [JsonProperty("dimensions")] public int Dimensions { get; set; }


        public class Builder : ModelRequestBuilder<Builder, EmbeddingRequest>
        {

            public Builder SetInput(StringOr<string> input)
            {
                _req.Input = input;
                return this;
            }

            public Builder SetInput(string input)
            {
                _req.Input = new(input);
                return this;
            }

            public Builder SetEncodingFormat(EncodingFormat encodingFormat)
            {
                _req.EncodingFormat = encodingFormat;
                return this;
            }

            public Builder SetDimensions(int dimensions)
            {
                _req.Dimensions = dimensions;
                return this;
            }
        }
    }
}