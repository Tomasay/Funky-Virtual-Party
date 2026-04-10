using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Kamgam.ExcludeFromBuild
{
    /// <summary>
    /// Checks if the build configuration has changed and triggers an event.
    /// BuildTargetChangeDetector, BuildPlatformSettingsChangeDetector and BuildProfileChangeDetector work together
    /// to trigger events only once per switch.
    /// </summary>
    public class BuildTargetChangeDetector : IActiveBuildTargetChanged
    {
        public int callbackOrder => 100;

        private const string SESSION_SCHEDULED_KEY = "Kamgam.BuildTargetChangeDetector.TargetChangedScheduled";

        public static double LastPotentialChangeEventTime;
        public static double LastPotentialChangeEventDeltaTime => EditorApplication.timeSinceStartup - LastPotentialChangeEventTime;
        
        public static double LastChangeEventTime;
        public static double LastChangeEventDeltaTime => EditorApplication.timeSinceStartup - LastChangeEventTime;
        
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            // Now we know the target changed for sure but we still have to wait for the domain reload afterwards.
            // The reload always happens, source: https://docs.unity3d.com/Packages/com.unity.addressables@2.7/manual/build-scripting-recompiling.html
            SessionState.SetBool(SESSION_SCHEDULED_KEY, true);
            
            LastPotentialChangeEventTime = EditorApplication.timeSinceStartup;
        }
        
        [InitializeOnLoadMethod]
        static void initEditorEvents()
        {
            AssemblyReloadEvents.afterAssemblyReload -= onAssemblyReloaded; // <- Just to be future proof
            AssemblyReloadEvents.afterAssemblyReload += onAssemblyReloaded;
        }
        
        private static void onAssemblyReloaded()
        {
            if (SessionState.GetBool(SESSION_SCHEDULED_KEY, false))
            {
                SessionState.EraseBool(SESSION_SCHEDULED_KEY);
                
                LastChangeEventTime = EditorApplication.timeSinceStartup;
                
                var target = EditorUserBuildSettings.activeBuildTarget;
                ExcludeFromBuildController.OnActiveBuildTargetChanged(target);
            }
        }
    }
}
