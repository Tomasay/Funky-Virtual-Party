using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Glitch9.IO.Files
{
    /// <summary>
    /// Unity-Serializable class for <see cref="System.IO.FileInfo"/>.
    /// This class is used to serialize file information in Unity.
    /// </summary>
    [Serializable]
    public class SerializableFileInfo : ISerializable
    {
        [SerializeField, FormerlySerializedAs("path"), JsonProperty] protected string fullPath;
        [SerializeField] protected UnixTime creationTime;
        [SerializeField] protected UnixTime creationTimeUtc;
        [SerializeField] protected UnixTime lastWriteTime;
        [SerializeField] protected UnixTime lastWriteTimeUtc;
        [SerializeField] protected UnixTime lastAccessTime;
        [SerializeField] protected UnixTime lastAccessTimeUtc;
        [SerializeField] protected FileAttributes attributes;
        [SerializeField, JsonProperty] protected string id;
        [SerializeField, JsonProperty] protected string url; // Added 2025.04.23 to support URL loading in case of path is null or empty.  
        [SerializeField, JsonProperty] protected MIMEType mimeType;
        [SerializeField, JsonProperty] protected string note; // Added 2025.04.23 to support note for file info.
        [SerializeField, JsonProperty] protected float mediaLength;

        [JsonIgnore] public string Id => id;
        [JsonIgnore] public string FullPath => fullPath;

        /// <summary>
        /// URL of the file.
        /// This is used when the file is loaded from a URL.
        /// The system will try to load the file from this URL 
        /// if the FullPath is null or empty, 
        /// or if the file is not found.
        /// </summary>
        [JsonIgnore] public string Url => url;
        [JsonIgnore]
        public MIMEType MimeType
        {
            get
            {
                if (mimeType == MIMEType.Unknown && !string.IsNullOrEmpty(fullPath))
                    mimeType = MIMETypeUtil.ParseFromPath(fullPath);
                return mimeType;
            }
        }
        [JsonIgnore] public string Note { get => note; set => note = value; }
        [JsonIgnore] public float MediaLength => mediaLength;

        [JsonIgnore] public UnixTime CreationTime => creationTime;
        [JsonIgnore] public UnixTime CreationTimeUtc => creationTimeUtc;
        [JsonIgnore] public UnixTime LastWriteTime => lastWriteTime;
        [JsonIgnore] public UnixTime LastWriteTimeUtc => lastWriteTimeUtc;
        [JsonIgnore] public UnixTime LastAccessTime => lastAccessTime;
        [JsonIgnore] public UnixTime LastAccessTimeUtc => lastAccessTimeUtc;
        [JsonIgnore] public FileAttributes Attributes => attributes;

        public SerializableFileInfo(string fullPath, string url = null, MIMEType? mimeType = null, string note = null)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                SetFileInfo(null, url, mimeType, note);
            }
            else
            {
                SetFileInfo(new FileInfo(fullPath), url, mimeType, note);
            }
        }
        public SerializableFileInfo(FileInfo fileInfo, string url = null, MIMEType? mimeType = null, string note = null) => SetFileInfo(fileInfo, url, mimeType, note);
        protected SerializableFileInfo() { }
        protected SerializableFileInfo(SerializationInfo info, StreamingContext context)
        {
            fullPath = info.GetString(nameof(FullPath));
            lastWriteTime = info.GetDateTime(nameof(LastWriteTime));
            lastAccessTimeUtc = info.GetDateTime(nameof(LastAccessTimeUtc));
            lastAccessTime = info.GetDateTime(nameof(LastAccessTime));
            creationTime = info.GetDateTime(nameof(CreationTime));
            lastWriteTimeUtc = info.GetDateTime(nameof(LastWriteTimeUtc));
            attributes = (FileAttributes)info.GetValue(nameof(Attributes), typeof(FileAttributes));
            creationTimeUtc = info.GetDateTime(nameof(CreationTimeUtc));
        }

        private FileInfo _fileInfo;
        [JsonIgnore] public FileInfo FileInfo => _fileInfo ??= new(FullPath);

        [JsonIgnore] public string FullName => FileInfo.FullName;
        [JsonIgnore] public string Extension => FileInfo.Extension;
        [JsonIgnore] public bool Exists => FileInfo.Exists;
        [JsonIgnore] public string Name => FileInfo.Name;
        [JsonIgnore] public string DirectoryName => FileInfo.DirectoryName;
        [JsonIgnore] public DirectoryInfo Directory => FileInfo.Directory;
        [JsonIgnore] public bool IsReadOnly => FileInfo.IsReadOnly;
        [JsonIgnore] public long Length => FileInfo.Length;
        public void Delete() => FileInfo.Delete();
        public void Refresh() => FileInfo.Refresh();
        public override string ToString() => FileInfo.ToString();
        public StreamWriter AppendText() => FileInfo.AppendText();
        public FileInfo CopyTo(string destFileName) => FileInfo.CopyTo(destFileName);
        public FileInfo CopyTo(string destFileName, bool overwrite) => FileInfo.CopyTo(destFileName, overwrite);
        public FileStream Create() => FileInfo.Create();
        public StreamWriter CreateText() => FileInfo.CreateText();
        public void Decrypt() => FileInfo.Decrypt();
        public void Encrypt() => FileInfo.Encrypt();
        public void MoveTo(string destFileName) => FileInfo.MoveTo(destFileName);
        public FileStream Open(FileMode mode, FileAccess access, FileShare share) => FileInfo.Open(mode, access, share);
        public FileStream Open(FileMode mode, FileAccess access) => FileInfo.Open(mode, access);
        public FileStream Open(FileMode mode) => FileInfo.Open(mode);
        public FileStream OpenRead() => FileInfo.OpenRead();
        public StreamReader OpenText() => FileInfo.OpenText();
        public FileStream OpenWrite() => FileInfo.OpenWrite();
        public FileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) => FileInfo.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
        public FileInfo Replace(string destinationFileName, string destinationBackupFileName) => FileInfo.Replace(destinationFileName, destinationBackupFileName);

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FullPath), FullPath);
            info.AddValue(nameof(CreationTime), CreationTime);
            info.AddValue(nameof(CreationTimeUtc), CreationTimeUtc);
            info.AddValue(nameof(LastWriteTime), LastWriteTime);
            info.AddValue(nameof(LastWriteTimeUtc), LastWriteTimeUtc);
            info.AddValue(nameof(LastAccessTime), LastAccessTime);
            info.AddValue(nameof(LastAccessTimeUtc), LastAccessTimeUtc);
            info.AddValue(nameof(Attributes), Attributes);
        }

        private void SetFileInfo(FileInfo fileInfo, string url, MIMEType? mimeType, string note)
        {
            this.url = url;
            this.note = note;

            if (fileInfo == null) return;

            _fileInfo = fileInfo;
            if (!_fileInfo.Exists) throw new FileNotFoundException("File not found", fileInfo.FullName);
            fullPath = fileInfo.FullName;
            creationTime = fileInfo.CreationTime;
            creationTimeUtc = fileInfo.CreationTimeUtc;
            lastWriteTime = fileInfo.LastWriteTime;
            lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
            lastAccessTime = fileInfo.LastAccessTime;
            lastAccessTimeUtc = fileInfo.LastAccessTimeUtc;
            attributes = fileInfo.Attributes;

            id = fileInfo.Name;

            if (mimeType.HasValue)
            {
                this.mimeType = mimeType.Value;
            }
            else
            {
                this.mimeType = MIMEType.Unknown;
                if (!string.IsNullOrEmpty(fullPath))
                    this.mimeType = MIMETypeUtil.ParseFromPath(fullPath);
            }
        }
    }
}