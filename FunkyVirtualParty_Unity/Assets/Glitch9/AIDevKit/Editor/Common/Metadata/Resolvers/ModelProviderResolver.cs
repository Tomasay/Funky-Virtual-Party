using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Glitch9.AIDevKit.Editor
{
    internal static class ModelProviderResolver
    {
        private static readonly Dictionary<string, Regex> ProviderPatterns = new Dictionary<string, Regex>
        {
            { AIProviders.OpenAI, new Regex(@"^(openai/|chatgpt|gpt-|o[1-9]|gpt4|gpt4o)", RegexOptions.IgnoreCase) },
            { AIProviders.Google, new Regex(@"^(google/|gemini|palm|gemma|learnim)", RegexOptions.IgnoreCase) },
            { AIProviders.Anthropic, new Regex(@"^(anthropic/|claude)", RegexOptions.IgnoreCase) },
            { AIProviders.Cohere, new Regex(@"^(cohere/|command(-r)?(\-|$))", RegexOptions.IgnoreCase) },
            { AIProviders.Microsoft, new Regex(@"^(microsoft/|phi[\d\-]|^phi$|wizard|mai-ds)", RegexOptions.IgnoreCase) },
            { AIProviders.MetaLlama, new Regex(@"^(meta-llama/|llama(?!-guard)|llama-3|llama-4|llama-guard)", RegexOptions.IgnoreCase) },
            { AIProviders.MistralAI, new Regex(@"^(mistral(ai)?/|mixtral|pixtral|codestral|ministral|mistral(?!-nemo))", RegexOptions.IgnoreCase) },
            { AIProviders.DeepSeek, new Regex(@"^(deepseek/|deepseek)", RegexOptions.IgnoreCase) },
            { AIProviders.Qwen, new Regex(@"^(qwen/|qwq|qwen\d?)", RegexOptions.IgnoreCase) },
            { AIProviders.NousResearch, new Regex(@"^(nousresearch/|hermes|nous-hermes)", RegexOptions.IgnoreCase) },
            { AIProviders.Perplexity, new Regex(@"^(perplexity/|sonar|r1-1776)", RegexOptions.IgnoreCase) },
            { AIProviders.Nvidia, new Regex(@"^(nvidia/|nemotron)", RegexOptions.IgnoreCase) },
            { AIProviders.X_AI, new Regex(@"^(x-ai/|grok)", RegexOptions.IgnoreCase) },
            { AIProviders.HuggingFaceH4, new Regex(@"^(huggingfaceh4/|zephyr)", RegexOptions.IgnoreCase) },
            { AIProviders.EleutherAI, new Regex(@"^(eleutherai/|llemma)", RegexOptions.IgnoreCase) },
            { AIProviders.EVA_UNIT_01, new Regex(@"^(eva-unit-01/|eva-llama|eva-qwen)", RegexOptions.IgnoreCase) },
            { AIProviders.Featherless, new Regex(@"^(featherless/|qwerky)", RegexOptions.IgnoreCase) },
            { AIProviders.Inflection, new Regex(@"^(inflection/)", RegexOptions.IgnoreCase) },
            { AIProviders.MoonshotAI, new Regex(@"^(moonshotai/|moonlight|kimi-vl)", RegexOptions.IgnoreCase) },
            { AIProviders.NeverSleep, new Regex(@"^(neversleep/|lumimaid|noromaid)", RegexOptions.IgnoreCase) },
            { AIProviders.TheDrummer, new Regex(@"^(thedrummer/|anubis|rocinante|skyfall|unslopnemo)", RegexOptions.IgnoreCase) },
            { AIProviders.Thudm, new Regex(@"^(thudm/|glm|g[l|I]m)", RegexOptions.IgnoreCase) },
            { AIProviders.CognitiveComputations, new Regex(@"^(cognitivecomputations/|dolphin)", RegexOptions.IgnoreCase) },
            { AIProviders.ShisaAI, new Regex(@"^(shisa-ai/)", RegexOptions.IgnoreCase) },
            { AIProviders.Sao10k, new Regex(@"^(sao10k/|euryale|lunaris|fimbulvetr)", RegexOptions.IgnoreCase) },
            { AIProviders.Tngtech, new Regex(@"^(tngtech/)", RegexOptions.IgnoreCase) },
            { AIProviders.Undi95, new Regex(@"^(undi95/|remm|toppy)", RegexOptions.IgnoreCase) },
            { AIProviders.Scb10x, new Regex(@"^(scb10x/|typhoon2)", RegexOptions.IgnoreCase) },
            { AIProviders.Sophosympatheia, new Regex(@"^(sophosympatheia/|midnight-rose)", RegexOptions.IgnoreCase) },
            { AIProviders.Steelskull, new Regex(@"^(steelskull/|electra)", RegexOptions.IgnoreCase) },
            { AIProviders.AgenticaOrg, new Regex(@"^(agentica-org/)", RegexOptions.IgnoreCase) },
            { AIProviders.Aetherwiing, new Regex(@"^(aetherwiing/)", RegexOptions.IgnoreCase) },
            { AIProviders.AllenAI, new Regex(@"^(allenai/|olmo|molmo)", RegexOptions.IgnoreCase) },
            { AIProviders.Amazon, new Regex(@"^(amazon/|nova)", RegexOptions.IgnoreCase) },
            { AIProviders.Alpindale, new Regex(@"^(alpindale/|goliath|magnum)", RegexOptions.IgnoreCase) },
            { AIProviders.AI21, new Regex(@"^(ai21/|jamba)", RegexOptions.IgnoreCase) },
            { AIProviders.AionLabs, new Regex(@"^(aion-labs/|aion)", RegexOptions.IgnoreCase) },
            { AIProviders.AlfredPros, new Regex(@"^(alfredpros/|codellama)", RegexOptions.IgnoreCase) },
            { AIProviders.AllHands, new Regex(@"^(all-hands/|openhands)", RegexOptions.IgnoreCase) },
            { AIProviders.Gryphe, new Regex(@"^(gryphe/|mythomax)", RegexOptions.IgnoreCase) },
            { AIProviders.Inception, new Regex(@"^(inception/|mercury)", RegexOptions.IgnoreCase) },
            { AIProviders.Infermatic, new Regex(@"^(infermatic/)", RegexOptions.IgnoreCase) },
            { AIProviders.Jondurbin, new Regex(@"^(jondurbin/|airoboros)", RegexOptions.IgnoreCase) },
            { AIProviders.LatitudeGames, new Regex(@"^(latitudegames/|wayfarer)", RegexOptions.IgnoreCase) },
            { AIProviders.Liquid, new Regex(@"^(liquid/|lfm)", RegexOptions.IgnoreCase) },
            { AIProviders.Mancer, new Regex(@"^(mancer/|weaver)", RegexOptions.IgnoreCase) },
            { AIProviders.Minimax, new Regex(@"^(minimax/|minimax-01)", RegexOptions.IgnoreCase) },
            { AIProviders.OpenR1, new Regex(@"^(open-r1/|olympiccoder)", RegexOptions.IgnoreCase) },
            { AIProviders.OpenGVLab, new Regex(@"^(opengvlab/|internv|internvl)", RegexOptions.IgnoreCase) },
            { AIProviders.PygmalionAI, new Regex(@"^(pygmalionai/|mythalion)", RegexOptions.IgnoreCase) },
            { AIProviders.Raifle, new Regex(@"^(raifle/|sorcererlm)", RegexOptions.IgnoreCase) },
            { AIProviders.RekaAI, new Regex(@"^(rekaai/|reka)", RegexOptions.IgnoreCase) },
            { AIProviders.ArliAI, new Regex(@"^(arliai/|qwq)", RegexOptions.IgnoreCase) },
            { AIProviders._01_AI, new Regex(@"^(01-ai/|yi-large)", RegexOptions.IgnoreCase) }
        };


        public static string Resolve(string modelId)
        {
            if (string.IsNullOrWhiteSpace(modelId))
                return null;

            modelId = modelId.Trim();

            int slashIndex = modelId.IndexOf('/');
            if (slashIndex > 0)
            {
                var prefix = modelId.Substring(0, slashIndex);
                foreach (var (provider, _) in ProviderPatterns)
                {
                    if (string.Equals(provider, prefix, StringComparison.OrdinalIgnoreCase))
                        return provider;
                }
            }

            foreach (var (provider, regex) in ProviderPatterns)
            {
                if (regex.IsMatch(modelId))
                    return provider;
            }

            return null;
        }

    }
}