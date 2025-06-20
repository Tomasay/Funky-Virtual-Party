using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class TuningTask
    {
        [JsonProperty("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonProperty("complete_time")]
        public DateTime? CompleteTime { get; set; }

        [JsonProperty("snapshots")]
        public List<TuningSnapshot> Snapshots { get; set; }

        [JsonProperty("hyperparameters")]
        public Hyperparameters Hyperparameters { get; set; }

        [JsonProperty("trainingData")]
        public Dataset TrainingData { get; set; }
    }


    /// <summary>
    /// Dataset for training or validation.
    /// </summary>
    public class Dataset : ICollection<TuningExample>
    {
        /// <summary>
        /// Optional. Inline examples.
        /// </summary>
        [JsonProperty("examples")]
        public TuningExamples Examples { get; set; }

        [JsonIgnore] public int Count => Examples?.Examples?.Count ?? 0;
        [JsonIgnore] public bool IsReadOnly => false;

        private void EnsureInstances()
        {
            Examples ??= new TuningExamples();
            Examples.Examples ??= new List<TuningExample>();
        }

        public void Add(TuningExample item)
        {
            EnsureInstances();
            Examples.Examples.Add(item);
        }

        public void AddRange(IEnumerable<TuningExample> items)
        {
            EnsureInstances();
            Examples.Examples.AddRange(items);
        }

        public void Clear()
        {
            Examples?.Examples?.Clear();
        }

        public bool Contains(TuningExample item)
        {
            return Examples?.Examples?.Contains(item) ?? false;
        }

        public void CopyTo(TuningExample[] array, int arrayIndex)
        {
            Examples?.Examples?.CopyTo(array, arrayIndex);
        }

        public bool Remove(TuningExample item)
        {
            return Examples?.Examples?.Remove(item) ?? false;
        }

        public IEnumerator<TuningExample> GetEnumerator()
        {
            return Examples?.Examples?.GetEnumerator() ?? new List<TuningExample>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    /// <summary>
    /// A set of tuning examples. Can be training or validation data.
    /// </summary>
    public class TuningExamples
    {
        /// <summary>
        /// Required. The examples. Example input can be for text or discuss, but all examples in a set must be of the same type.
        /// </summary>
        [JsonProperty("examples")]
        public List<TuningExample> Examples { get; set; }
    }

    /// <summary>
    /// A single example for tuning.
    /// </summary>
    public class TuningExample
    {
        /// <summary>
        /// Optional. Text model input.
        /// </summary>
        [JsonProperty("textInput")]
        public string TextInput { get; set; }

        /// <summary>
        /// Required. The expected model output.
        /// </summary>
        [JsonProperty("output")]
        public string Output { get; set; }

        public TuningExample()
        {
        }

        public TuningExample(string textInput, string output)
        {
            TextInput = textInput;
            Output = output;
        }
    }
}