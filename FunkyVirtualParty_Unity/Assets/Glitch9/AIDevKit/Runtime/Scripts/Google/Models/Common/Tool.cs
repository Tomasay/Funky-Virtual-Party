using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Google
{
    /*
        [ How function calling works (https://ai.google.dev/docs/function_calling) ]
     
        Functions are described using function declarations. 
        
        After you pass a list of function declarations in a query to a language model, 
        the model returns an object in an OpenAPI compatible schema format (https://spec.openapis.org/oas/v3.0.3#schema) 
        that includes the names of functions and their arguments and tries to answer the user query with one of the returned functions. 
        The language model understands the purpose of a function by analyzing its function declaration. 
    
        The model doesn't actually call the function. 
        Instead, a developer uses the OpenAPI compatible schema object in the response to call the function that the model returns.
       
        When you implement function calling, you create one or more function declarations, 
        then add the function declarations to a tools object that's passed to the model. 
        Each function declaration contains information about one function that includes the following:
       
        - Function name
        - Function parameters in an OpenAPI compatible schema format. A select subset is supported. When using curl, the schema is specified using JSON.
        - Function description (optional). For the best results, we recommend that you include a description.
        
        The Gemini API also supports parallel function calling, where you can call multiple functions in a single turn.
       
        This document includes curl examples that make REST calls with the GenerativeModel class and its methods.
        - https://ai.google.dev/gemini-api/docs/function-calling#function-calling-curl-samples
    
     */

    public class Tool
    {
        /// <summary>
        /// Optional.
        /// <para>
        /// A list of FunctionDeclarations available to the model that can be used for function calling.
        /// </para>
        /// <para>
        /// The model or system does not execute the function. Instead, the defined function may be
        /// returned as a [FunctionCall][content.part.function_call] with arguments to the client side for execution.
        ///
        /// The model may decide to call a subset of these functions by populating[FunctionCall][content.part.function_call] in the response.
        /// The next conversation turn may contain a [FunctionResponse][content.part.function_response]
        /// with the [content.role] "function" generation context for the next model turn.
        /// </para>
        /// </summary>
        [JsonProperty("function_declarations")] public List<FunctionDeclaration> FunctionDeclarations { get; set; }

        [JsonConstructor] public Tool() { }

        public Tool(IEnumerable<FunctionDeclaration> selectMany)
        {
            FunctionDeclarations = new List<FunctionDeclaration>(selectMany);
        }

        public Tool(params FunctionDeclaration[] functionDeclarations)
        {
            FunctionDeclarations = new List<FunctionDeclaration>(functionDeclarations);
        }

        public Tool(params string[] toolNames)
        {
            FunctionDeclarations = new List<FunctionDeclaration>();

            foreach (string toolName in toolNames)
            {
                FunctionDeclarations.Add(new FunctionDeclaration(toolName));
            }
        }
    }
}
