using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Glitch9.IO.Files
{
    public static class CsvConverter
    {
        private const string FIELD_FORMAT = "{fieldText}<metadata>{metadata}</metadata>";
        private const string METADATA_TAG_START = "<metadata>";
        private const string METADATA_TAG_END = "</metadata>";

        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

        private static PropertyInfo[] GetProperties<T>()
        {
            Type type = typeof(T);
            if (!_propertyCache.TryGetValue(type, out PropertyInfo[] properties))
            {
                properties = type.GetProperties();
                _propertyCache[type] = properties;
            }
            return properties;
        }

        public static string SerializeField<T>(string fieldText, T metadata)
        {
            string serializedMetadata = SerializeMetadata(metadata);  // 메타데이터를 JSON 형식으로 직렬화
            return FIELD_FORMAT.Replace("{fieldText}", fieldText).Replace("{metadata}", serializedMetadata);
        }

        public static (string, T) DeserializeField<T>(string csvFieldWithMetadata) where T : new()
        {
            int start = csvFieldWithMetadata.IndexOf(METADATA_TAG_START, StringComparison.Ordinal);
            int end = csvFieldWithMetadata.IndexOf(METADATA_TAG_END, StringComparison.Ordinal);

            // 메타데이터 태그가 없는 경우, 전체 문자열을 필드 텍스트로 리턴
            if (start == -1 || end == -1)
            {
                return (csvFieldWithMetadata.Trim(), new T());
            }

            start += METADATA_TAG_START.Length;

            string fieldText = csvFieldWithMetadata.Substring(0, start - METADATA_TAG_START.Length).Trim();
            string metadataText = csvFieldWithMetadata.Substring(start, end - start);
            T metadata = DeserializeMetadata<T>(metadataText);

            return (fieldText, metadata);
        }

        public static string SerializeMetadata<T>(T metadata)
        {
            StringBuilder csv = new();
            PropertyInfo[] properties = GetProperties<T>();

            // CSV 헤더 줄 생성
            foreach (PropertyInfo prop in properties)
            {
                csv.Append($"\"{EscapeCSV(prop.Name)}\",");
            }
            csv.Length--;  // 마지막 쉼표 제거
            csv.AppendLine();

            // JSON 객체를 직렬화 후 딕셔너리로 변환
            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                JsonConvert.SerializeObject(metadata));

            // CSV 데이터 줄 생성
            foreach (PropertyInfo prop in properties)
            {
                object value = dictionary[prop.Name];
                string valueString = value == null ? "" : EscapeCSV(value.ToString());
                csv.Append($"\"{valueString}\",");
            }
            csv.Length--;  // 마지막 쉼표 제거
            csv.AppendLine();

            return csv.ToString();
        }

        private static string EscapeCSV(string input)
        {
            return input.Replace("\"", "\"\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        }

        public static T DeserializeMetadata<T>(string csvData) where T : new()
        {
            string[] lines = csvData.Split('\n');
            if (lines.Length < 2) throw new ArgumentException("CSV data is not valid.");

            string[] headers = lines[0].Split(',');
            string[] values = lines[1].Split(',');
            if (headers.Length != values.Length) throw new ArgumentException("CSV header and data length mismatch.");

            T result = new();
            PropertyInfo[] properties = GetProperties<T>();
            Dictionary<string, string> dictionary = new();

            for (int i = 0; i < headers.Length; i++)
            {
                dictionary[headers[i].Trim('\"')] = values[i].Trim('\"');
            }

            foreach (PropertyInfo prop in properties)
            {
                if (dictionary.TryGetValue(prop.Name, out string value))
                {
                    try
                    {
                        object convertedValue = Convert.ChangeType(value, prop.PropertyType, CultureInfo.InvariantCulture);
                        prop.SetValue(result, convertedValue);
                    }
                    catch (FormatException ex)
                    {
                        throw new InvalidOperationException($"Error converting value '{value}' to {prop.PropertyType}", ex);
                    }
                }
            }

            return result;
        }
    }
}