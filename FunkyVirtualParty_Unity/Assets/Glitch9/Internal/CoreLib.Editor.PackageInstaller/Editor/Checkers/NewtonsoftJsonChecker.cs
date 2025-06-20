using UnityEditor;

namespace Glitch9.Editor.PackageInstaller
{
    [InitializeOnLoad]
    internal static class NewtonsoftJsonChecker
    {
        private const string kAssemblyName = "Newtonsoft.Json";
        private const string kSymbolDefine = "UNITY_NEWTONSOFT_JSON";
        private const string kPackageName = "com.unity.nuget.newtonsoft-json";
        private const bool kRequired = true;

        static NewtonsoftJsonChecker()
        {
            PackageInstaller.EnsureDependency(kSymbolDefine, kAssemblyName, kPackageName, kRequired);
        }
    }
}
