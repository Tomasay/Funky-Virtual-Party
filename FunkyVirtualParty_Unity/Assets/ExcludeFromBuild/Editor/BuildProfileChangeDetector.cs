#if UNITY_6000_0_OR_NEWER
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Profile;

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
    class BuildProfileChangeDetector : AssetPostprocessor
    {
        private static bool s_processed = false;

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
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
            
            AssemblyReloadEvents.afterAssemblyReload -= onAssemblyReloaded; // <- Just to be future proof
            AssemblyReloadEvents.afterAssemblyReload += onAssemblyReloaded;
        }

        private static BuildProfile s_lastSelectedProfile;
        private static bool s_lastSelectedProfileInitialized;
        
        private const string SESSION_FIRST_BOOT_KEY = "Kamgam.BuildProfileChangeDetector.FirstBoot";

        private const string SESSION_PROFILE_GUID_KEY = "Kamgam.BuildProfileChangeDetector.ProfileGUID";
        private static string s_profileGuid;
        
        private const string SESSION_SCHEDULED_KEY = "Kamgam.BuildProfileChangeDetector.ProfileChangedScheduled";

        public static double LastPotentialChangeEventTime;
        public static double LastPotentialChangeEventDeltaTime => EditorApplication.timeSinceStartup - LastPotentialChangeEventTime;
        
        // During InitializeOnLoadMethod the assets are not guaranteed to be loaded.
        // That's why we use AssetPostprocessor to trigger init() and then reliably
        // fetch the profile guid.
        static void init()
        {
            // First boot?
            if (SessionState.GetBool(SESSION_FIRST_BOOT_KEY, true))
            {
                SessionState.SetBool(SESSION_FIRST_BOOT_KEY, false);
                saveCurrentProfileGuid();
            }
        }

        private static void saveCurrentProfileGuid()
        {
            var guid = getCurrentProfileGuid();
            SessionState.SetString(SESSION_PROFILE_GUID_KEY, guid);
        }

        private static string getCurrentProfileGuid()
        {
            var profile = BuildProfileUtils.GetCurrentBuildProfile();

            string guid = null;
            if (profile != null)
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(profile, out guid, out _);
            return guid;
        }

        private static string loadSavedProfileGuid()
        {
            var guid = SessionState.GetString(SESSION_PROFILE_GUID_KEY, null);
            if (string.IsNullOrEmpty(guid)) // Unity is being funny and returns "" instead of null, *sigh*.
                guid = null;
            
            return guid;
        }

        private static void OnEditorUpdate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (EditorApplication.isCompiling)
                return;
                
            var profile = BuildProfileUtils.GetCurrentBuildProfile();
            if (profile != s_lastSelectedProfile || (!s_lastSelectedProfileInitialized && loadSavedProfileGuid() != null))
            {
                s_lastSelectedProfile = profile;

                LastPotentialChangeEventTime = EditorApplication.timeSinceStartup;

                // Check if really changed or changed ony due to domain reload.
                var savedGuid = loadSavedProfileGuid();
                var currentGuid = getCurrentProfileGuid();

                if (savedGuid != currentGuid)
                {
                    saveCurrentProfileGuid();

                    // Now we know the profile changed for sure but we still have to wait for the domain reload afterwards.
                    // The reload always happens, source: https://docs.unity3d.com/Packages/com.unity.addressables@2.7/manual/build-scripting-recompiling.html
                    SessionState.SetBool(SESSION_SCHEDULED_KEY, true);
                }                 
            }
            
            s_lastSelectedProfileInitialized = true;
        }

        private static void onAssemblyReloaded()
        {
            if (SessionState.GetBool(SESSION_SCHEDULED_KEY, false))
            {
                SessionState.EraseBool(SESSION_SCHEDULED_KEY);
                
                // Sadly a timing based fix to check if the profile changed due to a target change (if yes, then ignore)
                if (BuildTargetChangeDetector.LastChangeEventDeltaTime > 3)
                {
                    // Now, after the domain reload we can safely trigger the profile changed event.
                    var profile = BuildProfileUtils.GetCurrentBuildProfile();
                    ExcludeFromBuildController.OnBuildProfileChanged(profile);
                }
            }
        }
    }
}
#endif
