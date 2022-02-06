using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ambiens.webgltemplate
{
    class WebGLBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        var textures=PlayerSettings.GetIconsForTargetGroup( BuildTargetGroup.Unknown);
        if(textures.Length==1)
        {
            var iconPath=AssetDatabase.GetAssetPath(textures[0]);
            AssetDatabase.CopyAsset(iconPath, "Assets/WebGLTemplates/Ambiens/Template/logo.png" );
        }
    }
}
}

