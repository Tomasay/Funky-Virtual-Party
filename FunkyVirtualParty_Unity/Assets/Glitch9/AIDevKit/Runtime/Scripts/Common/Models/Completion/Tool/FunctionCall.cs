using Glitch9.IO.Json.Schema;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Represents a function call to be used with the AI chat.
    /// </summary>
    public class FunctionCall : ToolCall
    {
        /// <summary>
        /// Gets or sets the function to be used.
        /// This property is required.
        /// </summary>
        [JsonProperty("function")] public FunctionDeclaration Function { get; set; }
        [JsonIgnore] public string Name => Function?.Name;
        [JsonIgnore] public string Args => Function?.Arguments;

        [JsonConstructor] public FunctionCall() => Type = ToolType.Function;

        public FunctionCall(string id, string name, string args)
        {
            Id = id;
            Type = ToolType.Function;
            Function = new FunctionDeclaration()
            {
                Name = name,
                Arguments = args,
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCall"/> class with the specified function and delegate.
        /// Sets the type to <see cref="ToolType.Function"/>.
        /// </summary>
        /// <param name="function">The function to be used.</param>
        /// <param name="functionDelegate">The delegate that will execute the function.</param>
        public FunctionCall(FunctionDeclaration function)
        {
            Type = ToolType.Function;
            Function = function;
        }

        /// <summary>
        /// Creates a new <see cref="FunctionCall"/> instance with the specified name, description, and delegate.
        /// Using this method won't generate Parameters based on your return type class.
        /// Instead, the return type will be <see cref="string"/>.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="description">The description of the function.</param>
        /// <param name="functionDelegate">The delegate that will execute the function.</param>
        /// <returns>A new instance of the <see cref="FunctionCall"/> class.</returns>
        public FunctionCall(string name, string description)
        {
            Type = ToolType.Function;
            Function = new FunctionDeclaration()
            {
                Name = name,
                Description = description,
            };
        }

        /// <summary>
        /// Creates a new <see cref="FunctionCall"/> instance with the specified name, description, and delegate.
        /// The parameters for the function are generated based on the specified type <typeparamref name="T"/>.
        /// <typeparamref name="T"/> is the return type of the function.
        /// </summary>
        /// <typeparam name="T">The type used to generate the function's parameters schema.</typeparam>
        /// <param name="name">The name of the function.</param>
        /// <param name="description">The description of the function.</param> 
        /// <returns>A new instance of the <see cref="FunctionCall"/> class.</returns>
        public static FunctionCall FromJsonSchema<T>(string name, string description)
            where T : class
        {
            return new FunctionCall(
                new FunctionDeclaration()
                {
                    Name = name,
                    Description = description,
                    Parameters = JsonSchema.Create<T>(),
                }
            );
        }

        internal static FunctionCall Response(string id, string name, string args)
        {
            return new FunctionCall(id, name, args);
        }
    }
}