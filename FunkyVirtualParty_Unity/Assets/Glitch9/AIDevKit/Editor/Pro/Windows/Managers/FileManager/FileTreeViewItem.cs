using Glitch9.Editor.IMGUI;
using Glitch9.IO.Files;
using UnityEngine;


namespace Glitch9.AIDevKit.Editor.Files
{
    public class FileTreeViewItem : ExtendedTreeViewItem
        <
            FileTreeViewItem,
            ApiFile,
            TreeViewItemFilter
        >
    {
        internal Api Api => Data?.Api ?? Api.None;
        internal string FileName => GetFileName();
        internal string FileId => Data?.Id;
        internal string FormattedSize => _formattedSize ??= FileManager.ToBytesString(ByteSize);
        internal int ByteSize => Data?.ByteSize ?? 0;
        internal string Uri => Data?.Uri;
        internal MIMEType MimeType => Data?.MimeType ?? MIMEType.Unknown;
        internal UnixTime CreatedAt => Data?.CreatedAt ?? UnixTime.MinValue;
        internal UnixTime ExpiresAt => Data?.ExpiresAt ?? UnixTime.MinValue;
        internal Texture2D FileIcon => GetFileIcon();

        private string _fileName;
        private string _formattedSize;
        private Texture2D _fileIcon;

        public FileTreeViewItem(int id, int depth, string displayName, ApiFile data) : base(id, depth, displayName, data) { }

        public override int CompareTo(FileTreeViewItem anotherItem, int columnIndex, bool ascending)
        {
            return columnIndex switch
            {
                FileManagerWindow.ColumnIndex.API => CompareByInt(ascending, anotherItem, i => (int)i.Api),
                FileManagerWindow.ColumnIndex.FILE_NAME => CompareByString(ascending, anotherItem, i => i.FileName),
                FileManagerWindow.ColumnIndex.BYTE_SIZE => CompareByInt(ascending, anotherItem, i => i.ByteSize),
                FileManagerWindow.ColumnIndex.CREATED_AT => CompareByUnixTime(ascending, anotherItem, i => i.Data.CreatedAt),
                FileManagerWindow.ColumnIndex.EXPIRES_AT => CompareByUnixTime(ascending, anotherItem, i => i.Data.ExpiresAt),
                _ => 0
            };
        }

        public override bool Search(string searchString)
        {
            if (Data == null) return false;
            if (string.IsNullOrEmpty(searchString)) return true;
            if (Data.Name != null && Data.Name.Contains(searchString)) return true;
            return false;
        }

        private string GetFileName()
        {
            if (_fileName == null)
            {
                _fileName = Data?.Name ?? string.Empty;

                if (string.IsNullOrEmpty(_fileName))
                {
                    _fileName = Data?.Id ?? string.Empty;

                    if (string.IsNullOrEmpty(_fileName))
                    {
                        _fileName = "Unknown File";
                    }
                }
            }

            return _fileName;
        }

        private Texture2D GetFileIcon()
        {
            if (_fileIcon == null)
            {
                if (MimeType.IsImage())
                {
                    _fileIcon = AIDevKitIcons.Image;
                }
                else if (MimeType.IsVideo())
                {
                    _fileIcon = AIDevKitIcons.Video;
                }
                else if (MimeType.IsAudio())
                {
                    _fileIcon = AIDevKitIcons.Audio;
                }
                else
                {
                    _fileIcon = AIDevKitIcons.File;
                }
            }

            return _fileIcon;
        }

    }
}