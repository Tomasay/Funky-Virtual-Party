using Glitch9.IO.Files;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Glitch9.AIDevKit
{
    [Serializable]
    public class PromptRecord : IData
    {
        // log info -------------------------------------------------------------
        [SerializeField] private string id = Guid.NewGuid().ToString();
        [SerializeField, FormerlySerializedAs("taskType")] private int endpointType;
        [SerializeField] private string sender;
        [SerializeField] private UnixTime createdAt = UnixTime.Now;
        [SerializeField] private bool archived = false;

        // request details 1: model info ------------------------------------------------------------
        [SerializeField] private Api api;
        [SerializeField] private string modelId;
        [SerializeField] private string modelName;

        // request details 2: others ----------------------------------------------------------------
        [SerializeField, FormerlySerializedAs("metadata")] private Metadata requestOptions;
        [SerializeField] private Usage usage;
        [SerializeField] private Currency price;

        // input/output contents -------------------------------------------------
        [SerializeField] private int n;
        [SerializeField, FormerlySerializedAs("inputText")] private string prompt;
        [SerializeField] private string formattedPrompt;
        [SerializeField] private MIMEType outputMimeType = MIMEType.Json; // default to JSON
        [SerializeField] private List<string> outputTexts;
        [SerializeReference] private List<IFile> inputFiles;
        [SerializeReference] private List<IFile> outputFiles;


        public string Id => id;
        public string Name => modelName;
        public int TaskType => endpointType;
        public string Sender => sender;
        public UnixTime CreatedAt => createdAt;
        public Api Api => api;
        public string ModelId => modelId;
        public string ModelName => modelName;

        public List<IFile> InputFiles => inputFiles;
        public List<IFile> OutputFiles => outputFiles;

        public int N => n;
        public string InputText => prompt;
        public string Prompt => prompt;
        public string FormattedPrompt => formattedPrompt;
        public string OutputText => outputTexts?.FirstOrDefault();
        public List<string> OutputTexts => outputTexts;
        public MIMEType OutputMimeType => outputMimeType;

        public Metadata RequestOptions => requestOptions;
        public Usage Usage { get => usage; set => usage = value; }
        public Currency Price { get => price; set => price = value; }
        public bool IsArchived => archived;

        public PromptRecord() { }
        internal PromptRecord InitializeCommon(int endpointType, Model model, string sender, Usage usage, int n)
        {
            this.endpointType = endpointType;
            this.sender = sender;
            this.usage = usage;

            if (model != null)
            {
                api = model.Api;
                modelId = model.Id;
                modelName = model.Name;
                if (usage != null) price = model.EstimatePrice(usage);
            }

            this.n = n;
            inputFiles = new();
            outputFiles = new();

            return this;
        }

        internal void Archive()
        {
            archived = true;
        }

        #region Getters

        public string GetMetadata(string key)
        {
            if (requestOptions == null) return null;
            return requestOptions.TryGetValue(key, out string metadata1) ? metadata1 : string.Empty;
        }

        internal string FirstOutputText() => outputTexts?.FirstOrDefault() ?? string.Empty;
        internal File<AudioClip> FirstInputAudio() => FirstInputFile<File<AudioClip>>();
        internal File<AudioClip> FirstOutputAudio() => FirstOutputFile<File<AudioClip>>();

        internal List<T> GetInputFiles<T>() where T : IFile => FileUtil.GetFiles<T>(inputFiles);
        internal List<T> GetOutputFiles<T>() where T : IFile => FileUtil.GetFiles<T>(outputFiles);
        internal void RemoveOutputFileAt<T>(int index) where T : IFile
        {
            if (outputFiles == null || index < 0 || index >= outputFiles.Count) return;
            if (outputFiles[index] is T file)
            {
                outputFiles.RemoveAt(index);
                file.Delete();
            }
        }

        private T FirstInputFile<T>() where T : IFile => FileUtil.GetFirstFile<T>(inputFiles);
        private T FirstOutputFile<T>() where T : IFile => FileUtil.GetFirstFile<T>(outputFiles);

        #endregion Getters

        #region Setters  

        internal PromptRecord SetPromptText(Prompt prompt)
        {
            if (prompt == null) return this;
            if (!string.IsNullOrWhiteSpace(prompt)) this.prompt = prompt.text;
            if (!string.IsNullOrWhiteSpace(formattedPrompt)) formattedPrompt = prompt.formattedText ?? prompt.text;
            return this;
        }

        internal PromptRecord SetOutputTexts(params string[] texts)
        {
            outputTexts ??= new List<string>();
            outputTexts.AddRangeIfNotEmpty(texts);
            return this;
        }

        internal PromptRecord SetInputFiles(params IFile[] files)
        {
            inputFiles ??= new List<IFile>();
            inputFiles.AddRangeIfNotEmpty(files);
            return this;
        }

        internal PromptRecord SetOutputFiles(params IFile[] files)
        {
            outputFiles ??= new List<IFile>();
            outputFiles.AddRangeIfNotEmpty(files);
            return this;
        }

        internal PromptRecord AddRequestOption(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Prompt Record Metadata Key cannot be null or empty");

            requestOptions ??= new();
            requestOptions.AddOrUpdate(key, value);
            return this;
        }

        internal PromptRecord AddRequestOption(string key, int? value)
            => value.HasValue ? AddRequestOption(key, value.Value.ToString(CultureInfo.InvariantCulture)) : this;

        internal PromptRecord AddRequestOption(string key, float? value)
            => value.HasValue ? AddRequestOption(key, value.Value.ToString(CultureInfo.InvariantCulture)) : this;

        internal PromptRecord AddRequestOption<TEnum>(TEnum value) where TEnum : Enum
            => AddRequestOption(value.GetType().Name.ToPascalCase(), value);

        internal PromptRecord AddRequestOption<TEnum>(string key, TEnum value) where TEnum : Enum
            => AddRequestOption(key, value?.GetInspectorName());

        internal PromptRecord SetRequestOptions(Dictionary<string, object> map)
        {
            if (map.IsNullOrEmpty()) return this;

            foreach (var (key, value) in map)
            {
                switch (value)
                {
                    case string s: AddRequestOption(key, s); break;
                    case int i: AddRequestOption(key, i); break;
                    case float f: AddRequestOption(key, f); break;
                    case Enum e: AddRequestOption(key, e); break;
                }
            }

            return this;
        }

        internal PromptRecord SetOutputMimeType(MIMEType mimeType)
        {
            if (mimeType == MIMEType.Unknown)
            {
                //throw new ArgumentException("Output MIME type cannot be Unknown.");
                Debug.LogWarning("Output MIME type is set to Unknown. Defaulting to JSON.");
                mimeType = MIMEType.Json; // Default to JSON if unknown
            }
            outputMimeType = mimeType;
            return this;
        }

        #endregion Setters

        #region Utility Methods

        internal PromptRecord SaveToDatabase()
        {
            PromptHistory.Add(this);
            return this;
        }

        #endregion Utility Methods

        #region IEquatable Implementation

        public bool Equals(PromptRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PromptRecord)obj);
        }

        public override int GetHashCode()
        {
            return id != null ? id.GetHashCode() : 0;
        }

        public static bool operator ==(PromptRecord left, PromptRecord right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null)) return false;
            if (ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(PromptRecord left, PromptRecord right)
        {
            return !(left == right);
        }

        #endregion IEquatable Implementation
    }
}