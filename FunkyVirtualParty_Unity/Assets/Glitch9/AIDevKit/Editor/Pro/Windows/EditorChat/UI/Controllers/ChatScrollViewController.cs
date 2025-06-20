
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class ChatScrollViewController
    {
        // 평범하게 생성자에서 init하면 Unity리컴파일때 객체가 사라진다.
        // 시불노무새키들이 -_- 따라서 Lazy하게 생성.
        internal ScrollView ScrollView => _scrollView ??= EditorChatUtil.UI.CreateChatScrollView(_window.rootVisualElement);
        private ScrollView _scrollView;

        // _window에서 이 클래스를 계속 다시만들기 때문에 이 프로퍼티도 계속 초기화된다. 
        // 따라서 이 프로퍼티는 _window에서 관리한다. 
        private int DisplayedMessageCount { get => _window.DisplayedMessageCount; set => _window.DisplayedMessageCount = value; }

        private readonly EditorChatWindow _window;
        private readonly List<ChatMessageItem> _messageItems = new();
        private bool _showingShowMoreButton = false;
        private bool _lastShowingScrollToBottom = true;
        private ChatMessageItem _streamingItem;
        private int _streamingItemIndex = -1; // _messageItems에서의 인덱스
        private int _streamingItemIndexScrollView = -1;
        internal ChatScrollViewController(EditorChatWindow window) => _window = window;
        internal async void RebuildUI(IReadOnlyList<ChatMessage> messages, bool scrollToBottom = true)
        {
            ScrollView.Clear();
            _messageItems.Clear();

            int total = messages.Count;
            int start = Mathf.Max(0, total - DisplayedMessageCount);

            _showingShowMoreButton = DisplayedMessageCount < total;

            if (_showingShowMoreButton)
            {
                if (EditorChatSettings.DebugMode)
                    AIDevKitDebug.Blue($"Displaying only the last {DisplayedMessageCount} messages out of {total} total messages.");
                AddShowMoreButtonINTERNAL();
            }

            for (int i = start; i < total; i++)
            {
                var msg = messages[i];
                if (ChatScrollViewUtil.SkipThisMessage(msg)) continue;
                AddItemINTERNAL(await EditorChatFactory.CreateChatMessageItemAsync(i, msg));
            }

            ScrollView.schedule.Execute(UpdateScrollToBottomButton).Every(16); // ms 단위, 60fps 기준 약 16ms 

            if (scrollToBottom) ChatScrollViewUtil.ScrollToBottom(ScrollView, 5);
        }

        private void UpdateScrollToBottomButton()
        {
            bool isContentOverflowing = ScrollView.contentContainer.layout.height > ScrollView.layout.height;
            if (!isContentOverflowing)
            {
                _lastShowingScrollToBottom = false;
                _window.SetScrollToBottomButtonVisible(false);
                return;
            }

            bool isAtBottom = Mathf.Abs(ScrollView.scrollOffset.y - ScrollView.contentContainer.layout.height + ScrollView.layout.height) < 0.2f;

            if (isAtBottom != _lastShowingScrollToBottom)
            {
                _lastShowingScrollToBottom = isAtBottom;
                _window.SetScrollToBottomButtonVisible(!isAtBottom);
            }
        }

        internal async void AddUserMessage(ChatMessage msg, List<IFile> scriptFiles = null)
        {
            if (ChatScrollViewUtil.SkipThisMessage(msg)) return;
            AddItemINTERNAL(await EditorChatFactory.CreateChatMessageItemAsync(GetMessageIndex(), msg, scriptFiles));
            AddTypingIndicatorINTERNAL();
            ChatScrollViewUtil.ScrollToBottom(ScrollView);
        }

        internal async void AddOrReplaceResponseMessage(ResponseMessage msg)
        {
            int index = GetMessageIndex();
            var renderItem = await EditorChatFactory.CreateChatMessageItemAsync(index, msg);

            if (_streamingItemIndexScrollView < 0)
            {
                AddItemINTERNAL(renderItem);
                ChatScrollViewUtil.ScrollToBottom(ScrollView);
                return;
            }

            _messageItems[_streamingItemIndex] = renderItem;

            if (ScrollView.ElementAt(_streamingItemIndexScrollView) is ChatMessageElement element)
            {
                element.Bind(renderItem, ShowMoreMessages, SubmitEditedMessageINTERNAL);
            }
            else
            {
                Debug.LogWarning("Element at streaming index is not a ChatMessageView. Rebuilding the view.");
                ScrollView.RemoveAt(_streamingItemIndexScrollView);
                ScrollView.Insert(_streamingItemIndexScrollView, BuildMessageElement(renderItem));
            }

            ChatScrollViewUtil.ScrollToBottom(ScrollView);
        }

        internal void UpdateStreamingResponse(string streamingResponseContent)
        {
            if (_streamingItem == null)
            {
                Debug.LogWarning("Streaming item is null. Cannot update streaming response.");
                return;
            }

            _streamingItem.Content = streamingResponseContent;
            var element = ScrollView.ElementAt(_streamingItemIndexScrollView) as ChatMessageElement;
            element?.UpdateContentOnly(streamingResponseContent); // 텍스트만 교체하는 경량 메서드 따로 만듦
            ChatScrollViewUtil.ScrollToBottom(ScrollView);
        }

        internal void ResetDisplayedMessageCount() => DisplayedMessageCount = EditorChatConfig.DefaultDisplayedMessageCount;

        internal bool IsShowingTempMessage()
        {
            // 마지막 메시지가 TempMessage인지 확인
            var lastItem = _messageItems.Count > 0 ? _messageItems[_messageItems.Count - 1] : null;
            return lastItem != null && lastItem.IsTempMessage;
        }

        internal void AddTempMessage(ChatMessageItem item, bool scrollToBottom = false, bool replace = false)
        {
            if (IsShowingTempMessage())
            {
                if (replace)
                {
                    Debug.LogWarning("Temporary message already exists. Replacing the last one.");
                    _messageItems[_messageItems.Count - 1] = item;
                    if (ScrollView.ElementAt(ScrollView.childCount - 1) is ChatMessageElement lastElement)
                    {
                        lastElement.Bind(item, ShowMoreMessages, SubmitEditedMessageINTERNAL);
                        lastElement.UpdateContentOnly(item.Content); // 텍스트만 교체
                    }
                }

                return;
            }

            // 새로운 임시 메시지 추가
            AddItemINTERNAL(item, scrollToBottom);
        }

        internal void ClearTempMessage()
        {
            if (!IsShowingTempMessage()) return;

            _showingShowMoreButton = false; // ShowMore 버튼 제거 

            for (int i = ScrollView.childCount - 1; i >= 0; i--)
            {
                if (ScrollView.ElementAt(i) is ChatMessageElement element &&
                    (element.Type == ChatMessageItemType.TempInfo ||
                     element.Type == ChatMessageItemType.TempError ||
                     element.Type == ChatMessageItemType.Typing))
                {
                    ScrollView.RemoveAt(i);
                }
                else
                {
                    break; // TempMessages는 항상 맨 뒤에 쌓이므로, 더 이상 TempMessages가 없으면 루프 종료
                }
            }
        }

        internal void RemoveStreamingItem()
        {
            if (_streamingItemIndexScrollView < 0 || _streamingItemIndex < 0) return;

            // ScrollView에서 Streaming 아이템 제거
            ScrollView.RemoveAt(_streamingItemIndexScrollView);
            _messageItems.RemoveAt(_streamingItemIndex);

            _streamingItem = null;
            _streamingItemIndex = -1;
            _streamingItemIndexScrollView = -1;

            ChatScrollViewUtil.ScrollToBottom(ScrollView);
        }

        // 보조 유틸리티 함수들
        private int GetMessageIndex()
        {
            int index = ScrollView.childCount;
            if (index < 0) index = 0; // 음수 인덱스 방지
            return index;
        }

        private VisualElement BuildMessageElement(ChatMessageItem item)
        {
            var view = new ChatMessageElement();
            view.Bind(item, ShowMoreMessages, SubmitEditedMessageINTERNAL);
            return view;
        }

        private void SubmitEditedMessageINTERNAL(int index, string content)
        {
            _window.SubmitEditedMessage(index, content);
            AddTypingIndicatorINTERNAL();
            ChatScrollViewUtil.ScrollToBottom(ScrollView);
        }

        private void ShowMoreMessages()
        {
            DisplayedMessageCount += EditorChatConfig.ShowMoreMessagesIncrement;
            _window.RebuildChatScrollView(false);
        }

        private void AddItemINTERNAL(ChatMessageItem item, bool scrollToBottom = false)
        {
            _messageItems.Add(item);
            ScrollView.Add(BuildMessageElement(item));

            if (ScrollView.childCount > DisplayedMessageCount)
            {
                int zeroIndex = _showingShowMoreButton ? 1 : 0; // ShowMore 버튼이 있다면 첫 번째 메시지는 제거하지 않음
                ScrollView.RemoveAt(zeroIndex);
                _messageItems.RemoveAt(zeroIndex);
            }

            if (scrollToBottom)
            {
                ChatScrollViewUtil.ScrollToBottom(ScrollView);
            }
        }

        private void AddShowMoreButtonINTERNAL()
        {
            int hiddenCount = _window.MessageCount - DisplayedMessageCount;
            int increment = EditorChatConfig.ShowMoreMessagesIncrement;

            // 남은 메시지가 ShowMoreMessagesIncrement보다 적으면 그만큼만 표시
            if (hiddenCount < EditorChatConfig.ShowMoreMessagesIncrement) increment = hiddenCount;

            var showMoreItem = new ChatMessageItem(0, ChatMessageItemType.ShowMore, $"Show {increment} more messages...");
            // add하지말고 insert한다.
            ScrollView.Insert(0, BuildMessageElement(showMoreItem));

            //AddItemINTERNAL(new ChatMessageItem(0, ChatMessageItemType.ShowMore, $"Show {increment} more messages..."));
            _showingShowMoreButton = true;
        }

        private void AddTypingIndicatorINTERNAL()
        {
            int index = GetMessageIndex();
            _streamingItem = new ChatMessageItem(index, ChatMessageItemType.Typing, string.Empty, AIDevKitIcons.Assistant);
            _messageItems.Add(_streamingItem);
            ScrollView.Add(BuildMessageElement(_streamingItem));

            _streamingItemIndex = _messageItems.Count - 1; // _messageItems에서의 인덱스
            _streamingItemIndexScrollView = ScrollView.childCount - 1;
        }
    }
}