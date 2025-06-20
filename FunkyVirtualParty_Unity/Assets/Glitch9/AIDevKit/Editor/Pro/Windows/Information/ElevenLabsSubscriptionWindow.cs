using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public class ElevenLabsSubscriptionWindow : PaddedEditorWindow
    {
        private const string kTitle = "ElevenLabs Subscription Details";

        public static void ShowWindow()
        {
            var window = GetWindow<ElevenLabsSubscriptionWindow>(true, kTitle, true);
            window.minSize = new Vector2(300, 200);
            window.maxSize = new Vector2(500, 500);
            window.Show();
        }

        private Vector2 _scrollPos;

        protected override void DrawGUI()
        {
            const float kSpace = 5f;

            DrawHeader();
            GUILayout.Space(kSpace);

            UserSubscription sub = ElevenLabsSubscription.UserSubscription?.Value;

            if (sub == null)
            {
                ExGUILayout.HelpBoxExBig("Failed to retrieve user subscription from ElevenLabs API. Please try reloading with the button on the top right.", MessageTypeEx.Error);
                return;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                EditorGUIUtility.labelWidth = 200f;

                DrawPlanInfo(sub);
                GUILayout.Space(kSpace);

                DrawCharacterUsage(sub);
                GUILayout.Space(kSpace);

                DrawVoiceSlots(sub);
                GUILayout.Space(kSpace);

                DrawBillingInfo(sub);
                GUILayout.Space(kSpace);

                EditorGUIUtility.labelWidth = 0f;
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Subscription Info", TreeViewStyles.DetailsWindowTitle, GUILayout.MaxWidth(position.width - 44));

                if (GUILayout.Button("Reload"))
                {
                    ElevenLabsSubscription.GetUserSubscriptionAsync(true).Forget();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawPlanInfo(UserSubscription sub)
        {
            GUILayout.BeginHorizontal();
            {
                TreeViewGUI.HeaderLabel("Plan Info");

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Manage Your Plan"))
                {
                    const string kSubsUrl = "https://elevenlabs.io/app/subscription";
                    Application.OpenURL(kSubsUrl);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(ExStyles.helpBoxedSection);
            {
                AIDevKitGUI.CopiableLabelField("Tier", sub.Tier?.ToTitleCase());
                AIDevKitGUI.LabelField("Status", sub.Status);
                AIDevKitGUI.LabelField("Billing Period", sub.BillingPeriod);
                AIDevKitGUI.LabelField("Currency", sub.Currency);
            }
            GUILayout.EndVertical();
        }

        private void DrawCharacterUsage(UserSubscription sub)
        {
            TreeViewGUI.HeaderLabel("Character Usage");

            GUILayout.BeginVertical(ExStyles.helpBoxedSection);
            {
                AIDevKitGUI.CopiableLabelField("Characters Used", $"{sub.CharacterCount} / {sub.CharacterLimit}");
                AIDevKitGUI.LabelField("Can Extend Limit", sub.CanExtendCharacterLimit);
                AIDevKitGUI.LabelField("Allowed To Extend Limit", sub.AllowedToExtendCharacterLimit);
                AIDevKitGUI.LabelField("Max Limit Extension", sub.MaxCharacterLimitExtension);
                AIDevKitGUI.LabelField("Refresh Period", sub.CharacterRefreshPeriod);
                AIDevKitGUI.CopiableLabelField("Next Reset", sub.NextCharacterCountResetUnix?.ToString("g"));
            }
            GUILayout.EndVertical();
        }

        private void DrawVoiceSlots(UserSubscription sub)
        {
            TreeViewGUI.HeaderLabel("Voice Slots");

            GUILayout.BeginVertical(ExStyles.helpBoxedSection);
            {
                AIDevKitGUI.CopiableLabelField("Voice Slots Used", $"{sub.VoiceSlotsUsed} / {sub.VoiceLimit}");
                AIDevKitGUI.CopiableLabelField("Professional Voice Slots Used", $"{sub.ProfessionalVoiceSlotsUsed} / {sub.ProfessionalVoiceLimit}");
                AIDevKitGUI.LabelField("Voice Edits Used", sub.VoiceAddEditCounter);
                AIDevKitGUI.LabelField("Max Voice Edits", sub.MaxVoiceAddEdits);
                AIDevKitGUI.LabelField("Can Extend Limit", sub.CanExtendVoiceLimit);
                AIDevKitGUI.LabelField("Instant Cloning", sub.CanUseInstantVoiceCloning);
                AIDevKitGUI.LabelField("Professional Cloning", sub.CanUseProfessionalVoiceCloning);
            }
            GUILayout.EndVertical();
        }

        private void DrawBillingInfo(UserSubscription sub)
        {
            TreeViewGUI.HeaderLabel("Billing Info");

            GUILayout.BeginVertical(ExStyles.helpBoxedSection);
            {
                AIDevKitGUI.LabelField("Has Open Invoices", sub.HasOpenInvoices);

                if (sub.NextInvoice != null)
                {
                    AIDevKitGUI.CopiableLabelField("Amount Due", $"${sub.NextInvoice.AmountDueCents / 100f:F2}");
                    AIDevKitGUI.CopiableLabelField("Next Payment Attempt", sub.NextInvoice.NextPaymentAttemptUnix.ToString("g"));
                }
            }
            GUILayout.EndVertical();
        }

    }
}