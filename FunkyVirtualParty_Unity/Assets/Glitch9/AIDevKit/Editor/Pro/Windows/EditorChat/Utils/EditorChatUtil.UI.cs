
using System;
using Glitch9.AIDevKit.Editor.Pro;
using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static partial class EditorChatUtil
    {
        internal static class UI
        {
            internal static void StyleInputField_TextField(TextField textField)
            {
                const float inputFieldHeight = 40f;

                textField.AddManipulator(new BlinkingCursor());
                //textField.verticalScrollerVisibility = ScrollerVisibility.Auto;
                textField.style.overflow = Overflow.Visible;
                textField.style.whiteSpace = WhiteSpace.Normal;
                textField.style.minHeight = inputFieldHeight;
                textField.style.height = StyleKeyword.Auto;
            }

            internal static void StyleInputField_SendButton(Button sendButton)
            {
                sendButton.style.backgroundImage = AIDevKitIcons.Send;  // set icon to send button
            }

            internal static void StyleInputField_UnityTextInput(VisualElement unityTextInput)
            {
                unityTextInput.style.height = StyleKeyword.Auto;
                unityTextInput.style.unityTextAlign = TextAnchor.MiddleLeft; // 또는 MiddleCenter, MiddleRight
                unityTextInput.style.paddingLeft = 10;
                unityTextInput.style.paddingRight = 32;
                unityTextInput.style.paddingTop = 4;
                unityTextInput.style.paddingBottom = 4;

                Color inputBorderColor = EditorChatConfig.InputFieldBorderColor;

                unityTextInput.RegisterCallback<FocusInEvent>(evt =>
                {
                    unityTextInput.style.borderTopColor = new StyleColor(inputBorderColor);
                    unityTextInput.style.borderBottomColor = new StyleColor(inputBorderColor);
                    unityTextInput.style.borderLeftColor = new StyleColor(inputBorderColor);
                    unityTextInput.style.borderRightColor = new StyleColor(inputBorderColor);
                });

                unityTextInput.RegisterCallback<FocusOutEvent>(evt =>
                {
                    unityTextInput.style.borderTopColor = StyleKeyword.Null;
                    unityTextInput.style.borderBottomColor = StyleKeyword.Null;
                    unityTextInput.style.borderLeftColor = StyleKeyword.Null;
                    unityTextInput.style.borderRightColor = StyleKeyword.Null;
                });
            }

            internal static void SetupScrollToBottomButton(Button scrollToBottomButton, ScrollView chatScrollView)
            {
                var scrollToBottomIcon = scrollToBottomButton.Q<UnityEngine.UIElements.Image>("scroll-to-bottom-icon");
                scrollToBottomIcon.image = EditorIcons.IconDropdown; // set icon to scroll to bottom button
                scrollToBottomButton.clicked += () => ChatScrollViewUtil.ScrollToBottom(chatScrollView);
            }

            internal static void SetupToolbar(VisualElement toolbar, ChatIMGUIToolbar toolbarController)
            {
                if (toolbar == null || toolbarController == null)
                {
                    Debug.LogWarning("Toolbar or ToolbarController is null. Cannot setup toolbar.");
                    return;
                }
                toolbar.Clear();
                toolbar.Add(new IMGUIContainer(() => toolbarController.DrawToolbar()));
            }

            internal static void SetupTitleBar(VisualElement titleBar, string title, Action<string> onTitleEdited)
            {
                if (titleBar == null)
                {
                    Debug.LogWarning("TitleBar or TitleBarController is null. Cannot setup title bar.");
                    return;
                }
                titleBar.Clear();

                if (EditorChatSettings.DebugMode) AIDevKitDebug.Mark($"Setting up title bar with title: {title}");

                Label titleLabel = new(title);
                titleLabel.AddToClassList("title-bar-label");

                var titleBarBtnContainer = new VisualElement { name = "title-bar-button-container" };
                titleBarBtnContainer.AddToClassList("title-bar-button-container");

                // edit title buttle 
                IconButton editTitleButton = CreateTitleBarButton(
                    buttonName: "edit-title-button",
                    icon: EditorIcons.Edit,
                    tooltip: "Edit Title",
                    onClick: () => OnEditTitle(titleBar, title, onTitleEdited));

                titleBar.Add(titleLabel);
                titleBarBtnContainer.Add(editTitleButton);
                titleBar.Add(titleBarBtnContainer);

                static void OnEditTitle(VisualElement titleBar, string title, Action<string> onTitleEdited)
                {
                    titleBar.Clear();
                    var titleBarBtnContainer = new VisualElement { name = "title-bar-button-container" };
                    titleBarBtnContainer.AddToClassList("title-bar-button-container");

                    // Open a text field to edit the title
                    var textField = new TextField()
                    {
                        value = title,
                        name = "title-edit-field"
                    };
                    textField.RegisterCallback<ChangeEvent<string>>(evt => onTitleEdited?.Invoke(evt.newValue));

                    // Check(save), X(cancel) buttons
                    var saveButton = CreateTitleBarButton(
                        buttonName: "save-title-button",
                        icon: EditorIcons.Check,
                        tooltip: "Save Title",
                        onClick: () =>
                        {
                            onTitleEdited?.Invoke(textField.value);
                            SetupTitleBar(titleBar, textField.value, onTitleEdited);
                        });

                    var cancelButton = CreateTitleBarButton(
                        buttonName: "cancel-title-button",
                        icon: EditorIcons.Close,
                        tooltip: "Cancel",
                        onClick: () => SetupTitleBar(titleBar, title, onTitleEdited));

                    titleBar.Add(textField);
                    titleBarBtnContainer.Add(saveButton);
                    titleBarBtnContainer.Add(cancelButton);
                    titleBar.Add(titleBarBtnContainer);
                }
            }

            internal static IconButton CreateTitleBarButton(string buttonName, Texture icon, string tooltip, Action onClick)
            {
                var button = new IconButton(icon: icon as Texture2D, tooltip: tooltip, onClick: onClick) { name = buttonName };
                button.AddToClassList("title-bar-button");
                return button;
            }

            internal static ScrollView CreateChatScrollView(VisualElement rootVisualElement)
            {
                var chatArea = rootVisualElement.Q<VisualElement>("chat-area");
                if (chatArea == null) return null;

                chatArea.Clear();

                var chatScrollView = new ScrollView() { name = "chat-scroll-view" };
                chatScrollView.AddToClassList("chat-scroll-view");

                chatArea.Add(chatScrollView);
                return chatScrollView;
            }

            internal static VisualElement CreateChatErrorView(VisualElement rootVisualElement)
            {
                var chatArea = rootVisualElement.Q<VisualElement>("chat-area");
                if (chatArea == null) return null;

                chatArea.Clear();

                var errorView = new VisualElement() { name = "chat-error-view" };
                errorView.AddToClassList("chat-error-view");

                chatArea.Add(errorView);
                return errorView;
            }

            internal static VisualElement CreateFloatingContainer()
            {
                var floatingContainer = new VisualElement { name = "floating-container" };
                floatingContainer.AddToClassList("floating-container");
                return floatingContainer;
            }

            internal static Label CreateFloatingMessage(MessageType messageType, string messageName = "floating-message")
            {
                var floatingMessage = new Label { name = messageName };
                floatingMessage.AddToClassList(ResolveMessageTypeClassName(messageType));
                return floatingMessage;
            }

            internal static Button CreateFloatingButton(MessageType messageType, string buttonText, string buttonName = "floating-button", bool isBottom = false)
            {
                var floatingButton = new Button { name = buttonName, text = buttonText };
                floatingButton.AddToClassList(ResolveButtonClassName(messageType, isBottom));
                return floatingButton;
            }

            // EditorChatUtil.UI.BuildUsageTrackingUI(usageContainer, TotalUsage, TotalCostInUSD);
            internal static void BuildUsageTrackingUI(VisualElement usageContainer, Usage totalUsage, double totalCostInUSD)
            {
                usageContainer.Clear();

                if (totalUsage == null)
                {
                    if (EditorChatSettings.DebugMode)
                        AIDevKitDebug.Mark("Total usage is null. No usage data available.");
                    usageContainer.Add(new Label("No usage data available."));
                    return;
                }

                if (EditorChatSettings.DebugMode)
                    AIDevKitDebug.Mark($"Building usage tracking UI with total usage: {totalUsage} and total cost: ${totalCostInUSD:F2}");

                // string[] parts = totalUsage.ToInspectorTextParts();
                // foreach (string part in parts)
                // {
                //     if (string.IsNullOrEmpty(part))
                //         continue;
                //     var label = new Label(part);
                //     label.AddToClassList("usage-label");
                //     usageContainer.Add(label);
                // }

                var usages = totalUsage.usages;
                if (usages == null || usages.Count == 0)
                {
                    usageContainer.Add(new Label("No usage data available."));
                    return;
                }

                foreach (var usage in usages)
                {
                    if (usage.Value <= 0) continue; // Skip zero or negative usages

                    Texture2D icon = GetUsageIcon(usage.Key);
                    var label = new IconLabel(
                        icon: icon,
                        text: $"{usage.Value}",
                        tooltip: $"{usage.Key.GetInspectorName()}");
                    label.AddToClassList("usage-label");
                    usageContainer.Add(label);
                }

                //usage-flexible-spacer
                var flexibleSpacer = new VisualElement();
                flexibleSpacer.AddToClassList("usage-flexible-spacer");
                usageContainer.Add(flexibleSpacer);

                var totalCostLabel = new Label($"${totalCostInUSD:F2}")
                {
                    name = "total-cost-label",
                    tooltip = $"${totalCostInUSD:F6}"
                };
                totalCostLabel.AddToClassList("cost-label");
                usageContainer.Add(totalCostLabel);

                // button to open prompt history
                var historyButton = new IconButton(
                    icon: EditorIcons.History as Texture2D,
                    tooltip: "Open Prompt History",
                    onClick: () => PromptHistoryWindow.ShowWindow());

                historyButton.AddToClassList("prompt-history-button");
                usageContainer.Add(historyButton);
            }

            private static Texture2D GetUsageIcon(UsageType usageType)
            {
                return usageType switch
                {
                    UsageType.InputToken => AIDevKitIcons.User,
                    UsageType.OutputToken => AIDevKitIcons.Assistant,
                    UsageType.CachedInputToken => AIDevKitIcons.Caching,
                    UsageType.PerCharacter => AIDevKitIcons.TextToSpeech,
                    _ => EditorIcons.Question as Texture2D
                };
            }

            private static string ResolveMessageTypeClassName(MessageType messageType)
            {
                const string defaultClass = "floating-message--{messageType}";

                return messageType switch
                {
                    MessageType.Info => defaultClass.Replace("{messageType}", "info"),
                    MessageType.Warning => defaultClass.Replace("{messageType}", "warning"),
                    MessageType.Error => defaultClass.Replace("{messageType}", "error"),
                    _ => defaultClass.Replace("{messageType}", "default")
                };
            }

            private static string ResolveButtonClassName(MessageType messageType, bool isBottom)
            {
                // when error: 
                // background-color: rgb(132, 32, 32);
                // border-top-color: rgb(116, 20, 20);

                const string defaultClass = "floating-button--{messageType}";
                const string bottomClass = "floating-button-bottom--{messageType}";

                // return messageType switch
                // { 
                //     MessageType.Info => (new Color(0.2f, 0.8f, 0.2f), new Color(0.15f, 0.6f, 0.15f)),
                //     MessageType.Warning => (new Color(0.8f, 0.8f, 0.2f), new Color(0.6f, 0.6f, 0.15f)),
                //     MessageType.Error => (new Color(0.52f, 0.12f, 0.12f), new Color(0.45f, 0.08f, 0.08f)),
                //     _ => (new Color(0.1f, 0.1f, 0.1f), new Color(0.2f, 0.2f, 0.2f)),  
                // };

                string format = isBottom ? bottomClass : defaultClass;
                return messageType switch
                {
                    MessageType.Info => format.Replace("{messageType}", "info"),
                    MessageType.Warning => format.Replace("{messageType}", "warning"),
                    MessageType.Error => format.Replace("{messageType}", "error"),
                    _ => format.Replace("{messageType}", "default")
                };
            }


        }
    }
}