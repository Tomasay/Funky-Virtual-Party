using UnityEditor;

namespace Glitch9.Editor
{
    internal static class UnityEditorCompat
    {
        public static string GetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup)
        {
#if UNITY_6000_0_OR_NEWER
            UnityEditor.Build.NamedBuildTarget buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            return PlayerSettings.GetScriptingDefineSymbols(buildTarget);
#else
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif
        }

        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup, string define)
        {
#if UNITY_6000_0_OR_NEWER
            UnityEditor.Build.NamedBuildTarget buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, define);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, define);
#endif
        }

        public static string SetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup)
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