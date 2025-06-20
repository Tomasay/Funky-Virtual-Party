using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking.RESTApi
{
    internal static class RequestSerializationExtensions
    {
        internal static byte[] SerializeBody<TReqBody>(this RESTRequest<TReqBody> req, RESTClient client)
        {
            TReqBody body = req.Body;
            if (body == null) return null;

            string json = JsonConvert.SerializeObject(body, client.JsonSettings);
            if (!req.IgnoreLogs) client.Logger.ReqBody(json);
            return Encoding.UTF8.GetBytes(json);
        }

        internal static async UniTask<List<IMultipartFormSection>> SerializeBodyAsMultipartAsync<TReqBody>(this RESTRequest<TReqBody> req, RESTClient client)
        {
            if (req == null || req.Body == null) return null;

            List<IMultipartFormSection> formData = new();

            // use reflection to get all public properties of the request
            List<PropertyInfo> properties = PropertyInfoCache.Get<TReqBody>();
            JsonSerializer jsonSerializer = JsonSerializer.Create(client.JsonSettings);
            Dictionary<string, string> serializedValues = new();

            foreach (PropertyInfo prop in properties)
            {
                try
                {
                    //UnityEngine.Debug.LogWarning("0");
                    // Skip properties with [JsonIgnore]
                    JsonIgnoreAttribute jsonIgnore = AttributeCache<JsonIgnoreAttribute>.Get(prop);
                    if (jsonIgnore != null) continue;
                    //UnityEngine.Debug.LogWarning("1");
                    // Handle properties with [JsonProperty] for renaming
                    JsonPropertyAttribute jsonProp = AttributeCache<JsonPropertyAttribute>.Get(prop);
                    string key = jsonProp != null ? jsonProp.PropertyName : prop.Name;
                    //UnityEngine.Debug.LogWarning("2");
                    object value = prop.GetValue(req.Body);
                    if (value == null) continue;

                    // if the value if 'IFile' or 'byte[]' then add it as a file section
                    if (value is byte[] bytes)
                    {
                        formData.Add(ToFileSection(key, bytes));
                        continue;
                    }
                    else if (value is IFile file)
                    {
                        formData.Add(await ToFileSectionAsync(key, file));
                        continue;
                    }

                    string serializedValue;

                    if (value is SystemLanguage lang)
                    {
                        serializedValue = lang.ToISOCode();
                    }
                    else if (value is Enum enumValue)
                    {
                        serializedValue = enumValue.ToApiValue();
                        if (string.IsNullOrEmpty(serializedValue)) continue;
                        Debug.Log($"Multipart Enum value {enumValue} => {serializedValue}");
                    }
                    else
                    {
                        using (var writer = new StringWriter())
                        {
                            jsonSerializer.Serialize(writer, value);
                            serializedValue = writer.ToString();

                            // Remove quotes from serialized string
                            serializedValue = serializedValue.Trim('"');
                        }
                    }

                    serializedValues.Add(key, serializedValue);
                    formData.Add(ToDataSection(key, serializedValue));
                }
                catch (Exception ex)
                {
                    client.Logger.Error($"[MultipartForm] Failed to read property '{prop.Name}' of type {prop.PropertyType}: {ex}");
                    continue; // skip faulty property
                }
            }

            if (!req.IgnoreLogs && client.LogLevel.RequestBody())
            {
                using (StringBuilderPool.Get(out StringBuilder sb))
                {
                    sb.AppendLine("Multipart Form Data:");
                    // foreach (IMultipartFormSection section in formData)
                    // {
                    //     if (section is MultipartFormDataSection)
                    //     {
                    //         sb.AppendLine($"{section.sectionName}: {section.sectionData}");
                    //     }
                    //     else
                    //     {
                    //         sb.AppendLine($"{section.sectionName}: <file>");
                    //     }
                    // }

                    foreach (var kvp in serializedValues)
                    {
                        sb.AppendLine($"{kvp.Key}: {kvp.Value}");
                    }
                    client.Logger.ReqBody(sb.ToString());
                }
            }

            return formData;
        }


        private static IMultipartFormSection ToDataSection(string key, string value)
        => new MultipartFormDataSection(key, value);

        private static IMultipartFormSection ToFileSection(string key, byte[] bytes)
        => new MultipartFormFileSection(key, bytes, "file", "application/octet-stream");

        private static async UniTask<IMultipartFormSection> ToFileSectionAsync(string key, IFile file)
        {
            byte[] data = await file.ReadAllBytesAsync();
            return new MultipartFormFileSection(key, data, file.Name, file.MimeType.ToApiValue());
        }
    }
}