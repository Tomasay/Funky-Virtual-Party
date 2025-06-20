using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class GUIContents
    {
        internal static readonly GUIContent ApiKeySectionTitle = new(
            "API Access & Configuration",
            "An API key is required to connect to external AI services. Configure your key and encryption preferences here.");

        internal static readonly GUIContent UsefulLinksSectionTitle = new(
            "Useful Links",
            "Links to the official documentation and support channels for the AIDevKit.");

        internal static readonly GUIContent DefaultModelsSectionTitle = new(
            "Default Models",
            "The default models to use for the API. This is used when no model is specified in the request.");

        internal static readonly GUIContent DefaultMediaModelsSectionTitle = new(
            "Default Media Models",
            "The default media models (e.g., image, video) to use for the API. This is used when no model is specified in the request.");

        internal static readonly GUIContent DefaultAudioModelsAndVoiceSectionTitle = new(
            "Default Audio Models & Voice",
            "Default audio models (e.g., TTS, STT) and voice actor to use for the API. " +
            "This is used when no model or voice is specified in the request.");

        internal static readonly GUIContent DefaultModelsForAdvancedAPISectionTitle = new(
            "Default Advanced API Models",
            "The default models to use for the advanced API. This is used when no model is specified in the request.");

        internal static readonly GUIContent AdminApiKeyLabel = new(
            "Admin API Key (Optional)",
            "The primary API key used for administrative access or shared service-level requests. Keep this key secure.");

        internal static readonly GUIContent ApiOrganizationLabel = new(
            "Organization (Optional)",
            "This is used to specify the unique identifier of the organization under which the API call is being made. " +
            "This is particularly relevant for users who are part of an organization that has its own OpenAI account, " +
            "separate from personal accounts. By including the organization ID in the API request header, " +
            "it ensures that the usage and billing for that call are correctly attributed to the organization's account, " +
            "rather than an individual's account.");

        internal static readonly GUIContent ApiProjectIdLabel = new(
            "Project ID (Optional)",
            "This is used to specify the unique identifier of the project under which the API call is being made. " +
            "This is particularly relevant for users who are part of a project that has its own OpenAI account, " +
            "separate from personal accounts. By including the project ID in the API request header, " +
            "it ensures that the usage and billing for that call are correctly attributed to the project's account, " +
            "rather than an individual's account.");

        internal static readonly GUIContent MaxTokens = new(
            "Max Prompt Tokens",
            "The maximum number of tokens allowed for the prompt in a request. " +
            "This setting helps prevent the prompt from exceeding the model's token limit.");

        internal static readonly GUIContent UpdateModelsOnStartup = new(
            "Auto Update Models On Startup",
            "If enabled, the plugin will automatically update the models on startup. Only available in the Pro version.");

        internal static readonly GUIContent AddToLibrary = new(EditorIcons.Import, "Add to Library");
        internal static readonly GUIContent RemoveFromLibrary = new(EditorIcons.Delete, "Remove from Library");
        internal static readonly GUIContent OnlineDocument = new("Online Document", "Open the AIDevKit online documentation in your default browser.");
        internal static readonly GUIContent JoinDiscord = new("Join Discord", "Join the AIDevKit Discord server for support and community discussions.");
        internal static readonly GUIContent Preferences = new("Preferences", "Open the AIDevKit preferences window.");
        internal static readonly GUIContent ApiDefaultModel = new("Default Model", "The default model to use for the API. This is used when no model is specified in the request.");
        internal static readonly GUIContent SaveHistory = new("Save Request History", "If checked, all chat requests will be saved in 'PromptHistory.asset' file.");
        internal static readonly GUIContent SaveOutputs = new("Save Outputs", "If checked, the output files will be saved to the specified output path. This is used to store the output files after they are processed.");
        internal static readonly GUIContent OutputPath = new("Output Folder", "The path to the folder where the output files will be saved. This is used to store the output files after they are processed.");


        // Default Models & Voice - Added 2025.05.01
        internal static readonly GUIContent DefaultLLM = new("Text Generation", "The default model to use for text generation. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultIMG = new("Image Generation", "The default model to use for image generation. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultTTS = new("Text-to-Speech", "The default model to use for text to speech. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultSTT = new("Speech-to-Text", "The default model to use for speech to text. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultEMB = new("Text-Embedding", "The default model to use for embeddings generation. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultMOD = new("Moderation", "The default model to use for moderation. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultVCM = new("Voice Changer", "The default model to use for voice synthesis. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultASS = new("Assistants API", "The default GPT model to use for the Assistants API.");
        internal static readonly GUIContent DefaultRTM = new("Realtime API", "The default model to use for real-time generation. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultVID = new("Video Generation", "The default model to use for video generation. This is used when no model is specified in the request.");
        internal static readonly GUIContent DefaultVoice = new("Voice Actor", "The default voice to use for text to speech or voice changer. This is used when no voice is specified in the request.");

        // Common Options
        internal static readonly GUIContent N = new("Count",
            "The number of contents to generate. If set to 1, only one content will be generated. If set to 2, two contents will be generated, and so on. " +
            "This is useful for generating multiple variations of the same content in a single request.");

        internal static readonly GUIContent Seed = new("Seed",
            "The seed value for random number generation. This is used to ensure that the same input will always produce the same output. " +
            "If not specified, a random seed will be used, which means that the output will be different each time.");

        internal static readonly GUIContent Model = new("Model",
            "Select the model to use to generate the content.");

        internal static readonly GUIContent ChatModel = new("Chat Model",
            "Model used for chat interactions, such as conversations, question answering, and dialogue generation.");

        internal static readonly GUIContent UtilityModel = new("Utility Model",
            "Model used for utility tasks such as conversation summary, auto-naming, and metadata generation. " +
            "Selected model must support 'Structured Outputs(JsonSchema)' feature.");

        internal static readonly GUIContent UseProjectContext = new("Use Project Context",
            "Apply the project context set in AIDevKit settings to the prompt.");

        internal static readonly GUIContent PromptInfluence = new("Influence",
            "The influence of the prompt on the generated content. A value of 0 means the prompt has no influence, " +
            "while a value of 1 means the prompt has full influence. Values between 0 and 1 will apply a partial influence to the prompt.");

        // LLM Options
        internal static readonly GUIContent Instructions = new("Instructions (Optional)", "The system instructions to guide the LLM model's behavior.");
        internal static readonly GUIContent StartingMessage = new("Starting Message (Optional)", "The initial message from the assistant.");
        internal static readonly GUIContent Temperature = new("Temperature", "The temperature controls the randomness of the output. Higher values (e.g., 1) make the output more random, while lower values (e.g., 0.1) make it more focused and deterministic.");
        internal static readonly GUIContent TopP = new("Top P", "An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass.");
        internal static readonly GUIContent FrequencyPenalty = new("Frequency Penalty", "Penalty for how often new tokens should be different from existing ones.");
        internal static readonly GUIContent MaxInputTokens = new("Max Input Tokens", "The maximum number of tokens in the input. -1 means no limit.");
        internal static readonly GUIContent MaxOutputTokens = new("Max Output Tokens", "The maximum number of tokens to generate. -1 means no limit.");
        internal static readonly GUIContent Stream = new("Stream", "Whether to stream the response.");
        internal static readonly GUIContent ReasoningEffort = new("Reasoning Effort", "The effort level for reasoning. Higher values indicate more effort.");

        // Image Options
        internal static readonly GUIContent ImageSize = new("Size", "The size of the image to be generated.");
        internal static readonly GUIContent ImageQuality = new("Quality", "The quality of the image to be generated.");
        internal static readonly GUIContent ImageStyle = new("Style", "The style of the image to be generated.");
        internal static readonly GUIContent IsTileable = new("Tileable", "If checked, AI will try to generate a tileable texture, but it may not be perfect.");
        internal static readonly GUIContent RemoveBackground = new("Remove Background", "If checked, AI will try to remove the background from the generated image.");
        internal static readonly GUIContent PersonGeneration = new("Person Generation", "Allow the model to generate images (or videos) of people.");
        internal static readonly GUIContent RmbgThreshold = new("RMBG Threshold", "The threshold for the remove background feature. A value between 0 and 1, where 0 means no background removal and 1 means full background removal. Default is 0.5.");
        internal static readonly GUIContent AspectRatio = new("Aspect Ratio", "The aspect ratio of the generated image (or video). Supported values are \"1:1\", \"3:4\", \"4:3\", \"9:16\", and \"16:9\". The default is \"1:1\".");

        // TTS Options
        internal static readonly GUIContent Voice = new("Voice Actor", "Select the voice to use to generate the content.");
        internal static readonly GUIContent VoiceSpeed = new("Speed", "The speed of the generated audio.");

        // SoundFX Options
        internal static readonly GUIContent SoundFXDuration = new("Duration", "The duration of the sound effect in seconds. If not specified, a default duration will be used.");

        // Recording Options (e.g., for STT)
        internal static readonly GUIContent RecordingsFolder = new("Recordings Folder", "The folder where the recording files are stored.");
        internal static readonly GUIContent RecordingFrequency = new("Recording Frequency", "The frequency of the recording.");
        internal static readonly GUIContent MaxRecordingDuration = new("Max Recording Duration", "The maximum duration of the recording.");
        internal static readonly GUIContent SpokenLanguage = new("Spoken Language",
            "The language spoken in the audio input. This is used for speech-to-text and text-to-speech operations. " +
            "It helps the model understand the language context of the audio input or output, ensuring accurate transcription or synthesis.");
        internal static readonly GUIContent RemoveBackgroundNoise = new("Remove Background Noise",
            "If checked, the background noise will be removed from the audio input during speech-to-text operations. This can improve transcription accuracy by focusing on the spoken content.");

        // Custom Unity Components
        internal static readonly GUIContent FunctionManager = new("Function Manager", "The function manager for handling function calls.");
        internal static readonly GUIContent ChatEventReceiver = new("Chat Event Receiver", "An event receiver for handling chat events.");
        internal static readonly GUIContent ToolCallReceiver = new("Tool Call Receiver", "An event receiver for handling tool calls, including function calls.");
        internal static readonly GUIContent StreamEventReceiver = new("Streami Event Receiver", "An event receiver for handling streaming text events.");
        internal static readonly GUIContent RealtimeEventReceiver = new("Realtime Event Receiver", "An event receiver for handling real-time API events.");
        internal static readonly GUIContent WebSocketEventReceiver = new("WebSocket Event Receiver", "An event receiver for handling WebSocket events.");

        // Custom Unity Component Options
        internal static readonly GUIContent DebugMode = new("Debug Mode",
            "If checked, debug information will be logged to the console. This is useful for troubleshooting and development purposes.");
        internal static readonly GUIContent ErrorReceiver = new("Error Receiver",
            "An event handler for handling errors that occur during API calls or processing. " +
            "This allows you to define custom error handling logic, such as logging errors or displaying messages to the user.");

        internal static readonly GUIContent GameGenre = new("Game Genre",
            "The genre of the game for which the AI content is being generated. This can help tailor the AI's responses to fit the specific context of the game, such as RPG, FPS, or puzzle games.");
        internal static readonly GUIContent GameTheme = new("Game Theme",
            "The theme of the game for which the AI content is being generated. This can include elements like fantasy, sci-fi, horror, or adventure, helping to align the AI's output with the game's overall aesthetic and narrative style.");
        internal static readonly GUIContent ArtStyle = new("Art Style",
            "The art style of the game for which the AI content is being generated. This can include styles like pixel art, realistic, cartoonish, or abstract, allowing the AI to generate content that visually matches the game's design and artistic direction.");

        internal static readonly GUIContent Is2D = new("Is 2D",
            "If checked, the AI will generate code suitable for 2D games. This can affect how the AI interprets prompts and generates codes, ensuring that the output is optimized for 2D game development.");

        internal static readonly GUIContent IsMultiplayer = new("Is Multiplayer",
            "If checked, the AI will generate code suitable for multiplayer games. This can affect how the AI interprets prompts and generates codes, ensuring that the output is optimized for multiplayer game development.");

        internal static readonly GUIContent IsVR = new("Is VR",
            "If checked, the AI will generate code suitable for VR games. This can affect how the AI interprets prompts and generates codes, ensuring that the output is optimized for VR game development.");

        internal static readonly GUIContent SafetySettings = new("Safety Settings",
            "Settings to ensure the generated content adheres to safety and ethical guidelines. This can include filters for harmful content, profanity, or sensitive topics.");


        // Chat Session 
        internal static readonly GUIContent ChatSessionAutoSave = new("Auto Save Chat Session",
            "If checked, the chat session will be automatically saved to a file after each interaction. This is useful for keeping a record of conversations and can help in debugging or reviewing past interactions.");

        internal static readonly GUIContent MaxContextMessages = new("Max Context Messages",
            "The maximum number of messages to keep in the chat context. This helps manage memory usage and performance by limiting the amount of historical data retained for generating responses.");

        internal static readonly GUIContent AutoTitle = new("Auto Title",
            "If checked, the chat session will automatically generate a title based on the conversation context. This can help in organizing and identifying chat sessions without manually setting titles.");

        internal static readonly GUIContent ChatTitle = new("Chat Title",
            "The title of the chat session. This can be set manually or generated automatically based on the conversation context. It helps in organizing and identifying chat sessions within the application.");
    }
}