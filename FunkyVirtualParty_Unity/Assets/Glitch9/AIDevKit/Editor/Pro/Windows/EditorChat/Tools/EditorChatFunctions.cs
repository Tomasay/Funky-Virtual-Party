

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static class EditorChatFunctions
    {
        private static class Names
        {
            internal const string GetActiveSelection = "get_active_selection";
        }

        // private class GetActiveSelectionDelegate : IFunctionDelegate
        // {
        //     public string FunctionName => Names.GetActiveSelection;

        //     public async UniTask<Result<string>> Invoke(string argument)
        //     {
        //         await UniTask.Yield();
        //         UnityEngine.Object selected = Selection.activeObject;
        //         UnityObjectProfile profile = selected != null
        //             ? new UnityObjectProfile(selected)
        //             : UnityObjectProfile.Null;
        //         return Result<string>.Success(JsonConvert.SerializeObject(profile, JsonConfig.DefaultSerializerSettings));
        //     }
        // }


        private static FunctionDeclaration CreateGetActiveSelectionFunction()
        {
            return FunctionDeclaration.FromSchema<UnityObjectProfile>(
                Names.GetActiveSelection,
                "Get the currently selected Unity object in the editor."
            );
        }

        internal static FunctionDeclaration[] Get()
        {
            return new FunctionDeclaration[]
            {
                CreateGetActiveSelectionFunction()
            };
        }
    }
}