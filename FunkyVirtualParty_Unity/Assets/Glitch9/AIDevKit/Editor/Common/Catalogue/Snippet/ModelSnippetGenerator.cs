using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.Ollama;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenRouter;
using Glitch9.Editor.CodeGen;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class ModelSnippetGenerator
    {
        internal static async UniTask GenerateAllAsync()
        {
            if (OpenAISettings.Instance.HasApiKey()) Generate(Api.OpenAI);
            if (GenerativeAISettings.Instance.HasApiKey()) Generate(Api.Google);
            if (ElevenLabsSettings.Instance.HasApiKey()) Generate(Api.ElevenLabs);
            if (OpenRouterSettings.Instance.HasApiKey()) Generate(Api.OpenRouter);
            if (await OllamaSettings.CheckConnectionAsync()) Generate(Api.Ollama);
        }

        internal static void Generate(Api api)
        {
            List<Model> models = ModelLibrary.GetModelsByAPI(api);
            if (models.IsNullOrEmpty()) throw new System.Exception("No models to generate.");

            Debug.Log($"Generating model snippets for {api}...");

            string className = AssetSnippetUtil.ResolveModelClassName(api);
            string namespaceName = AssetSnippetUtil.ResolveNamespace(api);
            string targetDir = AIDevKitEditorPath.FindConfigFilePath(api);
            string writePath = System.IO.Path.Combine(targetDir, $"{className}.cs");

            CodeGenBuilder builder = new();

            builder.AddDirectiveComment(DirectiveComment.ReSharperDisableAll);
            builder.SetNamespace(namespaceName);
            builder.AddClass(className);

            // Sort models by feature for better organization
            // models.Sort((x, y) => x.Feature.CompareTo(y.Feature));

            List<ModelCatalogueEntry> snippetModels = new();

            HashSet<(string fieldName, string modelId)> existingFields = AssetSnippetUtil.ExtractExistingFields(writePath);

            // models 에 existingFields 의 modelId와 일치하는 모델이 있다면 existingFields 에서 제거
            foreach (Model model in models)
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Id)) continue;

                if (existingFields.Count > 0)
                {
                    for (int i = existingFields.Count - 1; i >= 0; i--)
                    {
                        var (fieldName, modelId) = existingFields.ElementAt(i);
                        if (model.Id == modelId) existingFields.Remove((fieldName, modelId));
                    }
                }

                ModelCatalogueEntry modelEntry = ModelCatalogue.Instance.GetEntry(model.Id);

                if (modelEntry == null)
                {
                    Debug.LogWarning($"ModelCatalogueEntry for {model.Id} not found. Skipping...");
                    continue;
                }

                snippetModels.Add(modelEntry);
            }

            if (existingFields.Count > 0)
            {
                // 아직도 뭔가 남아있다면, 유저가 라이브러리에서 제거했지만 Snippet 파일에는 남아있는 모델들
                // 본래대로라면 Snippet 내용에서 제외되었지만, 
                // 이러면 초보 유저들은 Snippet이 없어져서 생기는 Complier Error를 버그로 인식할 수 있음
                // 따라서, 그냥 그대로 남긴다. 

                foreach (var (fieldName, modelId) in existingFields)
                {
                    ModelCatalogueEntry modelEntry = ModelCatalogue.Instance.GetEntry(modelId);
                    modelEntry ??= ModelCatalogueEntry.ObsoleteEntry(modelId, fieldName);
                    snippetModels.Add(modelEntry);
                }
            }

            // Sort models by feature for better organization
            snippetModels.Sort((x, y) => x.Feature.CompareTo(y.Feature));

            HashSet<string> dupeChecks = new();
            bool deprecatedExists = false;

            foreach (ModelCatalogueEntry model in snippetModels)
            {
                if (model == null)
                {
                    Debug.LogWarning($"{typeof(Model).Name} is null. Skipping...");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    Debug.LogWarning($"{typeof(Model).Name} ID is null or empty. Skipping...");
                    continue;
                }

                //if (model.Model.IsFineTuned) continue;
                //if (model.Model.IsDeprecated) continue;

                string propertyName = AssetSnippetUtil.ResolveModelPropertyName(model.Id);
                string value = model.Id;

                if (dupeChecks.Contains(value))
                {
                    Debug.LogWarning($"Duplicate {typeof(Model).Name} name '{value}' found. Skipping...");
                    continue;
                }

                dupeChecks.Add(value);

                List<CodeGenComment> comments = new();
                List<CodeGenAttribute> attributes = new();

                //List<string> commentLines = new();
                //if (!string.IsNullOrEmpty(model.Description)) commentLines.Add(model.Description); 
                //if (commentLines.Count > 0) comments.Add(CodeGenComment.Summary(commentLines.ToArray()));

                if (model.OutputTokenLimit != null && model.OutputTokenLimit != 0)
                {
                    // (e.g,) Returns a maximum of 128,000 tokens.
                    comments.Add(CodeGenComment.Remarks($"Returns a maximum of {model.OutputTokenLimit.Value} tokens."));
                }

                if (model.InputTokenLimit != null && model.InputTokenLimit != 0)
                {
                    // (e.g,) A maximum of 4096 tokens can be processed at a time.
                    comments.Add(CodeGenComment.Remarks($"A maximum of {model.InputTokenLimit.Value} tokens can be processed at a time."));
                }

                // string inspectorName = ModelNameRegex.ResolveInspectorName(model.Name);
                // attributes.Add(CodeGenAttribute.Create("ApiEnum", $"\"{inspectorName}\"", $"\"{model.Id}\""));

                if (model.IsDeprecated)
                {
                    string message = $"This {typeof(Model).Name} is deprecated.";
                    attributes.Add(CodeGenAttribute.Obsolete($"\"{message}\""));
                    deprecatedExists = true;
                }

                builder.AddContString(className, propertyName, value, AccessModifier.Public, comments, attributes);
            }

            if (deprecatedExists) builder.AddUsing("System"); // to use Obsolete attribute

            builder.Generate(writePath);
        }
    }
}