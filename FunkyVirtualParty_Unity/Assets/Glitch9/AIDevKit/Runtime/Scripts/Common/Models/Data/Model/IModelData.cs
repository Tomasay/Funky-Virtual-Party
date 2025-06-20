namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Super important interface for all model data classes. 
    /// This interface defines the properties that are required and optional for model data.
    /// </summary>
    internal interface IModelData : IData
    {
        // --- Required Properties ---
        Api Api { get; } // the API 
        //string Id { get; }
        string OwnedBy { get; }

        // --- Optional Properties --- 
        ModelFeature? Feature => null;
        string Provider => null; // the provider of the Model
        //string Name => null;
        string Family => null;
        string FamilyVersion => null; // the version of the family, if applicable
        string ModelVersion => null;
        UnixTime? CreatedAt => null; // some APIs (like OpenAI) don't provide this information, so it's nullable
        string Description => null;
        Modality? InputModality => null; // the type of input the model accepts (e.g. text, image, etc.)
        Modality? OutputModality => null; // the type of output the model produces (e.g. text, image, etc.)

        // --- Token Limit Properties ---
        int? InputTokenLimit => null;   // also known as max input tokens or max prompt tokens
        int? OutputTokenLimit => null;  // also known as max output tokens or max completion tokens

        // --- Fine-tuning Related Properties ---
        string BaseId => null;  // in case of fine-tuned models, this is the base model id
        bool? IsFineTuned => null;

        // --- Model Pricing Properties ---
        string CostPerInputToken => null; // the cost per input token (for LLMs)
        string CostPerOutputToken => null; // the cost per output token (for LLMs) 
        string CostPerMinute => null; // the cost per minute (TTS models)
        string CostPerCharacter => null; // the cost per character (STT models)  
        string CostPerImage => null; // the cost per image (for image generation models)
        string CostPerRequest => null; // the cost per request (for image generation models)
        string CostPerInputCacheRead => null; // the cost per input cache read (for image generation models)
        string CostPerInputCacheWrite => null; // the cost per input cache write (for image generation models)
        string CostPerWebSearch => null; // the cost per web search (for image generation models)
        string CostPerInternalReasoning => null; // the cost per internal reasoning (for image generation models)  
    }
}