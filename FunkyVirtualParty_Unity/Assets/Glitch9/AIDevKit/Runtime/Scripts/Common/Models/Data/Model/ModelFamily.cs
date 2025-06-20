namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Defines the family names of various AI models and services.
    /// </summary>
    public static class ModelFamily
    {
        /// <summary>Unknown or unidentified model family.</summary>
        public const string Unknown = "Unknown";

        #region ------------------------- OpenAI Family -------------------------

        /// <summary>OpenAI's general GPT series, including GPT-3, GPT-4, etc.</summary>
        public const string GPT = "GPT";

        /// <summary>OpenAI's GPT-4o ("omni") model, released in 2024.</summary>
        public const string o = "Omni";

        /// <summary>OpenAI's image generation model (DALL·E series).</summary>
        public const string DALL_E = "DALL·E";

        /// <summary>OpenAI's speech recognition model (Whisper).</summary>
        public const string Whisper = "Whisper";

        /// <summary>OpenAI's text embedding models.</summary>
        public const string TextEmbedding_OpenAI = "Text Embedding (OpenAI)";

        /// <summary>OpenAI's content moderation models.</summary>
        public const string Moderation_OpenAI = "Moderation (OpenAI)";

        /// <summary>OpenAI's real-time multimodal interaction models (Assistant API etc.).</summary>
        public const string Realtime = "Realtime";

        /// <summary>OpenAI's text-to-speech models.</summary>
        public const string TTS_OpenAI = "TTS (OpenAI)";

        #endregion ------------------------- OpenAI Family -------------------------


        #region ------------------------- Google Family -------------------------

        /// <summary>Google's Gemini series of multimodal AI models.</summary>
        public const string Gemini = "Gemini";

        /// <summary>Google's lightweight open-source language models (Gemma series).</summary>
        public const string Gemma = "Gemma";

        /// <summary>Google's AI models optimized for educational purposes (LearnLM).</summary>
        public const string LearnLM = "LearnLM";

        /// <summary>Google's image generation model (Imagen series).</summary>
        public const string Imagen = "Imagen";

        /// <summary>Google's text generation model based on PaLM 2 (Text PaLM2).</summary>
        public const string TextPaLM2 = "Text PaLM2";

        /// <summary>Google's chat-optimized model based on PaLM 2 (Chat PaLM2).</summary>
        public const string ChatPaLM2 = "Chat PaLM2";

        /// <summary>Google's text embedding models.</summary>
        public const string TextEmbedding_Google = "Text Embedding (Google)";

        #endregion ------------------------- Google Family -------------------------


        #region ------------------------- ElevenLabs Family -------------------------

        /// <summary>ElevenLabs' text-to-speech models.</summary>
        public const string TTS_ElevenLabs = "TTS (ElevenLabs)";

        /// <summary>ElevenLabs' speech-to-text model (Scribe).</summary>
        public const string Scribe = "Scribe";

        /// <summary>ElevenLabs' voice changer models for real-time voice transformation.</summary>
        public const string VoiceChanger_ElevenLabs = "Voice Changer (ElevenLabs)";

        #endregion ------------------------- ElevenLabs Family -------------------------


        #region ------------------------- Third-Party and Open Models -------------------------

        /// <summary>Meta's Llama family of open large language models.</summary>
        public const string Llama = "Llama";

        /// <summary>Google's Gemma model distributed via Ollama platform.</summary>
        public const string Gemma_Ollama = "Gemma (Ollama)";

        /// <summary>Mistral's open language models.</summary>
        public const string Mistral = "Mistral";

        /// <summary>Microsoft's Phi family of lightweight LLMs.</summary>
        public const string Phi = "Phi";

        /// <summary>DeepSeek's open large language model.</summary>
        public const string DeepSeek = "DeepSeek";

        /// <summary>Vicuna open-source fine-tuned chat models.</summary>
        public const string Vicuna = "Vicuna";

        /// <summary>Orca model (fine-tuned variant of LLMs).</summary>
        public const string Orca = "Orca";

        /// <summary>Starling open-source LLM project.</summary>
        public const string Starling = "Starling";

        /// <summary>TinyLlama ultra-small LLM project.</summary>
        public const string TinyLlama = "TinyLlama";

        /// <summary>GLM large language model (General Language Model) from Tsinghua University.</summary>
        public const string GLM = "GLM";

        /// <summary>EVA vision-language model (open-source).</summary>
        public const string EVA = "EVA";

        /// <summary>Command model family (e.g., Cohere Command R).</summary>
        public const string Command = "Command";

        /// <summary>Anthropic's Claude series (Claude 1, 2, 3).</summary>
        public const string Claude = "Claude";

        /// <summary>Anubis open model (Ollama distributed).</summary>
        public const string Anubis = "Anubis";

        /// <summary>Aion open model (Ollama distributed).</summary>
        public const string Aion = "Aion";

        /// <summary>Airoboros fine-tuned models for instruction following.</summary>
        public const string Airoboros = "Airoboros";

        /// <summary>Fimbulvetr lightweight LLM project.</summary>
        public const string Fimbulvetr = "Fimbulvetr";

        /// <summary>Deepcoder model focused on code generation.</summary>
        public const string Deepcoder = "Deepcoder";

        /// <summary>Llemma model optimized for code and math reasoning.</summary>
        public const string Llemma = "Llemma";

        /// <summary>Qwen large language models (Alibaba).</summary>
        public const string Qwen = "Qwen";

        /// <summary>Jamba mixture-of-experts LLM (Mamba/Jamba family).</summary>
        public const string Jamba = "Jamba";

        /// <summary>WizardLM instruction-tuned language models.</summary>
        public const string WizardLM = "WizardLM";

        /// <summary>Midnight Rose fine-tuned conversational models.</summary>
        public const string MidnightRose = "Midnight Rose";

        /// <summary>Dolphin open LLM project (Dolphin 2.6, 2.7 etc.).</summary>
        public const string Dolphin = "Dolphin";

        /// <summary>Zephyr lightweight chat-optimized LLMs.</summary>
        public const string Zephyr = "Zephyr";

        /// <summary>Yi model family from 01.AI (Yi-6B, Yi-34B).</summary>
        public const string Yi = "Yi";

        /// <summary>Weaver model family by Alibaba (multimodal capabilities).</summary>
        public const string Weaver = "Weaver";

        /// <summary>Toppy lightweight LLMs (Ollama distributed).</summary>
        public const string Toppy = "Toppy";

        /// <summary>Sonar family models (real-time applications).</summary>
        public const string Sonar = "Sonar";

        /// <summary>Rogue Rose fine-tuned conversational models.</summary>
        public const string RogueRose = "Rogue Rose";


        //public const string Nou

        #endregion ------------------------- Third-Party and Open Models -------------------------
    }
}
