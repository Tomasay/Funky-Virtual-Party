using System;
using Newtonsoft.Json;
using UnityEngine;


namespace Glitch9.AIDevKit.Google
{

    [Serializable]
    public class TunedModel : GenerativeAIRequest
    {
        [JsonIgnore, SerializeField] private string name;
        [JsonIgnore, SerializeField] private string sourceModel;
        [JsonIgnore, SerializeField] private string baseModel;
        [JsonIgnore, SerializeField] private string displayName;
        [JsonIgnore, SerializeField] private string description;
        [JsonIgnore, SerializeField] private float temperature;
        [JsonIgnore, SerializeField] private float topP;
        [JsonIgnore, SerializeField] private float topK;
        [JsonIgnore, SerializeField] private string state;
        [JsonIgnore, SerializeField] private ZuluTime createTime;
        [JsonIgnore, SerializeField] private ZuluTime updateTime;
        [JsonIgnore, SerializeField] private TuningTask tuningTask;
        [JsonIgnore, SerializeField] private TunedModelSource tunedModelSource;

        [JsonProperty("name")]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [JsonProperty("source_model")]
        public string SourceModel
        {
            get => sourceModel;
            set => sourceModel = value;
        }

        [JsonProperty("base_model")]
        public string BaseModel
        {
            get => baseModel;
            set => baseModel = value;
        }

        [JsonProperty("display_name")]
        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        [JsonProperty("description")]
        public string Description
        {
            get => description;
            set => description = value;
        }

        [JsonProperty("temperature")]
        public float? Temperature
        {
            get => temperature;
            set => temperature = value ?? 0; // 기본값 0으로 설정
        }

        [JsonProperty("top_p")]
        public float? TopP
        {
            get => topP;
            set => topP = value ?? 0; // 기본값 0으로 설정
        }

        [JsonProperty("top_k")]
        public float? TopK
        {
            get => topK;
            set => topK = value ?? 0; // 기본값 0으로 설정
        }

        [JsonProperty("state")]
        public string State
        {
            get => state;
            set => state = value;
        }

        [JsonProperty("create_time")]
        public ZuluTime? CreateTime
        {
            get => createTime;
            set => createTime = value ?? ZuluTime.MinValue; // 기본값 설정
        }

        [JsonProperty("update_time")]
        public ZuluTime? UpdateTime
        {
            get => updateTime;
            set => updateTime = value ?? ZuluTime.MinValue; // 기본값 설정
        }

        [JsonProperty("tuning_task")]
        public TuningTask TuningTask
        {
            get => tuningTask;
            set => tuningTask = value;
        }

        [JsonProperty("tuned_model_source")]
        public TunedModelSource TunedModelSource
        {
            get => tunedModelSource;
            set => tunedModelSource = value;
        }

        public override string ToString()
        {
            return $"Model(name='{name}', " +
                   $"source_model='{sourceModel}', " +
                   $"base_model='{baseModel}', " +
                   $"display_name='{displayName}', " +
                   $"description='{description}', " +
                   $"temperature={temperature}, " +
                   $"top_p={topP}, " +
                   $"top_k={topK}, " +
                   $"state='{state}', " +
                   $"create_time={createTime}, " +
                   $"update_time={updateTime}, " +
                   $"tuning_task={tuningTask}, " +
                   $"tuned_model_source={tunedModelSource})";
        }

        public class Builder : GenerativeAIRequestBuilder<Builder, TunedModel>
        {
            public Builder SetName(string name)
            {
                _req.Name = name;
                return this;
            }

            public Builder SetSourceModel(string sourceModel)
            {
                _req.SourceModel = sourceModel;
                return this;
            }

            public Builder SetBaseModel(string baseModel)
            {
                _req.BaseModel = baseModel;
                return this;
            }

            public Builder SetDisplayName(string displayName)
            {
                _req.DisplayName = displayName;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _req.Description = description;
                return this;
            }

            public Builder SetTemperature(float temperature)
            {
                _req.Temperature = temperature;
                return this;
            }

            public Builder SetTopP(float topP)
            {
                _req.TopP = topP;
                return this;
            }

            public Builder SetTopK(float topK)
            {
                _req.TopK = topK;
                return this;
            }

            public Builder SetTrainingData(params TuningExample[] trainingData)
            {
                _req.TuningTask ??= new TuningTask();
                _req.TuningTask.TrainingData.AddTrainingData(trainingData);
                return this;
            }

            public Builder SetEpochCount(int epochCount)
            {
                _req.TuningTask ??= new TuningTask();
                _req.TuningTask.Hyperparameters ??= new Hyperparameters();
                _req.TuningTask.Hyperparameters.EpochCount = epochCount;
                return this;
            }

            public Builder SetBatchSize(int batchSize)
            {
                _req.TuningTask ??= new TuningTask();
                _req.TuningTask.Hyperparameters ??= new Hyperparameters();
                _req.TuningTask.Hyperparameters.BatchSize = batchSize;
                return this;
            }

            public Builder SetLearningRate(float learningRate)
            {
                _req.TuningTask ??= new TuningTask();
                _req.TuningTask.Hyperparameters ??= new Hyperparameters();
                _req.TuningTask.Hyperparameters.LearningRate = learningRate;
                return this;
            }
        }
    }

    /// <summary>
    /// Tuned model as a source for training a new model.
    /// </summary>
    public class TunedModelSource
    {
        [JsonIgnore, SerializeField] private string tunedModel;
        [JsonIgnore, SerializeField] private string baseModel;


        /// <summary>
        /// Immutable. The name of the TunedModel to use as the starting point for training the new model. Example: tunedModels/my-tuned-model
        /// </summary>
        [JsonProperty("tunedModel")]
        public string TunedModel
        {
            get => tunedModel;
            set => tunedModel = value;
        }

        /// <summary>
        /// Output only. The name of the base Model this TunedModel was tuned from. Example: models/text-bison-001
        /// </summary>
        [JsonProperty("baseModel")]
        public string BaseModel
        {
            get => baseModel;
            set => baseModel = value;
        }
    }
}