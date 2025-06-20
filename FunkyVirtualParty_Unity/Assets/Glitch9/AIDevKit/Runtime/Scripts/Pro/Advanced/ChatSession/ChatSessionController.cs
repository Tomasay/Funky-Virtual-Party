using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.AIDevKit.Advanced.Chat
{
    /// <summary>
    /// Controller for managing <see cref="ChatSession"/>.
    /// </summary>
    internal class ChatSessionController
    {
        private readonly ChatSummaryGenerator _summaryGenerator = new();
        private readonly ChatTitleGenerator _titleGenerator = new();
        internal ChatMessage _lastSent;
        internal ChatCompletion _lastReceivedChatCompletion;
        internal ResponseMessage _lastReceivedFormattedMessage;
        internal Exception _lastException;
        private DateTime _lastSaveTime = DateTime.MinValue;
        private bool _saveScheduled = false;
        private readonly ChatSession _session;
        internal ChatSessionController(ChatSession session) => _session = session ?? throw new ArgumentNullException(nameof(session));

        private List<ChatMessage> Messages
        {
            get => _session._messages;
            set => _session._messages = value;
        }

        private List<ChatMessage> RecentMessages
        {
            get => _session._recentMessages;
            set => _session._recentMessages = value;
        }

        private string LastMessage
        {
            set => _session.LastMessage = value;
        }

        internal List<ChatMessage> GetMessages()
        {
            if (_lastReceivedChatCompletion != null)
            {
                ChatChoice firstChoice = _lastReceivedChatCompletion.Choices.FirstOrDefault();
                StopReason finishReason = firstChoice?.FinishReason ?? StopReason.None;
                Debug.Log($"FinishReason: {finishReason}");

                if (finishReason != StopReason.None &&
                    finishReason != StopReason.Stop &&
                    finishReason != StopReason.MaxTokens)
                {
                    InterruptedResponseException error = new(finishReason);
                    _lastException = error;
                }
            }

            if (_lastException != null)
            {
                throw new BrokenResponseException("Unable to build a coherent chat history due to a broken streaming response.", _lastException);
            }

            PushLastInput();
            PushLastOutput();

            return Messages;
        }

        internal void SetMessages(List<ChatMessage> value)
        {
            Messages = value ?? new();
            _lastSent = null;
            _lastReceivedChatCompletion = null;
        }

        internal void SetLastInput(ChatMessage inputMessage)
        {
            if (inputMessage == null || inputMessage.Content.IsNull)
            {
                Debug.Log("Empty input message");
                return;
            }

            _lastException = null;
            _lastSent = inputMessage;
            LastMessage = _lastSent.Content;
        }

        internal ResponseMessage SetLastOutput(ChatCompletion response)
        {
            if (response == null)
            {
                //AIDevKitDebug.Mark("Received null response from chat completion.");
                return null;
            }

            GenerateTitleIfNecessary();

            _lastException = null;
            _lastReceivedChatCompletion = response;
            LastMessage = _lastReceivedChatCompletion.ToString();

            if (response.Usage != null) // Log usage for debugging purposes
                AIDevKitDebug.Green($"Response usage: {response.Usage}");

            var msg = ResponseMessageFactory.FromChatCompletion(response);

            _lastReceivedFormattedMessage = msg;
            PushLastOutput();

            return msg;
        }

        private async void GenerateTitleIfNecessary()
        {
            if (_session != null
                && (string.IsNullOrWhiteSpace(_session.Name) || _session.Name == ChatSession.kDefaultTitle)
                && _lastSent is UserMessage userMessage
                && _session.AutoTitle && !_session.IsTitleGenerated)
            {
                (string generatedTitle, Usage usage) = await _titleGenerator.GenerateTitleAsync(_session.UtilityModel, userMessage);

                if (!string.IsNullOrWhiteSpace(generatedTitle))
                {
                    _session.Name = generatedTitle;
                    _session.IsTitleGenerated = true;
                    Debug.Log($"Generated chat session title: {_session.Name}");
                }

                if (usage != null)
                {
                    Currency cost = _session.UtilityModel.EstimatePrice(usage);

                    if (cost != null)
                    {
                        _session.TotalCostInUSD += (float)cost.PriceInUsd;
                        AddTotalUsage(usage, cost);
                    }
                }
            }
        }

        private void PushLastInput()
        {
            if (_lastSent == null) return;

            Messages.Add(_lastSent);
            RecentMessages.Add(_lastSent);
            _lastSent = null;
            _session.UpdatedAt = UnixTime.Now;
        }

        private void PushLastOutput()
        {
            if (_lastReceivedFormattedMessage == null) return;

            Messages.Add(_lastReceivedFormattedMessage);
            RecentMessages.Add(_lastReceivedFormattedMessage);
            AddTotalUsage(_lastReceivedChatCompletion?.Usage);

            // Make sure to reset BOTH _lastReceivedChatCompletion and _lastReceivedFormattedMessage
            // Otherwise, the saving process will be repeatedly triggered
            _lastReceivedChatCompletion = null;
            _lastReceivedFormattedMessage = null;

            _session.UpdatedAt = UnixTime.Now;

            if (_session.AutoSave) ScheduleAutoSave();
            UpdateSummaryIfNecessary();
        }

        internal void ClearMessages()
        {
            Messages.Clear();
            _session.UpdatedAt = UnixTime.Now;
            if (_session.AutoSave) _session.SaveFile();
        }

        internal (ChatMessage lastSent, ChatMessage lastReceived) Rewind()
        {
            if (_lastReceivedChatCompletion == null)
            {
                ChatMessage lastSent = Messages[Messages.Count - 2];
                ChatMessage lastReceived = Messages.Last();
                Messages.RemoveRange(Messages.Count - 2, 2);
                return (lastSent, lastReceived);
            }
            else
            {
                ChatMessage lastSent = _lastSent;
                ChatMessage lastReceived = _lastReceivedChatCompletion.FirstResponseMessage();
                _lastSent = null;
                _lastReceivedChatCompletion = null;
                return (lastSent, lastReceived);
            }
        }

        internal List<ChatMessage> GetContextMessages()
        {
            if (Messages.IsNullOrEmpty()) return null;

            List<ChatMessage> contextMessages = new();
            int count = Math.Min(Messages.Count, _session.MaxContextMessages);
            for (int i = 0; i < count; i++)
            {
                contextMessages.Add(Messages[Messages.Count - 1 - i]);
            }

            return contextMessages;
        }

        internal void AddTotalUsage(Usage usage, Currency cost = null)
        {
            if (usage == null) return;

            _session.TotalUsage = _session.TotalUsage.Merge(usage);

            if (cost != null)
            {
                _session.TotalCostInUSD += (float)cost.PriceInUsd;
                return;
            }

            Model model = _session.Model;

            if (model == null)
            {
                Debug.LogWarning("Session model is null, cannot calculate usage cost.");
                return;
            }

            cost = model.EstimatePrice(usage);
            if (cost != null)
            {
                _session.TotalCostInUSD += (float)cost.PriceInUsd;
            }
        }

        internal Usage CalcTotalUsage()
        {
            Usage totalUsage = new();

            foreach (ChatMessage message in Messages)
            {
                if (message is ResponseMessage responseMessage && responseMessage.Usage != null)
                {
                    totalUsage.Merge(responseMessage.Usage);
                }
            }

            return totalUsage;
        }

        internal float CalcTotalCostInUSD()
        {
            Usage totalUsage = CalcTotalUsage();
            Model model = _session.Model;

            if (model == null)
            {
                Debug.LogWarning("Session model is null, cannot calculate total cost.");
                return 0f;
            }

            Currency result = model.EstimatePrice(totalUsage);
            if (result == null)
            {
                Debug.LogWarning("Model estimate price returned null, cannot calculate total cost.");
                return 0f;
            }

            return (float)result.PriceInUsd;
        }

        private async void UpdateSummaryIfNecessary()
        {
            if (RecentMessages.IsNullOrEmpty() || RecentMessages.Count < _session.MaxContextMessages) return;

            try
            {
                (string newSummary, Usage usage) = await _summaryGenerator.UpdateSummaryAsync(_session.UtilityModel, _session.Summary, RecentMessages);

                if (!string.IsNullOrWhiteSpace(newSummary))
                {
                    _session.Summary = newSummary;
                }

                if (usage != null)
                {
                    Currency cost = _session.UtilityModel.EstimatePrice(usage);

                    if (cost != null)
                    {
                        _session.TotalCostInUSD += (float)cost.PriceInUsd;
                        AddTotalUsage(usage, cost);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating summary: {e.Message}");
            }
            finally
            {
                _session.LastSummaryUpdate = UnixTime.Now;
                RecentMessages.Clear();
                if (_session.AutoSave) ScheduleAutoSave();
            }
        }

        private async void ScheduleAutoSave()
        {
            if (_saveScheduled) return;

            _saveScheduled = true;

            await System.Threading.Tasks.Task.Delay(500); // 0.5초 딜레이로 디바운스

            if ((DateTime.UtcNow - _lastSaveTime).TotalSeconds >= 0.3)
            {
                _session.SaveFileAsync().Forget();
                _lastSaveTime = DateTime.UtcNow;
            }

            _saveScheduled = false;

            //Debug.Log($"Auto-saved chat session: {Id} at {_lastSaveTime}");
        }

    }
}