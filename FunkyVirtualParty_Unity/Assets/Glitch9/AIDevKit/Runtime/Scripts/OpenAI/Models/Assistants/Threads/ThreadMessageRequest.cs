using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glitch9.AIDevKit.OpenAI
{
    [JsonConverter(typeof(ThreadMessageRequestConverter))]
    public class ThreadMessageRequest : AIRequest
    {
        /// <summary>
        /// Required. The role of the entity that is creating the message. 
        /// Currently only User is supported.
        /// </summary>
        [JsonProperty("role")] public ChatRole Role => ChatRole.User;

        /// <summary>
        /// Required. The content of the message.
        /// </summary>
        [JsonProperty("content")] public List<ContentPartWrapper> ContentParts { get; set; }

        /// <summary>
        /// A list of files attached to the message, and the tools they should be added to.
        /// </summary>
        [JsonProperty("attachments")] public List<Attachment> Attachments { get; set; }

        [JsonIgnore] internal List<IFile> UploadFiles { get; set; }

        public ThreadMessageRequest() { }
        public ThreadMessageRequest(
            string prompt,
            IEnumerable<ToolCall> tools = null,
            IEnumerable<IFile> uploadFiles = null,
            IEnumerable<string> imageUrls = null,
            IEnumerable<string> fileIds = null,
            IEnumerable<string> imageFileIds = null)
        {
            SetPrompt(prompt);
            SetTools(tools?.ToArray());
            SetUploadFiles(uploadFiles?.ToArray());
            SetImageUrls(imageUrls);
            SetFileIds(fileIds);
            SetImageFileIds(imageFileIds);
        }

        public ThreadMessageRequest SetPrompt(string prompt)
        {
            AddContentPart(new TextContentPart(prompt));
            return this;
        }

        public ThreadMessageRequest SetImageFileIds(IEnumerable<string> imageFileIds)
        {
            if (imageFileIds.IsNullOrEmpty()) return this;
            ContentParts ??= new();
            foreach (string imageFileId in imageFileIds)
            {
                if (imageFileId == null) continue;
                AddContentPart(ImageContentPart.FromId(imageFileId));
            }
            return this;
        }

        public ThreadMessageRequest SetImageUrls(IEnumerable<string> imageUrls)
        {
            if (imageUrls.IsNullOrEmpty()) return this;
            ContentParts ??= new();
            foreach (string imageUrl in imageUrls)
            {
                if (imageUrl == null) continue;
                AddContentPart(ImageContentPart.FromUrl(imageUrl));
            }
            return this;
        }

        public ThreadMessageRequest SetFileIds(IEnumerable<string> attachmentFileIds)
        {
            if (attachmentFileIds.IsNullOrEmpty()) return this;
            Attachments ??= new();
            Attachments.AddRange(attachmentFileIds.Select(attachmentFileId => new Attachment(attachmentFileId)));
            return this;
        }

        public ThreadMessageRequest SetTools(params ToolCall[] tools)
        {
            if (tools.IsNullOrEmpty()) return this;
            Attachments ??= new();
            Attachments.Add(tools);
            return this;
        }

        public ThreadMessageRequest SetUploadFiles(params IFile[] files)
        {
            if (files.IsNullOrEmpty()) return this;
            UploadFiles = files?.ToList();
            return this;
        }

        public ThreadMessageRequest AddUploadFiles(params IFile[] files)
        {
            if (files.IsNullOrEmpty()) return this;
            UploadFiles ??= new();
            UploadFiles.AddRange(files);
            return this;
        }

        private void AddContentPart(ContentPart part)
        {
            if (part == null) return;
            ContentParts ??= new();
            ContentParts.Add(new ContentPartWrapper(part));
        }
    }

    internal class ThreadMessageRequestConverter : JsonConverter<ThreadMessageRequest>
    {
        public override void WriteJson(JsonWriter writer, ThreadMessageRequest value, JsonSerializer serializer)
        {
            if (value == null)
            {
                //writer.WriteNull();
                return;
            }

            JObject obj = new()
            {
                ["role"] = JToken.FromObject(value.Role, serializer),
                ["content"] = value.ContentParts.IsNullOrEmpty() ? null : OpenAIUtils.CreateThreadMessageContentJToken(value.ContentParts.ToList(), serializer),
                ["attachments"] = value.Attachments.IsNullOrEmpty() ? null : JToken.FromObject(value.Attachments, serializer),
                ["upload_files"] = value.UploadFiles.IsNullOrEmpty() ? null : JToken.FromObject(value.UploadFiles, serializer),
                ["model"] = string.IsNullOrEmpty(value.Model) ? null : JToken.FromObject(value.Model, serializer),
                ["user"] = string.IsNullOrEmpty(value.User) ? null : JToken.FromObject(value.User, serializer),
                ["n"] = value.N == null ? null : JToken.FromObject(value.N, serializer),
                ["metadata"] = value.Metadata.IsNullOrEmpty() ? null : JToken.FromObject(value.Metadata, serializer),
            };

            obj.RemoveNulls();
            obj.WriteTo(writer);
        }

        public override ThreadMessageRequest ReadJson(JsonReader reader, Type objectType, ThreadMessageRequest existingValue, bool hasExistingValue, JsonSerializer serializer)
         => throw new NotImplementedException("Deserialization is not implemented for ThreadMessageRequest.");
    }
}
