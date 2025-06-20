using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_Shared_Models
    {

        internal static readonly Dictionary<string, (int performance, int speed)> Performances = new()
        {
            { "nousresearch/deephermes-3-mistral-24b-preview", (4, 2) }, // reasoning 강하며 전 세대보다 빠름 :contentReference[oaicite:1]{index=1}
            { "mistralai/mistral-medium-3", (4, 2) },                    // 높은 성능, 다소 느림 :contentReference[oaicite:2]{index=2}
            { "arcee-ai/caller-large", (3, 2) },                        // function‑calling 특화, 구조적 산출에 최적화 :contentReference[oaicite:3]{index=3}
            { "arcee-ai/spotlight", (3, 4) },                            // 시각 언어 7B, 경량·고속 :contentReference[oaicite:4]{index=4}
            { "arcee-ai/maestro-reasoning", (5, 2) },                   // 32B reasoning 최강, MATH/GSM-8K 2×향상 :contentReference[oaicite:5]{index=5}
            { "arcee-ai/virtuoso-large", (5, 1) },                      // 72B 범용, 크로스도메인 최고 :contentReference[oaicite:6]{index=6}
            { "arcee-ai/coder-large", (4, 2) },                         // 코드 성능 5–8% 우위 :contentReference[oaicite:7]{index=7}
            { "arcee-ai/virtuoso-medium-v2", (4, 3) },                   // 중급 범용 모델, 균형 잡힌 성능
            { "arcee-ai/arcee-blitz", (4, 4) },                         // 24B Mistral 기반, 빠르고 효율적 :contentReference[oaicite:8]{index=8}
            { "microsoft/phi-4-reasoning-plus", (5, 1) },                // phi-4 reasoning 최상, 속도 희생
            { "microsoft/phi-4-reasoning", (5, 1) },                     // reasoning 고성능, 속도는 느림
            { "qwen/qwen3-0.6b-04-28:free", (2, 5) },                    // 0.6B 매우 경량, 속도 빠름 :contentReference[oaicite:9]{index=9}
            { "inception/mercury-coder-small-beta", (3, 3) },            // 소형 코딩 모델, 중급 성능으로 추정
            { "qwen/qwen3-1.7b:free", (3, 4) },                          // 1.7B 경량, MMLU 준수, 빠름 :contentReference[oaicite:10]{index=10}
            { "qwen/qwen3-4b:free", (3, 3) },                            // 4B 모델, 좋은 퍼포먼스/속도 균형 :contentReference[oaicite:11]{index=11}
            { "opengvlab/internvl3-14b:free", (3, 3) },                  // 14B 모델, 평균 수준 추정
            { "opengvlab/internvl3-2b:free", (2, 4) },                   // 2B 경량, 준수 속도
            { "deepseek/deepseek-prover-v2:free", (4, 2) },              // reasoning 특화, 약간 느림
            { "deepseek/deepseek-prover-v2", (4, 2) },                   // 위와 동일
            { "meta-llama/llama-guard-4-12b", (3, 3) },                   // 보안/준수용, 평균 수준
            { "qwen/qwen3-30b-a3b:free", (4, 2) },                        // 30B MoE, 성능/효율 우수 :contentReference[oaicite:12]{index=12}
            { "qwen/qwen3-30b-a3b", (4, 2) },                             // 동일
            { "qwen/qwen3-8b:free", (3, 3) },                             // 8B 모델, 균형 잡힌 성능 :contentReference[oaicite:13]{index=13}
            { "qwen/qwen3-8b", (3, 3) },                                  // 동일
            { "qwen/qwen3-14b:free", (3, 3) },                            // 14B, 중급 추정
            { "qwen/qwen3-14b", (3, 3) },                                 // 동일
            { "qwen/qwen3-32b:free", (4, 2) },                            // 32B dense 고성능 :contentReference[oaicite:14]{index=14}
            { "qwen/qwen3-32b", (4, 2) },                                 // 동일
            { "qwen/qwen3-235b-a22b:free", (5, 1) },                      // 235B MoE, 최고 성능, 속도 낮음 :contentReference[oaicite:15]{index=15}
            { "qwen/qwen3-235b-a22b", (5, 1) },                           // 동일 
            { "tngtech/deepseek-r1t-chimera:free", (5, 3) },                // R1 reasoning 능력 유지 + V3 효율성 → 최고 성능, 속도는 중급 :contentReference[oaicite:1]{index=1}
            { "thudm/glm-z1-rumination-32b", (5, 1) },                      // GLM‑Z1‑Rumination‑32B: deep‑thinking 특화, 속도 희생 :contentReference[oaicite:2]{index=2}
            { "thudm/glm-z1-rumination-9b:free", (4, 2) },                  // 9B 크기지만 rumination 채택 → 고성능, 속도 양호 :contentReference[oaicite:3]{index=3}
            { "thudm/glm-4-9b:free", (4, 2) },                              // GLM‑4‑9B: GPT‑4 수준 성능(open-source) :contentReference[oaicite:4]{index=4}
            { "microsoft/mai-ds-r1:free", (5, 1) },                         // MAI‑DS‑R1: DeepSeek R1 기반 최고 reasoning, 속도 느림
            { "thudm/glm-z1-32b:free", (5, 1) },                            // GLM‑Z1‑32B: deep reasoning 강화, 속도 희생 :contentReference[oaicite:5]{index=5}
            { "thudm/glm-z1-32b", (5, 1) },                                 // 동일
            { "thudm/glm-4-32b:free", (5, 1) },                             // GLM‑4‑32B 역시 GPT‑4‑급 deep thinking :contentReference[oaicite:6]{index=6}
            { "thudm/glm-4-32b", (5, 1) },                                  // 동일
            { "shisa-ai/shisa-v2-llama3.3-70b:free", (4, 2) },               // Llama3.3‑70B 기반, 범용성 좋지만 속도 중간
            { "qwen/qwen2.5-coder-7b-instruct", (4, 3) },                   // 인스트럭션 + 코드 강화, 성능/속도 모두 준수
            { "eleutherai/llemma_7b", (3, 3) },                             // 일반 7B LLaMA‑like, 평균
            { "alfredpros/codellama-7b-instruct-solidity", (4, 2) },        // 코드(instruct+solidity) 특화 → 코드 성능 우수, 속도 중간
            { "arliai/qwq-32b-arliai-rpr-v1:free", (4, 2) },                // RPR fine-tuned 32B → 성능 좋음, 속도 중간
            { "agentica-org/deepcoder-14b-preview:free", (4, 2) },          // 코딩 특화, 중간 수준 reasoning & 속도
            { "moonshotai/kimi-vl-a3b-thinking:free", (3, 3) },             // multimodal‑VL a3b, think 모드지만 중급
            { "x-ai/grok-3-mini-beta", (4, 4) },                            // Grok‑3 mini beta: 경량 reasoning, 속도 빠름 :contentReference[oaicite:7]{index=7}
            { "x-ai/grok-3-beta", (5, 2) },                                 // Grok‑3 beta: 최상급 reasoning, 속도는 중급 :contentReference[oaicite:8]{index=8}
            { "nvidia/llama-3.3-nemotron-super-49b-v1:free", (4, 1) },       // 대형 LLaMA3.3 49B: 성능 우수, 속도 느림
            { "nvidia/llama-3.3-nemotron-super-49b-v1", (4, 1) },            // 동일
            { "nvidia/llama-3.1-nemotron-ultra-253b-v1:free", (5, 1) },      // 253B 초대형: 최고 성능, 속도 많이 느림
            { "meta-llama/llama-4-maverick:free", (5, 2) },                 // Llama‑4 Maverick: GPT‑4‑급 reasoning, 속도 중간 :contentReference[oaicite:9]{index=9}
            { "meta-llama/llama-4-maverick", (5, 2) },                      // 동일
            { "meta-llama/llama-4-scout:free", (4, 3) },                    // 범용 4-scout: 균형형 성능·속도
            { "meta-llama/llama-4-scout", (4, 3) },                         // 동일
            { "all-hands/openhands-lm-32b-v0.1", (3, 3) },                   // 32B 오픈핸즈, 평균
            { "mistral/ministral-8b", (3, 4) },                             // 경량 Mistral 8B, 우수한 속도
            { "deepseek/deepseek-v3-base:free", (3, 3) },                   // v3 기본, 평균
            { "scb10x/llama3.1-typhoon2-8b-instruct", (3, 3) },              // instruction 8B, 평균
            { "scb10x/llama3.1-typhoon2-70b-instruct", (4, 2) },             // instruction 70B, 고성능/중속도
            { "allenai/molmo-7b-d:free", (3, 3) },                           // 일반 7B, 평균
            { "bytedance-research/ui-tars-72b:free", (4, 2) },               // Ui‑Tars 72B, 범용성 좋음, 속도 중간
            { "qwen/qwen2.5-vl-3b-instruct:free", (3, 4) },                  // 3B VL instruct path, 경량 + 좋은 속도
            { "qwen/qwen2.5-vl-32b-instruct:free", (4, 2) },
            { "qwen/qwen2.5-vl-32b-instruct", (4, 2) },
            { "deepseek/deepseek-chat-v3-0324:free", (4, 3) },
            { "deepseek/deepseek-chat-v3-0324", (4, 3) },
            { "featherless/qwerky-72b:free", (4, 2) },

            { "mistralai/mistral-small-3.1-24b-instruct:free", (4, 3) },
            { "mistralai/mistral-small-3.1-24b-instruct", (4, 3) },
            { "open-r1/olympiccoder-32b:free", (4, 2) },

            { "ai21/jamba-1.6-large", (4, 2) },
            { "ai21/jamba-1.6-mini", (3, 4) },

            { "cohere/command-a", (3, 3) },

            { "rekaai/reka-flash-3:free", (5, 3) },
            { "thedrummer/anubis-pro-105b-v1", (5, 1) },
            { "thedrummer/skyfall-36b-v2", (4, 2) },
            { "microsoft/phi-4-multimodal-instruct", (5, 2) },

            { "perplexity/sonar-reasoning-pro", (5, 2) },
            { "perplexity/sonar-pro", (4, 3) },
            { "perplexity/sonar-deep-research", (5, 1) },

            { "deepseek/deepseek-r1-zero:free", (4, 3) },
            { "qwen/qwq-32b:free", (4, 2) },
            { "qwen/qwq-32b", (4, 2) },
            { "moonshotai/moonlight-16b-a3b-instruct:free", (4, 2) },
            { "nousresearch/deephermes-3-llama-3-8b-preview:free", (4, 2) },

            { "anthropic/claude-3.7-sonnet", (5, 3) },
            { "anthropic/claude-3.7-sonnet:thinking", (5, 2) },
            { "anthropic/claude-3.7-sonnet:beta", (5, 2) },
            { "perplexity/r1-1776", (5, 2) },

            { "mistralai/mistral-saba", (4, 2) },
            { "cognitivecomputations/dolphin3.0-r1-mistral-24b:free", (4, 3) },
            { "cognitivecomputations/dolphin3.0-mistral-24b:free", (4, 3) },

            { "meta-llama/llama-guard-3-8b", (3, 3) },
            { "openai/o3-mini-high", (4, 3) }, // 참고용, 이미 OpenAI 점수 있음
            { "deepseek/deepseek-r1-distill-llama-8b", (4, 3) },

            { "qwen/qwen-vl-plus", (4, 2) },
            { "aion-labs/aion-1.0", (3, 3) },
            { "aion-labs/aion-1.0-mini", (2, 4) },
            { "aion-labs/aion-rp-llama-3.1-8b", (3, 3) },

            { "qwen/qwen-vl-max", (5, 2) },
            { "qwen/qwen-turbo", (4, 3) },
            { "qwen/qwen2.5-vl-72b-instruct:free", (5, 1) },
            { "qwen/qwen2.5-vl-72b-instruct", (5, 1) },
            { "qwen/qwen-plus", (4, 3) },
            { "qwen/qwen-max", (5, 2) },
            { "deepseek/deepseek-r1-distill-qwen-1.5b", (3, 4) },
            { "mistralai/mistral-small-24b-instruct-2501:free", (4, 3) },
            { "mistralai/mistral-small-24b-instruct-2501", (4, 3) },
            { "deepseek/deepseek-r1-distill-qwen-32b:free", (4, 2) },
            { "deepseek/deepseek-r1-distill-qwen-32b", (4, 2) },
            { "deepseek/deepseek-r1-distill-qwen-14b:free", (4, 3) },
            { "deepseek/deepseek-r1-distill-qwen-14b", (4, 3) },

            { "perplexity/sonar-reasoning", (5, 2) },
            { "perplexity/sonar", (4, 3) },

            { "liquid/lfm-7b", (3, 3) },
            { "liquid/lfm-3b", (2, 4) },

            { "deepseek/deepseek-r1-distill-llama-70b:free", (5, 2) },
            { "deepseek/deepseek-r1-distill-llama-70b", (5, 2) },
            { "deepseek/deepseek-r1:free", (5, 2) },
            { "deepseek/deepseek-r1", (5, 2) },

            { "minimax/minimax-01", (3, 3) },

            { "mistralai/codestral-2501", (5, 3) },
            { "microsoft/phi-4", (5, 2) },
            { "deepseek/deepseek-chat:free", (4, 3) },
            { "deepseek/deepseek-chat", (4, 3) },

            { "sao10k/l3.3-euryale-70b", (4, 2) },
            { "eva-unit-01/eva-llama-3.33-70b", (4, 2) },
            { "x-ai/grok-2-vision-1212", (4, 3) },
            { "x-ai/grok-2-1212", (4, 3) },

            { "cohere/command-r7b-12-2024", (4, 3) },

            { "meta-llama/llama-3.3-70b-instruct:free", (5, 2) },
            { "meta-llama/llama-3.3-70b-instruct", (5, 2) },

            { "amazon/nova-lite-v1", (3, 3) },
            { "amazon/nova-micro-v1", (2, 4) },
            { "amazon/nova-pro-v1", (4, 2) },

            { "qwen/qwq-32b-preview:free", (4, 2) },
            { "qwen/qwq-32b-preview", (4, 2) },

            { "eva-unit-01/eva-qwen-2.5-72b", (5, 1) },

            { "mistralai/mistral-large-2411", (5, 2) },
            { "mistralai/mistral-large-2407", (5, 2) },
            { "mistralai/pixtral-large-2411", (4, 2) },

            { "x-ai/grok-vision-beta", (4, 3) },
            { "infermatic/mn-inferor-12b", (3, 3) },

            { "qwen/qwen-2.5-coder-32b-instruct:free", (5, 2) },
            { "qwen/qwen-2.5-coder-32b-instruct", (5, 2) },

            { "raifle/sorcererlm-8x22b", (5, 2) },
            { "eva-unit-01/eva-qwen-2.5-32b", (4, 2) },
            { "thedrummer/unslopnemo-12b", (4, 2) },

            { "anthropic/claude-3.5-haiku:beta", (4, 4) },
            { "anthropic/claude-3.5-haiku", (4, 4) },
            { "anthropic/claude-3.5-haiku-20241022:beta", (4, 4) },
            { "anthropic/claude-3.5-haiku-20241022", (4, 4) },

            { "neversleep/llama-3.1-lumimaid-70b", (4, 2) },
            { "anthracite-org/magnum-v4-72b", (5, 2) },
            { "anthropic/claude-3.5-sonnet:beta", (5, 2) },
            { "anthropic/claude-3.5-sonnet", (5, 2) },
            { "x-ai/grok-beta", (5, 2) },

            { "mistralai/ministral-8b", (3, 4) },
            { "mistralai/ministral-3b", (2, 4) },

            { "qwen/qwen-2.5-7b-instruct:free", (4, 3) },
            { "qwen/qwen-2.5-7b-instruct", (4, 3) },

            { "nvidia/llama-3.1-nemotron-70b-instruct", (5, 2) },

            { "inflection/inflection-3-productivity", (3, 3) },
            { "inflection/inflection-3-pi", (3, 3) },

            { "thedrummer/rocinante-12b", (4, 2) },
            { "anthracite-org/magnum-v2-72b", (5, 2) },
            { "liquid/lfm-40b", (4, 2) },
            { "meta-llama/llama-3.2-3b-instruct:free", (3, 4) },
            { "meta-llama/llama-3.2-3b-instruct", (3, 4) },
            { "meta-llama/llama-3.2-1b-instruct:free", (2, 5) },
            { "meta-llama/llama-3.2-1b-instruct", (2, 5) },
            { "meta-llama/llama-3.2-90b-vision-instruct", (5, 1) },
            { "meta-llama/llama-3.2-11b-vision-instruct:free", (4, 3) },
            { "meta-llama/llama-3.2-11b-vision-instruct", (4, 3) },

            { "qwen/qwen-2.5-72b-instruct:free", (5, 1) },
            { "qwen/qwen-2.5-72b-instruct", (5, 1) },

            { "neversleep/llama-3.1-lumimaid-8b", (4, 3) },
            { "mistralai/pixtral-12b", (4, 2) },

            { "cohere/command-r-plus-08-2024", (4, 3) },
            { "cohere/command-r-08-2024", (4, 3) },

            { "qwen/qwen-2.5-vl-7b-instruct:free", (4, 3) },
            { "qwen/qwen-2.5-vl-7b-instruct", (4, 3) },

            { "sao10k/l3.1-euryale-70b", (4, 2) },
            { "google/gemini-flash-1.5-8b-exp", (4, 3) },
            { "microsoft/phi-3.5-mini-128k-instruct", (4, 3) },

            { "nousresearch/hermes-3-llama-3.1-70b", (5, 2) },
            { "nousresearch/hermes-3-llama-3.1-405b", (5, 1) },

            { "sao10k/l3-lunaris-8b", (4, 3) },
            { "aetherwiing/mn-starcannon-12b", (4, 2) },

            { "meta-llama/llama-3.1-405b:free", (5, 1) },
            { "meta-llama/llama-3.1-405b", (5, 1) },
            { "nothingiisreal/mn-celeste-12b", (4, 2) },

            { "perplexity/llama-3.1-sonar-small-128k-online", (4, 3) },
            { "perplexity/llama-3.1-sonar-large-128k-online", (5, 2) },

            { "meta-llama/llama-3.1-8b-instruct:free", (4, 3) },
            { "meta-llama/llama-3.1-8b-instruct", (4, 3) },
            { "meta-llama/llama-3.1-405b-instruct", (5, 1) },
            { "meta-llama/llama-3.1-70b-instruct", (5, 2) },

            { "mistralai/codestral-mamba", (5, 3) },
            { "mistralai/mistral-nemo", (4, 3) },

            { "alpindale/magnum-72b", (5, 2) },
            { "01-ai/yi-large", (4, 2) },
            { "ai21/jamba-instruct", (4, 3) },

            { "anthropic/claude-3.5-sonnet-20240620:beta", (5, 2) },
            { "anthropic/claude-3.5-sonnet-20240620", (5, 2) },

            { "sao10k/l3-euryale-70b", (4, 2) },
            { "cognitivecomputations/dolphin-mixtral-8x22b", (4, 3) },
            { "qwen/qwen-2-72b-instruct", (5, 1) },

            { "mistralai/mistral-7b-instruct:free", (4, 3) },
            { "mistralai/mistral-7b-instruct", (4, 3) },
            { "nousresearch/hermes-2-pro-llama-3-8b", (4, 3) },
            { "mistralai/mistral-7b-instruct-v0.3", (4, 3) },

            { "microsoft/phi-3-mini-128k-instruct", (4, 3) },
            { "microsoft/phi-3-medium-128k-instruct", (5, 2) },

            { "neversleep/llama-3-lumimaid-70b", (4, 2) },
            { "deepseek/deepseek-coder", (5, 2) },

            { "meta-llama/llama-guard-2-8b", (3, 3) },
            { "allenai/olmo-7b-instruct", (4, 3) },
            { "neversleep/llama-3-lumimaid-8b:extended", (4, 3) },
            { "neversleep/llama-3-lumimaid-8b", (4, 3) },
            { "sao10k/fimbulvetr-11b-v2", (4, 2) },
            { "meta-llama/llama-3-8b-instruct", (4, 3) },
            { "meta-llama/llama-3-70b-instruct", (5, 2) },
            { "mistralai/mixtral-8x22b-instruct", (5, 2) },
            { "microsoft/wizardlm-2-8x22b", (5, 2) },

            { "cohere/command-r-plus", (5, 2) },
            { "cohere/command-r-plus-04-2024", (5, 2) },
            { "sophosympatheia/midnight-rose-70b", (4, 2) },
            { "cohere/command", (4, 3) },
            { "cohere/command-r", (4, 3) },

            { "anthropic/claude-3-haiku:beta", (4, 4) },
            { "anthropic/claude-3-haiku", (4, 4) },
            { "anthropic/claude-3-opus:beta", (5, 1) },
            { "anthropic/claude-3-opus", (5, 1) },
            { "anthropic/claude-3-sonnet:beta", (5, 2) },
            { "anthropic/claude-3-sonnet", (5, 2) },
            { "cohere/command-r-03-2024", (4, 3) },

            { "mistralai/mistral-large", (5, 2) },
            { "nousresearch/nous-hermes-2-mixtral-8x7b-dpo", (5, 2) },
            { "mistralai/mistral-medium", (4, 3) },
            { "mistralai/mistral-small", (3, 4) },
            { "mistralai/mistral-tiny", (2, 5) },
            { "mistralai/mistral-7b-instruct-v0.2", (4, 3) },
            { "mistralai/mixtral-8x7b-instruct", (5, 2) },

            { "neversleep/noromaid-20b", (4, 2) },

            { "anthropic/claude-2.1:beta", (4, 3) },
            { "anthropic/claude-2.1", (4, 3) },
            { "anthropic/claude-2:beta", (4, 3) },
            { "anthropic/claude-2", (4, 3) },

            { "undi95/toppy-m-7b", (3, 3) },
            { "alpindale/goliath-120b", (5, 1) },
            { "openrouter/auto", (3, 3) },
            { "jondurbin/airoboros-l2-70b", (4, 2) },
            { "mistralai/mistral-7b-instruct-v0.1", (4, 3) },
            { "pygmalionai/mythalion-13b", (4, 2) },
            { "mancer/weaver", (4, 2) },

            { "anthropic/claude-2.0:beta", (4, 3) },
            { "anthropic/claude-2.0", (4, 3) },
            { "undi95/remm-slerp-l2-13b", (4, 2) },
            { "gryphe/mythomax-l2-13b", (4, 2) },

            { "meta-llama/llama-2-70b-chat", (4, 2) },
            { "meta-llama/llama-3.3-8b-instruct:free", (4, 3) },

            { "mistralai/devstral-small:free", (3, 4) },
            { "mistralai/devstral-small", (3, 4) },

            { "anthropic/claude-opus-4", (5, 1) },
            { "anthropic/claude-sonnet-4", (5, 2) },

            { "thedrummer/valkyrie-49b-v1", (5, 2) },
            { "sarvamai/sarvam-m", (4, 3) },

            { "deepseek/deepseek-r1-0528-qwen3-8b:free", (4, 3) },
            { "deepseek/deepseek-r1-0528-qwen3-8b", (4, 3) },
            { "deepseek/deepseek-r1-0528:free", (5, 2) },
            { "deepseek/deepseek-r1-0528", (5, 2) },
            { "sarvamai/sarvam-m:free", (4, 3) },

            { "nvidia/llama-3.1-nemotron-ultra-253b-v1", (5, 1) },
            { "deepseek/deepseek-r1-distill-qwen-7b", (4, 3) },
            { "sentientagi/dobby-mini-unhinged-plus-llama-3.1-8b", (3, 4) },

            { "mistralai/magistral-small-2506", (4, 3) },
            { "mistralai/magistral-medium-2506", (5, 2) },
            { "mistralai/magistral-medium-2506:thinking", (5, 1) },
        };
    }
}