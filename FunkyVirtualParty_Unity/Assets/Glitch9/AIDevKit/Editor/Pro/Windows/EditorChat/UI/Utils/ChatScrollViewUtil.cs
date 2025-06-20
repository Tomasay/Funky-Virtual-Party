using System;
using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static class ChatScrollViewUtil
    {
        internal static bool SkipThisMessage(ChatMessage msg)
        {
            if (msg is SystemMessage systemMessage && string.IsNullOrWhiteSpace(systemMessage.DisplayMessage))
            {
                return true; // 시스템 메시지의 DisplayMessage(Content와 다름)가 비어있으면 건너뜀 
            }
            return false;
        }

        internal static async void ScrollToBottom(ScrollView scrollView, int frames = -1)
        {
            if (frames < 1)
            {
                await UniTask.Yield(); // UI 업데이트가 완료될 때까지 대기
            }
            else
            {
                await UniTask.DelayFrame(frames); // 지정된 프레임 수만큼 대기 
            }

            try
            {
                await UniTask.WaitUntil(() => scrollView != null && scrollView.panel != null)
                    .Timeout(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException)
            {
                if (EditorChatSettings.DebugMode) AIDevKitDebug.Warning("ScrollView or panel is still null after waiting, cannot scroll to bottom.");
                return;
            }

            // 컨텐츠가 넘치지 않으면 스크롤할 필요 없음
            bool isContentOverflowing = scrollView.contentContainer.layout.height > scrollView.layout.height;
            if (!isContentOverflowing) return;

            // 계속 시도하면서 layout이 완전히 잡힐 때까지 기다림
            scrollView.schedule.Execute(() =>
            {
                if (scrollView.verticalScroller != null)
                {
                    scrollView.verticalScroller.value = scrollView.verticalScroller.highValue;
                    //AIDevKitDebug.Mark("Scrolled to bottom (scheduled).");
                }
            }).Until(() => scrollView.panel != null && scrollView.panel.visualTree.layout.width > 0);
        }

        internal static string[] ResolveItemContainerClassNames(ChatMessageItemType type)
        {
            return type switch
            {
                ChatMessageItemType.User => "chat-message-container--user".ToArray(),
                ChatMessageItemType.Assistant or ChatMessageItemType.Streaming or ChatMessageItemType.Typing => "chat-message-container--ai".ToArray(),
                ChatMessageItemType.TempInfo => new string[] { "chat-message-container--temp", "chat-message-container--temp-info" },
                ChatMessageItemType.TempWarning => new string[] { "chat-message-container--temp", "chat-message-container--temp-warning" },
                ChatMessageItemType.TempError => new string[] { "chat-message-container--temp", "chat-message-container--temp-error" },
                ChatMessageItemType.ShowMore => "chat-message-container--show-more".ToArray(),
                _ => "chat-message-container--system".ToArray()
            };
        }

        internal static string ResolveButtonClassName(ChatMessageItemType type)
        {
            return type switch
            {
                ChatMessageItemType.TempInfo => "chat-message-button--info",
                ChatMessageItemType.TempWarning => "chat-message-button--warning",
                ChatMessageItemType.TempError => "chat-message-button--error",
                _ => "chat-message-button--info"
            };
        }

        internal static VisualElement CreateContentDetailsRow(ChatMessageItem item, out Label timestampLabel)
        {
            bool showUsage = EditorChatSettings.ShowUsage && item.Usage != null;
            timestampLabel = null;
            if (!EditorChatSettings.ShowTimestampAI && !showUsage) return null;

            var detailsRow = new VisualElement { name = "details-row" };
            detailsRow.AddToClassList("chat-message-details-row");

            bool itemAdded = false;

            if (((item.Type == ChatMessageItemType.User && EditorChatSettings.ShowTimestampUser)
                || (item.Type == ChatMessageItemType.Assistant && EditorChatSettings.ShowTimestampAI))
                && item.Timestamp.Year != 1970)
            {
                timestampLabel = new Label(item.Timestamp.GetInspectorName())
                {
                    name = "timestamp",
                    pickingMode = PickingMode.Ignore
                };
                timestampLabel.AddToClassList("chat-message-details");
                detailsRow.Add(timestampLabel);
                itemAdded = true;
            }

            if (showUsage)
            {
                var usage = new Label(item.Usage.ToString())
                {
                    name = "usage",
                    pickingMode = PickingMode.Ignore,
                    style = { color = EditorColors.orange }
                };
                usage.AddToClassList("chat-message-details");
                detailsRow.Add(usage);
                itemAdded = true;
            }

            if (!itemAdded) return null; // 아무것도 추가되지 않았다면 null 반환 
            return detailsRow;
        }
    }
}