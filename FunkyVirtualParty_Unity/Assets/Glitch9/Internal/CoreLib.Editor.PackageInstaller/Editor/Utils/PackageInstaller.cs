using System;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Glitch9.Editor.PackageInstaller
{
    internal static class PackageInstaller
    {
        private const bool kDebugMode = false;

        internal static void EnsureDependency(string symbolDefine, string assemblyName, string packageName, bool required)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (kDebugMode) Debug.Log($"Checking {assemblyName} dependency...");


            if (DefineSymbolUtil.HasDefineSymbol(symbolDefine))
            {
                if (kDebugMode) Debug.Log($"✔ {symbolDefine} define already present!");
                return;
            }

            if (required || kDebugMode) Debug.Log($"✔ {symbolDefine} define not present!");

            if (IsAssemblyPresent(assemblyName))
            {
                DefineSymbolUtil.AddDefineSymbol(symbolDefine);
                Debug.Log($"✔ {symbolDefine} define added!");
                return;
            }

            if (!required) return;

            try
            {
                // don't ask the user to install the package, just install it directly
                // user has to install it anyway. extra step can be annoying
                InstallPackage(packageName);
                Debug.Log($"✔ {assemblyName} package installed!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to install {assemblyName} package: {ex.Message}");
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        private static bool IsAssemblyPresent(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.FullName.Contains(assemblyName));
        }


        private static void InstallPackage(string packageName)
        {
            Debug.Log($"Installing {packageName} package...");
            Client.Add(packageName);
        }
    }
}
