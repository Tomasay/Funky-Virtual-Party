using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal enum ChatMessageItemType
    {
        // Sent from user
        System,
        User,

        // Sent from AI 
        // Typing > Streaming > Assistant
        // Typing is a special type that shows a typing indicator
        Assistant,
        Typing,
        Streaming,

        // Special types
        // ShowMore is used to show a button that expands the message
        ShowMore,
        TempInfo, // Used for temporary info messages, always placed at last
        TempWarning, // Used for temporary warning messages, always placed at last
        TempError, // Used for temporary error messages, always placed at last
    }

    internal class ChatMessageItem
    {
        public int Index;
        public ChatMessageItemType Type;
        public bool IsTempMessage => Type == ChatMessageItemType.TempInfo
            || Type == ChatMessageItemType.TempWarning
            || Type == ChatMessageItemType.TempError;

        // Content is the main text of the message
        public string Content;

        // Icon is used to show the user or AI icon
        // If the message is from the user, this will be the user's icon
        public Texture2D Icon;

        // Metadata that can be shown in the bottom row of the message
        public Usage Usage;
        public UnixTime Timestamp;

        // For now, only user messages can have attached images
        // But later, if I decide to allow AI to send images, this can be expanded
        public List<Texture2D> AttachedImages;
        public List<ChatContextItem> AttachedFiles; // files other than images
        public List<ButtonEntry> Buttons;

        public ChatMessageItem() { }
        public ChatMessageItem(int index, ChatMessageItemType type, string content, Texture2D icon = null)
        {
            Index = index;
            Type = type;
            Content = content;
            Icon = icon;
        }
    }

    internal class ChatMessageElement : VisualElement
    {

        private const string kContent = "content";
        internal ChatMessageItemType Type { get; private set; }
        private readonly VisualElement _headerRow;
        private readonly VisualElement _contentContainer;
        private readonly VisualElement _buttonRow;
        private ExButton _editButton;
        private Label _timestampLabel;

        public ChatMessageElement()
        {
            AddToClassList("chat-message");

            _headerRow = new VisualElement { name = "header-row" };
            _contentContainer = new VisualElement { name = "content-container" };
            _buttonRow = new VisualElement { name = "button-row" };

            _headerRow.AddToClassList("chat-message-header-row");
            _contentContainer.AddToClassList("chat-message-content-container");
            _buttonRow.AddToClassList("chat-message-button-row");

            Add(_headerRow);
            Add(_contentContainer);
            Add(_buttonRow);
        }


        public void Bind(ChatMessageItem item, Action onShowMore, Action<int, string> onEdited)
        {
            Type = item.Type;

            ClearStyles();
            ClearColumns();

            BuildHeaderRow(item); // Icon
            BuildContentRow(item, onShowMore); // Main Content + Details + Images
            BuildButtonRow(item, onEdited);    // Edit or Copy buttons

            // Temp메시지중 하나일때는 Buttons를 넣고, 
            // 그외는 Details Row를 넣음
            if (item.Type == ChatMessageItemType.TempInfo
                || item.Type == ChatMessageItemType.TempWarning
                || item.Type == ChatMessageItemType.TempError)
            {
                if (item.Buttons.IsNotNullOrEmpty())
                {
                    foreach (var button in item.Buttons)
                    {
                        string additionalClass = ChatScrollViewUtil.ResolveButtonClassName(item.Type);
                        AddButtonINTERNAL(button, additionalClass);
                    }
                }
            }
            else if (item.Type != ChatMessageItemType.ShowMore
                && item.Type != ChatMessageItemType.System)
            {
                var detailsRow = ChatScrollViewUtil.CreateContentDetailsRow(item, out _timestampLabel);
                if (detailsRow != null)
                {
                    if (item.Type == ChatMessageItemType.User)
                        detailsRow.AddToClassList("chat-message-details-row--user");
                    else if (item.Type == ChatMessageItemType.Assistant)
                        detailsRow.AddToClassList("chat-message-details-row--ai");

                    Add(detailsRow);
                }
            }

            if (item.Type == ChatMessageItemType.User)
            {
                _contentContainer.AddToClassList("chat-message-content-container--user");
                _buttonRow.AddToClassList("chat-message-button-row--user");
            }
            else if (item.Type == ChatMessageItemType.Assistant
                || item.Type == ChatMessageItemType.Streaming
                || item.Type == ChatMessageItemType.Typing)
            {
                _contentContainer.AddToClassList("chat-message-content-container--ai");
                _buttonRow.AddToClassList("chat-message-button-row--ai");
            }

            // System or ShowMore 메시지가 아닐때만 공용 ContainerClassName를 설정
            if (item.Type != ChatMessageItemType.System && item.Type != ChatMessageItemType.ShowMore)
                AddToClassList("chat-message-container");

            string[] containerClasses = ChatScrollViewUtil.ResolveItemContainerClassNames(item.Type);
            foreach (var className in containerClasses) AddToClassList(className);

            if (item.Type == ChatMessageItemType.Assistant)
            {
                // add separator line
                var separator = new VisualElement { name = "separator" };
                separator.AddToClassList("chat-message-separator");
                Add(separator);
            }
        }

        private void BuildHeaderRow(ChatMessageItem item)
        {
            _headerRow.Clear();

            if (item.Icon != null)
            {
                var icon = new Image()
                {
                    image = item.Icon,
                    name = "chat-message-icon",
                    tooltip = EditorChatTextFormatter.ParseUsageTooltip(item.Usage),
                };
                icon.AddToClassList("chat-message-icon");
                _headerRow.Add(icon);
            }
        }

        private void BuildContentRow(ChatMessageItem item, Action onShowMore)
        {
            _contentContainer.Clear();

            if (item.Type == ChatMessageItemType.User)
            {
                AddAttachedImagesToUserMessage(item.AttachedImages);
                AddAttachedFilesToUserMessage(item.AttachedFiles);
            }

            var content = CreateContentElement(item, onShowMore);
            content.name = kContent;

            if (item.Type == ChatMessageItemType.ShowMore)
            {
                Clear(); // 전체 클리어 (self.Clear())
                var showMoreBtn = content as Button;
                showMoreBtn.RemoveFromClassList("unity-button"); // 기존 스타일 제거 
                showMoreBtn.AddToClassList("show-more-button");
                Add(showMoreBtn);
            }
            else if (item.Type == ChatMessageItemType.System)
            {
                Clear();
                content.AddToClassList("chat-message-content-system");
                Add(content); // 시스템 메시지는 그냥 추가
            }
            else if (item.Type == ChatMessageItemType.TempInfo ||
                     item.Type == ChatMessageItemType.TempWarning ||
                     item.Type == ChatMessageItemType.TempError)
            {
                content.AddToClassList("chat-message-content-temp-message");
                _contentContainer.Add(content);
            }
            else
            {
                content.AddToClassList("chat-message-content");
                _contentContainer.Add(content);
            }
        }

        private VisualElement CreateContentElement(ChatMessageItem item, Action onShowMore) => item.Type switch
        {
            ChatMessageItemType.Typing => new TypingIndicator(),

            // ChatMessageItemType.Assistant or ChatMessageItemType.Streaming => new IMGUIContainer(() =>
            // {
            //     float maxWidth = _contentContainer.resolvedStyle.width - 10f;
            //     TextBlock.DrawIMGUI(item.Content, maxWidth);
            // }),

            // 원래는 위와같이 IMGUI를 사용했지만, 시간을 들여서 UIToolkit으로 변경하였다. 
            // 이것으로 인해 UI렉이 많이 줄어들었다.

            ChatMessageItemType.Assistant or ChatMessageItemType.Streaming
                => TextBlock.BuildTextBlockElement(item.Content), // BuildTextBlockElement가 UIToolkit기반의 컨테이너를 생성한다.

            ChatMessageItemType.ShowMore => new Button(onShowMore)
            {
                text = item.Content,
                pickingMode = PickingMode.Position
            },

            _ => new Label(item.Content)
            {
                pickingMode = PickingMode.Position,
                style = { whiteSpace = WhiteSpace.Normal },
                //selection = { isSelectable = true },
            }
        };

        private void BuildButtonRow(ChatMessageItem item, Action<int, string> onEdited)
        {
            _buttonRow.Clear();

            switch (item.Type)
            {
                case ChatMessageItemType.User:
                    //AddEditButton(item, onEdited);
                    AddEditButtonINTERNAL(item, onEdited);
                    break;

                case ChatMessageItemType.Assistant:
                    AddButtonINTERNAL(new ButtonEntry
                    {
                        Label = "Copy",
                        Icon = EditorIcons.Copy as Texture2D,
                        Callback = () =>
                        {
                            GUIUtility.systemCopyBuffer = item.Content;
                            Debug.Log($"Copied: {item.Content}");
                        },
                    });
                    break;
            }
        }

        // 기타 보조 유틸리티 함수들
        private ExButton AddButtonINTERNAL(ButtonEntry button, string additionalClass = null)
        {
            var btn = new ExButton(
                onClick: button.Callback,
                label: button.Label,
                icon: button.Icon,
                tooltip: button.Tooltip
            );

            btn.AddToClassList("chat-message-button");
            if (!string.IsNullOrEmpty(additionalClass))
                btn.AddToClassList(additionalClass);
            _buttonRow.Add(btn);
            return btn;
        }

        private void AddEditButtonINTERNAL(ChatMessageItem item, Action<int, string> onEdited)
        {
            if (_editButton != null)
            {
                return; // 이미 추가된 경우 중복 방지
            }

            _editButton = AddButtonINTERNAL(new ButtonEntry
            {
                Label = "Edit",
                Icon = EditorIcons.EditOutline as Texture2D,
                Callback = () =>
                {
                    _buttonRow.Clear();
                    EditContent(item, onEdited);
                }
            });
        }

        private void ClearStyles()
        {
            RemoveFromClassList("chat-message-container--user");
            RemoveFromClassList("chat-message-container--ai");
            RemoveFromClassList("chat-message-container--temp");
            RemoveFromClassList("chat-message-container--temp-info");
            RemoveFromClassList("chat-message-container--temp-warning");
            RemoveFromClassList("chat-message-container--temp-error");
            RemoveFromClassList("chat-message-container--system");
            RemoveFromClassList("chat-message-container--show-more");
        }

        private void ClearColumns()
        {
            _headerRow.Clear();
            _contentContainer.Clear();
            _buttonRow.Clear();
        }

        private void AddAttachedImagesToUserMessage(List<Texture2D> images)
        {
            if (images == null || images.Count == 0)
                return;

            var container = new VisualElement { name = "attached-images-container" };
            container.AddToClassList("chat-message-attached-images-container");

            foreach (var tex in images)
            {
                var img = new Image { image = tex, pickingMode = PickingMode.Position };
                img.AddToClassList("chat-message-attached-image");
                container.Add(img);
            }

            _contentContainer.Add(container);
        }

        private void AddAttachedFilesToUserMessage(List<ChatContextItem> files)
        {
            if (files == null || files.Count == 0)
                return;

            var container = new VisualElement { name = "attached-files-container" };
            container.AddToClassList("chat-message-attached-files-container");

            foreach (var file in files)
            {
                var fileElement = new ChatAttachedFileElement(file);
                container.Add(fileElement);
            }

            _contentContainer.Add(container);
        }

        private void EditContent(ChatMessageItem item, Action<int, string> onEdited)
        {
            _contentContainer.Clear();
            _buttonRow.Clear();

            var textField = new TextField { name = kContent, value = item.Content, multiline = true };
            textField.AddToClassList("chat-message-content-editing");
            _contentContainer.Add(textField);
            textField.Focus();

            AddButtonINTERNAL(new ButtonEntry
            {
                Label = "Send",
                Icon = EditorIcons.Check as Texture2D,
                Callback = () =>
                {
                    item.Content = textField.value;
                    onEdited?.Invoke(item.Index, item.Content);
                    FinishEdit(item, onEdited);
                }
            });

            AddButtonINTERNAL(new ButtonEntry
            {
                Label = "Cancel",
                Icon = EditorIcons.Close as Texture2D,
                Callback = () => FinishEdit(item, onEdited)
            });
        }

        private void FinishEdit(ChatMessageItem item, Action<int, string> onEdited)
        {
            _buttonRow.Clear();
            _contentContainer.Clear();

            var label = new Label(item.Content)
            {
                name = kContent,
                pickingMode = PickingMode.Position,
                style = { whiteSpace = WhiteSpace.Normal }
            };

            label.AddToClassList("chat-message-content");
            _contentContainer.Add(label);

            if (EditorChatSettings.ShowTimestampAI && _timestampLabel != null)
            {
                _timestampLabel.text = item.Timestamp.GetInspectorName();
            }

            AddEditButtonINTERNAL(item, onEdited);
        }

        public void UpdateContentOnly(string content)
        {
            var typingIndicator = _contentContainer.Q<TypingIndicator>(kContent);
            if (typingIndicator != null)
            {
                // remove the typing indicator and add imguiContainer
                _contentContainer.Remove(typingIndicator);
                var newImgui = new IMGUIContainer(() =>
                {
                    float maxWidth = _contentContainer.resolvedStyle.width - 10f;
                    TextBlock.DrawIMGUI(content, maxWidth);
                })
                {
                    name = kContent
                };

                newImgui.AddToClassList("chat-message-content");
                _contentContainer.Add(newImgui);
                return;
            }

            var imgui = this.Q<IMGUIContainer>(kContent);
            if (imgui != null)
            {
                imgui.onGUIHandler = () =>
                {
                    float maxWidth = _contentContainer.resolvedStyle.width - 10f;
                    TextBlock.DrawIMGUI(content, maxWidth);
                };
                return;
            }

            var label = this.Q<Label>(kContent);
            if (label != null)
            {
                label.text = content;
                return;
            }
        }
    }
}