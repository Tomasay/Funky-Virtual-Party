using System;
using System.Collections.Generic;

namespace Glitch9.IO.Json.Schema
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonSchemaAttribute : Attribute
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool AdditionalProperties { get; set; }
        public string[] Enum { get; set; }
        public bool? Nullable { get; set; }
        public int? MaxItems { get; set; }
        public int? MinItems { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public List<JsonSchema> AnyOf { get; set; }
        public JsonSchemaAttribute(string name) => Name = name;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JsonSchemaPropertyAttribute : Attribute
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool AdditionalProperties { get; set; }
        public string[] Enum { get; set; }
        public bool? Nullable { get; set; }
        public int? MaxItems { get; set; }
        public int? MinItems { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public List<JsonSchema> AnyOf { get; set; }
        public JsonSchemaPropertyAttribute(string name) => Name = name;
    }
}