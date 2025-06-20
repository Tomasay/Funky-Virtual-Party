using System;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    public interface IGeneratedResult
    {
        int Count { get; }
        Usage Usage { get; }
    }

    public interface IGeneratedFile : IGeneratedResult
    {
        void SetNote(string note);
    }


    /// <summary>
    /// You will never know if the AI generated result is a single or multiple values.
    /// So this class is used to represent both cases: a value or an array of values.
    /// </summary> 
    public abstract class GeneratedResult<T> : RESTResponse, IGeneratedResult
    {
        public T[] Values => values;
        public int Count => values.Length;
        public Usage Usage { get => usage; set => usage = value; }
        public bool IsEmpty => values == null || values.Length == 0;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= values.Length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range. Count: {values.Length}");
                return values[index];
            }
        }

        protected readonly T[] values;
        protected Usage usage;

        protected GeneratedResult(T value, Usage usage)
        {
            if (value == null) Debug.LogWarning("The generation request returned no content. Please check the request and try again.");
            values = new[] { value };
            this.usage = usage;
        }

        protected GeneratedResult(T[] value, Usage usage)
        {
            if (value.IsNullOrEmpty()) Debug.LogWarning("The generation request returned no content. Please check the request and try again.");
            this.values = value;
            this.usage = usage;
        }
    }

    /// <summary>
    /// Base class for generated assets: images, audios, and videos.
    /// These assets are 'files' that can be downloaded and used in the application.
    /// </summary>
    public abstract class GeneratedFile<TAsset, TFile> : GeneratedResult<TAsset>, IGeneratedFile
        where TFile : RawFile
    {
        private readonly string[] paths;
        public string[] Paths => paths;

        public string this[string path]
        {
            get
            {
                int index = Array.IndexOf(paths, path);
                if (index < 0 || index >= values.Length)
                    throw new ArgumentException($"Path '{path}' not found in generated {typeof(TAsset).Name.ToLower()}s.");
                return paths[index];
            }
        }
        public string fileNote;

        protected GeneratedFile(TAsset asset, string path, Usage usage = null) : base(asset, usage) => paths = new[] { path };
        protected GeneratedFile(TAsset[] asset, string[] paths, Usage usage = null) : base(asset, usage) => this.paths = paths;
        public void SetNote(string note) => fileNote = note;
        public abstract TFile[] ToFiles();
    }

    /// <summary>
    /// Base class for generated contents via <see cref="CompletionRequest"/> or <see cref="ChatCompletionRequest"/>.
    /// This class contains <see cref="ToolCall[]"/>s that the AI model wants to call.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CompletionResult<T> : GeneratedResult<T>
    {
        public readonly ToolCall[] toolCalls;
        protected CompletionResult(T value, Usage usage) : base(value, usage) { }
        protected CompletionResult(T[] values, Usage usage) : base(values, usage) { }
        protected CompletionResult(T value, ToolCall[] toolCalls, Usage usage = null) : base(value, usage) => this.toolCalls = toolCalls;
        protected CompletionResult(T[] values, ToolCall[] toolCalls, Usage usage = null) : base(values, usage) => this.toolCalls = toolCalls;
    }
}
