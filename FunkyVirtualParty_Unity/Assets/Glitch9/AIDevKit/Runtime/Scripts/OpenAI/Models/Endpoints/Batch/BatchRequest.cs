using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Creates and executes a batch from an uploaded file of requests
    /// </summary>
    public class BatchRequest : AIRequest
    {
        /// <summary>
        /// [Required] The ID of an uploaded file that contains requests for the new batch.
        /// </summary>
        [JsonProperty("input_file_id")] public string InputFileId { get; set; }

        /// <summary>
        /// [Required] The endpoint to be used for all requests in the batch.
        /// Currently, /v1/chat/completions, /v1/embeddings, and /v1/completions are supported.
        /// Note that /v1/embeddings batches are also restricted to a maximum of 50,000 embedding inputs across all requests in the batch.
        /// </summary>
        [JsonProperty("endpoint")] public string BatchEndpoint { get; set; }

        /// <summary>
        /// [Required] The time frame within which the batch should be processed.
        /// Currently only 24h is supported.
        /// </summary>
        [JsonProperty("completion_window")] public string CompletionWindow { get; set; }



        public class Builder : ModelRequestBuilder<Builder, BatchRequest>
        {
            public Builder InputFileId(string inputFileId)
            {
                _req.InputFileId = inputFileId;
                return this;
            }

            public Builder BatchEndpoint(string batchEndpoint)
            {
                _req.BatchEndpoint = batchEndpoint;
                return this;
            }

            public Builder CompletionWindow(string completionWindow)
            {
                _req.CompletionWindow = completionWindow;
                return this;
            }
        }
    }
}