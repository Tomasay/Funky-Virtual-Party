using System;
using System.Reflection;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    public static class ApiEnumUtil
    {
        public static string ToApiValue(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            try
            {
                ApiEnumAttribute attr = AttributeCache<ApiEnumAttribute>.Get(field);
                if (attr != null) return attr.ApiName;
                return value.ToString();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return string.Empty;
            }
        }

        public static TEnum Parse<TEnum>(string raw, TEnum fallback = default) where TEnum : Enum
        {
            try
            {
                foreach (FieldInfo field in typeof(TEnum).GetFields())
                {
                    ApiEnumAttribute attr = AttributeCache<ApiEnumAttribute>.Get(field);
                    if (attr == null) continue;

                    if (!string.IsNullOrEmpty(attr.ParseKey) &&
                        raw.Contains(attr.ParseKey, StringComparison.OrdinalIgnoreCase))
                        return (TEnum)field.GetValue(null);

                    if (!string.IsNullOrEmpty(attr.ApiName) &&
                        raw.Equals(attr.ApiName, StringComparison.OrdinalIgnoreCase))
                        return (TEnum)field.GetValue(null);
                }

                // parse normally
                return (TEnum)Enum.Parse(typeof(TEnum), raw);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ApiEnumUtil] Failed to parse enum: {raw} - {e.Message}");
                return fallback;
            }
        }


        public static bool TryParse(Type enumType, string apiName, out object result, bool ignoreCase = false)
        {
            foreach (FieldInfo field in enumType.GetFields())
            {
                ApiEnumAttribute attribute = AttributeCache<ApiEnumAttribute>.Get(field);

                if (attribute != null)
                {
                    if (string.Equals(attribute.ApiName, apiName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        result = field.GetValue(null);
                        return true;
                    }
                }
            }

            // parse normally
            return Enum.TryParse(enumType, apiName, ignoreCase, out result);
        }
    }
}