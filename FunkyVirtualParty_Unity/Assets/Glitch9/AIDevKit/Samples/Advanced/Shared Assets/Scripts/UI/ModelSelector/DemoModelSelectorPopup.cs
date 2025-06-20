using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.OpenAI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoModelSelectorPopup : MonoBehaviour
    {
        [SerializeField] private DemoModelPrefab modelPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private DemoButton closeButton;

        public Model selected { get; private set; }

        private DemoModelSelector _selector;
        private List<DemoModelPrefab> _prefabs;
        private bool _isInitialized = false;

        public void Show(DemoModelSelector selector, string modelFamily)
        {
            if (_isInitialized)
            {
                RefreshSelection();
                return;
            }

            _selector = selector;
            _prefabs = new();

            Type enumType = modelFamily switch
            {
                // ModelFamily.OpenAI_GPT => typeof(GPTModel),
                // ModelFamily.OpenAI_DallE => typeof(OpenAI.DallEModel),
                // ModelFamily.OpenAI_TTS => typeof(OpenAITTS),
                // ModelFamily.OpenAI_Whisper => typeof(WhisperModel),
                // ModelFamily.OpenAI_Embeddings => typeof(EmbeddingModel),
                // ModelFamily.OpenAI_Moderation => typeof(ModerationModel),
                // ModelFamily.Google_Gemini => typeof(GeminiModel),
                _ => typeof(string)
            };

            // foreach (Enum model in Enum.GetValues(enumType))
            // {
            //     DemoModelPrefab prefab = Instantiate(modelPrefab, container);
            //     prefab.model = model;
            //     prefab.IsSelected = prefab.model == selected;
            //     prefab.onSelect += _selector.Select;
            //     _prefabs.Add(prefab);
            // }

            closeButton.onClick += selector.Hide;
            RefreshSelection();

            _isInitialized = true;
        }

        private void RefreshSelection()
        {
            foreach (DemoModelPrefab prefab in _prefabs)
            {
                prefab.IsSelected = prefab.model == selected;
            }
        }
    }
}