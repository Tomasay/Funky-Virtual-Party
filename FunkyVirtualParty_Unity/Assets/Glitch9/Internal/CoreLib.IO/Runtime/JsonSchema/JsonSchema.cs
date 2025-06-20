using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Glitch9.IO.Json.Schema
{
    /// <summary>
    /// The Schema object allows the definition of input and output data types.
    /// These types can be objects, but also primitives and arrays.
    /// Represents a select subset of an OpenAPI 3.0 schema object.
    /// </summary>
    public class JsonSchema
    {
        /// <summary>
        /// Required.
        /// Data type.
        /// </summary>
        public JsonSchemaType Type { get; set; } = JsonSchemaType.Object;

        /// <summary>
        /// Optional.
        /// The format of the data.
        /// This is used only for primitive data types.
        /// <para>
        /// OpenAI Supported formats: string only.
        /// </para>
        /// <para>
        /// Google Generative AI Supported formats: for NUMBER type: float, double for INTEGER type: int32, int64
        /// </para>
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Optional. The title of the schema.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional.
        /// A brief description of the parameter.
        /// This could contain examples of use.
        /// Parameter description may be formatted as Markdown.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional.
        /// Indicates if the value may be null.
        /// </summary>
        public bool? Nullable { get; set; }

        /// <summary>
        /// Optional.
        /// Possible values of the element of Type.STRING with enum format.
        /// For example we can define an Enum Direction as : {type:STRING, format:enum, enum:["EAST", NORTH", "SOUTH", "WEST"]}
        /// </summary>
        public List<string> Enum { get; set; }

        /// <summary>
        /// Optional. Maximum number of the elements for Type.ARRAY.
        /// </summary>
        public int? MaxItems { get; set; }

        /// <summary>
        /// Optional. Minimum number of the elements for Type.ARRAY.
        /// </summary>
        public int? MinItems { get; set; }

        /// <summary>
        /// map (key: string, value: object (Schema))
        /// Optional. Properties of Type.OBJECT.
        /// An object containing a list of "key": value pairs. Example: { "name": "wrench", "mass": "1.3kg", "count": "3" }.
        /// </summary>
        public Dictionary<string, JsonSchema> Properties { get; set; }

        /// <summary>
        /// Optional.
        /// Required properties of Type.OBJECT.
        /// </summary>
        public List<string> Required { get; set; }

        /// <summary>
        /// Optional. The value should be validated against any (one or more) of the subschemas in the list.
        /// </summary>
        public List<JsonSchema> AnyOf { get; set; }

        /// <summary>
        /// Optional.
        /// Schema of the elements of Type.ARRAY.
        /// </summary>
        public JsonSchema Items { get; set; }

        /// <summary>
        /// Optional.
        /// The name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public bool AdditionalProperties { get; set; }

        /// <summary>
        /// Optional. 
        // SCHEMA FIELDS FOR TYPE INTEGER and NUMBER Minimum value of the Type.INTEGER and Type.NUMBER
        /// </summary>
        public int? Minimum { get; set; }

        /// <summary>
        /// Optional. 
        // Maximum value of the Type.INTEGER and Type.NUMBER
        /// </summary>
        public int? Maximum { get; set; }


        public static JsonSchema Read(JsonReader reader)
        {
            if (reader == null) throw new System.ArgumentNullException(nameof(reader));
            JsonSchemaReader jsonSchemaReader = new();
            return jsonSchemaReader.Read(reader);
        }

        public void WriteTo(JsonWriter writer, TextCase typeStringCase = TextCase.SnakeCase)
        {
            JsonSchemaWriter jsonSchemaWriter = new(writer);
            jsonSchemaWriter.WriteSchema(this, typeStringCase);
        }

        public string ToString(TextCase typeStringCase = TextCase.SnakeCase)
        {
            StringWriter writer = new(CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                WriteTo(jsonWriter, typeStringCase);
            }

            return writer.ToString();
        }

        public void AddParameter(JsonSchemaType type, string name, string description, bool required, string[] enumValues = null, JsonSchemaType? arrayItemType = null)
        {
            Properties ??= new Dictionary<string, JsonSchema>();

            if (Properties.ContainsKey(name))
                throw new ArgumentException($"Parameter '{name}' already exists in the schema.");

            JsonSchema parameter = new()
            {
                Type = type,
                Name = name,
                Description = description,
                Enum = enumValues?.ToList(),
            };

            if (type == JsonSchemaType.Array && arrayItemType.HasValue)
            {
                parameter.Items = new JsonSchema
                {
                    Type = arrayItemType.Value,
                };
            }

            if (required)
            {
                Required ??= new List<string>();
                Required.Add(name);
            }

            Properties.Add(name, parameter);
        }


        public static JsonSchema Create<TToolResponse>()
        {
            return Create(typeof(TToolResponse));
        }

        public static JsonSchema Create(Type type)
        {
            return new JsonSchemaFactory().CreateFromType(type);
        }

        public static JsonSchema Create(Dictionary<string, object> dictionary)
        {
            return new JsonSchemaFactory().CreateFromMap(dictionary);
        }
    }
}
