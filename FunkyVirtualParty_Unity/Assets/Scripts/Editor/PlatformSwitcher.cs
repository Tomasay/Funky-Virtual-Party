using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

public class PlatformSwitcher : EditorWindow
{
    [MenuItem("Window/Platform Switcher/Switch to WebGL")]
    public static void SwitchToWebGL()
    {
        //Build Target
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

        //Color Space
        PlayerSettings.colorSpace = ColorSpace.Gamma;

        //Scene list
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/MainMenu/MainMenuClient.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/ChaseGame/ChaseGameClient.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Shootout/ShootoutClient.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Kaiju/KaijuClient.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/VRTistry/VRTistryClient.unity", true));
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        //Normcore Package Version
        Client.Add("com.normalvr.normcore@3.0.0-webgl-alpha.1");
    }

    [MenuItem("Window/Platform Switcher/Switch to Android")]
    public static void SwitchToAndroid()
    {
        //Build Target
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        //Color Space
        PlayerSettings.colorSpace = ColorSpace.Linear;

        //Scene list
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/MainMenu/MainMenu.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/ChaseGame/ChaseGame.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Shootout/Shootout.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Kaiju/Kaiju.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/VRTistry/VRTistry.unity", true));
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        //Normcore Package Version
        Client.Add("com.normalvr.normcore@2.4.2");
    }
}