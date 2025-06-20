using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using Glitch9.IO.Files;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public partial class PromptHistoryWindow
    {
        public class PromptHistoryTreeViewDetailsWindow : ExtendedTreeViewDetailsWindow
        {
            private class AssetLoadState<TAsset>
            {
                public bool IsLoaded;
                public bool IsLoading;
                public TAsset[] Assets;
                public string[] Paths;
            }

            private enum ContentType { Input, Output, }
            private static class Labels
            {
                internal const string LABEL_API = "API";
                internal const string LABEL_MODEL_ID = "Model ID";
                internal const string LABEL_MODEL_NAME = "Model Name";

                internal const string Usage = "Usage Details";
                internal const string Metadata = "Metadata";

                internal const string InputPrompt = "Prompt Text";
                internal const string InputFile = "Input File(s)";
                internal const string InputAudio = "Input Audio";
                internal const string InputImage = "Input Image(s)";

                internal const string OutputText = "Generated Text";
                internal const string OutputFile = "Output File(s)";
                internal const string OutputImage = "Generated Image(s)";
                internal const string OutputAudio = "Generated Audio";
            }

            protected const float BTN_HEIGHT = 28f;
            private const float kAssetContentSize = 80f;
            private const float kMinTextContentHeight = 50f;
            private const float kMaxTextContentHeight = 300f;

            private string DateString
            {
                get
                {
                    if (string.IsNullOrEmpty(dateString))
                    {
                        dateString = Data?.CreatedAt == null ? "-" : Data.CreatedAt.ToString("yyyy-MM-dd HH:mm tt");
                    }
                    return dateString;
                }
            }
            private string dateString;

            string _inputText;
            List<RawFile> _inputFiles;
            List<File<Texture2D>> _inputImageFiles;
            List<File<AudioClip>> _inputAudioFiles;

            List<string> _outputTexts;
            List<RawFile> _outputFiles;
            List<File<Texture2D>> _outputImageFiles;
            List<File<AudioClip>> _outputAudioFiles;

            private readonly AssetLoadState<byte[]> _fileState = new();
            private readonly AssetLoadState<Texture2D> _imageState = new();
            private readonly AssetLoadState<AudioClip> _audioState = new();

            private Vector2 _inputScrollPos = Vector2.zero;
            private Vector2 _outputScrollPos = Vector2.zero;

            private bool _contentsCached;

            protected override GUIContent CreateTitle() => new(EndpointType.GetName(Data.TaskType) + " Request");

            protected override void DrawSubtitle()
            {
                string sender = string.IsNullOrEmpty(Data.Sender) ? "-" : Data.Sender;
                TreeViewGUI.Subtitle($"Sender: {sender}", $"Created At: {DateString}");
            }

            private void CacheContents()
            {
                if (_contentsCached) return;
                _contentsCached = true;

                _inputText = Data.InputText;
                _outputTexts = Data.OutputTexts ?? new List<string>();

                _inputFiles = new List<RawFile>();
                _outputFiles = new List<RawFile>();

                _inputImageFiles = new List<File<Texture2D>>();
                _outputImageFiles = new List<File<Texture2D>>();

                _inputAudioFiles = new List<File<AudioClip>>();
                _outputAudioFiles = new List<File<AudioClip>>();

                foreach (var content in Data.InputFiles)
                {
                    if (content is File<Texture2D> imageFile)
                    {
                        _inputImageFiles.Add(imageFile);
                    }
                    else if (content is File<AudioClip> audioFile)
                    {
                        _inputAudioFiles.Add(audioFile);
                    }
                    else if (content is RawFile file)
                    {
                        _inputFiles.Add(file);
                    }
                }

                foreach (var content in Data.OutputFiles)
                {
                    if (content is RawFile file)
                    {
                        _outputFiles.Add(file);
                    }
                    else if (content is File<Texture2D> imageFile)
                    {
                        _outputImageFiles.Add(imageFile);
                    }
                    else if (content is File<AudioClip> audioFile)
                    {
                        _outputAudioFiles.Add(audioFile);
                    }
                }
            }

            protected override void DrawBody()
            {
                CacheContents();

                DrawRequestDetails(Data.Api, Data.ModelId, Data.ModelName, Data.RequestOptions);

                GUILayout.Space(3f);

                DrawInputContents();
                DrawOutputContents();

                if (Data.Usage != null && !Data.Usage.IsEmpty)
                {
                    GUILayout.Space(3f);
                    DrawUsageDetails(Data.Usage, Data.Price);
                }

                GUILayout.Space(10f);
            }

            private void DrawInputContents()
            {
                TextContentField(0, ContentType.Input, _inputText);
                if (_inputFiles.Count > 0) FileContentField(ContentType.Input, _inputFiles);
                if (_inputImageFiles.Count > 0) ImagesContentField(ContentType.Input, _inputImageFiles);
                if (_inputAudioFiles.Count > 0) AudioContentField(ContentType.Input, _inputAudioFiles);
            }

            private void DrawOutputContents()
            {
                if (_outputTexts.Count > 0)
                {
                    for (int i = 0; i < _outputTexts.Count; i++)
                    {
                        TextContentField(i, ContentType.Output, _outputTexts[i]);
                    }
                }
                if (_outputFiles.Count > 0) FileContentField(ContentType.Output, _outputFiles);
                if (_outputImageFiles.Count > 0) ImagesContentField(ContentType.Output, _outputImageFiles);
                if (_outputAudioFiles.Count > 0) AudioContentField(ContentType.Output, _outputAudioFiles);
            }


            private void DrawRequestDetails(Api api, string modelId, string modelName, Metadata metadata)
            {
                GUILayout.Label("Request Details", EditorStyles.boldLabel);
                GUILayout.BeginVertical(ExStyles.helpBox);
                {
                    Texture2D apiIcon = AIDevKitGUIUtility.GetApiIcon(api);
                    GUIContent apiContent = new(api.GetInspectorName(), apiIcon);
                    AIDevKitGUI.CopiableLabelField(Labels.LABEL_API, apiContent);
                    AIDevKitGUI.CopiableLabelField(Labels.LABEL_MODEL_ID, modelId);
                    AIDevKitGUI.CopiableLabelField(Labels.LABEL_MODEL_NAME, modelName);

                    if (!metadata.IsNullOrEmpty())
                    {
                        foreach (KeyValuePair<string, string> item in metadata)
                        {
                            AIDevKitGUI.CopiableLabelField(item.Key, item.Value);
                        }
                    }
                }
                GUILayout.EndVertical();
            }

            private void DrawUsageDetails(Usage usage, Currency price)
            {
                GUILayout.Label(Labels.Usage, EditorStyles.boldLabel);

                GUILayout.BeginVertical(ExStyles.helpBox);
                {
                    foreach (var kvp in usage.usages)
                    {
                        AIDevKitGUI.TokenField(kvp.Key.GetInspectorName(), kvp.Value);
                    }

                    if (price != null)
                    {
                        AIDevKitGUI.CurrencyField("Total Cost", price);
                    }
                }
                GUILayout.EndVertical();
            }

            private void TextContentField(int index, ContentType type, string text)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(20f));
                {
                    string label = type == ContentType.Input ? Labels.InputPrompt : Labels.OutputText;
                    GUILayout.Label($"{label} ({index + 1})", EditorStyles.boldLabel, GUILayout.Height(20f));
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Copy", GUILayout.Width(50)))
                    {
                        EditorGUIUtility.systemCopyBuffer = text;
                        EditorUtility.DisplayDialog($"{label} copied to clipboard", text, "OK");
                    }

                    if (GUILayout.Button("Open", GUILayout.Width(50)))
                    {
                        TextBlockWindow.ShowWindow($"{label} Details", text);
                    }
                }
                GUILayout.EndHorizontal();

                if (string.IsNullOrEmpty(text))
                {
                    ExGUILayout.BoxedLabel("No content available.", GUILayout.Height(kMinTextContentHeight));
                    return;
                }

                GUILayout.BeginVertical(PromptHistoryGUI.TextContentBox);
                {
                    Vector2 scrollPos = type == ContentType.Input ? _inputScrollPos : _outputScrollPos;
                    scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(kMinTextContentHeight), GUILayout.MaxHeight(kMaxTextContentHeight), GUILayout.ExpandHeight(true));
                    {
                        GUILayout.Label(text, EditorStyles.wordWrappedLabel);
                    }
                    GUILayout.EndScrollView();
                    if (type == ContentType.Input) _inputScrollPos = scrollPos;
                    else _outputScrollPos = scrollPos;
                }
                GUILayout.EndVertical();
            }

            private void FileContentField(ContentType type, List<RawFile> files)
            {
                RenderContentField(type, files, Labels.InputFile, Labels.OutputFile, f => f.ReadAllBytesAsync(),
                    (_, filePath) =>
                    {
                        if (GUILayout.Button("Open", GUILayout.Height(kAssetContentSize)))
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                            }
                            else
                            {
                                ExGUILayout.BoxedLabel("File not found.");
                            }
                        }
                    },
                    _fileState
                );
            }

            private void ImagesContentField(ContentType type, List<File<Texture2D>> files)
            {
                RenderContentField(type, files, Labels.InputImage, Labels.OutputImage, f => f.LoadAssetAsync(true),
                    (tex, _) => ExGUILayout.TextureField(tex, new Vector2(kAssetContentSize, kAssetContentSize)),
                    _imageState
                );
            }

            private void AudioContentField(ContentType type, List<File<AudioClip>> files)
            {
                RenderContentField(type, files, Labels.InputAudio, Labels.OutputAudio, f => f.LoadAssetAsync(true),
                    (clip, _) =>
                    {
                        if (GUILayout.Button("Play", GUILayout.Height(kAssetContentSize)))
                        {
                            EditorAudioPlayer.Play(clip);
                        }
                    },
                    _audioState
                );
            }

            private async void RenderContentField<TAsset, TFile>(
                ContentType type,
                List<TFile> files,
                string inputLabel,
                string outputLabel,
                Func<TFile, UniTask<TAsset>> loader,
                Action<TAsset, string> drawer,
                AssetLoadState<TAsset> state)
            {
                string label = type == ContentType.Input ? inputLabel : outputLabel;
                GUILayout.Label(label, EditorStyles.boldLabel);

                if (files.IsNullOrEmpty())
                {
                    ExGUILayout.BoxedLabel("No content.", GUILayout.Height(kAssetContentSize));
                    return;
                }

                GUILayout.BeginVertical(ExStyles.helpBox);

                if (!state.IsLoaded)
                {
                    ExGUILayout.BoxedLabel("Loading assets...", GUILayout.Width(kAssetContentSize), GUILayout.Height(kAssetContentSize));
                    GUILayout.EndVertical();

                    if (state.IsLoading) return;
                    state.IsLoading = true;

                    state.Assets = new TAsset[files.Count];
                    state.Paths = new string[files.Count];

                    for (int i = 0; i < files.Count; i++)
                    {
                        state.Assets[i] = await loader(files[i]);
                        state.Paths[i] = (files[i] as IFile)?.FullPath;
                    }

                    await UniTask.Delay(500);
                    state.IsLoaded = true;
                    state.IsLoading = false;
                    return;
                }

                GUILayout.BeginHorizontal();
                for (int i = 0; i < state.Assets.Length; i++)
                {
                    var asset = state.Assets[i];
                    var path = state.Paths[i];
                    if (asset == null)
                    {
                        ExGUILayout.BoxedLabel($"<color=red>Failed to load asset from</color> <color=yellow>{path}</color>", GUILayout.Width(kAssetContentSize), GUILayout.Height(kAssetContentSize));
                        continue;
                    }
                    drawer(asset, path);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

        }
    }
}