// ReSharper disable All
using System;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.IO.Json.Schema
{
    public enum JsonSchemaType
    {
        [ApiEnum("string", "string")] String,
        [ApiEnum("int", "int")] Integer,
        [ApiEnum("float", "float")] Float,
        [ApiEnum("bool", "bool")] Bool,
        [ApiEnum("object", "object")] Object,
        [ApiEnum("array", "array")] Array,
        [ApiEnum("null", "null")] Null,
        [ApiEnum("enum", "string")] Enum,
        [ApiEnum("number", "number")] Number,
    }

    public class JsonSchemaTypes
    {
        public const string Object = "object";
        public const string Array = "array";
        public const string Integer = "integer";
        public const string Float = "number";
        public const string String = "string";
        public const string Boolean = "boolean";
        public const string Null = "null";
        public const string Number = "number";

        public static JsonSchemaType ConvertType(Type type)
        {
            // Nullable<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return ConvertType(Nullable.GetUnderlyingType(type));

            // Array / List<T>
            if (type.IsArray || (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                return JsonSchemaType.Array;

            if (type == typeof(string)) return JsonSchemaType.String;
            if (type == typeof(bool)) return JsonSchemaType.Bool;
            if (type == typeof(int) || type == typeof(long)) return JsonSchemaType.Integer;
            if (type == typeof(float)) return JsonSchemaType.Float;
            if (type == typeof(double) || type == typeof(decimal)) return JsonSchemaType.Number;
            if (type == typeof(void)) return JsonSchemaType.Null;
            if (type.IsEnum) return JsonSchemaType.Enum;

            return JsonSchemaType.Object;
        }

        public static string GetValue(JsonSchemaType type, TextCase stringCase)
        {
            //Debug.LogError($"JsonSchemaTypes.GetValue({type}, {stringCase})");

            return type switch
            {
                JsonSchemaType.String => String.ConvertToCase(stringCase),
                JsonSchemaType.Integer => Integer.ConvertToCase(stringCase),
                JsonSchemaType.Float => Float.ConvertToCase(stringCase),
                JsonSchemaType.Bool => Boolean.ConvertToCase(stringCase),
                JsonSchemaType.Object => Object.ConvertToCase(stringCase),
                JsonSchemaType.Array => Array.ConvertToCase(stringCase),
                JsonSchemaType.Null => Null.ConvertToCase(stringCase),
                JsonSchemaType.Enum => String.ConvertToCase(stringCase),
                _ => null,
            };
        }

        public static JsonSchemaType Parse(string typeString)
        {
            return typeString switch
            {
                String => JsonSchemaType.String,
                Float => JsonSchemaType.Float,
                Integer => JsonSchemaType.Integer,
                Boolean => JsonSchemaType.Bool,
                Object => JsonSchemaType.Object,
                Array => JsonSchemaType.Array,
                Null => JsonSchemaType.Null,
                _ => JsonSchemaType.String,
            };
        }
    }
}