
namespace Glitch9.AIDevKit.Editor
{
    /// <summary>
    /// 모델 ID로부터 모델 Family를 추론하는 클래스입니다.
    /// This is fuckin dumb shit but this is the only accurate way to do this.
    /// </summary>
    internal class ModelFamilyResolver
    {
        private static readonly string[] _partOfFamilyVersion ={
            "v",
            "turbo",
            "flash",
            "pro",
            "embedding",
            "image",
            "lite",
            "micro",
        };

        private static bool IsVersionPart(string part)
        {
            if (part.StartsWith("00")) return false;
            if (part.StartsWith("0") && part[1] != '.') return false;

            // check if it's number like 1, 1.0, 1.5, etc
            if (float.TryParse(part, out float v)) return v < 20;
            foreach (string versionPart in _partOfFamilyVersion)
            {
                if (part == versionPart) return true;
            }
            return false;
        }

        internal static (string family, string familyVersion) Resolve(Api api, string id, string name)
        {
            string family, version = null;

            if (id.Contains('/'))
            {
                string[] idSplit = id.Split('/');
                id = idSplit[1].Trim();
            }

            id = id.ToLowerInvariant().Replace(" ", "-");

            bool versionDefined = false;

            if (id.Contains("davinci") || id.Contains("curie") || id.Contains("babbage") || id.Contains("gpt"))
            {
                family = ModelFamily.GPT;

                if (id.Contains("davinci") || id.Contains("curie") || id.Contains("babbage"))
                {
                    version = "3";
                    versionDefined = true;
                }
            }
            else if (id.Contains("whisper")) family = ModelFamily.Whisper;
            else if (id.Contains("learnlm")) family = ModelFamily.LearnLM;
            else if (id.Contains("gpt")) family = ModelFamily.GPT;
            else if (id.Contains("gemini")) family = ModelFamily.Gemini;
            else if (ModelMetadataUtil.IsOModel(id)) family = ModelFamily.o;
            else if (id.Contains("dall-e")) family = ModelFamily.DALL_E;
            else if (id.Contains("text-bison")) family = ModelFamily.TextPaLM2;
            else if (id.Contains("chat-bison")) family = ModelFamily.ChatPaLM2;
            else if (id.Contains("imagen")) family = ModelFamily.Imagen;
            else if (id.Contains("mistral")) family = ModelFamily.Mistral;
            else if (id.Contains("dolphin")) family = ModelFamily.Dolphin;
            else if (id.Contains("phi")) family = ModelFamily.Phi;
            else if (id.Contains("deepseek")) family = ModelFamily.DeepSeek;
            else if (id.Contains("vicuna")) family = ModelFamily.Vicuna;
            else if (id.Contains("orca")) family = ModelFamily.Orca;
            else if (id.Contains("starling")) family = ModelFamily.Starling;
            else if (id.Contains("tinyllama")) family = ModelFamily.TinyLlama;
            else if (id.Contains("llama")) family = ModelFamily.Llama;
            else if (id.Contains("aion")) family = ModelFamily.Aion;
            else if (id.Contains("aero")) family = ModelFamily.Airoboros;
            else if (id.Contains("fimbulvetr")) family = ModelFamily.Fimbulvetr;
            else if (id.Contains("deepcoder")) family = ModelFamily.Deepcoder;
            else if (id.Contains("claude")) family = ModelFamily.Claude;
            else if (id.Contains("anubis")) family = ModelFamily.Anubis;
            else if (id.Contains("glm")) family = ModelFamily.GLM;
            else if (id.Contains("eva")) family = ModelFamily.EVA;
            else if (id.Contains("command")) family = ModelFamily.Command;
            else if (id.Contains("llemma")) family = ModelFamily.Llemma;
            else if (id.Contains("qwen")) family = ModelFamily.Qwen;
            else if (id.Contains("jamba")) family = ModelFamily.Jamba;
            else if (id.Contains("wizardlm")) family = ModelFamily.WizardLM;
            else if (id.Contains("midnight")) family = ModelFamily.MidnightRose;
            else if (id.Contains("weaver")) family = ModelFamily.Weaver;
            else if (id.Contains("yi")) family = ModelFamily.Yi;
            else if (id.Contains("zephyr")) family = ModelFamily.Zephyr;
            else if (id.Contains("toppy")) family = ModelFamily.Toppy;
            else if (id.Contains("sonar")) family = ModelFamily.Sonar;
            else if (id.Contains("rogue-rose")) family = ModelFamily.RogueRose;
            else
            {
                family = api switch
                {
                    Api.OpenAI => ResolveOpenAIFamily(id),
                    Api.Google => ResolveGoogleFamily(id),
                    Api.ElevenLabs => ResolveElevenLabsFamily(id),
                    Api.Ollama => ResolveOllamaFamily(id),
                    Api.OpenRouter => ResolveOpenRouterFamily(name),
                    _ => "Unknown",
                };
            }

            if (api == Api.OpenRouter)
            {
                id = name.ToLowerInvariant().Replace(" ", "-");
                if (ModelMetadataUtil.IsOModel(family)) family = ModelFamily.o;
            }

            if (!versionDefined && !id.Contains("text-embed"))
            {
                string[] idSplit = id.Split('-');

                if (idSplit.Length > 1)
                {
                    string secondPart = idSplit[1].Trim();
                    if (IsVersionPart(secondPart))
                    {
                        version = secondPart.CapFirstChar();

                        if (idSplit.Length > 2)
                        {
                            string thirdPart = idSplit[2].Trim();
                            if (IsVersionPart(thirdPart)) version += $" {thirdPart.CapFirstChar()}";
                        }
                    }
                }
            }

            return (family, version);
        }


        private static string ResolveOpenAIFamily(string id)
        {
            id = id.ToLower();

            if (id.Contains("realtime")) return ModelFamily.Realtime;
            if (id.Contains("moderation")) return ModelFamily.Moderation_OpenAI;
            if (id.Contains("embedding")) return ModelFamily.TextEmbedding_OpenAI;
            if (id.Contains("tts")) return ModelFamily.TTS_OpenAI;

            return ModelFamily.Unknown;
        }

        private static string ResolveGoogleFamily(string id)
        {
            id = id.ToLower();

            if (id.Contains("gemma")) return ModelFamily.Gemma;
            if (id.Contains("embedding")) return ModelFamily.TextEmbedding_Google;

            return ModelFamily.Gemini;
        }

        private static string ResolveElevenLabsFamily(string id)
        {
            id = id.ToLower();

            if (id.Contains("scribe")) return ModelFamily.Scribe;
            if (id.Contains("sts")) return ModelFamily.VoiceChanger_ElevenLabs;

            return ModelFamily.TTS_ElevenLabs;
        }

        private static string ResolveOllamaFamily(string id)
        {
            id = id.ToLowerInvariant();

            if (id.Contains("gemma")) return ModelFamily.Gemma_Ollama;

            return ModelFamily.Unknown;
        }

        private static string ResolveOpenRouterFamily(string id)
        {
            string provider = string.Empty;
            string rawName, family;

            if (id.Contains(':'))
            {
                string[] nameSplit = id.Split(':');
                provider = nameSplit[0].Trim();
                rawName = nameSplit[1].Trim();
            }
            else
            {
                rawName = id.Trim();
            }

            if (rawName.Contains(' '))
            {
                string[] nameSplit2 = rawName.Split(' ');
                family = nameSplit2[0].Trim();
            }
            else
            {
                family = rawName;
            }

            if (!string.IsNullOrWhiteSpace(provider)) family += $" ({provider})";

            return family;
        }
    }
}