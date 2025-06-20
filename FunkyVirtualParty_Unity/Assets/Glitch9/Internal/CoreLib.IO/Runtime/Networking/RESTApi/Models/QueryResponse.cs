using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    [JsonObject]
    public class QueryResponse<T>
    {
        public T[] Data { get; set; }

        /// <summary>
        /// The next page token for pagination (optional).
        /// </summary>
        public string NextPageToken { get; set; }

        /// <summary>
        /// Total number of items found (if requested).
        /// </summary>
        public int? TotalCount { get; set; }

        /// <summary>
        /// Indicates if there are more items beyond the current page.
        /// </summary>
        public bool HasMore { get; set; }
        public string Object { get; set; }
        public string FirstId { get; set; }
        public string LastId { get; set; }
        public UnixTime? Created { get; set; }
    }

    internal static class QueryResponseExtensions
    {
        public static QueryResponse<TSoftRef> ToSoftRef<T, TSoftRef>(this QueryResponse<T> res)
            where T : TSoftRef
            where TSoftRef : class
        {
            if (res == null) return null;
            TSoftRef[] castedData = res.Data?.Select(item => item as TSoftRef).ToArray();

            var softRef = new QueryResponse<TSoftRef>
            {
                Data = castedData,
                NextPageToken = res.NextPageToken,
                TotalCount = res.TotalCount,
                HasMore = res.HasMore,
                Object = res.Object,
                FirstId = res.FirstId,
                LastId = res.LastId,
                Created = res.Created
            };
            return softRef;
        }
    }

    internal class QueryResponseConverter<T> : JsonConverter<QueryResponse<T>>
    {
        private readonly string _dataPropertyName;

        public QueryResponseConverter(string dataPropertyName = "data")
        {
            _dataPropertyName = dataPropertyName;
        }

        public override QueryResponse<T> ReadJson(JsonReader reader, Type objectType, QueryResponse<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var result = new QueryResponse<T>();

            // 1. Data만 수동 처리
            if (token.Type == JTokenType.Array)
            {
                result.Data = token.ToObject<T[]>(serializer);
                return result;
            }

            if (token is not JObject obj)
                return result;

            if (obj.TryGetValue(_dataPropertyName, out var dataToken) && dataToken.Type == JTokenType.Array)
            {
                result.Data = dataToken.ToObject<T[]>(serializer);
            }
            else
            {
                // fallback: search for any array property
                foreach (var prop in obj.Properties())
                {
                    if (prop.Value.Type == JTokenType.Array)
                    {
                        try
                        {
                            result.Data = prop.Value.ToObject<T[]>(serializer);
                            break;
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"[QueryResponse] Fallback array parse failed at '{prop.Name}': {e.Message}");
                        }
                    }
                }
            }

            // 2. Populate the rest (total_count, next_page_token, etc.)
            serializer.Populate(obj.CreateReader(), result);
            return result;
        }


        public override void WriteJson(JsonWriter writer, QueryResponse<T> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            // Write Data
            writer.WritePropertyName(_dataPropertyName);
            serializer.Serialize(writer, value.Data);

            // Write additional properties
            WriteIfNotNull(writer, serializer, nameof(value.NextPageToken), value.NextPageToken);
            WriteIfNotNull(writer, serializer, nameof(value.TotalCount), value.TotalCount);
            WriteIfNotNull(writer, serializer, nameof(value.HasMore), value.HasMore);
            WriteIfNotNull(writer, serializer, nameof(value.Object), value.Object);
            WriteIfNotNull(writer, serializer, nameof(value.FirstId), value.FirstId);
            WriteIfNotNull(writer, serializer, nameof(value.LastId), value.LastId);
            WriteIfNotNull(writer, serializer, nameof(value.Created), value.Created);

            writer.WriteEndObject();
        }

        private void WriteIfNotNull(JsonWriter writer, JsonSerializer serializer, string name, object value)
        {
            if (value == null) return;
            writer.WritePropertyName(name);
            serializer.Serialize(writer, value);
        }
    }
}
