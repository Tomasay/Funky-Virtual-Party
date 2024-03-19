using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildCommand : MonoBehaviour
{
    static void BuildWebGL()
    {
        // Build the player.\
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {
        "Assets/Scenes/MainMenu/MainMenuClient.unity",
        "Assets/Scenes/ChaseGame/ChaseGameClient.unity",
        "Assets/Scenes/Shootout/ShootoutClient.unity",
        "Assets/Scenes/Kaiju/KaijuClient.unity",
        "Assets/Scenes/VRTistry/VRTistryClient.unity",
        "Assets/Scenes/MazeGame/MazeGameClient.unity" };
        buildPlayerOptions.locationPathName = "Build/WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None; // set whatever you want here
        buildPlayerOptions.targetGroup = BuildTargetGroup.WebGL;
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        BuildPipeline.BuildPlayer(buildPlayerOptions);  // apply the setting changes


    }
}
