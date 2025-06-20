using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    internal class EndpointFormatter
    {
        internal static string FormatEndpoint<TReqBody>(RESTRequest<TReqBody> request, JsonSerializerSettings jsonSerializerSettings)
        {
            string endpoint = request.Endpoint;
            var properties = typeof(TReqBody).GetProperties();
            var queryParams = new List<string>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(request.Body);
                if (value == null) continue;

                var pathAttr = AttributeCache<PathParameterAttribute>.Get(prop);
                if (pathAttr != null)
                {
                    endpoint = endpoint.Replace($"{{{pathAttr.Name}}}", Uri.EscapeDataString(ParseParamValue(value, jsonSerializerSettings)));
                    continue;
                }

                var queryAttr = AttributeCache<QueryParameterAttribute>.Get(prop);
                if (queryAttr != null)
                {
                    string name = queryAttr.Name ?? prop.Name;
                    queryParams.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(ParseParamValue(value, jsonSerializerSettings))}");
                }
            }

            if (queryParams.Count > 0)
                endpoint += "?" + string.Join("&", queryParams);

            return endpoint;
        }

        private static string ParseParamValue(object value, JsonSerializerSettings jsonSerializerSettings)
        {
            // use the converters inside the jsonSerializerSettings to serialize the value
            // like ApiEnumAttributeConverter, etc

            if (value is string strValue) return strValue;
            if (value is SystemLanguage language) return LocaleUtil.ToISOCode(language);
            if (value is Enum enumValue) return enumValue.ToApiValue();
            if (value is bool boolValue) return boolValue ? "true" : "false";
            if (value is int intValue) return intValue.ToString();
            if (value is long longValue) return longValue.ToString();
            if (value is double doubleValue) return doubleValue.ToString("G17", System.Globalization.CultureInfo.InvariantCulture);
            if (value is float floatValue) return floatValue.ToString("G17", System.Globalization.CultureInfo.InvariantCulture);
            if (value is DateTime dateTimeValue) return dateTimeValue.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
            if (value is DateTimeOffset dateTimeOffsetValue) return dateTimeOffsetValue.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

            return JsonConvert.SerializeObject(value, jsonSerializerSettings).Trim('"');
        }


        internal static string HideKeyFromEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return endpoint;
            if (endpoint.Contains("key="))
            {
                int keyStart = endpoint.IndexOf("key=", StringComparison.Ordinal);  // replace the key with [api_key] 
                int keyEnd = endpoint.IndexOf('&', StringComparison.Ordinal);       // find & or end of string
                if (keyEnd == -1) keyEnd = endpoint.Length;
                endpoint = endpoint.Remove(keyStart + 4, keyEnd - keyStart - 4).Insert(keyStart + 4, "[api_key]");
            }
            return endpoint;
        }

    }
}