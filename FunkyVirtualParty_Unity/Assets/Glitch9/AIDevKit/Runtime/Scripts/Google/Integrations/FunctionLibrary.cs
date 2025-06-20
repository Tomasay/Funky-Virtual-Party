using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glitch9.AIDevKit.Google
{
    public class FunctionLibrary
    {
        public static implicit operator FunctionLibrary(string toolName) => new(toolName);
        public static implicit operator FunctionLibrary(string[] toolNames) => new(toolNames);
        public static implicit operator FunctionLibrary(List<Tool> tools) => new(tools);
        public static implicit operator FunctionLibrary(Tool tool) => new(tool);

        private readonly List<Tool> _tools;
        private readonly Dictionary<string, FunctionDeclaration> _index;
        public bool IsEmpty => _tools.Count == 0;

        public FunctionLibrary(IEnumerable<Tool> tools)
        {
            _tools = MakeTools(tools).ToList();
            _index = new Dictionary<string, FunctionDeclaration>();
            InitializeTools();
        }

        public FunctionLibrary(params Tool[] tools)
        {
            _tools = MakeTools(tools).ToList();
            _index = new Dictionary<string, FunctionDeclaration>();
            InitializeTools();
        }

        public FunctionLibrary(params string[] toolNames)
        {
            _tools = new();

            foreach (string toolName in toolNames)
            {
                Tool tool = new(toolName);
                _tools.Add(tool);
            }

            _index = new Dictionary<string, FunctionDeclaration>();
            InitializeTools();
        }

        private void InitializeTools()
        {

            foreach (Tool tool in _tools)
            {
                foreach (FunctionDeclaration declaration in tool.FunctionDeclarations)
                {
                    string name = declaration.Name;
                    if (!_index.TryAdd(name, declaration))
                    {
                        throw new ArgumentException($"Invalid operation: A `FunctionDeclaration` named '{name}' is already defined. Each `FunctionDeclaration` must have a unique name. Please use a different name.");
                    }
                }
            }
        }

        public FunctionDeclaration this[string name]
        {
            get
            {
                if (!_index.TryGetValue(name, out FunctionDeclaration declaration))
                {
                    throw new KeyNotFoundException($"FunctionDeclaration with name '{name}' not found.");
                }
                return declaration;
            }
        }

        public FunctionDeclaration this[FunctionCall fc] => this[fc.Name];

        // public async UniTask<ContentPart> CallAsync(FunctionCall fc)
        // {
        //     FunctionDeclaration declaration = this[fc];
        //     if (declaration == null || !declaration.IsCallable)
        //     {
        //         return null;
        //     }

        //     FunctionResponse response = await declaration.Invoke(fc);
        //     return new ContentPart { FunctionResponse = response };
        // }

        private static IEnumerable<Tool> MakeTools(IEnumerable<Tool> tools)
        {
            List<Tool> toolList = tools.ToList();
            if (toolList.Count > 1 && toolList.All(t => t.FunctionDeclarations.Count == 1))
            {
                toolList = new List<Tool> { new(toolList.SelectMany(t => t.FunctionDeclarations)) };
            }
            return toolList;
        }

        public List<Tool> ToProto() => _tools;
    }
}