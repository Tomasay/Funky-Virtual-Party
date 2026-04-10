using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Two responsibilities:
/// 1. One-time menu command to patch all Meta XR .asmdef files to exclude WebGL.
/// 2. Build pre/post processor that hides Meta XR Resources folders during WebGL
///    builds so their assets aren't bundled, then restores them afterward.
///
/// Resources folders are hidden by renaming them to "Resources~" — Unity's
/// convention for folders that are invisible to the asset database.
/// </summary>
public class MetaPackageWebGLExcluder : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    private static readonly string[] PackageFolders =
    {
        "Packages/com.meta.xr.sdk.core@85.0.0",
        "Packages/com.meta.xr.sdk.interaction@85.0.0",
        "Packages/com.meta.xr.sdk.interaction.ovr@85.0.0",
    };

    // -------------------------------------------------------------------------
    // IPreprocessBuildWithReport / IPostprocessBuild
    // -------------------------------------------------------------------------

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL) return;
        Debug.Log("[MetaWebGLExcluder] WebGL build detected — hiding Meta Resources folders.");
        HideMetaResourcesFolders();
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL) return;
        Debug.Log("[MetaWebGLExcluder] Restoring Meta Resources folders after WebGL build.");
        RestoreMetaResourcesFolders();
    }

    // -------------------------------------------------------------------------
    // Resources folder hiding (rename Resources <-> Resources~)
    // -------------------------------------------------------------------------

    private static string ProjectRoot => Path.GetDirectoryName(Application.dataPath);

    private static void HideMetaResourcesFolders()
    {
        foreach (string folder in FindMetaResourcesFolders())
        {
            string hidden = folder + "~";
            if (Directory.Exists(hidden)) continue; // already hidden
            Directory.Move(folder, hidden);

            // Also hide the .meta file Unity generates for the folder, if present.
            string meta = folder + ".meta";
            string hiddenMeta = hidden + ".meta";
            if (File.Exists(meta) && !File.Exists(hiddenMeta))
                File.Move(meta, hiddenMeta);

            Debug.Log($"[MetaWebGLExcluder] Hidden: {folder}");
        }

        AssetDatabase.Refresh();
    }

    private static void RestoreMetaResourcesFolders()
    {
        foreach (string packageFolder in PackageFolders)
        {
            string fullPath = Path.Combine(ProjectRoot, packageFolder);
            if (!Directory.Exists(fullPath)) continue;

            foreach (string hidden in Directory.GetDirectories(fullPath, "Resources~", SearchOption.AllDirectories))
            {
                string original = hidden.Substring(0, hidden.Length - 1); // strip trailing ~
                if (Directory.Exists(original)) continue;
                Directory.Move(hidden, original);

                string hiddenMeta = hidden + ".meta";
                string originalMeta = original + ".meta";
                if (File.Exists(hiddenMeta) && !File.Exists(originalMeta))
                    File.Move(hiddenMeta, originalMeta);

                Debug.Log($"[MetaWebGLExcluder] Restored: {original}");
            }
        }

        AssetDatabase.Refresh();
    }

    private static List<string> FindMetaResourcesFolders()
    {
        var result = new List<string>();
        foreach (string packageFolder in PackageFolders)
        {
            string fullPath = Path.Combine(ProjectRoot, packageFolder);
            if (!Directory.Exists(fullPath)) continue;
            foreach (string dir in Directory.GetDirectories(fullPath, "Resources", SearchOption.AllDirectories))
                result.Add(dir);
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // Safety: restore on domain reload in case a build was interrupted.
    // -------------------------------------------------------------------------

    [InitializeOnLoadMethod]
    private static void RestoreOnDomainReload()
    {
        // Only restore if not currently mid-build (build sets a flag we can check).
        // A simple heuristic: if hidden folders exist outside of a build, restore them.
        bool anyHidden = false;
        foreach (string packageFolder in PackageFolders)
        {
            string fullPath = Path.Combine(ProjectRoot, packageFolder);
            if (!Directory.Exists(fullPath)) continue;
            if (Directory.GetDirectories(fullPath, "Resources~", SearchOption.AllDirectories).Length > 0)
            {
                anyHidden = true;
                break;
            }
        }

        if (anyHidden)
        {
            Debug.Log("[MetaWebGLExcluder] Found hidden Resources~ folders on domain reload — restoring.");
            RestoreMetaResourcesFolders();
        }
    }

    // -------------------------------------------------------------------------
    // One-time menu: patch .asmdef files to exclude WebGL
    // -------------------------------------------------------------------------

    [MenuItem("Tools/Exclude Meta Packages From WebGL")]
    public static void PatchAllAsmdefs()
    {
        int patched = 0;
        int skipped = 0;

        foreach (string packageFolder in PackageFolders)
        {
            string fullPath = Path.Combine(ProjectRoot, packageFolder);
            if (!Directory.Exists(fullPath))
            {
                Debug.LogWarning($"[MetaWebGLExcluder] Folder not found, skipping: {fullPath}");
                continue;
            }

            foreach (string asmdef in Directory.GetFiles(fullPath, "*.asmdef", SearchOption.AllDirectories))
            {
                if (PatchAsmdef(asmdef)) patched++;
                else skipped++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"[MetaWebGLExcluder] Done. Patched: {patched}, Already excluded / skipped: {skipped}");
        EditorUtility.DisplayDialog(
            "Meta WebGL Excluder",
            $"Patched {patched} asmdef file(s).\n{skipped} were already correct or skipped.",
            "OK");
    }

    private static bool PatchAsmdef(string path)
    {
        string text = File.ReadAllText(path);

        // If includePlatforms is non-empty, WebGL is excluded by omission — skip.
        var includeMatch = Regex.Match(text, @"""includePlatforms""\s*:\s*\[([^\]]*)\]");
        if (includeMatch.Success && includeMatch.Groups[1].Value.Trim().Length > 0)
            return false;

        // Already excluded.
        if (Regex.IsMatch(text, @"""excludePlatforms""\s*:\s*\[[^\]]*""WebGL""[^\]]*\]"))
            return false;

        string newText;
        var excludeMatch = Regex.Match(text, @"""excludePlatforms""\s*:\s*\[([^\]]*)\]");
        if (excludeMatch.Success)
        {
            string inner = excludeMatch.Groups[1].Value.Trim();
            string replacement = inner.Length > 0
                ? $"\"excludePlatforms\": [\n        {inner},\n        \"WebGL\"\n    ]"
                : "\"excludePlatforms\": [\n        \"WebGL\"\n    ]";
            newText = text.Remove(excludeMatch.Index, excludeMatch.Length).Insert(excludeMatch.Index, replacement);
        }
        else
        {
            var insertAfter = Regex.Match(text, @"""includePlatforms""\s*:\s*\[[^\]]*\]");
            if (!insertAfter.Success)
            {
                Debug.LogWarning($"[MetaWebGLExcluder] Could not find insertion point in: {Path.GetFileName(path)}");
                return false;
            }
            int insertPos = insertAfter.Index + insertAfter.Length;
            newText = text.Insert(insertPos, ",\n    \"excludePlatforms\": [\n        \"WebGL\"\n    ]");
        }

        File.WriteAllText(path, newText);
        Debug.Log($"[MetaWebGLExcluder] Patched: {Path.GetFileName(path)}");
        return true;
    }
}
