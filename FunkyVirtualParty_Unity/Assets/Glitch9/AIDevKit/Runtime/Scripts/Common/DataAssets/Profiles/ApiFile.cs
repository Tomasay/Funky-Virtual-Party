using System;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Serialization;

namespace Glitch9.AIDevKit
{
    public interface IApiFile : IData
    {
        Api Api { get; }
        int ByteSize { get; }
        UnixTime CreatedAt { get; }
        UnixTime ExpiresAt { get; }
        MIMEType MimeType { get; }
        string Uri => null;
        Metadata BuildMetadata();
    }

    [Serializable]
    public class ApiFile : IApiFile
    {
        internal static ApiFile AddToLibrary(IApiFile newFile)
        {
            if (newFile == null) throw new ArgumentNullException(nameof(newFile));

            ApiFile newFileToAdd = new()
            {
                api = newFile.Api,
                id = newFile.Id,
                name = newFile.Name,
                uri = newFile.Uri,
                byteSize = newFile.ByteSize,
                createdAt = newFile.CreatedAt,
                expiresAt = newFile.ExpiresAt,
                mimeType = newFile.MimeType,
                metadata = newFile.BuildMetadata()
            };

            FileLibrary.Add(newFileToAdd);

            return newFileToAdd;
        }


        [SerializeField] private Api api;
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string uri;
        [SerializeField, FormerlySerializedAs("bytes")] private int byteSize;
        [SerializeField, FormerlySerializedAs("created")] private UnixTime createdAt;
        [SerializeField] private UnixTime expiresAt;
        [SerializeField] private MIMEType mimeType;
        [SerializeField] private Metadata metadata;

        public Api Api => api;
        public string Id => id;
        public string Name => name;
        public int ByteSize => byteSize;
        public UnixTime CreatedAt => createdAt;
        public UnixTime ExpiresAt => expiresAt;
        public MIMEType MimeType => mimeType;
        public Metadata Metadata => metadata;
        public string Uri => uri;
        public Metadata BuildMetadata() => metadata;

        public bool Equals(ApiFile other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Name == other.Name && ByteSize == other.ByteSize && Nullable.Equals(CreatedAt, other.CreatedAt);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ApiFile)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, ByteSize, CreatedAt);
        }
    }
}