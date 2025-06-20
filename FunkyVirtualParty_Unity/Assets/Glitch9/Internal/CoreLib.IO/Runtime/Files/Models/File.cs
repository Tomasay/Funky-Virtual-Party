using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Glitch9.IO.Files
{
    public enum FileState
    {
        Unknown,
        Loading,
        Loaded,
        Error,
    }

    public interface IFile : IData
    {
        string FileType { get; }
        string FullPath { get; }
        string Extension { get; }
        string Url { get; }
        string Note { get; set; }
        MIMEType MimeType { get; }
        string LastError { get; }
        FileState State { get; }
        bool HasData { get; }
        float MediaLength { get; }
        bool IsLoading => State == FileState.Loading;
        bool IsLoaded => State == FileState.Loaded;
        bool IsError => State == FileState.Error;
        UniTask<byte[]> ReadAllBytesAsync();
        FileInfo CopyTo(string destinationPath);
        FileInfo CopyTo(string destinationPath, bool overwrite);
        void Delete();
    }

    [Serializable]
    public class RawFile : SerializableFileInfo, IFile
    {
        protected const int kDelayBeforeCancelLoad = 500;
        [JsonProperty("fileType")] public virtual string FileType => "Raw";
        [JsonIgnore] public string LastError { get; protected set; }
        [JsonIgnore] public FileState State { get; protected set; }

        [JsonIgnore] public bool IsLoading => State == FileState.Loading;
        [JsonIgnore] public bool IsLoaded => State == FileState.Loaded;
        [JsonIgnore] public bool IsError => State == FileState.Error;
        [JsonIgnore] public virtual bool HasData => data != null && data.Length > 0;

        protected byte[] data;

        [JsonConstructor] public RawFile() { }
        public RawFile(FileInfo fileInfo, string url = null, MIMEType? mimeType = null, string note = null) : base(fileInfo.FullName, url, mimeType, note) { }
        public RawFile(string filePath, string url = null, MIMEType? mimeType = null, string note = null) : base(filePath.ToFullPath(), url, mimeType, note) { }
        public RawFile(byte[] data, string filePath, string url = null, MIMEType? mimeType = null, string note = null) : base(filePath.ToFullPath(), url, mimeType, note) => this.data = data;


        public async virtual UniTask<byte[]> ReadAllBytesAsync()
        {
            if (data != null) return data;

            if (State == FileState.Loading)
            {
                Debug.LogWarning($"File is already loading. Path: {FullPath}");
                return null;
            }

            if (string.IsNullOrWhiteSpace(FullPath) && string.IsNullOrWhiteSpace(Url))
            {
                LastError = "File path and URL are both null or empty.";
                State = FileState.Error;
                Debug.LogError(LastError);
                return null;
            }

            State = FileState.Loading;

            data = await FileUtil.ReadAllBytesAsync(FullPath);

            if (data == null)
            {
                LastError = $"Failed to load file. Path: {FullPath}";
                State = FileState.Error;
                Debug.LogError(LastError);
            }

            State = FileState.Loaded;
            return data;
        }

        public virtual byte[] ReadAllBytes() => data;

        public string EncodeToBase64()
        {
            byte[] data = ReadAllBytes();
            if (data == null) return null;
            return Convert.ToBase64String(data);
        }

        public async UniTask<string> EncodeToBase64Async()
        {
            byte[] data = await ReadAllBytesAsync();
            if (data == null) return null;
            return Convert.ToBase64String(data);
        }

        public async UniTask CancelLoadAsync()
        {
            await UniTask.Delay(kDelayBeforeCancelLoad);
            State = FileState.Error;
            LastError = "File loading was cancelled.";
        }
    }

    [Serializable]
    public class File<T> : RawFile where T : UnityEngine.Object
    {
        public override string FileType => typeof(T).Name;
        [JsonIgnore] public override bool HasData => _asset != null;
        [JsonIgnore] public T Asset => _asset; // Loaded asset 
        private T _asset;

        [JsonConstructor] public File() { }
        public File(FileInfo fileInfo, string url = null, MIMEType? mimeType = null, string note = null) : base(fileInfo.FullName, url, mimeType, note) { }
        public File(string filePath, string url = null, MIMEType? mimeType = null, string note = null) : base(filePath, url, mimeType, note) { }
        public File(T asset, FileInfo fileInfo, string url = null, MIMEType? mimeType = null, string note = null) : base(fileInfo.FullName, url, mimeType, note) => _asset = asset;
        public File(T asset, string filePath, string url = null, MIMEType? mimeType = null, string note = null) : this(filePath, url, mimeType, note) => _asset = asset;
        public File(T asset)
        {
            _asset = asset;

#if UNITY_EDITOR
            if (asset != null)
            {
                fullPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
                url = FullPath.ToFullPath();
            }
#endif
        }

        public async override UniTask<byte[]> ReadAllBytesAsync()
        {
            if (data != null) return data;

            if (_asset == null) _asset = await LoadAssetAsync();
            if (_asset == null) return null;

            return ReadAllBytes();
        }

        public override byte[] ReadAllBytes()
        {
            if (data != null) return data;

            if (_asset == null) return null;

            if (_asset is Texture2D texture)
            {
                data = texture.EncodeToPNG();
            }
            else if (_asset is AudioClip audioClip)
            {
                data = audioClip.EncodeToWAV();
            }
            else if (_asset is VideoClip)
            {
                Debug.LogWarning("VideoClip cannot be converted to binary data.");
            }
            else
            {
                Debug.LogWarning($"Unsupported asset type: {typeof(T)}");
            }

            return data;
        }

        public void EnsureAssetLoaded()
        {
            if (_asset == null && !IsLoading)
            {
                Debug.LogWarning($"Asset is not loaded. Attempting to load: {FullPath}");
                LoadAssetAsync().Forget();
            }
        }

        public async UniTask<T> LoadAssetAsync(bool forceReload = false, Action<T> onResult = null)
        {
            if (_asset != null && !forceReload)
            {
                onResult?.Invoke(_asset);
                return _asset;
            }

            if (forceReload) Debug.Log($"Reloading file at <color=yellow>{FullPath}</color>");

            if (string.IsNullOrWhiteSpace(FullPath))
            {
                LastError = "File path is null or empty.";
                State = FileState.Error;
                onResult?.Invoke(_asset);
                return _asset;
            }

            if (!forceReload)
            {
                if (State == FileState.Loading)
                {
                    // wait until the file is loaded
                    await UniTask.WaitUntil(() => State != FileState.Loading);
                    if (_asset == null)
                    {
                        Debug.LogWarning($"File loading failed. Path: {FullPath}");
                        onResult?.Invoke(_asset);
                        return _asset;
                    }

                    //Debug.LogWarning($"File is already loading. Path: {FullPath}");
                    onResult?.Invoke(_asset);
                    return _asset;
                }

                if (State == FileState.Loaded)
                {
                    Debug.Log($"File already loaded: <color=green>{FullPath}</color>");
                    onResult?.Invoke(_asset);
                    return _asset;
                }

                if (State == FileState.Error)
                {
                    Debug.LogWarning($"File loading failed previously. Path: {FullPath}. Attempting to reload.");
                    onResult?.Invoke(_asset);
                    return _asset;
                }
            }

            State = FileState.Loading;
            LastError = null;

            try
            {
                // check if the file exists
                if (!File.Exists(FullPath)) throw new FileNotFoundException($"File not found at {FullPath}");
                _asset = await LoadAssetFromPathAsyncINTERNAL();
            }
            catch (Exception e)
            {
                LastError = e.Message;
                State = FileState.Error;
                Debug.LogError(e);
            }
            finally
            {
                if (_asset != null)
                {
                    State = FileState.Loaded;
                    //Debug.Log($"File loaded successfully: <color=green>{FullPath}</color>");
                }
                else
                {
                    State = FileState.Error;
                    LastError = $"Failed to load file: {FullPath}";
                    //Debug.LogError($"Failed to load file: <color=red>{FullPath}</color>");
                }

                onResult?.Invoke(_asset);
            }

            //await CancelLoadAsync();
            return _asset;
        }

        private async UniTask<T> LoadAssetFromPathAsyncINTERNAL()
        {
            if (typeof(T) == typeof(Texture2D))
            {
                return await Texture2DLoader.LoadAsync(FullPath, Url) as T;
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                AudioClip audioClip = await AudioClipLoader.LoadAsync(FullPath, Url);
                if (audioClip != null) mediaLength = audioClip.length;
                return audioClip as T;
            }
            else if (typeof(T) == typeof(VideoClip))
            {
                // VideoClip cannot be loaded from a file path
                Debug.LogWarning("VideoClip cannot be loaded from a file path.");
                return null;
            }
            else
            {
                Debug.LogWarning($"Unsupported asset type: {typeof(T)}");
                return null;
            }
        }

        public async UniTask WriteFileAsync()
        {
            if (_asset == null)
            {
                Debug.LogWarning("Asset is null. Cannot write file.");
                return;
            }

            if (string.IsNullOrWhiteSpace(FullPath))
            {
                Debug.LogWarning("File path is null or empty. Cannot write file.");
                return;
            }

            try
            {
                if (_asset is Texture2D texture)
                {
                    await ImageFileWriter.WriteFileAsync(texture, FullPath);
                }
                else if (_asset is AudioClip audioClip)
                {
                    await AudioFileWriter.WriteFileAsync(audioClip, FullPath);
                }
                else
                {
                    Debug.LogWarning($"Unsupported asset type for writing: {typeof(T)}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write file: {e.Message}");
            }
        }
    }

    public class IFileJsonConverter : JsonConverter<IFile>
    {
        public override IFile ReadJson(JsonReader reader, Type objectType, IFile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //Debug.LogError("Reading IFile from JSON. (Debugging purposes only)");

            try
            {
                JObject jObj = JObject.Load(reader);
                string fileType = jObj["fileType"]?.ToString() ?? "Raw";
                string fullPath = jObj["fullPath"]?.ToString();
                string url = jObj["url"]?.ToString();
                string mimeTypeStr = jObj["mimeType"]?.ToString();
                MIMEType mimeType = MIMETypeUtil.ParseApiValue(mimeTypeStr);
                string note = jObj["note"]?.ToString();

                if (fileType == "Raw")
                {
                    return new RawFile(fullPath, url, mimeType, note);
                }
                else if (fileType == "Texture2D")
                {
                    return new File<Texture2D>(fullPath, url, mimeType, note);
                }
                else if (fileType == "AudioClip")
                {
                    return new File<AudioClip>(fullPath, url, mimeType, note);
                }
                else if (fileType == "VideoClip")
                {
                    return new File<VideoClip>(fullPath, url, mimeType, note);
                }
                else
                {
                    Debug.LogWarning($"Unknown file type: {fileType}. Returning RawFile.");
                    return new RawFile(fullPath, url, mimeType, note);
                }
            }
            catch
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, IFile value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jObj = new()
            {
                ["fileType"] = value.FileType,
                ["fullPath"] = value.FullPath,
                ["url"] = value.Url,
                ["mimeType"] = value.MimeType.ToApiValue(),
                ["note"] = value.Note,
            };

            serializer.Serialize(writer, jObj);
        }

        public override bool CanWrite => true;
    }
}