using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    public interface IThreadMessageEventReceiver
    {
        void OnMessageCreated(ThreadMessage message);
        void OnMessageRetrieved(ThreadMessage message);
        void OnMessageCompleted(ThreadMessage message);
    }

    /// <summary>
    /// Represents a message within a thread.
    /// </summary>
    [Serializable, JsonConverter(typeof(ThreadMessageConverter))]
    public class ThreadMessage : ChatMessage
    {
        [JsonProperty("role")] public override ChatRole Role => _role;
        private readonly ChatRole _role;
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// The OpenAI object type
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object. 
        /// This can be useful for storing additional information about the object in a structured format. 
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonProperty("metadata")] public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The <see cref="Thread"/> that this message belongs to.
        /// </summary>
        [JsonProperty("thread_id")] public string ThreadId { get; set; }

        /// <summary>
        /// If applicable, the <see cref="Assistant"/> that authored this message.
        /// </summary>
        [JsonProperty("assistant_id")] public string AssistantId { get; set; }

        /// <summary>
        /// The <see cref="Run"/> associated with the creation of this message. 
        /// Value is null when messages are created manually using the create message or create thread endpoints.
        /// </summary>
        [JsonProperty("run_id")] public string RunId { get; set; }

        /// <summary>
        /// A list of files attached to the message, and the tools they were added to.
        /// </summary>
        [JsonProperty("attachments")] public Attachment[] Attachments { get; set; }

        /// <summary>
        /// Optional. A list of tool calls (functions) the model wants to use.
        /// </summary>
        [JsonProperty("tool_calls")] public ToolCall[] Tools { get; set; }

        /// <summary>
        /// [Required] Tool call that this message is responding to.
        /// </summary>
        [JsonProperty("tool_call_id")] public string ToolCallId { get; set; }

        public ThreadMessage() { }
        public ThreadMessage(ChatRole role, string content = null) : base(content) => _role = role;
    }

    #region JsonConverter
    internal class ThreadMessageConverter : JsonConverter<ThreadMessage>
    {
        public override ThreadMessage ReadJson(JsonReader reader, Type objectType, ThreadMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //AIDevKitDebug.Log("[ThreadMessageConverter] Reading ThreadMessage JSON...");

            JObject obj = JObject.Load(reader);

            JToken contentToken = obj["content"];

            Content content = null;

            if (contentToken != null && contentToken.Type != JTokenType.Null)
            {
                if (contentToken.Type == JTokenType.Object || contentToken.Type == JTokenType.Array)
                {
                    try
                    {
                        content = contentToken.ToObject<Content>(serializer);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[ThreadMessageConverter] Failed to deserialize ChatContent: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[ThreadMessageConverter] Unexpected content token type: {contentToken.Type}");
                }
            }

            ToolCall[] toolCalls;

            if (obj["tool_calls"] != null && obj["tool_calls"].Type != JTokenType.Null)
            {
                toolCalls = obj["tool_calls"].ToObject<ToolCall[]>(serializer);
            }
            else
            {
                toolCalls = null;
            }

            return new ThreadMessage(obj["role"]?.ToObject<ChatRole>(serializer) ?? ChatRole.User)
            {
                Tools = toolCalls,
                Content = content,
                Name = obj["name"]?.ToString(),
                Id = obj["id"]?.ToString(),
                Object = obj["object"]?.ToString(),
                Metadata = obj["metadata"]?.ToObject<Dictionary<string, string>>(serializer),
                ThreadId = obj["thread_id"]?.ToString(),
                AssistantId = obj["assistant_id"]?.ToString(),
                RunId = obj["run_id"]?.ToString(),
                Attachments = obj["attachments"]?.ToObject<Attachment[]>(serializer),
            };
        }

        public override void WriteJson(JsonWriter writer, ThreadMessage value, JsonSerializer serializer)
        {
            //AIDevKitDebug.Log("Writing ThreadMessage to JSON");

            JObject obj = new()
            {
                ["role"] = JToken.FromObject(value.Role, serializer),
                ["content"] = value.Content != null ? JToken.FromObject(value.Content, serializer) : null,
                ["name"] = value.Name != null ? JToken.FromObject(value.Name, serializer) : null,
                ["id"] = value.Id != null ? JToken.FromObject(value.Id, serializer) : null,
                ["object"] = value.Object != null ? JToken.FromObject(value.Object, serializer) : null,
                ["metadata"] = value.Metadata != null ? JToken.FromObject(value.Metadata, serializer) : null,
                ["thread_id"] = value.ThreadId != null ? JToken.FromObject(value.ThreadId, serializer) : null,
                ["assistant_id"] = value.AssistantId != null ? JToken.FromObject(value.AssistantId, serializer) : null,
                ["run_id"] = value.RunId != null ? JToken.FromObject(value.RunId, serializer) : null,
                ["attachments"] = value.Attachments != null ? JToken.FromObject(value.Attachments, serializer) : null,
            };

            obj.RemoveNulls(); // null 제거 
            obj.WriteTo(writer);
        }
    }
    #endregion JsonConverter

    #region Extensions
    /// <summary>
    /// Extension methods for <see cref="ThreadMessage"/> and <see cref="ChatMessage"/>.
    /// </summary> 
    public static class ThreadMessageExtensions
    {
        public static ChatMessage ToChatMessage(this ThreadMessage message)
        {
            if (message == null) return null;

            return ChatMessage.Create(message.Role, message.Content);
        }

        public static ThreadMessageRequest ToThreadMessageRequest(this ChatMessage message)
        {
            if (message == null) return null;

            ThreadMessageRequest req = new();
            req.SetPrompt(message.Content);

            if (message is UserMessage userMessage)
            {
                req.SetUploadFiles(userMessage.AttachedFiles?.ToArray());
            }

            return req;
        }
    }
    #endregion Extensions
}
