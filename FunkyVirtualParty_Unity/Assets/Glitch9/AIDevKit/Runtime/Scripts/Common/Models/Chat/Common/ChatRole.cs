using System;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public enum ChatRole
    {
        Unset,

        /// <summary>
        /// Developer-provided instructions that the model should follow, 
        /// regardless of messages sent by the user.   
        /// <remarks>
        /// [OpenAI] With o1 models and newer, use 'developer' for this purpose instead.
        /// </remarks>
        [ApiEnum("system")] System,

        /// <summary>
        /// Messages sent by an end user, containing prompts or additional context information. 
        /// </summary>
        [ApiEnum("user")] User,

        /// <summary>
        /// Messages sent by the model in response to user messages.
        /// 
        /// The <see cref="Api.Google"/> equivalent for this role is 'model',
        /// and it is converted to 'assistant' with the JsonConverter.
        /// </summary>
        [ApiEnum("assistant")] Assistant,

        /// <summary>
        /// Messages sent by the model.
        /// The message includes the function calls the model wants to make.
        /// 
        /// The <see cref="Api.Google"/> equivalent for this role is 'function',
        /// and it is converted to 'tool' with the JsonConverter.
        /// </summary>
        [ApiEnum("tool")] Tool,
    }

    internal class ChatRoleConverter : JsonConverter<ChatRole>
    {
        private readonly Api _api;
        internal ChatRoleConverter(Api api) => _api = api;

        public override ChatRole ReadJson(JsonReader reader, Type objectType, ChatRole existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return ChatRole.Unset;

            string value = reader.Value.ToString();

            switch (value)
            {
                case "system":
                    if (_api == Api.Google) return ChatRole.Assistant;
                    return ChatRole.System;
                case "user":
                    return ChatRole.User;
                case "assistant":
                case "model":
                    return ChatRole.Assistant;
                case "tool":
                case "function":
                    return ChatRole.Tool;
                case "developer":
                    return ChatRole.System;
                default:
                    break;
            }

            throw new ArgumentException($"Invalid value for {nameof(ChatRole)}: {value}");
        }

        public override void WriteJson(JsonWriter writer, ChatRole value, JsonSerializer serializer)
        {
            switch (value)
            {
                case ChatRole.System:

                    if (_api == Api.Google)
                    {
                        writer.WriteValue("assistant");
                        break;
                    }
                    else
                    {
                        writer.WriteValue("system");
                        break;
                    }

                case ChatRole.User:
                    writer.WriteValue("user");
                    break;
                case ChatRole.Assistant:
                    writer.WriteValue(_api == Api.Google ? "model" : "assistant");
                    break;
                case ChatRole.Tool:
                    writer.WriteValue(_api == Api.Google ? "function" : "tool");
                    break;
                // case ChatRole.Developer:
                //     writer.WriteValue("developer");
                //     break;
                default:
                    //writer.WriteNull();
                    writer.WriteValue("user");
                    break;
            }
        }
    }
}