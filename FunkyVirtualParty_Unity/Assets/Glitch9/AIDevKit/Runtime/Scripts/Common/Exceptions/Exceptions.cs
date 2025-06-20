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
        public EmptyResponseException(int endpointType) : base($"{EndpointType.GetName(endpointType)} task returned null or empty result.") { }
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

    public class NotSupportedFeatureException : NotSupportedException
    {
        public ModelFeature Capability { get; }
        public Model Model { get; }
        public NotSupportedFeatureException(Model model, ModelFeature cap) : base($"Model {model.Id} does not support {cap} feature. Please use a different model.")
        {
            Model = model;
            Capability = cap;
        }
    }

    public class ModelNotFoundOnApiException : Exception
    {
        public Api Api { get; }
        public string ModelId { get; }
        public ModelNotFoundOnApiException(Api api, string modelId)
            : base($"Model with ID '{modelId}' not found on {api} API.")
        {
            Api = api;
            ModelId = modelId;
        }
    }
}