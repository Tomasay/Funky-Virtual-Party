using System;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    internal class OpenAIChatMessageConverter : ChatMessageConverter
    {
        internal OpenAIChatMessageConverter() : base(Api.OpenAI) { }

        public override ChatMessage ReadJson(JsonReader reader, Type objectType, ChatMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (existingValue is ThreadMessage threadMessage)
            {
                return ReadThreadMessageJson(reader, objectType, threadMessage, hasExistingValue, serializer);
            }
            else
            {
                return base.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
            }
        }

        public override void WriteJson(JsonWriter writer, ChatMessage value, JsonSerializer serializer)
        {
            if (value is ThreadMessage threadMessage)
            {
                WriteThreadMessageJson(writer, threadMessage, serializer);
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }

        private ThreadMessage ReadThreadMessageJson(JsonReader reader, Type objectType, ThreadMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            AIDevKitDebug.Info("[ThreadMessageConverter] Reading ThreadMessage JSON...");

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

        private void WriteThreadMessageJson(JsonWriter writer, ThreadMessage value, JsonSerializer serializer)
        {
            AIDevKitDebug.Info("[ThreadMessageConverter] Writing ThreadMessage JSON...");

            JObject obj = new()
            {
                ["role"] = JToken.FromObject(value.Role, serializer),
                ["content"] = value.Content != null ? OpenAIUtil.CreateThreadMessageContentJToken(value.Content.ToList(), serializer) : null,
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
}