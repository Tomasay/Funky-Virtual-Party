using UnityEngine;

namespace Glitch9.AIDevKit
{
    public enum Api
    {
        [InspectorName("Unknown"), HideInInspector] None, // Refers to 'Local' when used in JsonConverter
        [InspectorName("OpenAI")] OpenAI,
        [InspectorName("Google")] Google,
        [InspectorName("ElevenLabs")] ElevenLabs,
        [InspectorName("Mubert")] Mubert,
        [InspectorName("Ollama")] Ollama,
        [InspectorName("OpenRouter")] OpenRouter,
        [InspectorName("All"), HideInInspector] All,
    }

    public class AIProviders
    {
        public const string OpenAI = "OpenAI";
        public const string Google = "Google";
        public const string ElevenLabs = "ElevenLabs";
        public const string Mubert = "Mubert";
        public const string Ollama = "Ollama";
        public const string OpenRouter = "OpenRouter";
        public const string Naver = "Naver HyperCLOVA";
        public const string PlayHT = "PlayHT";
        public const string Cohere = "Cohere";
        public const string Anthropic = "Anthropic";
        public const string Mistral = "Mistral";
        public const string Speechmatics = "Speechmatics";
        public const string CognitiveComputations = "CognitiveComputations";
        public const string NousResearch = "NousResearch";
        public const string TheDrummer = "TheDrummer";
        public const string OpenGVLab = "OpenGVLab";
        public const string Perplexity = "Perplexity";
        public const string HuggingFaceH4 = "HuggingFaceH4";
        public const string EleutherAI = "EleutherAI";
        public const string Meta = "Meta";
        public const string _01_AI = "01.ai";
        public const string Aetherwiing = "Aetherwiing";
        public const string AgenticaOrg = "Agentica Org";
        public const string AI21 = "AI21";
        public const string AionLabs = "Aion Labs";
        public const string AlfredPros = "AlfredPros";
        public const string AllHands = "All Hands";
        public const string AllenAI = "AllenAI";
        public const string Alpindale = "Alpindale";
        public const string Amazon = "Amazon";
        public const string AnthraciteOrg = "Anthracite Org";
        public const string ArliAI = "ArliAI";
        public const string DeepSeek = "DeepSeek";
        public const string EVA_UNIT_01 = "EVA Unit 01";
        public const string Featherless = "Featherless";
        public const string Gryphe = "Gryphe";
        public const string Inception = "Inception";
        public const string Infermatic = "Infermatic";
        public const string Inflection = "Inflection";
        public const string Jondurbin = "JonDurbin";
        public const string LatitudeGames = "Latitude Games";
        public const string Liquid = "Liquid";
        public const string Mancer = "Mancer";
        public const string MetaLlama = "Meta-Llama";
        public const string Microsoft = "Microsoft";
        public const string Minimax = "Minimax";
        public const string MistralAI = "MistralAI";
        public const string MoonshotAI = "MoonshotAI";
        public const string NeverSleep = "NeverSleep";
        public const string NothingIsReal = "NothingIsReal";
        public const string Nvidia = "NVIDIA";
        public const string OpenR1 = "OpenR1";
        public const string PygmalionAI = "PygmalionAI";
        public const string Qwen = "Qwen";
        public const string Raifle = "Raifle";
        public const string RekaAI = "RekaAI";
        public const string Sao10k = "Sao10k";
        public const string Scb10x = "SCB10x";
        public const string ShisaAI = "ShisaAI";
        public const string Sophosympatheia = "Sophosympatheia";
        public const string Steelskull = "Steelskull";
        public const string Thudm = "THUDM";
        public const string Tngtech = "TNGTech";
        public const string Undi95 = "Undi95";
        public const string X_AI = "xAI";
    }
}