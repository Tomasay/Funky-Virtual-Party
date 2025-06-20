using System.Linq;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class Attachment
    {
        public static implicit operator Attachment(string fileId) => new(fileId);
        public static implicit operator Attachment(ToolCall[] tools) => new(tools);

        [JsonProperty("file_id")] public string FileId { get; }

        /// <summary>
        /// This is only for 'code_interpreter' and 'file_search'.
        /// This is not for 'function'.
        /// </summary>
        [JsonProperty("tools")] public ToolCall[] Tools { get; set; }

        [JsonIgnore] public bool IsFileId { get; }
        [JsonIgnore] public bool IsTools => !IsFileId;


        public Attachment(string fileId, ToolType targetTool = ToolType.None)
        {
            FileId = fileId;
            IsFileId = true;
            InitializeTools(targetTool);
        }

        public Attachment(params ToolCall[] tools)
        {
            Tools = tools;
            IsFileId = false;
        }

        private void InitializeTools(ToolType targetTool)
        {
            if (targetTool == ToolType.CodeInterpreter)
            {
                Tools = new[] { new CodeInterpreterCall() };
            }
            else if (targetTool == ToolType.FileSearch)
            {
                Tools = new[] { new FileSearchCall() };
            }
        }

        // equal check
        public override bool Equals(object obj)
        {
            if (obj is not Attachment other) return false;
            if (IsFileId && other.IsFileId) return FileId == other.FileId;
            if (IsTools && other.IsTools) return Tools.SequenceEqual(other.Tools);
            return false;
        }

        public override int GetHashCode()
        {
            return IsFileId ? FileId.GetHashCode() : Tools.GetHashCode();
        }

        public override string ToString()
        {
            return IsFileId ? FileId : Tools.ToString();
        }
    }
}