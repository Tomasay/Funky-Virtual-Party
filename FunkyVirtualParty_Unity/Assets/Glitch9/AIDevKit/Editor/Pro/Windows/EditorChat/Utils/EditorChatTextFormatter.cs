using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static class EditorChatTextFormatter
    {
        internal static string ParseUsageTooltip(Usage usage)
        {
            if (usage == null) return "No usage data available";

            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                if (usage.InputTokens != null) sb.AppendLine($"Input Tokens: {usage.InputTokens}");
                if (usage.OutputTokens != null) sb.AppendLine($"Output Tokens: {usage.OutputTokens}");
                return sb.ToString();
            }
        }

        internal static string FormatStringList(List<string> stringList)
        {
            if (stringList.IsNotNullOrEmpty())
            {
                using (StringBuilderPool.Get(out StringBuilder sb))
                {
                    sb.AppendLine("--------------------");
                    foreach (string text in stringList)
                    {
                        sb.AppendLine($"- {text}");
                    }
                    sb.AppendLine("--------------------");
                    return sb.ToString();
                }
            }

            return null;
        }

        internal static async UniTask<string> FormatTextAssetsAsync(List<string> attachedTextAssetPaths)
        {
            if (attachedTextAssetPaths.IsNotNullOrEmpty())
            {
                Dictionary<string, string> textAssetContents = new();
                foreach (string filePath in attachedTextAssetPaths)
                {
                    if (File.Exists(filePath))
                    {
                        string fileContent = await File.ReadAllTextAsync(filePath);
                        if (!string.IsNullOrEmpty(fileContent))
                        {
                            textAssetContents.Add(filePath, fileContent);
                        }
                    }
                }

                if (textAssetContents.Count > 0)
                {
                    using (StringBuilderPool.Get(out StringBuilder sb))
                    {
                        sb.AppendLine("--------------------");
                        foreach (var kvp in textAssetContents)
                        {
                            string filePath = kvp.Key;
                            string fileContent = kvp.Value;
                            sb.AppendLine(FormatCodeBlock(filePath, fileContent));
                        }
                        sb.AppendLine("--------------------");
                        return sb.ToString();
                    }
                }
            }

            return null;
        }

        internal static string FormatCodeBlock(string filePath, string textContent)
        {
            return $"```{GetCodeBlockKey(filePath)}\n{textContent}\n```";
        }

        private static string GetCodeBlockKey(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "text";

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".cs" => "csharp",
                ".txt" => "text",
                ".json" or ".jsonc" => "json",
                ".xml" => "xml",
                ".md" => "markdown",
                ".html" or ".htm" => "html",
                ".css" or ".uss" => "css",
                ".js" => "javascript",
                ".ts" => "typescript",
                ".shader" or ".shadertoy" or ".shadergraph" or ".compute" => "glsl",
                ".uxml" => "xml",
                ".yaml" or ".yml" => "yaml",
                ".csv" or ".tsv" => "csv",
                ".log" or ".ini" or ".config" => "ini",
                _ => "text"
            };
        }
    }
}