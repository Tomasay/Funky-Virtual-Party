using UnityEngine;
using UnityEditor;

namespace Kamgam.ExcludeFromBuild
{
    // Using this because InitializeOnLoad is unreliable if assets need to be created during that callback.
    // see: https://docs.unity3d.com/ScriptReference/InitializeOnLoadAttribute.html
    // Asset operations such as asset loading should be avoided in InitializeOnLoad methods. InitializeOnLoad methods are
    // called before asset importing is completed and therefore the asset loading can fail resulting in a null object.
    // To do initialization after a domain reload which requires asset operations use the
    // AssetPostprocessor.OnPostprocessAllAssets callback
    /// <summary>
    /// Checks if the build configuration has changed and triggers an event.
    /// BuildTargetChangeDetector, BuildPlatformSettingsChangeDetector and BuildProfileChangeDetector work together
    /// to trigger events only once per switch.
    /// </summary>
    class BuildPlatformSettingsChangeDetector : AssetPostprocessor
    {
        private static bool s_processed = false;

#if UNITY_2021_2_OR_NEWER
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
#else
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
#endif
        {
            if (s_processed)
                return;
    
            s_processed = true;

            init();
        }

        // Can not be done in init() since these need to be re-registered with every domain reload.
        [InitializeOnLoadMethod]
        static void initEditorEvents()
        {
            EditorApplication.update -= OnEditorUpdate; // <- Just to be future proof
            EditorApplication.update += OnEditorUpdate;
        }

        private static bool? s_lastKnownDevelopmentValue;
        
        private const string SESSION_FIRST_BOOT_KEY = "Kamgam.BuildPlatformSettingsChangeDetector.FirstBoot";

        private const string SESSION_PROFILE_IS_DEVELOPMENT_KEY = "Kamgam.BuildPlatformSettingsChangeDetector.ProfileGUID";
        private static bool s_isDevelopmentBuild;
        
        // During InitializeOnLoadMethod the assets are not guaranteed to be loaded.
        // That's why we use AssetPostprocessor to trigger init() and then reliably
        // fetch the platform settings.
        static void init()
        {
            // First boot?
            if (SessionState.GetBool(SESSION_FIRST_BOOT_KEY, true))
            {
                SessionState.SetBool(SESSION_FIRST_BOOT_KEY, false);
                saveCurrentFlag();
            }
        }

        private static void saveCurrentFlag()
        {
            SessionState.SetBool(SESSION_PROFILE_IS_DEVELOPMENT_KEY, getCurrentFlag());
        }

        private static bool getCurrentFlag()
        {
            return EditorUserBuildSettings.development;
        }

        private static bool loadSavedFlag()
        {
            return SessionState.GetBool(SESSION_PROFILE_IS_DEVELOPMENT_KEY, false);
        }

        private static void OnEditorUpdate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (EditorApplication.isCompiling)
                return;

            bool isDevelopmentBuild = EditorUserBuildSettings.development;
            if (!s_lastKnownDevelopmentValue.HasValue || isDevelopmentBuild != s_lastKnownDevelopmentValue.Value)
            {
                s_lastKnownDevelopmentValue = isDevelopmentBuild;
                
                // Check if really changed or changed only due to domain reload.
                var saved = loadSavedFlag();
                var current = getCurrentFlag();

                if (saved != current)
                {
                    saveCurrentFlag();

                    // Now we know the value changed for sure.
                    // However this also happens automatically if the build target or profile was changed.
                    // We want to ignore these. Sadly a timing based fix is currently the best solution.
                    double executeIfLastChangeWasNSecondsAgo = 3;
                    if (BuildTargetChangeDetector.LastPotentialChangeEventDeltaTime > executeIfLastChangeWasNSecondsAgo
                        && BuildTargetChangeDetector.LastChangeEventDeltaTime > executeIfLastChangeWasNSecondsAgo
#if UNITY_6000_0_OR_NEWER
                        && BuildProfileChangeDetector.LastPotentialChangeEventDeltaTime > executeIfLastChangeWasNSecondsAgo
#endif
)
                    {
                        ExcludeFromBuildController.OnPlatformSettingsDevelopmentChanged(isDevelopmentBuild);
                    }
                }                 
            }
        }
    }
}