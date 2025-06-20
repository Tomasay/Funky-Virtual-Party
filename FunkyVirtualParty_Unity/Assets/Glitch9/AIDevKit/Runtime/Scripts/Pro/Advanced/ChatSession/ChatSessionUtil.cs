using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Glitch9.AIDevKit.Advanced.Chat
{
    internal static class ChatSessionUtil
    {
        internal static readonly JsonSerializerSettings localJsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ContractResolver = new RESTContractResolver { NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true } },
            Converters = new List<JsonConverter>
            {
                new ApiEnumConverter(),
                new ChatMessageConverter(Api.None),
                new ChatRoleConverter(Api.None),
                new ContentConverter(Api.None),

                // new ZuluTimeJsonConverter(),
                // new UnixTimeJsonConverter(),

                // //new ApiEnumConverter(),
                new SystemLanguageISOConverter(),
                new StringOrConverter<string>(),
                new IFileJsonConverter(),
                new SafeEnumConverter<UsageType>(),
                // //new StringEnumConverter(),

                // new NullableStopReasonConverter(),
                // new ModelConverter(),
                // new VoiceConverter(),
                // new VoiceTypeConverter(), 

                // // chat completion
                // new ContentPartWrapperConverter(),
                // new ImageContentPartConverter(),
                // new AnnotationConverter(),
                // new TextContentPartConverter(),
                // new HarmCategoryConverter(),

                // new StrictJsonSchemaConverter(),
                // new ResponseFormatConverter(),
            },
        };

        private static string CreateID() => $"chat_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        internal static string GetSavePath() => Path.Combine(Application.persistentDataPath, "Chats");

        internal static string FormatInformation(ChatSession session)
        {
            string modelString = session.Model.SafeGetName();
            string lastReceivedString = session.Last != null ? (session.LastException != null ? "<STREAMING ERROR>" : "<STREAMING IN PROGRESS>") : "";

            return $"Chat Session: {session.Id}\n" +
                   $"Name: {session.Name}\n" +
                   $"Model: {modelString}\n" +
                   $"Messages: {session.Messages.Count}\n" +
                   $"Last Received: {lastReceivedString}\n" +
                   $"Created At: {session.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Last Modified: {session.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
        }

        internal static ChatSession CreateSessionFile(string id = null, string name = null, string startingMessage = null)
        {
            ChatSession session = new(id ?? CreateID(), name, startingMessage);
            Debug.Log($"Created new chat session: {session.Id}");
            session.SaveFile();
            return session;
        }

        internal static ChatSession LoadSessionFromFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            string filePath = ResolveFilePath(id);

            return LoadSessionFromPath(filePath);
        }

        internal static ChatSession LoadSessionFromPath(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ChatSession>(json, localJsonSerializerSettings);
        }

        internal static void SaveSessionAsFile(ChatSession session, string customPath = null)
        {
            (string path, string json) = ResolvePathAndSerialize(session, customPath);
            File.WriteAllText(path, json);
        }

        internal static async UniTask SaveSessionAsFileAsync(ChatSession session, string customPath = null)
        {
            (string path, string json) = ResolvePathAndSerialize(session, customPath);
            //AIDevKitDebug.Mark($"Writing chat session with last message: {session.LastMessage} to file: {path}");
            await File.WriteAllTextAsync(path, json);
        }

        internal static bool DeleteSessionFile(string id)
        {
            string path = ResolveFilePath(id);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        internal static bool DeleteSessionFile(ChatSession session)
        {
            return DeleteSessionFile(session.Id);
        }

        private static string ResolveFilePath(string id, string customPath = null)
        {
            string path = customPath ?? GetSavePath();
            string fileName = id + ".json";
            return Path.Combine(path, fileName);
        }

        private static (string path, string json) ResolvePathAndSerialize(ChatSession session, string customPath)
        {
            string path = ResolveFilePath(session.Id, customPath);
            string json = JsonConvert.SerializeObject(session, localJsonSerializerSettings);

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            return (path, json);
        }

        internal static bool IsDefaultSessionTitle(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return name == ChatSession.kDefaultTitle;
        }
    }
}