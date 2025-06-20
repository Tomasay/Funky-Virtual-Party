using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class FineTuningRequest : AIRequest
    {
        /// <summary>
        /// The ID of an uploaded file that contains training data.
        /// See upload file for how to upload a file.
        /// Your dataset must be formatted as a JSONL file. Additionally, you must upload your file with the purpose fine-tune.
        /// </summary>
        [JsonProperty("training_file")] public string TrainingFile { get; set; }

        /// <summary>
        /// The hyperparameters used for the fine-tuning job.
        /// </summary>
        [JsonProperty("hyperparameters")] public HyperParameters HyperParameters { get; set; }

        /// <summary>
        /// A string of up to 18 characters that will be added to your fine-tuned model name.
        /// For example, a suffix of "custom-model-name" would produce a model name like ft:gpt-3.5-turbo:openai:custom-model-name:7p4lURel.
        /// 
        /// </summary>
        [JsonProperty("suffix")] public string Suffix { get; set; }

        /// <summary>
        /// The ID of an uploaded file that contains validation data.
        /// If you provide this file, the data is used to generate validation metrics periodically during fine-tuning. These metrics can be viewed in the fine-tuning results file. The same data should not be present in both train and validation files.
        /// Your dataset must be formatted as a JSONL file. You must upload your file with the purpose fine-tune.
        /// See the fine-tuning guide for more details. 
        /// </summary>
        [JsonProperty("validation_file")] public string ValidationFile { get; set; }


        public class Builder : ModelRequestBuilder<Builder, FineTuningRequest>
        {
            public Builder SetBaseModel(Model model)
            {
                if (model == null) return this;
                _req.Model = model;
                return this;
            }

            public void SetTrainingFile(string trainingFile)
            {
                _req.TrainingFile = trainingFile;
            }

            public void SetHyperParameters(HyperParameters hyperparameters)
            {
                _req.HyperParameters = hyperparameters;
            }

            public void SetSuffix(string suffix)
            {
                _req.Suffix = suffix;
            }

            public void SetValidationFile(string validationFile)
            {
                _req.ValidationFile = validationFile;
            }
        }
    }
}