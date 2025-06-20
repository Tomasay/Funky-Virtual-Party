using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// The hyperparameters used for the fine-tuning job.
    /// </summary>
    public class HyperParameters
    {
        /// <summary>
        /// Number of examples in each batch.
        /// A larger batch size means that model parameters are updated less frequently, but with lower variance.
        /// </summary>
        [JsonProperty("batch_size")] public string BatchSize { get; set; }

        /// <summary>
        /// Scaling factor for the learning rate.
        /// A smaller learning rate may be useful to avoid overfitting.
        /// </summary>
        [JsonProperty("learning_rate_multiplier")] public string LearningRateMultiplier { get; set; }

        /// <summary>
        /// The number of epochs to train the model for.
        /// An epoch refers to one full cycle through the training dataset.
        /// </summary>
        [JsonProperty("n_epochs")] public string NEpochs { get; set; }
    }
}