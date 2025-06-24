using System;
using Glitch9.AIDevKit.GENTasks;

namespace Glitch9.AIDevKit
{
    public class EmptyPromptException : Exception
    {
        public EmptyPromptException(Type promptType) : base($"Your prompt({promptType.Name}) is empty or null.") { }
    }

    public class BlockedPromptException : Exception
    {
        public PromptFeedback Feedback { get; }
        public BlockedPromptException(PromptFeedback feedback) : base(feedback.ToString()) => Feedback = feedback;
    }

    public class EmptyResponseException : Exception
    {
        public EmptyResponseException(Model model) : base($"Model {model.Id} returned null or empty result.") { }
        public EmptyResponseException(string requestType) : base($"{RequestType.GetDisplayName(requestType)} task returned null or empty result.") { }
    }

    public class BrokenVoiceException : Exception
    {
        public BrokenVoiceException() : base("Currently selected voice is broken. This is a critical error. Please report this issue to the developer.") { }
    }

    public class BrokenModelException : Exception
    {
        public BrokenModelException() : base("Currently selected model is broken. This is a critical error. Please report this issue to the developer.") { }
    }

    public class InterruptedResponseException : Exception
    {
        public InterruptedResponseException(StopReason stopReason) : base(stopReason.GetMessage()) { }
    }

    public class BrokenResponseException : Exception
    {
        public BrokenResponseException(string message, Exception exception) : base(message, exception) { }
    }

    public class DeprecatedModelException : Exception
    {
        public DeprecatedModelException(Model model) : base($"Model {model.Id} is deprecated. Please use a different model.") { }
    }

    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(string message = "Request rate limit exceeded.") : base(message) { }
    }

    public class NotSupportedModelFeatureException : NotSupportedException
    {
        public ModelFeature Feature { get; }
        public Model Model { get; }
        public NotSupportedModelFeatureException(Model model, ModelFeature feature) : base($"Model {model.Id} does not support {feature} feature. Please use a different model.")
        {
            Model = model;
            Feature = feature;
        }
    }

    public class ModelNotFoundOnServerException : Exception
    {
        public Api Api { get; }
        public string ModelId { get; }
        public ModelNotFoundOnServerException(Api api, string modelId)
            : base($"Model with ID '{modelId}' not found on {api} API.")
        {
            Api = api;
            ModelId = modelId;
        }
    }
}