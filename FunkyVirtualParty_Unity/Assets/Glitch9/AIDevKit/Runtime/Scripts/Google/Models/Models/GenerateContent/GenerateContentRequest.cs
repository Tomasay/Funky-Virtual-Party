using Glitch9.IO.Files;
using Glitch9.IO.Json.Schema;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Generates a response from the model given an input <see cref="GenerateContentRequest"/>.
    /// Input capabilities differ between models, including tuned models.See the model guide and tuning guide for details.
    /// </summary>
    /// <returns><see cref="GenerateContentResponse"/></returns>
    public class GenerateContentRequest : GenerativeAIRequest
    {
        /// <summary>
        /// Required.
        /// The content of the current conversation with the model.
        /// For single-turn queries, this is a single instance.For multi-turn queries,
        /// this is a repeated field that contains conversation history + latest request.
        /// </summary>
        [JsonProperty("contents")] public List<Content> Contents { get; set; } = new();

        /// <summary>
        /// Optional. (Beta)
        /// A list of <see cref="Tool"/> the model may use to generate the next response.
        /// </summary>
        [JsonProperty("tools")] public List<Tool> Tools { get; set; }

        /// <summary>
        /// Optional.
        /// Tool configuration for any Tool (<see cref="GenerateContentRequest.Tools"/>) specified in the request.
        /// </summary>
        [JsonProperty("toolConfig")] public ToolConfig ToolConfig { get; set; }

        /// <summary>
        /// Optional.
        /// <para>
        /// A list of unique <see cref="SafetySetting"/> instances for blocking unsafe content.
        /// </para>
        /// This will be enforced on the <see cref="GenerateContentRequest.Contents"/> and  <see cref="GenerateContentResponse.Candidates"/>.
        /// There should not be more than one setting for each SafetyCategory type.
        /// The API will block any contents and responses that fail to meet the thresholds set by these settings.
        /// This list overrides the default settings for each SafetyCategory specified in the safetySettings.
        /// If there is no SafetySetting for a given SafetyCategory provided in the list, the API will use the default safety setting for that category.
        /// Harm categories <see cref="HarmCategory.Hate"/>, <see cref="HarmCategory.SexuallyExplicit"/>, <see cref="HarmCategory.DangerousContent"/>, <see cref="HarmCategory.Harassment"/> are supported.
        /// </summary>
        [JsonProperty("safetySettings")] public List<SafetySetting> SafetySettings { get; set; }

        /// <summary>
        /// Optional. (Beta)
        /// Developer set system instruction. Currently, text only.
        /// </summary>
        [JsonProperty("systemInstruction")] public Content SystemInstruction { get; set; }

        /// <summary>
        /// Optional.
        /// Configuration options for model generation and outputs.
        /// </summary>
        [JsonProperty("generationConfig")] public GenerationConfig Config { get; set; } = new();

        /// <summary>
        /// Optional. 
        /// The name of the cached content used as context to serve the prediction. 
        /// <para>Note: only used in explicit caching, where users can have control over caching (e.g. what content to cache) and enjoy guaranteed cost savings.</para>       
        /// <para>Format: cachedContents/{cachedContent}</para>
        /// </summary>
        [JsonProperty("cachedContent")] public string CachedContent { get; set; }


        public override void ValidateBody()
        {
            ThrowIf.ArgumentIsNull(Config);
            ThrowIf.ListIsNullOrEmpty(Contents);

            Tools = Tools.SetNullIfEmpty();
            SafetySettings = SafetySettings.SetNullIfEmpty();
            if (Config.CandidateCount < 1) Config.CandidateCount = 1;
        }


        public class Builder : GenerativeAIRequestBuilder<Builder, GenerateContentRequest>
        {
            /// <summary>
            /// Adds a system instruction to the beginning of the message list.
            /// </summary>
            /// <param name="systemInstruction"></param>
            /// <returns></returns>
            public Builder SetInstruction(string systemInstruction)
            {
                if (string.IsNullOrEmpty(systemInstruction)) return this;
                _req.SystemInstruction = new Content(ChatRole.System, systemInstruction);
                return this;
            }

            /// <summary>
            /// Adds a system instruction and an assistant message to the beginning of the message list.
            /// </summary>
            /// <param name="startingMessage"></param>
            /// <returns></returns>
            public Builder SetStartingMessage(string startingMessage)
            {
                if (string.IsNullOrEmpty(startingMessage)) return this;

                if (_req.Contents.Count > 0 && _req.Contents[0].Role == ChatRole.System)
                {
                    // if so, add the starting message after the instruction
                    _req.Contents.Insert(1, new Content(ChatRole.Assistant, startingMessage));
                }
                else
                {
                    // otherwise, add it at the beginning of the message list
                    _req.Contents.Insert(0, new Content(ChatRole.Assistant, startingMessage));
                }

                return this;
            }

            public Builder SetMessages(List<ChatMessage> messages)
            {
                if (messages == null || messages.Count == 0) return this;

                _req.Contents.Clear();
                foreach (ChatMessage message in messages)
                {
                    PushMessage(message);
                }

                return this;
            }

            public Builder PushMessage(ChatMessage message)
            {
                if (message == null) return this;

                if (message.Content.IsString)
                {
                    _req.Contents.Add(new Content(message.Role, message.Content));
                }
                else
                {
                    List<ContentPart> parts = new();

                    foreach (AIDevKit.ContentPart part in message.Content.ToPartArray())
                    {
                        if (part is TextContentPart textPart)
                        {
                            parts.Add(ContentPart.FromText(textPart.ToString()));
                        }
                        else if (part is ImageUrlContentPart imageUrlPart)
                        {
                            parts.Add(ContentPart.FromUrl(imageUrlPart.Image?.Url));
                        }
                        else if (part is AudioBase64ContentPart audioBase64Part)
                        {
                            parts.Add(ContentPart.FromBase64(audioBase64Part.InputAudio.Data, audioBase64Part.InputAudio.MimeType));
                        }
                        else if (part is ImageBase64ContentPart imageBase64Part)
                        {
                            parts.Add(ContentPart.FromBase64(imageBase64Part.Image?.FileData, MIMEType.PNG));
                        }
                        else if (part is FileBase64ContentPart fileBase64Part)
                        {
                            parts.Add(ContentPart.FromBase64(fileBase64Part.File.FileData, fileBase64Part.File.MimeType));
                        }
                    }
                }

                return this;
            }


            public Builder SetFunctions(params FunctionDeclaration[] functions)
            {
                if (functions.IsNullOrEmpty()) return this;

                _req.Tools = new List<Tool>();
                List<FunctionDeclaration> declarations = new();

                foreach (FunctionDeclaration function in functions)
                {
                    var declaration = new FunctionDeclaration
                    {
                        Name = function.Name,
                        Description = function.Description,
                        Parameters = function.Parameters
                    };

                    declarations.Add(declaration);
                }

                _req.Tools.Add(new Tool(declarations));

                _req.ToolConfig = new ToolConfig
                {
                    FunctionCallingConfig = new FunctionCallingConfig
                    {
                        Mode = Mode.Any,
                        AllowedFunctionNames = declarations.ConvertAll(f => f.Name)
                    }
                };

                return this;
            }

            public Builder SetPrompt(string prompt)
            {
                _req.Contents.Add(new Content(ChatRole.User, prompt));
                return this;
            }

            public Builder SetSafetySettings(IEnumerable<SafetySetting> safetySettings)
            {
                if (safetySettings.IsNullOrEmpty()) return this;
                _req.SafetySettings = new List<SafetySetting>(safetySettings);
                return this;
            }

            public Builder SetConfig(GenerationConfig generationConfig)
            {
                _req.Config = generationConfig;
                return this;
            }

            public Builder SetTemperature(float temperature)
            {
                _req.Config.Temperature = temperature;
                return this;
            }

            public Builder SetTopP(float topP)
            {
                _req.Config.TopP = topP;
                return this;
            }

            public Builder SetTopK(int topK)
            {
                _req.Config.TopK = topK;
                return this;
            }

            public Builder SetFrequencyPenalty(float frequencyPenalty)
            {
                _req.Config.FrequencyPenalty = frequencyPenalty;
                return this;
            }

            public Builder SetPresencePenalty(float presencePenalty)
            {
                _req.Config.PresencePenalty = presencePenalty;
                return this;
            }

            public Builder SetMaxTokens(int maxTokens)
            {
                _req.Config.MaxTokens = maxTokens;
                return this;
            }

            public Builder SetSeed(int seed)
            {
                _req.Config.Seed = seed;
                return this;
            }

            public Builder SetResponseModalities(params Modality[] responseModalities)
            {
                _req.Config.ResponseModalities = new List<Modality>(responseModalities);
                return this;
            }

            public Builder SetResponseCount(int count)
            {
                _req.Config.CandidateCount = count;
                return this;
            }

            public Builder SetTools(params Tool[] tools)
            {
                _req.Tools = new List<Tool>(tools);
                return this;
            }

            public Builder SetSystemInstruction(Content systemInstruction)
            {
                _req.SystemInstruction = systemInstruction;
                return this;
            }

            public Builder SetPrompt(InpaintPrompt prompt)
            {
                /* \"contents\": [{
                    \"parts\":[
                        {\"text\": \"'Hi, This is a picture of me. Can you add a llama next to me\"},
                        {
                        \"inline_data\": {
                            \"mime_type\":\"image/jpeg\",
                            \"data\": \"$IMG_BASE64\"
                        }
                        }
                    ]
                }],*/
                if (prompt == null || !prompt.IsValid) return this;

                string base64 = Convert.ToBase64String(prompt.image.EncodeToPNG());

                Blob inlineData = new()
                {
                    MimeType = IO.Files.MIMEType.PNG,
                    Data = base64
                };

                _req.Contents.Add(new Content(ChatRole.User, prompt.text, inlineData));
                return this;
            }

            public Builder SetJsonSchema(Type type)
            {
                if (type == null) return this;

                JsonSchemaPropertyAttribute attribute = AttributeCache<JsonSchemaPropertyAttribute>.Get(type);
                if (attribute == null) throw new ArgumentNullException(nameof(attribute), "No JsonSchemaAttribute found for type '" + type + "'.");

                JsonSchema jsonSchema = JsonSchema.Create(type);
                if (jsonSchema == null) return this;

                _req.Config.ResponseMimeType = "application/json";
                _req.Config.ResponseSchema = jsonSchema;

                return this;
            }

            public Builder SetModelOptions(ModelSettings options)
            {
                if (options == null) return this;

                _req.Config ??= new GenerationConfig();

                if (options.MaxTokens != null) _req.Config.MaxTokens = options.MaxTokens;
                if (options.Temperature != null) _req.Config.Temperature = options.Temperature;
                if (options.TopP != null) _req.Config.TopP = options.TopP;
                if (options.TopK != null) _req.Config.TopK = options.TopK;
                if (options.FrequencyPenalty != null) _req.Config.FrequencyPenalty = options.FrequencyPenalty;
                if (options.PresencePenalty != null) _req.Config.PresencePenalty = options.PresencePenalty;
                if (options.Seed != null) _req.Config.Seed = options.Seed;
                if (options.Logprobs != null) _req.Config.Logprobs = options.Logprobs;

                return this;
            }

            public Builder SetVoice(string voiceId)
            {
                if (string.IsNullOrEmpty(voiceId)) return this;
                _req.Config ??= new GenerationConfig();
                _req.Config.SpeechConfig ??= new SpeechConfig();
                _req.Config.SpeechConfig.VoiceConfig ??= new VoiceConfig();
                _req.Config.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig ??= new PrebuiltVoiceConfig();
                _req.Config.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName = voiceId;
                _req.Config.ResponseModalities ??= new List<Modality>();
                if (!_req.Config.ResponseModalities.Contains(Modality.Audio))
                {
                    _req.Config.ResponseModalities.Add(Modality.Audio);
                }
                return this;
            }
        }
    }

    /// <summary>
    /// The Tool configuration containing parameters for specifying <see cref="Tool"/> use in the request.
    /// </summary>
    public class ToolConfig
    {
        /// <summary>
        /// Optional.
        /// Function calling config.
        /// </summary>
        [JsonProperty("functionCallingConfig")] public FunctionCallingConfig FunctionCallingConfig { get; set; }
    }

    /// <summary>
    /// Configuration for specifying function calling behavior.
    /// </summary>
    public class FunctionCallingConfig
    {
        /// <summary>
        /// Required.
        /// Optional. Specifies the mode in which function calling should execute.
        /// If unspecified, the default value will be set to AUTO.
        /// </summary>
        [JsonProperty("mode")] public Mode Mode { get; set; }

        /// <summary>
        /// Optional.
        /// <para>
        /// A set of function names that, when provided, limits the functions the model will call.
        /// </para>
        /// <para>
        /// This should only be set when the Mode is ANY.
        /// Function names should match <see cref="FunctionDeclaration.Name"/>.
        /// With mode set to ANY, model will predict a function call from the set of function names provided.
        /// </para>
        /// </summary>
        [JsonProperty("allowedFunctionNames")] public List<string> AllowedFunctionNames { get; set; }
    }

    /// <summary>
    /// Defines the execution behavior for function calling by defining the execution mode.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// Unspecified function calling mode. This value should not be used.
        /// </summary>
        [ApiEnum("MODE_UNSPECIFIED")] Unspecified,

        /// <summary>
        /// Default model behavior, model decides to predict either a function call or a natural language response.
        /// </summary>
        [ApiEnum("AUTO")] Auto,

        /// <summary>
        /// Model is constrained to always predicting a function call only.
        /// If "allowedFunctionNames" are set,
        /// the predicted function call will be limited to any one of "allowedFunctionNames",
        /// else the predicted function call will be any one of the provided "functionDeclarations".
        /// </summary>
        [ApiEnum("ANY")] Any,

        /// <summary>
        /// Model will not predict any function call.
        /// </summary>
        [ApiEnum("NONE")] None
    }
}