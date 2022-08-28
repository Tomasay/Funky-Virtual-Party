using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace  ambiens.webgltemplate
{
    public class WebGLTemplateInstaller : EditorWindow
    {

        public static WebGLTemplateInstaller instance;
        [MenuItem("Tools/Ambiens/MultiPlatform WebGL Template/Installer")]
        public static void Init()
        {

            var window = (WebGLTemplateInstaller)EditorWindow.GetWindow(typeof(WebGLTemplateInstaller), false, "WebGL Template Installer");

            window.maxSize = new Vector2(600, 600);
            window.minSize = new Vector2(600, 600);

            instance = window;

            window.Show();
        }


        void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            
            //HTML 5 LOGO
            //Name
            //Made with love by Ambiens VR
            
            GUILayout.EndHorizontal();

            GUILayout.Space(10);


            GUILayout.BeginVertical();
            
            GUILayout.Label("This procedure will automatically set up the project for multiplatform WebGL. \n \n In particular it will:");
            GUILayout.Label("- Switch the platform to WebGL");
            GUILayout.Label("- Set the color space to Gamma");
            GUILayout.Label("- Set Lightmap Encoding to Normal Quality");
            GUILayout.Label("- Disable the compression for WebGL ");
            
            GUILayout.EndVertical();
            
            GUILayout.Space(10);

            GUILayout.BeginVertical();
            
            GUILayout.Label("This procedure will take a while and after that possibly will force you to rebake lightmaps for scenes.");

            if(GUILayout.Button("Ok, I'll be fine, DO IT!")){
                ApplyOptions();
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();

        }

        void ApplyOptions()
        {
             
            //Switch Platform
            if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL) {
                if(!EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTargetGroup.WebGL, BuildTarget.WebGL )) {
                    Debug.Log("<color=red>Multiplatform WebGL Template</color> Error switching platform, is it installed already?");
                    return;
                }
            }
             

            //Color -> Gamma
            PlayerSettings.colorSpace = ColorSpace.Gamma;
            
            //Disable WebGL Compression
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            
            //WebGL Graphics API 2 & 1
            PlayerSettings.SetGraphicsAPIs( BuildTarget.WebGL, new UnityEngine.Rendering.GraphicsDeviceType[]{ 
                UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 } );

            //Encoding quality of Lightmaps to normal
            SerializedObject playerSettingsSo = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);

            var lmEnc=playerSettingsSo.FindProperty("m_BuildTargetGroupLightmapEncodingQuality");
            bool found=false;
            for( int i=0; i<lmEnc.arraySize; i++){
                
                var target= lmEnc.GetArrayElementAtIndex(i);
                if(target.FindPropertyRelative("m_BuildTarget").stringValue == "WebGL"){
                    found=true;
                    target.FindPropertyRelative("m_EncodingQuality").intValue=1;
                }
            }
            if(!found){
                lmEnc.arraySize++;
                var target= lmEnc.GetArrayElementAtIndex(lmEnc.arraySize-1);
                target.FindPropertyRelative("m_BuildTarget").stringValue = "WebGL";
                target.FindPropertyRelative("m_EncodingQuality").intValue=1;
            }
            playerSettingsSo.ApplyModifiedProperties();

            //If it does not exists-> copy the template files
            var pluginPath =  Path.Combine(Application.dataPath, "Ambiens","MultiPlatformWebGLTemplate", "Template", "Ambiens");
            var templatesRoot = Path.Combine(Application.dataPath, "WebGLTemplates");
            var templatePath = Path.Combine(templatesRoot, "Ambiens");
            //"WebGLTemplates";
            if(!Directory.Exists( templatePath)){
                Directory.CreateDirectory( templatesRoot);
                FileUtil.CopyFileOrDirectory(pluginPath, templatePath);
                AssetDatabase.Refresh();
            }

            //Set the template
            PlayerSettings.WebGL.template= templatePath;

            Debug.Log("<color=green>Multiplatform WebGL Template</color> No error here, everything should work fine now");

           
        }

    }

}
