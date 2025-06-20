using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal enum GeneratorWindowType
    {
        CodeGenerator,
        UxmlGenerator,
        IconGenerator,
        AvatarGenerator,
        BackgroundGenerator,
        TextureGenerator,
        SoundFXGenerator,
        SpeechGenerator,
        VideoGenerator,
    }

    internal static class GeneratorWindowTypeExtensions
    {
        // icon
        internal static Texture2D GetIcon(this GeneratorWindowType type)
        {
            return type switch
            {
                GeneratorWindowType.CodeGenerator => EditorIcons.CsScript as Texture2D,
                GeneratorWindowType.UxmlGenerator => EditorIcons.UxmlScript as Texture2D,
                GeneratorWindowType.IconGenerator => AIDevKitIcons.Image,
                GeneratorWindowType.AvatarGenerator => EditorIcons.Portrait as Texture2D,
                GeneratorWindowType.BackgroundGenerator => EditorIcons.ImageIcon as Texture2D,
                GeneratorWindowType.TextureGenerator => EditorIcons.Texture as Texture2D,
                GeneratorWindowType.SoundFXGenerator => EditorIcons.AudioClip as Texture2D,
                GeneratorWindowType.SpeechGenerator => AIDevKitIcons.TextToSpeech,
                GeneratorWindowType.VideoGenerator => EditorIcons.VideoPlayer as Texture2D,
                _ => null,
            };
        }

        // tab label
        internal static string GetLabel(this GeneratorWindowType type)
        {
            return type switch
            {
                GeneratorWindowType.CodeGenerator => "C# Script",
                GeneratorWindowType.UxmlGenerator => "UXML(UI Toolkit)",
                GeneratorWindowType.IconGenerator => "Icon Sprite",
                GeneratorWindowType.AvatarGenerator => "Avatar Sprite",
                GeneratorWindowType.BackgroundGenerator => "Background Sprite",
                GeneratorWindowType.TextureGenerator => "Mesh Texture",
                GeneratorWindowType.SoundFXGenerator => "Sound FX",
                GeneratorWindowType.SpeechGenerator => "Voice Clip",
                GeneratorWindowType.VideoGenerator => "Video",
                _ => type.ToString(),
            };
        }

        internal static GUIContent GetWindowTitle(this GeneratorWindowType type)
        {
            return type switch
            {
                GeneratorWindowType.CodeGenerator => new GUIContent("C# Script Generator", type.GetIcon()),
                GeneratorWindowType.UxmlGenerator => new GUIContent("UXML Generator", type.GetIcon()),
                GeneratorWindowType.IconGenerator => new GUIContent("Icon Sprite Generator", type.GetIcon()),
                GeneratorWindowType.AvatarGenerator => new GUIContent("Avatar Sprite Generator", type.GetIcon()),
                GeneratorWindowType.BackgroundGenerator => new GUIContent("Background Sprite Generator", type.GetIcon()),
                GeneratorWindowType.TextureGenerator => new GUIContent("Mesh Texture Generator", type.GetIcon()),
                GeneratorWindowType.SoundFXGenerator => new GUIContent("Sound FX Generator", type.GetIcon()),
                GeneratorWindowType.SpeechGenerator => new GUIContent("Voice Clip Generator", type.GetIcon()),
                GeneratorWindowType.VideoGenerator => new GUIContent("Video File Generator", type.GetIcon()),
                _ => new GUIContent(type.ToString(), type.GetIcon()),
            };
        }
    }

    [InitializeOnLoad]
    internal class GeneratorHubWindow : EditorWindow
    {
        static GeneratorHubWindow()
        {
            AIDevKitEditor.onShowGeneratorHubWindow += () => ShowWindow(GeneratorStyles.LastGeneratorWindowType, AIDevKitSettings.OutputPath);
        }

        internal static void ShowWindow(GeneratorWindowType type, string selectedPath)
        {
            var title = type.GetWindowTitle();
            var wnd = GetWindow<GeneratorHubWindow>(title.text, true);
            wnd.titleContent = title;
            GeneratorStyles.LastGeneratorWindowType = type;
            wnd._currentPath = selectedPath;
        }

        private string _currentPath;
        //private TabView _tabView;

        private async UniTask<VisualElement> CreateWindowViewAsync(GeneratorWindowType type)
        {
            return type switch
            {
                GeneratorWindowType.CodeGenerator => await CodeGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.UxmlGenerator => await UxmlGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.IconGenerator => await IconGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.AvatarGenerator => await AvatarGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.BackgroundGenerator => await BackgroundGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.TextureGenerator => await TextureGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.SoundFXGenerator => await SoundFXGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.SpeechGenerator => await SpeechGeneratorWindow.CreateViewAsync(_currentPath),
                GeneratorWindowType.VideoGenerator => await VideoGeneratorWindow.CreateViewAsync(_currentPath),
                _ => null,
            };
        }

        /*
        private async UniTask<TabView> BuildTapViewAsync()
        {
            AIDevKitDebug.Green("Building TabView for GeneratorHubWindow");
            var tabView = rootVisualElement.Q<TabView>("tab-view");

            if (tabView == null)
            {
                Debug.LogError("TabView not found in the root visual element.");
                return null;
            }

            Dictionary<GeneratorWindowType, VisualElement> windowViews = new();

            foreach (var rawType in Enum.GetValues(typeof(GeneratorWindowType)))
            {
                var type = (GeneratorWindowType)rawType;

                var windowView = await CreateWindowViewAsync(type);
                if (windowView == null)
                {
                    Debug.LogError($"Failed to create view for {type}");
                    continue;
                }

                windowView.AddToClassList("child-root");
                windowViews.Add(type, windowView);
            }

            foreach (var kvp in windowViews)
            {
                var type = kvp.Key;

                var tab = new Tab
                {
                    iconImage = type.GetIcon(),
                    name = type.ToString(),
                    dataSource = type,
                    viewDataKey = type.ToString(),
                    label = type.GetLabel(),
                };

                var windowView = kvp.Value;
                if (windowView == null)
                {
                    Debug.LogError($"Failed to create view for {type}");
                    continue;
                }
                windowView.AddToClassList("child-root");
                tab.Add(windowView);
                tabView.Add(tab);
            }

            tabView.selectedTabIndex = (int)GeneratorStyles.LastGeneratorWindowType;

            tabView.activeTabChanged += (tab1, tab2) =>
            {
                if (tab2 == null) return;
                var newType = (GeneratorWindowType)tab2.dataSource;
                titleContent = newType.GetWindowTitle();
                GeneratorStyles.LastGeneratorWindowType = newType; // 마지막으로 선택한 탭을 저장
            };

            return tabView;
        }
        */

        public void CreateGUI() => rootVisualElement.schedule.Execute(CreateGUIAfterDelay).ExecuteLater(1); // 1프레임 뒤에 실행 

        internal async void CreateGUIAfterDelay()
        {
            AIDevKitDebug.Mark("GeneratorHubWindow.RebuildUI");
            // 최상단에 window를 고를 수 있는 탭을 넣고 그 아래는 _currentWindow를 그대로 표시한다.

            UIToolkitUtil.InitializeRootElement(
                root: rootVisualElement,
                style: "GeneratorWindowHub",
                marker: "styles_generator"
            );

            //_tabView ??= await BuildTapViewAsync();
            //rootVisualElement.Add(_tabView);
        }
    }
}