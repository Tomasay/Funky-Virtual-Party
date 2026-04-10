#if UNITY_6000_0_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Profile;

namespace Kamgam.ExcludeFromBuild
{
    public static class BuildProfileUtils
    {
        private static string[] s_buildProfilesFolders = { "Assets/Settings/" };
        private static Dictionary<string, BuildProfile> s_cachedBuildProfiles = new Dictionary<string, BuildProfile>();
        private static List<BuildProfile> s_tmpResults = new List<BuildProfile>();
        
        /// <summary>
        /// The profile used as placeholder for no build profile in a build target (aka "Default" in the list). 
        /// </summary>

        /// <summary>
        /// Returns the build profiles in the project but it does some shenanigans to make this efficient so it can be used
        /// from immediate mode GUIs.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static List<BuildProfile> GetAllBuildProfiles(List<BuildProfile> results = null)
        {
            if (results == null)
                results = s_tmpResults;
            
            results.Clear();

            // Shenanigan #1: only search in certain folder (notice: singular, despite being a array!).
            if (System.IO.Directory.Exists(s_buildProfilesFolders[0]))
            {
                string[] guids = AssetDatabase.FindAssets("t:BuildProfile", s_buildProfilesFolders);
                foreach (var guid in guids)
                {
                    // Shenanigan #2: cache by guid and load asset only if necessary.
                    BuildProfile profile;
                    if (s_cachedBuildProfiles.TryGetValue(guid, out profile))
                    {
                        results.Add(profile);
                    }
                    else
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
                        results.Add(profile);
                        s_cachedBuildProfiles.Add(guid, profile);
                    }
                }
            }

            return results;
        }

        public static BuildProfile GetCurrentBuildProfile()
        {
            return BuildProfile.GetActiveBuildProfile();
        }
    }
}
#endif