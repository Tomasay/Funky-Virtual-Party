using UnityEditor;

namespace Glitch9.Editor.PackageInstaller
{
    [InitializeOnLoad]
    internal static class UniTaskChecker
    {
        private const string kAssemblyName = "UniTask";
        private const string kSymbolDefine = "CYSHARP_UNITASK";
        private const string kPackageName = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
        private const bool kRequired = true;

        static UniTaskChecker()
        {
            PackageInstaller.EnsureDependency(kSymbolDefine, kAssemblyName, kPackageName, kRequired);
        }
    }
}
