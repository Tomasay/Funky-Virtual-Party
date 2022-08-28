using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildUnityPlayer : MonoBehaviour
{
    public static void PerformBuild()
    {
        // You must add the scenes in this array or they will not be included in the build
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { 
        "Scenes/MainMenu/MainMenuClient.unity", 
        "Scenes/ChaseGame/ChaseGameClient.unity",
        "Scenes/Shootout/ShootoutClient.unity" };
        buildPlayerOptions.locationPathName = "Build/WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None; // set whatever you want here
        BuildPipeline.BuildPlayer(buildPlayerOptions);  // apply the setting changes
    }
}
