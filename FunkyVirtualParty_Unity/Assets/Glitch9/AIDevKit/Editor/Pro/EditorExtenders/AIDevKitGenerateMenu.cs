using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.AIDevKit.Editor.UIToolKit;
using UnityEditor;
//using UnityEditor.Callbacks;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    // internal static class GeneratorOpener
    // {
    //     [OnOpenAsset]
    //     internal static bool OnOpenGenerator(int instanceID, int line)
    //     {
    //         Object obj = EditorUtility.InstanceIDToObject(instanceID);
    //         if (obj is GeneratorScriptableObject genAsset)
    //         {
    //             // GeneratorScriptableObject를 통해 창 타입을 가져옴
    //             GeneratorWindowType windowType = genAsset.windowType;
    //             GeneratorHubWindow.ShowWindow(windowType, AssetDatabase.GetAssetPath(obj));
    //             return true; // 핸들링 완료
    //         }

    //         return false; // 기본 동작 유지
    //     }
    // }

    // internal class GeneratorScriptableObject : ScriptableObject
    // {
    //     internal GeneratorWindowType windowType;
    // }

    internal static class AIDevKitGenerateMenu
    {
        private const int kStartingPriority = 10;
        private const string kMenuBase = "Assets/Generate with AI/";
        private const string kCodeMenu = "Script && Text/";
        private const string kImageMenu = "Sprite && Texture/";
        private const string kAudioMenu = "Voice && Audio/";
        private const string kVideoMenu = "Video/";

        // Generate Menu -----------------------------------------------------------

        // 테스트
        // [MenuItem("Open Hub", false, kStartingPriority - 1)]
        // private static void OpenHub(MenuCommand menuCommand)
        // {
        //     GeneratorHubWindow.ShowWindow(GeneratorWindowType.CodeGenerator, GetCurrentPath());
        // } 

        // [CreateAssetMenu(menuName = "AI Dev Kit/Script Generator")]
        // public class ScriptGenerator : GeneratorScriptableObject
        // {
        //     public string someSetting;
        // }

        [MenuItem(kMenuBase + kCodeMenu + "C# Script", false, kStartingPriority)]
        private static void GenerateCSharpScript(MenuCommand menuCommand)
        {
            //CodeGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.CodeGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kCodeMenu + "UXML File", false, kStartingPriority + 1)]
        private static void GenerateUXML(MenuCommand menuCommand)
        {
            //UxmlGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.UxmlGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kImageMenu + "Icon Sprite", false, kStartingPriority + 2)]
        private static void GenerateIcon(MenuCommand menuCommand)
        {
            //IconGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.IconGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kImageMenu + "Avatar Sprite", false, kStartingPriority + 3)]
        private static void GenerateAvatar(MenuCommand menuCommand)
        {
            //AvatarGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.AvatarGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kImageMenu + "Background Sprite", false, kStartingPriority + 4)]
        private static void GenerateBackground(MenuCommand menuCommand)
        {
            //BackgroundGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.BackgroundGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kImageMenu + "Mesh Texture", false, kStartingPriority + 5)]
        private static void GenerateTexture(MenuCommand menuCommand)
        {
            //TextureGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.TextureGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kAudioMenu + "Sound Effect", false, kStartingPriority + 6)]
        private static void GenerateSoundEffect(MenuCommand menuCommand)
        {
            // SoundFXGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.SoundFXGenerator, GetCurrentPath());
        }

        [MenuItem(kMenuBase + kAudioMenu + "Voice Clip", false, kStartingPriority + 7)]
        private static void GenerateVoiceClip(MenuCommand menuCommand)
        {
            //SpeechGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.SpeechGenerator, GetCurrentPath());
        }

        // [MenuItem("Audio/Music Track", false, kStartingPriority + 7)]
        // private static void GenerateMusicTrack(MenuCommand menuCommand)
        // {
        //     //MusicGenWindow.ShowWindow(GetCurrentPath());
        //     MusicGeneratorWindow.ShowWindow(GetCurrentPath());
        // }

        [MenuItem(kMenuBase + kVideoMenu + " Video File", false, kStartingPriority + 8)]
        private static void GenerateVideo(MenuCommand menuCommand)
        {
            //VideoGeneratorWindow.ShowWindow(GetCurrentPath());
            GeneratorHubWindow.ShowWindow(GeneratorWindowType.VideoGenerator, GetCurrentPath());
        }

        private static string GetCurrentPath()
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!System.IO.Directory.Exists(selectedPath)) throw new System.Exception("Selected path is not a directory.");
            return selectedPath.Replace("Assets/", string.Empty);
        }

        internal static bool OpenChatSaveFolder()
        {
            string folderPath = ChatSessionUtil.GetSavePath();

            if (System.IO.Directory.Exists(folderPath))
            {
                EditorUtility.RevealInFinder(folderPath + "/");
                return true;
            }

            return false;
        }

        // Context Menu -----------------------------------------------------------
        [MenuItem("CONTEXT/MonoBehaviour/Edit Script with AI", false, 610)]
        private static void EditScriptWithAI(MenuCommand menuCommand)
        {
            if (menuCommand.context is MonoBehaviour monoBehaviour)
            {
                MonoScript script = MonoScript.FromMonoBehaviour(monoBehaviour);
                if (script != null)
                {
                    ComponentEditorWindow.ShowWindow(script);
                }
                else
                {
                    Debug.LogWarning("Context menu item 'Edit Script with AI' can only be used on a MonoBehaviour with an associated MonoScript.");
                }
            }
            else
            {
                Debug.LogWarning("Context menu item 'Edit Script with AI' can only be used on a MonoBehaviour.");
            }
        }
    }
}