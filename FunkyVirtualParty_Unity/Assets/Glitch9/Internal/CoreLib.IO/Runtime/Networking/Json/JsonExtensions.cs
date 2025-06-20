using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Glitch9.IO.Networking.RESTApi
{
    internal static class JsonExtensions
    {
        public static int GetInt(this JObject obj, string key, int fallback = 0)
        => obj.TryGetValue(key, out var token) && token.Type != JTokenType.Null ? token.ToObject<int>() : fallback;
        public static long GetLong(this JObject obj, string key, long fallback = 0L)
        => obj.TryGetValue(key, out var token) && token.Type != JTokenType.Null ? token.ToObject<long>() : fallback;
        public static bool GetBool(this JObject obj, string key, bool fallback = false)
        => obj.TryGetValue(key, out var token) && token.Type != JTokenType.Null ? token.ToObject<bool>() : fallback;
        public static string GetString(this JObject obj, string key, string fallback = null)
        => obj.TryGetValue(key, out var token) && token.Type != JTokenType.Null ? token.ToString() : fallback;
        public static TEnum GetEnum<TEnum>(this JObject obj, string key, TEnum fallback) where TEnum : Enum
        => obj.TryGetValue(key, out var token) && token.Type != JTokenType.Null ? ApiEnumUtil.Parse(token.ToString(), fallback) : fallback;

        public static void RemoveNulls(this JObject obj)
        {
            var propsToRemove = new List<string>();
            foreach (var prop in obj.Properties())
            {
                if (prop.Value.Type == JTokenType.Null)
                    propsToRemove.Add(prop.Name);
            }

            foreach (var prop in propsToRemove)
            {
                obj.Remove(prop);
            }
        }
    }
}