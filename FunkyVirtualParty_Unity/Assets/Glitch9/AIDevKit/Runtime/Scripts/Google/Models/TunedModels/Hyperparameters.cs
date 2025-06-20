using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class Hyperparameters
    {
        [JsonProperty("epoch_count")]
        public int EpochCount { get; set; }

        [JsonProperty("batch_size")]
        public int BatchSize { get; set; }

        [JsonProperty("learning_rate")]
        public float LearningRate { get; set; }
    }
}