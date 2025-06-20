using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor.Collections;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;
using OpenAIClient = Glitch9.AIDevKit.OpenAI.OpenAI;

namespace Glitch9.AIDevKit.Editor.Assistants
{
    internal static class AssistantManager
    {
        internal static List<Assistant> Assistants => _assistants.Value;
        private static readonly EPrefsList<Assistant> _assistants = new($"{AIDevKitEditor.Labels.OpenAIAssistants}.{nameof(Assistants)}", settings: OpenAIClient.DefaultInstance.JsonSettings);

        internal static Assistant GetAssistant(string assistantId)
        {
            if (string.IsNullOrEmpty(assistantId)) return null;

            foreach (Assistant assistant in Assistants)
            {
                if (assistant.Id == assistantId) return assistant;
            }

            return null;
        }

        internal static void RemoveAssistantLocally(string assistantId)
        {
            if (string.IsNullOrEmpty(assistantId)) return;

            foreach (Assistant assistant in Assistants)
            {
                if (assistant.Id == assistantId)
                {
                    Assistants.Remove(assistant);
                    break;
                }
            }
        }

        internal static void Save() => _assistants.Save();

        internal static async UniTask<List<Assistant>> ReloadAssistantsAsync()
        {
            QueryResponse<Assistant> queryResponse = await OpenAIClient.DefaultInstance.Beta.Assistants.ListAsync(new(100));
            Assistant[] assistants = queryResponse?.Data;
            if (assistants.IsNullOrEmpty()) return null;

            Debug.Log($"Loaded {assistants!.Length} assistants from OpenAI");

            Assistants.Clear();
            List<Assistant> treeViewData = new();

            foreach (Assistant assistant in assistants)
            {
                Assistants.Add(assistant);
                treeViewData.Add(assistant);
            }

            Save();
            return treeViewData;
        }

        internal static async UniTask<bool> UpdateAssistantAsync(string id, AssistantRequest req)
        {
            await OpenAIClient.DefaultInstance.Beta.Assistants.UpdateAsync(id, req);
            return true;
        }

        internal static async UniTask<Assistant> CreateAssistantAsync()
        {
            AssistantRequest req = new AssistantRequest.Builder()
                  .SetName("New Assistant")
                  .SetDescription("This is a new assistant created in Unity Editor.")
                  .SetInstructions("Provide instructions for the assistant here.")
                  .Build();

            Assistant assistant = await OpenAIClient.DefaultInstance.Beta.Assistants.CreateAsync(req);
            if (assistant == null) return null;

            Assistants.Add(assistant);
            Save();
            return assistant;
        }
    }
}