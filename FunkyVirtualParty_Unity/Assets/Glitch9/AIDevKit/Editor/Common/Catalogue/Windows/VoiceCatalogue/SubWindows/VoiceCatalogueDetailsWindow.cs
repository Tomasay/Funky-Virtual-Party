using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class VoiceCatalogueWindow
    {
        public class VoiceCatalogueDetailsWindow : ExtendedTreeViewDetailsWindow
        {
            private static readonly string[] kVoiceSamples = new string[]
            {
                "Hello there. It's good to see you again.",
                "How can I assist you with your task today?",
                "Testing, one two three. Let’s make sure this is working properly.",
                "Is this microphone live? Let's give it a try.",
                "That sounds about right to me. Let’s go with it.",
                "I'm ready when you are. Just say the word.",
                "Loading complete. Everything is up and running.",
                "All systems are now online and fully operational.",
                "Just a moment, please. I’m processing your request.",
                "Let’s begin. I’ll guide you through the next steps.",
                "Initializing voice protocol. Please stand by.",
                "Connection has been successfully established.",
                "Warning: the system is reaching critical levels.",
                "Rebooting now. This may take a few seconds.",
                "Command received. Executing the requested action.",
                "I wasn’t expecting that response, but let’s move on.",
                "Are you sure this is the direction you want to take?",
                "I think we’re good to go. Everything looks stable.",
                "What would you like to do next? I’m here to help.",
                "That was kind of fun. Want to try again?",
                "Whoa, that was closer than I thought it’d be!",
                "Seriously? That’s the choice you’re going with?",
                "Ugh, not again… This keeps happening.",
                "Ha! I knew you were going to say that.",
                "Let’s try something else this time, shall we?",
                "Welcome back, Commander. Your mission awaits.",
                "Don’t worry, I’ve got your back this time.",
                "Shall we proceed with the operation now?",
                "You can count on me. I won’t let you down.",
                "Ending transmission. I’ll be here if you need me."
            };

            private string DateDisplay
            {
                get
                {
                    if (string.IsNullOrEmpty(_dateDisplay))
                    {
                        _dateDisplay = Data.CreatedAt?.ToString("yyyy-MM-dd");
                    }
                    return _dateDisplay;
                }
            }
            private string _dateDisplay;
            private string TypeDisplay
            {
                get
                {
                    if (string.IsNullOrEmpty(_typeDisplay))
                    {
                        using (StringBuilderPool.Get(out var sb))
                        {
                            sb.Append(Data.Type.GetInspectorName());

                            bool descriptiveExists = !string.IsNullOrEmpty(Data.Descriptive);
                            if (descriptiveExists)
                            {
                                sb.Append(" (");
                                sb.Append(Data.Descriptive.ToTitleCase());
                                sb.Append(")");
                            }

                            _typeDisplay = sb.ToString();
                        }
                    }
                    return _typeDisplay;
                }
            }
            private string _typeDisplay;
            private string _testPrompt;
            private bool _isLoading = false;


            private bool? _isFreeUserForElevenLabs;
            private bool IsFreeUserForElevenLabs
            {
                get
                {
                    if (_isFreeUserForElevenLabs == null) CheckIfFreeUserForElevenLabs();
                    return _isFreeUserForElevenLabs ?? false;
                }
            }
            private bool _isCheckingFreeUserForElevenLabs = false;
            private string _availableForTiersDisplay;
            private int _currentSampleIndex = 0;

            private async void CheckIfFreeUserForElevenLabs()
            {
                if (_isFreeUserForElevenLabs != null) return;
                if (_isCheckingFreeUserForElevenLabs) return;
                _isCheckingFreeUserForElevenLabs = true;

                try
                {
                    _isFreeUserForElevenLabs = await AIDevKitEditor.IsElevenLabsFreeTierAsync();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to check Eleven Labs user status: {e.Message}");
                }
                finally
                {
                    _isCheckingFreeUserForElevenLabs = false;
                }
            }

            protected override float CalcTitleHeight() => 22f;

            protected override GUIContent CreateTitle()
            {
                string titleText = !string.IsNullOrEmpty(Data.Name) ? Data.Name : kFallbackTitle;
                return new GUIContent(titleText, AIDevKitGUIUtility.GetApiIcon(Data.Api));
            }

            protected override void DrawSubtitle()
            {
                if (!string.IsNullOrEmpty(DateDisplay))
                    TreeViewGUI.SubtitleLeft($"Created At: {DateDisplay}");
                TreeViewGUI.SubtitleLeft($"Owned by: {Data.OwnedBy ?? "Unknown"}");
            }

            protected override void DrawBody()
            {
                DrawLocalStatus();
                GUILayout.Space(5);
                DrawVoiceTestPanel();
                GUILayout.Space(5);
                DrawVoiceDetails();

                if (!string.IsNullOrEmpty(Data.Description))
                {
                    GUILayout.Space(5);
                    DrawDescription();
                }

                // if (AIDevKitDebug.kDebugMode)
                // {
                //     GUILayout.Space(5);
                //     DrawDebugMenu();
                // }

                // GUILayout.Space(5);
                // DrawUsageStats();
                // GUILayout.Space(5);
                // DrawStatusAndSettings();
                // GUILayout.Space(5);
                // DrawOwnerInfo();
            }

            private void DrawLocalStatus()
            {
                const float kHeight = 36f;

                if (!Item.IsFree && Item.Api == Api.ElevenLabs && IsFreeUserForElevenLabs)
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        GUILayout.Label(EditorIcons.StatusObsolete, GUILayout.Width(30), GUILayout.Height(kHeight));
                        GUILayout.Label("This voice is not available for free users.", ExStyles.verticallyCenteredLabel, GUILayout.Height(kHeight));
                    }
                    GUILayout.EndHorizontal();
                    return;
                }

                Texture statusIcon;
                string message;

                if (Item.InMyLibrary)
                {
                    statusIcon = EditorIcons.StatusCheck;
                    message = "This voice is already in your library.";
                }
                else
                {
                    statusIcon = EditorIcons.StatusWarning;
                    message = "This voice is not in your library.";
                }

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    if (statusIcon != null)
                    {
                        GUILayout.Label(statusIcon, GUILayout.Width(30), GUILayout.Height(kHeight));
                    }

                    GUILayout.Label(message, ExStyles.verticallyCenteredLabel, GUILayout.Height(kHeight));

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        if (Item.InMyLibrary)
                        {
                            GUI.color = ExGUI.red;
                            if (GUILayout.Button(GUIContents.RemoveFromLibrary, AIDevKitStyles.WordWrappedButton, GUILayout.Width(40), GUILayout.Height(kHeight)))
                            {
                                VoiceCatalogueUtil.RemoveFromLibrary(Item);
                                Repaint();
                            }
                            GUI.color = Color.white;
                        }
                        else
                        {
                            GUI.color = ExGUI.green;
                            if (GUILayout.Button(GUIContents.AddToLibrary, AIDevKitStyles.WordWrappedButton, GUILayout.Width(40), GUILayout.Height(kHeight)))
                            {
                                VoiceCatalogueUtil.AddToLibrary(Item);
                                Repaint();
                            }
                            GUI.color = Color.white;
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }



            private void DrawVoiceTestPanel()
            {
                const float kHeight = 40f;
                const float kArrowWidth = 30f;

                TreeViewGUI.BeginSection("Voice Test");
                try
                {
                    EditorGUI.BeginDisabledGroup(_isLoading);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(EditorIcons.ArrowNavigationLeft2x, GUILayout.Width(kArrowWidth), GUILayout.Height(kHeight)))
                            {
                                _currentSampleIndex = _currentSampleIndex.IncrementIndex(kVoiceSamples.Length);
                                _testPrompt = kVoiceSamples[_currentSampleIndex];
                            }

                            _testPrompt = GUILayout.TextField(_testPrompt, GUILayout.Height(kHeight), GUILayout.ExpandWidth(true));

                            if (GUILayout.Button(EditorIcons.ArrowNavigationRight2x, GUILayout.Width(kArrowWidth), GUILayout.Height(kHeight)))
                            {
                                _currentSampleIndex = _currentSampleIndex.DecrementIndex(kVoiceSamples.Length);
                                _testPrompt = kVoiceSamples[_currentSampleIndex];
                            }
                        }
                        GUILayout.EndHorizontal();


                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Play Preview", GUILayout.Width(100f))) Data.PlayPreviewAsync().Forget();
                            if (GUILayout.Button("Play")) PlayVoiceTest();
                        }
                        GUILayout.EndHorizontal();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                finally
                {
                    TreeViewGUI.EndSection();
                }
            }

            private void DrawVoiceDetails()
            {
                TreeViewGUI.BeginSection("Voice Details");
                {
                    AIDevKitGUI.CopiableLabelField("Name", Data.Name);
                    AIDevKitGUI.LabelField("Gender", Data.Gender);
                    AIDevKitGUI.CopiableLabelField("Language", Item.LanguageDisplay);
                    AIDevKitGUI.LabelField("Age", Data.Age);
                    AIDevKitGUI.CopiableLabelField("Type", TypeDisplay);
                    AIDevKitGUI.LabelField("Category", Data.Category);
                    AIDevKitGUI.CopiableLabelField("Image URL", Data.ImageUrl);
                    AIDevKitGUI.LabelField("Is Free", Data.IsFree);

                    if (!Data.AvailableForTiers.IsNullOrEmpty())
                    {
                        if (_availableForTiersDisplay == null)
                        {
                            using (StringBuilderPool.Get(out var sb))
                            {
                                foreach (var tier in Data.AvailableForTiers)
                                {
                                    sb.AppendLine(tier);
                                }
                                _availableForTiersDisplay = sb.ToString();
                            }
                        }
                        AIDevKitGUI.CopiableLabelField("Available For Tiers", _availableForTiersDisplay);
                    }
                }
                TreeViewGUI.EndSection();
            }

            private void DrawDescription()
            {
                TreeViewGUI.HeaderLabel("Voice Description");

                GUILayout.BeginVertical(ExStyles.helpBoxedSection);
                {
                    EditorGUILayout.LabelField(Data.Description, EditorStyles.wordWrappedLabel);
                }
                GUILayout.EndVertical();
            }

            // private void DrawUsageStats()
            // {
            //     TreeViewGUI.HeaderLabel("Usage Stats");

            //     GUILayout.BeginVertical(ExEditorStyles.helpBoxedSection);
            //     {
            //         AIDevKitGUI.LabelField("Playback Rate", Data.PlaybackRate);
            //         AIDevKitGUI.LabelField("Usage (1 Year)", Data.UsageCharacterCount1Y);
            //         AIDevKitGUI.LabelField("Usage (7 Days)", Data.UsageCharacterCount7D);
            //         AIDevKitGUI.LabelField("Play API Usage (1 Year)", Data.PlayApiUsageCharacterCount1Y);
            //         AIDevKitGUI.LabelField("Cloned By Count", Data.ClonedByCount);
            //     }
            //     GUILayout.EndVertical();
            // }

            // private void DrawStatusAndSettings()
            // {
            //     TreeViewGUI.HeaderLabel("Status & Settings");

            //     GUILayout.BeginVertical(ExEditorStyles.helpBoxedSection);
            //     {
            //         AIDevKitGUI.LabelField("Live Moderation", Data.LiveModerationEnabled);
            //         AIDevKitGUI.LabelField("Featured", Data.Featured);
            //         AIDevKitGUI.LabelField("Added by User", Data.IsAddedByUser);
            //         AIDevKitGUI.LabelField("Notice Period (Days)", Data.NoticePeriod);
            //     }
            //     GUILayout.EndVertical();
            // }

            // private void DrawOwnerInfo()
            // {
            //     TreeViewGUI.HeaderLabel("Owner Info");

            //     GUILayout.BeginVertical(ExEditorStyles.helpBoxedSection);
            //     {
            //         AIDevKitGUI.LabelField("Instagram", Data.InstagramUsername);
            //         AIDevKitGUI.LabelField("Twitter", Data.TwitterUsername);
            //         AIDevKitGUI.LabelField("TikTok", Data.TikTokUsername);
            //         AIDevKitGUI.LabelField("YouTube", Data.YouTubeUsername);
            //     }
            //     GUILayout.EndVertical();
            // } 
            // private void DrawDebugMenu()
            // {
            //     TreeViewGUI.BeginSection("Debug Menu");
            //     {
            //         if (GUILayout.Button("Create Preview Clip", GUILayout.Height(30)))
            //         {
            //             CreatePreviewClip();
            //         }
            //     }
            //     TreeViewGUI.EndSection();
            // }

            private async void PlayVoiceTest()
            {
                if (string.IsNullOrEmpty(_testPrompt)) return;

                _isLoading = true;

                try
                {
                    var clip = await _testPrompt.GENSpeech()
                        .SetVoice(Data.Api, Data.Id)
                        .ExecuteAsync();

                    if (clip != null) EditorAudioPlayer.Play(clip);
                    else Debug.LogError("Failed to play voice test.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to play voice test: {e.Message}");
                }
                finally
                {
                    _isLoading = false;
                }
            }

            // private async void CreatePreviewClip()
            // {
            //     bool alreadyHasPreview = await Data.PlayPreviewAsync(true);
            //     if (alreadyHasPreview)
            //     {
            //         ShowDialog.Info("Preview clip already exists for this voice.");
            //         return;
            //     }

            //     const string previewPrompt = "Hello, I hope you are having a great day! This is a test voice preview.";
            //     string absolutePreviewPath = AIDevKitEditorPath.GetVoiceSampleFullPath(Data.Api, Data.Id);

            //     AudioClip previewClip = await previewPrompt.GENSpeech()
            //         .SetVoice(Data.Api, Data.Id)
            //         .SetEncoding(CoreLib.IO.Audio.AudioEncoding.MP3)
            //         .SetOutputPath(absolutePreviewPath)
            //         .ExecuteAsync();

            //     if (previewClip != null)
            //     {
            //         ShowDialog.Info($"Preview clip created successfully at: {absolutePreviewPath}");
            //         AssetDatabase.Refresh();
            //         EditorAudioPlayer.Play(previewClip);
            //     }
            //     else
            //     {
            //         ShowDialog.Error("Failed to create preview clip.");
            //     }
            // }
        }
    }
}