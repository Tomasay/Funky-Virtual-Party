using Glitch9.IO.Json.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Glitch9.AIDevKit.Components
{
    public interface IFunctionManager
    {
        /// <summary>
        /// List of functions registered in this manager.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Retrieves function declarations for AI function calling.
        /// </summary>
        FunctionDeclaration[] GetFunctionDeclarations();

        bool HasFunction(string methodName);

        /// <summary>
        /// Handles a tool call and invokes the corresponding function.
        /// </summary>
        void OnReceiveToolCalls(ToolCall[] toolCalls)
        {
            if (toolCalls.IsNullOrEmpty()) return;
            foreach (ToolCall toolCall in toolCalls)
            {
                if (toolCall is FunctionCall functionCall)
                {
                    ExecuteFunction(functionCall.Name, functionCall.Args);
                }
            }
        }

        /// <summary>
        /// Invokes a function by name with JSON-serialized arguments.
        /// </summary>
        JToken ExecuteFunction(string methodName, string jsonArguments);
    }

    /// <summary>
    /// Manages and invokes serialized functions with metadata for AI function calling.
    /// </summary>
    public class FunctionManager : MonoBehaviour, IFunctionManager
    {
        [SerializeField] private List<FunctionReference> functions = new();
        public bool IsEmpty => functions == null || functions.Count == 0;


        public FunctionDeclaration[] GetFunctionDeclarations()
        {
            List<FunctionDeclaration> functionTools = new();

            foreach (FunctionReference function in functions)
            {
                if (function == null) continue;
                if (string.IsNullOrEmpty(function.MethodName))
                {
                    Debug.LogError($"Function '{function.MethodName}' is null or empty.");
                    continue;
                }

                JsonSchema schema = null;

                if (!function.Parameters.IsNullOrEmpty())
                {
                    schema = new();

                    foreach (FunctionParameter parameter in function.Parameters)
                    {
                        if (parameter == null) continue;

                        JsonSchemaType? arrayItemType =
                            parameter.Type == JsonSchemaType.Array
                            ? parameter.ElementType
                            : null;

                        schema.AddParameter(
                            parameter.Type,
                            parameter.Name,
                            parameter.Description,
                            parameter.IsRequired,
                            parameter.EnumValues,
                            arrayItemType
                        );
                    }
                }

                FunctionDeclaration functionTool = new()
                {
                    Name = function.MethodName,
                    Description = function.Description,
                    Parameters = schema,
                };

                functionTools.Add(functionTool);
            }

            return functionTools.ToArray();
        }

        public bool HasFunction(string methodName) => functions.Any(f => f.MethodName == methodName);

        // ExecuteFunction Ver4 - Enum support added  
        // ExecuteFunction Ver5 - Return value support added
        // Dynamically calls a registered method by name with JSON-serialized argument 
        public JToken ExecuteFunction(string methodName, string jsonArguments)
        {
            foreach (FunctionReference function in functions)
            {
                if (function.MethodName != methodName)
                    continue;

                if (function.Target == null)
                {
                    Debug.LogWarning($"Function '{methodName}' has no target.");
                    return JValue.CreateNull();
                }

                MethodInfo method = function.Target.GetType().GetMethod(methodName);
                if (method == null)
                {
                    Debug.LogWarning($"Method '{methodName}' not found in {function.Target.name}.");
                    return JValue.CreateNull();
                }

                ParameterInfo[] parameters = method.GetParameters();
                object[] args;
                object result = null;

                try
                {
                    if (parameters.Length == 0)
                    {
                        args = Array.Empty<object>();
                    }
                    else if (parameters.Length == 1)
                    {
                        var paramType = parameters[0].ParameterType;

                        if (jsonArguments.TrimStart().StartsWith("{"))
                        {
                            JObject jObj = JObject.Parse(jsonArguments);
                            JToken token = jObj.GetValue(parameters[0].Name, StringComparison.OrdinalIgnoreCase);
                            args = new object[] { token.ToObject(paramType) };
                        }
                        else
                        {
                            args = new object[] { JsonConvert.DeserializeObject(jsonArguments, paramType) };
                        }
                    }
                    else
                    {
                        JObject jObj = JObject.Parse(jsonArguments);
                        args = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var param = parameters[i];

                            JToken token = jObj.GetValue(param.Name, StringComparison.OrdinalIgnoreCase) ??
                                throw new ArgumentException($"Missing argument: {param.Name}");

                            args[i] = token.ToObject(param.ParameterType);
                        }
                    }

                    result = method.Invoke(function.Target, args);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FunctionManager] Failed to call '{methodName}': {ex.Message}\nArgs: {jsonArguments}");
                }

                // Convert to JToken safely
                JToken resultToken = result != null
                    ? JToken.FromObject(result)
                    : JValue.CreateNull();

                return resultToken;
            }

            Debug.LogWarning($"Function '{methodName}' not registered in FunctionManager.");
            return JValue.CreateNull();
        }
    }

    /// <summary>
    /// Represents a single parameter in a function declaration.
    /// </summary>
    [Serializable]
    public class FunctionParameter
    {
        [SerializeField] private JsonSchemaType type;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private bool isRequired = true;
        [SerializeField] private string[] enumValues;
        [SerializeField] private JsonSchemaType elementType;

        /// <summary>
        /// The JSON-compatible data type.
        /// </summary>
        public JsonSchemaType Type => type;

        /// <summary>
        /// The parameter name.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Parameter description.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Whether the parameter is required.
        /// </summary>
        public bool IsRequired => isRequired;

        /// <summary>
        /// If the parameter is an enum, its valid values.
        /// </summary>
        public string[] EnumValues => enumValues;

        /// <summary>
        /// For array types, defines the element type.
        /// </summary>
        public JsonSchemaType ElementType => elementType;

        public FunctionParameter(JsonSchemaType type, string name, JsonSchemaType? elementType = null)
        {
            this.type = type;
            this.name = name;
            this.elementType = elementType ?? JsonSchemaType.String;
        }

        public FunctionParameter(string name, Type paramType)
        {
            this.name = name;
            this.type = JsonSchemaTypes.ConvertType(paramType);

            if (paramType.IsEnum)
            {
                enumValues = Enum.GetNames(paramType);
            }
            else if (type == JsonSchemaType.Enum)
            {
                Debug.LogError($"Type {paramType} marked as Enum but is not an enum. Name: {name}");
            }
            else if (type == JsonSchemaType.Array)
            {
                Type elementType = paramType.IsArray
                    ? paramType.GetElementType()
                    : paramType.GetGenericArguments().FirstOrDefault();

                if (elementType != null && elementType.IsEnum)
                {
                    enumValues = Enum.GetNames(elementType);
                }
            }
        }
    }

    /// <summary>
    /// Contains a reference to a MonoBehaviour method and its metadata.
    /// </summary>
    [Serializable]
    public class FunctionReference
    {
        [SerializeField] private MonoBehaviour target;
        [SerializeField] private string methodName;
        [SerializeField] private string description;
        [SerializeReference] private List<FunctionParameter> parameters = new();

        /// <summary>
        /// The method name to invoke.
        /// </summary>
        public string MethodName => methodName;

        /// <summary>
        /// Optional description of the method.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// The MonoBehaviour target where the method exists.
        /// </summary>
        public MonoBehaviour Target => target;

        /// <summary>
        /// List of input parameters for the method.
        /// </summary>
        public List<FunctionParameter> Parameters => parameters;
    }
}