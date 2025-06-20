using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static partial class EditorChatUtil
    {
        internal static class Event
        {
            internal static void RegisterInputFieldCallbacks(TextField textField, Button sendButton, Action sendRequest)
            {
                textField.RegisterValueChangedCallback(evt =>
                {
                    bool hasText = !string.IsNullOrWhiteSpace(evt.newValue);

                    if (hasText)
                        sendButton.AddToClassList("chat-send-button--active");
                    else
                        sendButton.RemoveFromClassList("chat-send-button--active");
                });

                textField.RegisterCallback<MouseDownEvent>(e =>
                {
                    bool isInputFocused = textField.focusController?.focusedElement == textField;

                    if (isInputFocused && e.clickCount == 1)
                    {
                        textField.Blur();
                        e.StopPropagation();
                    }
                });

                textField.RegisterCallback<KeyDownEvent>(e =>
                {
                    AIDevKitDebug.Blue($"Key pressed: {e.keyCode} - Shift: {e.shiftKey}");

                    if (!e.shiftKey && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
                    {
                        if (!string.IsNullOrWhiteSpace(textField.value))
                        {
                            //RequestWithCurrentPrompt();
                            sendRequest?.Invoke();
                            // clear input field
                            textField.value = string.Empty;
                            e.StopPropagation();
                        }
                    }
                });


                textField.RegisterValueChangedCallback(evt =>
                {
                    // 내부 텍스트 요소 가져오기
                    var uti = textField.Q("unity-text-input");
                    if (uti == null) return;

                    uti.style.height = StyleKeyword.Auto; // 내부 input도 auto
                    textField.style.height = StyleKeyword.Auto; // 텍스트필드 자체도 auto

                    // 스타일 초기화 (자동 높이로)
                    //uti.style.height = StyleKeyword.Auto;

                    // 다음 프레임에 실제 렌더링 높이 측정 후 반영
                    textField.schedule.Execute(() =>
                    {
                        float preferredHeight = uti.resolvedStyle.height;
                        textField.style.height = preferredHeight;
                        // textField.parent.style.minHeight = preferredHeight;
                    }).ExecuteLater(1);
                });

                sendButton.clicked += () =>
                {
                    if (!string.IsNullOrWhiteSpace(textField.value))
                    {
                        //RequestWithCurrentPrompt();
                        sendRequest?.Invoke();
                        textField.value = string.Empty; // 입력 필드 비우기 
                    }
                };
            }

            internal static void RegisterInputFieldFocusEventToRoot(VisualElement rootVisualElement, TextField textField)
            {
                rootVisualElement.focusable = true;
                rootVisualElement.pickingMode = PickingMode.Position;
                rootVisualElement.RegisterCallback<KeyDownEvent>(e =>
                {
                    bool isInputFocused = textField.focusController?.focusedElement == textField;
                    //Debug.Log($"Key pressed: {e.keyCode} - Input Focused: {isInputFocused}");

                    if (!isInputFocused)
                    {
                        // 문자 키나 스페이스, 엔터 등
                        if (e.keyCode >= KeyCode.A && e.keyCode <= KeyCode.Z ||
                            e.keyCode == KeyCode.Space || e.keyCode == KeyCode.Return ||
                            e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Backspace ||
                            e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Tab)
                        {
                            //Debug.Log($"Key pressed: {e.keyCode} - Focusing input field");

                            textField.Focus();
                            e.StopPropagation();
                        }
                    }
                });
            }
        }
    }
}