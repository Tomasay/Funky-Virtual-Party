using System;
using System.Collections.Generic;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Server-Sent Event field keys.
    /// </summary>
    public enum SSEField
    {
        Unset = 0,
        Id,
        Event,
        Data,
        Retry,
        Error
    }

    public class SSEParser
    {
        private static readonly Dictionary<SSEField, string> kDefaultFieldMap = new()
        {
            { SSEField.Id, "id" },
            { SSEField.Event, "event" },
            { SSEField.Data, "data" },
            { SSEField.Retry, "retry" },
            { SSEField.Error, "error" },
        };

        public readonly Func<string, bool> IsDonePredicate;
        private readonly Dictionary<SSEField, string> _fieldMap;
        private readonly char _separator;

        public SSEParser(Dictionary<SSEField, string> customFieldMap = null, char separator = ':', Func<string, bool> isDonePredicate = null) //"[DONE]")
        {
            _fieldMap = customFieldMap ?? kDefaultFieldMap;
            _separator = separator;
            IsDonePredicate = isDonePredicate ?? CreateDefaultIsDonePredicate();
        }

        private static Func<string, bool> CreateDefaultIsDonePredicate()
        {
            return (sseString) => // Default predicate to check if the SSE string indicates completion
            {
                if (string.IsNullOrEmpty(sseString)) return false;
                return sseString.Contains("[DONE]")
                 || sseString.Contains("END_OF_STREAM");
            };
        }

        public List<(SSEField field, string result)> Parse(string sseString)
        {
            List<(SSEField field, string result)> results = new();
            string[] lines = sseString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                (SSEField field, string data)? field = GetField(line);
                if (field == null) continue;
                results.Add((field.Value.field, field.Value.data));
            }

            return results;
        }

        public bool IsDone(string sseString)
        {
            if (string.IsNullOrEmpty(sseString)) return true;
            return IsDonePredicate(sseString);
        }

        private (SSEField field, string value)? GetField(string line)
        {
            int sepIndex = line.IndexOf(_separator);
            if (sepIndex <= 0) return null;

            string key = line[..sepIndex].Trim();
            string value = line[(sepIndex + 1)..].TrimStart();

            foreach (var kvp in _fieldMap)
            {
                if (kvp.Value == key) return (kvp.Key, value);
            }

            return null;
        }
    }
}