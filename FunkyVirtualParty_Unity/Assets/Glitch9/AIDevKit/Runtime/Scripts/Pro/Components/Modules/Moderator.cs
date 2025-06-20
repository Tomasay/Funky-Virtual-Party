using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Moderator")]
    public class Moderator : AIModuleComponent
    {
        [SerializeField] private List<SafetySetting> safetySettings = new();
        [SerializeField] private UnityEvent<Moderation> onMessageModerated;
        [SerializeField] private UnityEvent<Moderation> onMessageFlagged;

        public List<SafetySetting> SafetySettings => safetySettings ??= new();
        public async UniTask<Moderation> ModerateMessageAsync(string inputMessage, List<IFile> attachedFiles = null)
        {
            if (string.IsNullOrWhiteSpace(inputMessage) && attachedFiles.IsNullOrEmpty()) return null;

            if (IsBusy)
            {
                OnError("Moderator is already processing another request.");
                return null;
            }

            IsBusy = true;
            try
            {
                Moderation moderation = await inputMessage
                    .GENModeration(safetySettings)
                    .SetModel(model)
                    .Attach(attachedFiles?.ToArray())
                    .ExecuteAsync();

                if (moderation == null || moderation.IsEmpty)
                {
                    // OnError("Moderation returned no content. Please check the request and try again.");
                    // return null;
                    throw new EmptyResponseException("Moderation returned no content. Please check the request and try again.");
                }

                if (moderation.IsFlagged) onMessageFlagged?.Invoke(moderation);

                onMessageModerated?.Invoke(moderation);

                return moderation;
            }
            catch (Exception ex)
            {
                OnError(ex);
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}