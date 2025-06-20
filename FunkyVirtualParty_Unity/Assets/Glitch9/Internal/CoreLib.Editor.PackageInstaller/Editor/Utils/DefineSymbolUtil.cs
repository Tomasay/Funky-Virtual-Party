using UnityEditor;

namespace Glitch9.Editor.PackageInstaller
{
    internal static class DefineSymbolUtil
    {
        internal static bool HasDefineSymbol(string define)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            return GetScriptingDefineSymbolsForGroup(target).Contains(define);
        }

        internal static void AddDefineSymbol(string define)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = GetScriptingDefineSymbolsForGroup(target);
            if (!defines.Contains(define))
            {
                SetScriptingDefineSymbolsForGroup(target, defines + ";" + define);
            }
        }

        private static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup, string define)
        {
#if UNITY_6000_0_OR_NEWER
            UnityEditor.Build.NamedBuildTarget buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, define);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, define);
#endif
        }

        private static string GetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup)
        {
#if UNITY_6000_0_OR_NEWER
            UnityEditor.Build.NamedBuildTarget buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            return PlayerSettings.GetScriptingDefineSymbols(buildTarget);
#else
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif
        }
    }
}
