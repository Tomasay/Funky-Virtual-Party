using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glitch9.IO.Json.Schema
{
    public class JsonSchemaFactory
    {
        private class TypeSchema
        {
            public Type Type { get; }
            public JsonSchema Schema { get; }

            public TypeSchema(Type type, JsonSchema schema)
            {
                ThrowIf.ArgumentIsNull(type, nameof(type));
                ThrowIf.ArgumentIsNull(schema, nameof(schema));

                Type = type;
                Schema = schema;
            }
        }

        private readonly IList<TypeSchema> _stack = new List<TypeSchema>();
        private JsonSchema CurrentSchema { get; set; }

        private void Push(TypeSchema typeSchema)
        {
            CurrentSchema = typeSchema.Schema;
            _stack.Add(typeSchema);
        }

        private TypeSchema Pop()
        {
            TypeSchema popped = _stack[_stack.Count - 1];
            _stack.RemoveAt(_stack.Count - 1);
            CurrentSchema = _stack.LastOrDefault()?.Schema;
            return popped;
        }

        public JsonSchema CreateFromType(Type type) => CreateFromTypeINTERNAL(type, null);
        public JsonSchema CreateFromMap(Dictionary<string, object> dictionary) => GenerateFromMapInternal(dictionary);

        private JsonSchema CreateFromTypeINTERNAL(Type type, JsonSchemaPropertyAttribute propertyAttribute)
        {
            if (_stack.Any(tc => tc.Type == type))
            {
                throw new JsonException($"Unresolved circular reference for type '{type}'.");
            }

            Push(new TypeSchema(type, new JsonSchema()));

            List<string> requiredProperties = new();
            CurrentSchema.Properties = new Dictionary<string, JsonSchema>();

            if (propertyAttribute == null)
            {
                CurrentSchema.Type = JsonSchemaType.Object;
            }
            else
            {
                bool isSet = false;

                if (!propertyAttribute.Enum.IsNullOrEmpty())
                {
                    CurrentSchema.Type = JsonSchemaType.Enum;
                    CurrentSchema.Enum = propertyAttribute.Enum.ToList();
                    isSet = true;
                }

                CurrentSchema.Name = propertyAttribute.Name;
                CurrentSchema.Title = propertyAttribute.Title;
                CurrentSchema.Description = propertyAttribute.Description;
                CurrentSchema.Nullable = propertyAttribute.Nullable;
                CurrentSchema.AdditionalProperties = propertyAttribute.AdditionalProperties;
                CurrentSchema.AnyOf = propertyAttribute.AnyOf;

                if (!isSet)
                {
                    if (type == typeof(string))
                    {
                        CurrentSchema.Type = JsonSchemaType.String;
                    }
                    else if (type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                             type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) ||
                             type == typeof(ushort) || type == typeof(sbyte))
                    {
                        CurrentSchema.Type = JsonSchemaType.Integer;
                        CurrentSchema.Minimum = propertyAttribute.Minimum;
                        CurrentSchema.Maximum = propertyAttribute.Maximum;
                    }
                    else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                    {
                        CurrentSchema.Type = JsonSchemaType.Float;
                    }
                    else if (type == typeof(bool))
                    {
                        CurrentSchema.Type = JsonSchemaType.Bool;
                    }
                    else if (type.IsArray ||
                            type.GetInterfaces().Any(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IList<>)))
                    {
                        CurrentSchema.Type = JsonSchemaType.Array;
                        Type genericType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
                        CurrentSchema.Items = CreateFromTypeINTERNAL(genericType, propertyAttribute);
                        CurrentSchema.MaxItems = propertyAttribute.MaxItems;
                        CurrentSchema.MinItems = propertyAttribute.MinItems;
                    }
                    else if (type == typeof(DateTime))
                    {
                        CurrentSchema.Type = JsonSchemaType.String;
                        CurrentSchema.Format = "date-time";
                    }
                    else if (type.IsEnum)
                    {
                        CurrentSchema.Type = JsonSchemaType.Enum;
                        CurrentSchema.Enum = Enum.GetNames(type).ToList();
                    }
                    else if (type.IsClass)
                    {
                        CurrentSchema.Type = JsonSchemaType.Object;
                    }
                }
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead && property.CanWrite)
                {
                    JsonSchemaPropertyAttribute pAttribute = AttributeCache<JsonSchemaPropertyAttribute>.Get(property);

                    if (pAttribute == null) continue;
                    //Debug.Log($"Found JsonSchemaPropertyAttribute for property '{property.Name}'.");

                    string propertyName = pAttribute.Name;
                    if (pAttribute.Required) requiredProperties.Add(propertyName);

                    JsonSchema propertySchema = CreateFromTypeINTERNAL(property.PropertyType, pAttribute);
                    CurrentSchema.Properties.Add(propertyName, propertySchema);
                }
            }

            CurrentSchema.Required = requiredProperties;
            return Pop().Schema;
        }

        private JsonSchema GenerateFromMapInternal(Dictionary<string, object> dictionary)
        {
            if (_stack.Any(tc => tc.Type == dictionary.GetType()))
            {
                throw new JsonException($"Unresolved circular reference for type '{dictionary.GetType()}'.");
            }

            Push(new TypeSchema(dictionary.GetType(), new JsonSchema()));
            List<string> requiredProperties = new();

            CurrentSchema.Type = JsonSchemaType.Object;
            CurrentSchema.Properties = new Dictionary<string, JsonSchema>();

            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                Type type = kvp.Value.GetType();
                JsonSchemaPropertyAttribute attribute = AttributeCache<JsonSchemaPropertyAttribute>.Get(type);
                JsonSchema propertySchema = CreateFromTypeINTERNAL(type, attribute);
                CurrentSchema.Properties.Add(kvp.Key, propertySchema);
            }

            CurrentSchema.Required = requiredProperties;
            return Pop().Schema;
        }
    }
}