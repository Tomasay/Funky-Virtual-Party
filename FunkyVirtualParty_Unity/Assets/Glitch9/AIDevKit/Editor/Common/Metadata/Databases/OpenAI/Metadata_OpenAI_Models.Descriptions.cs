using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_OpenAI_Models
    {
        // https://platform.openai.com/docs/models
        // Key: ID Keywords
        // Value: Description
        internal static readonly Dictionary<string, string> Descriptions = new()
        {
            { "curie,babbage,davinci", "GPT base models can understand and generate natural language or code but are not trained with instruction following. These models are made to be replacements for our original GPT-3 base models and use the legacy Completions API. Most customers should use GPT-3.5 or GPT-4." },

            { "omni-moderation", "Moderation models are free models designed to detect harmful content. This model is our most capable moderation model, accepting images as input as well." },
            { "text-moderation", "Moderation models are free models designed to detect harmful content. This is our text only moderation model; we expect omni-moderation-* models to be the best default moving forward." },

            { "text-embedding-3-small", "text-embedding-3-small is our improved, more performant version of our ada embedding model. Embeddings are a numerical representation of text that can be used to measure the relatedness between two pieces of text. Embeddings are useful for search, clustering, recommendations, anomaly detection, and classification tasks." },
            { "text-embedding-3-large", "text-embedding-3-large is our most capable embedding model for both english and non-english tasks. Embeddings are a numerical representation of text that can be used to measure the relatedness between two pieces of text. Embeddings are useful for search, clustering, recommendations, anomaly detection, and classification tasks." },
            { "text-embedding-ada-002", "text-embedding-ada-002 is our improved, more performant version of our ada embedding model. Embeddings are a numerical representation of text that can be used to measure the relatedness between two pieces of text. Embeddings are useful for search, clustering, recommendations, anomaly detection, and classification tasks." },

            { "gpt-4o-search-preview", "GPT-4o Search Preview is a specialized model trained to understand and execute web search queries with the Chat Completions API. In addition to token fees, web search queries have a fee per tool call. Learn more in the pricing page." },
            { "gpt-4o-mini-search-preview", "GPT-4o mini Search Preview is a specialized model trained to understand and execute web search queries with the Chat Completions API. In addition to token fees, web search queries have a fee per tool call. Learn more in the pricing page." },
            { "computer-use-preview", "The computer-use-preview model is a specialized model for the computer use tool. It is trained to understand and execute computer tasks. See the computer use guide for more information. This model is only usable in the Responses API." },

            { "gpt-4o-transcribe", "GPT-4o Transcribe is a speech-to-text model that uses GPT-4o to transcribe audio. It offers improvements to word error rate and better language recognition and accuracy compared to original Whisper models. Use it for more accurate transcripts." },
            { "gpt-4o-mini-transcribe", "GPT-4o mini Transcribe is a speech-to-text model that uses GPT-4o mini to transcribe audio. It offers improvements to word error rate and better language recognition and accuracy compared to original Whisper models. Use it for more accurate transcripts." },
            { "whisper", "Whisper is a general-purpose speech recognition model, trained on a large dataset of diverse audio. You can also use it as a multitask model to perform multilingual speech recognition as well as speech translation and language identification." },

            { "gpt-4o-mini-tts", "GPT-4o mini TTS is a text-to-speech model built on GPT-4o mini, a fast and powerful language model. Use it to convert text to natural sounding spoken text. The maximum number of input tokens is 2000." },
            { "tts-1", "TTS is a model that converts text to natural sounding spoken text. The tts-1 model is optimized for realtime text-to-speech use cases. Use it with the Speech endpoint in the Audio API." },
            { "tts-1-hd", "TTS is a model that converts text to natural sounding spoken text. The tts-1-hd model is optimized for high quality text-to-speech use cases. Use it with the Speech endpoint in the Audio API." },

            { "gpt-image-1", "GPT Image 1 is our new state-of-the-art image generation model. It is a natively multimodal language model that accepts both text and image inputs, and produces image outputs." },
            { "dall-e-3", "DALL·E is an AI system that creates realistic images and art from a natural language description. DALL·E 3 currently supports the ability, given a prompt, to create a new image with a specific size." },
            { "dall-e-2", "DALL·E is an AI system that creates realistic images and art from a natural language description. Older than DALL·E 3, DALL·E 2 offers more control in prompting and more requests at once." },

            { "gpt-4o-realtime", "This is a preview release of the GPT-4o Realtime model, capable of responding to audio and text inputs in realtime over WebRTC or a WebSocket interface." },
            { "gpt-4o-mini-realtime", "This is a preview release of the GPT-4o-mini Realtime model, capable of responding to audio and text inputs in realtime over WebRTC or a WebSocket interface." },

            { "o4-mini", "o4-mini is our latest small o-series model. It's optimized for fast, effective reasoning with exceptionally efficient performance in coding and visual tasks." },
            { "o3-mini", "o3-mini is our newest small reasoning model, providing high intelligence at the same cost and latency targets of o1-mini. o3-mini supports key developer features, like Structured Outputs, function calling, and Batch API." },
            { "o1-mini", "The o1 reasoning model is designed to solve hard problems across domains. o1-mini is a faster and more affordable reasoning model, but we recommend using the newer o3-mini model that features higher intelligence at the same latency and price as o1-mini." },
            { "gpt-4o-mini-audio", "This is a preview release of the smaller GPT-4o Audio mini model. It's designed to input audio or create audio outputs via the REST API." },
            { "gpt-4o-mini", "GPT-4.1 mini provides a balance between intelligence, speed, and cost that makes it an attractive model for many use cases." },
            { "gpt-4o-nano", "GPT-4.1 nano is the fastest, most cost-effective GPT-4.1 model." },

            { "o1-pro", "The o1 series of models are trained with reinforcement learning to think before they answer and perform complex reasoning. The o1-pro model uses more compute to think harder and provide consistently better answers.\n\no1-pro is available in the Responses API only to enable support for multi-turn model interactions before responding to API requests, and other advanced API features in the future." },
            { "o3", "o3 is a well-rounded and powerful model across domains. It sets a new standard for math, science, coding, and visual reasoning tasks. It also excels at technical writing and instruction-following. Use it to think through multi-step problems that involve analysis across text, code, and images." },
            { "o1", "The o1 series of models are trained with reinforcement learning to perform complex reasoning. o1 models think before they answer, producing a long internal chain of thought before responding to the user." },

            { "gpt-4.1", "GPT-4.1 is our flagship model for complex tasks. It is well suited for problem solving across domains." },
            { "gpt-4o", "GPT-4o (“o” for “omni”) is our versatile, high-intelligence flagship model. It accepts both text and image inputs, and produces text outputs (including Structured Outputs). It is the best model for most tasks, and is our most capable model outside of our o-series models." },
            { "gpt-4o-audio", "This is a preview release of the GPT-4o Audio models. These models accept audio inputs and outputs, and can be used in the Chat Completions REST API." },
            { "chatgpt-4o", "ChatGPT-4o points to the GPT-4o snapshot currently used in ChatGPT. GPT-4o is our versatile, high-intelligence flagship model. It accepts both text and image inputs, and produces text outputs. It is the best model for most tasks, and is our most capable model outside of our o-series models.." },

            { "gpt-4-turbo", "GPT-4 Turbo is the next generation of GPT-4, an older high-intelligence GPT model. It was designed to be a cheaper, better version of GPT-4. Today, we recommend using a newer model like GPT-4o." },
            { "gpt-4", "GPT-4 is an older version of a high-intelligence GPT model, usable in Chat Completions." },
            { "gpt-3.5-turbo", "GPT-3.5 Turbo models can understand and generate natural language or code and have been optimized for chat using the Chat Completions API but work well for non-chat tasks as well. As of July 2024, use gpt-4o-mini in place of GPT-3.5 Turbo, as it is cheaper, more capable, multimodal, and just as fast. GPT-3.5 Turbo is still available for use in the API." },

            // added new on 2025-06-15
            { "codex-mini-latest", "codex-mini-latest is a fine-tuned version of o4-mini specifically for use in Codex CLI. For direct use in the API, we recommend starting with gpt-4.1." },
        };
    }
}