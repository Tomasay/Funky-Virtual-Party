using System.Collections.Generic;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor.CodeGen;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class VoiceSnippetGenerator
    {
        internal static void GenerateAll()
        {
            if (OpenAISettings.Instance.HasApiKey()) Generate(Api.OpenAI);
            if (GenerativeAISettings.Instance.HasApiKey()) Generate(Api.Google);
            if (ElevenLabsSettings.Instance.HasApiKey()) Generate(Api.ElevenLabs);
        }

        internal static void Generate(Api api)
        {
            List<Voice> voices = VoiceLibrary.GetVoicesByAPI(api);

            if (voices.IsNullOrEmpty()) throw new System.Exception("No voices to generate.");

            Debug.Log($"Generating {api}({voices.Count}) voices...");

            string className = AssetSnippetUtil.ResolveVoiceClassName(api);
            string namespaceName = AssetSnippetUtil.ResolveNamespace(api);
            string targetDir = AIDevKitEditorPath.FindConfigFilePath(api);
            string writePath = System.IO.Path.Combine(targetDir, $"{className}.cs");

            CodeGenBuilder builder = new();

            builder.AddDirectiveComment(DirectiveComment.ReSharperDisableAll);
            builder.SetNamespace(namespaceName);
            builder.AddClass(className);

            //voices.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
            //voices.Sort((x, y) => x.Type.CompareTo(y.Type));

            HashSet<string> dupeChecks = new();
            bool deprecatedExists = false;

            foreach (Voice voice in voices)
            {
                if (voice == null)
                {
                    Debug.LogWarning($"{typeof(Voice).Name} is null. Skipping...");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(voice.Id))
                {
                    Debug.LogWarning($"{typeof(Voice).Name} ID is null or empty. Skipping...");
                    continue;
                }

                string propertyName = AssetSnippetUtil.ResolveVoicePropertyName(voice);
                string value = voice.Id;

                if (dupeChecks.Contains(propertyName))
                {
                    Debug.LogWarning($"Duplicate {typeof(Voice).Name} name '{propertyName}' found. Skipping...");
                    continue;
                }

                dupeChecks.Add(propertyName);

                List<CodeGenComment> comments = new();
                List<CodeGenAttribute> attributes = new();

                //List<string> commentLines = new();
                //if (voice.Type != VoiceType.None) commentLines.Add($"{api}'s {voice.Type.GetInspectorName()}");
                //if (!string.IsNullOrEmpty(voice.Description)) commentLines.Add(voice.Description);
                //if (commentLines.Count > 0) comments.Add(CodeGenComment.Summary(commentLines.ToArray()));

                // string inspectorName = ModelNameRegex.ResolveInspectorName(model.Name);
                // attributes.Add(CodeGenAttribute.Create("ApiEnum", $"\"{inspectorName}\"", $"\"{model.Id}\""));

                if (voice.IsDeprecated)
                {
                    string message = $"This {typeof(Voice).Name} is deprecated.";
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