using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    internal class CompletionRequestConverter<TRequest> : JsonConverter<TRequest> where TRequest : CompletionRequestBase
    {
        private readonly Api _api;
        internal CompletionRequestConverter(Api api) => _api = api;

        public override bool CanWrite => true;
        public override void WriteJson(JsonWriter writer, TRequest value, JsonSerializer serializer)
        {
            //AIDevKitDebug.Log("Writing CompletionRequestBase to JSON...");

            JObject obj = new()
            {
                ["model"] = value.Model != null ? JToken.FromObject(value.Model.Id, serializer) : null,
                ["stream"] = value.Stream != null ? JToken.FromObject(value.Stream, serializer) : null,
            };

            bool isOpenAIRequest = _api == Api.OpenAI;
            bool isOpenRouterRequest = _api == Api.OpenRouter;
            bool isOllamaRequest = _api == Api.Ollama;

            if (isOllamaRequest)
            {
                obj["system"] = value.SystemInstruction != null ? JToken.FromObject(value.SystemInstruction, serializer) : null;
                obj["keep_alive"] = value.KeepAlive != null ? JToken.FromObject(value.KeepAlive, serializer) : null;
                obj["options"] = value.ModelSettings != null ? JToken.FromObject(value.ModelSettings, serializer) : null; // 통째로 serialize 
            }
            else
            {
                obj["n"] = value.N != null ? JToken.FromObject(value.N, serializer) : null;

                if (value.ModelSettings != null)
                {
                    obj["max_tokens"] = value.ModelSettings.MaxTokens != null ? JToken.FromObject(value.ModelSettings.MaxTokens, serializer) : null;

                    obj["temperature"] = value.ModelSettings.Temperature != null ? JToken.FromObject(value.ModelSettings.Temperature, serializer) : null;
                    obj["seed"] = value.ModelSettings.Seed != null ? JToken.FromObject(value.ModelSettings.Seed, serializer) : null;
                    obj["stop"] = value.ModelSettings.Stop != null ? JToken.FromObject(value.ModelSettings.Stop, serializer) : null;

                    obj["frequency_penalty"] = value.ModelSettings.FrequencyPenalty != null ? JToken.FromObject(value.ModelSettings.FrequencyPenalty, serializer) : null;
                    obj["presence_penalty"] = value.ModelSettings.PresencePenalty != null ? JToken.FromObject(value.ModelSettings.PresencePenalty, serializer) : null;

                    obj["logit_bias"] = value.ModelSettings.LogitBias != null ? JToken.FromObject(value.ModelSettings.LogitBias, serializer) : null;
                    obj["logprobs"] = value.ModelSettings.Logprobs != null ? JToken.FromObject(value.ModelSettings.Logprobs, serializer) : null;
                    obj["top_logprobs"] = value.ModelSettings.TopLogprobs != null ? JToken.FromObject(value.ModelSettings.TopLogprobs, serializer) : null;

                    obj["top_p"] = value.ModelSettings.TopP != null ? JToken.FromObject(value.ModelSettings.TopP, serializer) : null;

                    if (isOpenRouterRequest)
                    {
                        // OpenRouter-only options: top_k, top_a, min_p, repetition_penalty                    
                        obj["top_k"] = value.ModelSettings.TopK != null ? JToken.FromObject(value.ModelSettings.TopK, serializer) : null;
                        obj["top_a"] = value.ModelSettings.TopA != null ? JToken.FromObject(value.ModelSettings.TopA, serializer) : null;
                        obj["min_p"] = value.ModelSettings.MinP != null ? JToken.FromObject(value.ModelSettings.MinP, serializer) : null;
                        obj["repetition_penalty"] = value.ModelSettings.RepeatPenalty != null ? JToken.FromObject(value.ModelSettings.RepeatPenalty, serializer) : null;
                    }
                }

                if (isOpenRouterRequest)
                {
                    obj["metadata"] = value.Metadata != null ? JToken.FromObject(value.Metadata, serializer) : null;
                    obj["user"] = value.User != null ? JToken.FromObject(value.User, serializer) : null;
                    obj["usage"] = value.StreamOptions != null ? JToken.FromObject(value.StreamOptions.IncludeUsage, serializer) : null;
                    obj["models"] = value.Models != null ? JToken.FromObject(value.Models, serializer) : null;
                    obj["transforms"] = value.Transforms != null ? JToken.FromObject(value.Transforms, serializer) : null;
                    obj["prompt"] = value.Prompt != null ? JToken.FromObject(value.Prompt, serializer) : null;
                }
            }

            if (value is CompletionRequest completion)
            {
                if (isOllamaRequest)
                {
                    obj["prompt"] = value.Prompt != null ? JToken.FromObject(value.Prompt, serializer) : null;
                    obj["suffix"] = completion.Suffix != null ? JToken.FromObject(completion.Suffix, serializer) : null;
                    obj["template"] = completion.Template != null ? JToken.FromObject(completion.Template, serializer) : null;
                    obj["raw"] = completion.Raw != null ? JToken.FromObject(completion.Raw, serializer) : null;
                    obj["context"] = completion.Context != null ? JToken.FromObject(completion.Context, serializer) : null;
                }
            }
            else if (value is ChatCompletionRequest chat)
            {
                if (!string.IsNullOrEmpty(value.Prompt))
                {
                    bool addPromptAsMessage = !isOllamaRequest;

                    if (!addPromptAsMessage && isOpenAIRequest)
                    {
                        bool isLegacy = value.Model != null && value.Model.IsLegacy;
                        if (isLegacy) obj["prompt"] = value.Prompt != null ? JToken.FromObject(value.Prompt, serializer) : null;
                    }

                    if (addPromptAsMessage)
                    {
                        UserMessage m = new(value.Prompt);
                        chat.Messages.Add(m);
                    }
                }

                if (chat.AttachedFiles.IsNotNullOrEmpty())
                {
                    if (isOllamaRequest)
                    {
                        List<string> base64Images = new();

                        foreach (IFile file in chat.AttachedFiles)
                        {
                            if (file is File<Texture2D> image)
                            {
                                string base64 = image.EncodeToBase64();
                                if (!string.IsNullOrEmpty(base64)) base64Images.Add(base64);
                            }
                        }

                        if (base64Images.IsNotNullOrEmpty()) obj["images"] = JToken.FromObject(base64Images, serializer);
                    }
                    else
                    {
                        List<ContentPart> parts = new();

                        foreach (IFile f in chat.AttachedFiles)
                        {
                            if (f is File<Texture2D> image)
                            {
                                AIDevKitDebug.Green($"Encoding image {image.Name} to base64...");
                                string base64 = image.EncodeToBase64();
                                if (string.IsNullOrEmpty(base64))
                                {
                                    AIDevKitDebug.Error($"Failed to encode image {image.Name} to base64.");
                                    continue;
                                }

                                parts.Add(ImageContentPart.FromBase64(base64));
                            }
                            else if (f is File<AudioClip> audio)
                            {
                                AIDevKitDebug.Green($"Encoding audio {audio.Name} to base64...");
                                string base64 = audio.EncodeToBase64();
                                if (string.IsNullOrEmpty(base64))
                                {
                                    AIDevKitDebug.Error($"Failed to encode audio {audio.Name} to base64.");
                                    continue;
                                }

                                parts.Add(AudioContentPart.FromBase64(base64));
                            }
                            else if (f is RawFile file)
                            {
                                AIDevKitDebug.Green($"Encoding file {file.Name} to base64...");
                                string base64 = file.EncodeToBase64();
                                if (string.IsNullOrEmpty(base64))
                                {
                                    AIDevKitDebug.Error($"Failed to encode file {file.Name} to base64.");
                                    continue;
                                }

                                string fileName = file.Name;
                                parts.Add(FileContentPart.FromBase64(base64, fileName));
                            }

                            if (parts.Count > 0)
                            {
                                AIDevKitDebug.Green($"Adding {parts.Count} content parts to the last user message in chat...");

                                ChatMessage m = chat.Messages.LastOrDefault();

                                if (m == null || m.Role != ChatRole.User)
                                {
                                    m = new UserMessage(null);
                                    chat.Messages.Add(m);
                                }

                                m.Content.AddPartRange(parts);
                                chat.Messages.ReplaceLastMessage(m);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(chat.StartingMessage)) chat.Messages.SetStartingMessage(chat.StartingMessage);  // Must do this after setting the messages 
                if (!string.IsNullOrEmpty(chat.Summary)) chat.Messages.SetSummary(chat.Summary); // Must do this after setting the messages

                if (!isOllamaRequest)
                {
                    obj["tool_choice"] = chat.ToolChoice != null ? JToken.FromObject(chat.ToolChoice, serializer) : null;
                    obj["response_format"] = chat.ResponseFormat != null ? JToken.FromObject(chat.ResponseFormat, serializer) : null;

                    if (!string.IsNullOrEmpty(chat.SystemInstruction)) chat.Messages.SetSystemInstruction(chat.SystemInstruction);

                    if (isOpenAIRequest) // OpenAI는 ChatCompletionRequest밖에 없다.
                    {
                        obj["audio"] = chat.Audio != null ? JToken.FromObject(chat.Audio, serializer) : null;

                        if (chat.Modalities != null)
                        {
                            List<string> modalities = chat.Modalities.ToStringList();
                            if (modalities.IsNotNullOrEmpty())
                                obj["modalities"] = new JArray(modalities);
                        }

                        obj["web_search_options"] = chat.WebSearchOptions != null ? JToken.FromObject(chat.WebSearchOptions, serializer) : null;

                        obj["stream_options"] = value.StreamOptions != null ? JToken.FromObject(value.StreamOptions, serializer) : null;

                        if (value.ReasoningOptions != null && value.ReasoningOptions.Effort != null && value.ReasoningOptions.Effort != ReasoningEffort.Medium)
                        {
                            obj["reasoning_effort"] = JToken.FromObject(value.ReasoningOptions.Effort, serializer);
                        }

                        obj["service_tier"] = chat.ServiceTier != null ? JToken.FromObject(chat.ServiceTier, serializer) : null;

                        if (value.ModelSettings != null)
                        {
                            // OpenAI-only options: logprobs
                            obj["logprobs"] = value.ModelSettings.Logprobs != null ? JToken.FromObject(value.ModelSettings.Logprobs, serializer) : null;
                        }
                    }
                }

                obj["messages"] = chat.Messages != null ? JToken.FromObject(chat.Messages, serializer) : null;
                obj["tools"] = chat.Tools != null ? JToken.FromObject(chat.Tools, serializer) : null;
            }

            // Check "messages" and "prompt" to see if they both exist
            if (obj.ContainsKey("messages") && obj.ContainsKey("prompt"))
            {
                // If both exist, remove "prompt"
                obj.Remove("prompt");
            }

            obj.RemoveNulls(); // null 제거 
            obj.WriteTo(writer);
        }

        public override bool CanRead => false;
        public override TRequest ReadJson(JsonReader reader, Type objectType, TRequest existingValue, bool hasExistingValue, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
