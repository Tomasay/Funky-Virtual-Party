using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.IO.Json.Schema
{
    internal class JsonSchemaWriter
    {
        private readonly JsonWriter _writer;

        public JsonSchemaWriter(JsonWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void WriteSchema(JsonSchema schema, TextCase typeStringCase, bool isArrayItem = false)
        {
            if (schema == null) throw new ArgumentNullException(nameof(schema));

            Debug.Log($"Writing JsonSchema: {schema.Name} ({schema.Type})");

            bool addProperties = !schema.Properties.IsNullOrEmpty();

            _writer.WriteStartObject();

            WritePropertyIfNotNull("type", JsonSchemaTypes.GetValue(schema.Type, typeStringCase));
            WritePropertyIfNotNull("nullable", schema.Nullable);
            WritePropertyIfNotNull("maximum", schema.Maximum);
            WritePropertyIfNotNull("minimum", schema.Minimum);

            bool isArray = schema.Type == JsonSchemaType.Array;
            //bool isObject = schema.Type == JsonSchemaType.Object;

            if (isArray)
            {
                addProperties = false;
                WritePropertyIfNotNull("description", schema.Description);

                if (schema.Items != null)
                {
                    _writer.WritePropertyName("items");
                    WriteSchema(schema.Items, typeStringCase, true);
                }

                WritePropertyIfNotNull("maxItems", schema.MaxItems);
                WritePropertyIfNotNull("minItems", schema.MinItems);
            }
            else if (schema.Enum != null)
            {
                WritePropertyIfNotNull("description", schema.Description);
                _writer.WritePropertyName("string");
                _writer.WriteStartArray();
                foreach (string enumValue in schema.Enum)
                {
                    _writer.WriteValue(enumValue);
                }
                _writer.WriteEndArray();
            }
            else if (isArrayItem)
            {
                WritePropertyIfNotNull("additionalProperties", schema.AdditionalProperties);
                WritePropertyIfNotNull("format", schema.Format);

                if (schema.Required != null)
                {
                    _writer.WritePropertyName("required");
                    _writer.WriteStartArray();
                    foreach (string required in schema.Required)
                    {
                        _writer.WriteValue(required);
                    }
                    _writer.WriteEndArray();
                }
            }
            else
            {
                //WritePropertyIfNotNull("additionalProperties", schema.AdditionalProperties);
                WritePropertyIfNotNull("description", schema.Description);
            }

            if (addProperties)
            {
                _writer.WritePropertyName("properties");
                _writer.WriteStartObject();

                foreach (KeyValuePair<string, JsonSchema> property in schema.Properties)
                {
                    _writer.WritePropertyName(property.Key);
                    WriteSchema(property.Value, typeStringCase);
                }

                _writer.WriteEndObject();

                WritePropertyIfNotNull("additionalProperties", schema.AdditionalProperties);

                if (schema.Required != null)
                {
                    _writer.WritePropertyName("required");
                    _writer.WriteStartArray();
                    foreach (string required in schema.Required)
                    {
                        _writer.WriteValue(required);
                    }
                    _writer.WriteEndArray();
                }
            }

            _writer.WriteEndObject();
        }

        private void WritePropertyIfNotNull(string propertyName, object value)
        {
            if (value != null)
            {
                Debug.Log("Writing property: " + propertyName + " with value: " + value);
                _writer.WritePropertyName(propertyName);
                _writer.WriteValue(value);
            }
        }
    }
}
