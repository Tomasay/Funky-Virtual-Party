using System;
using System.IO;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Generation
{
    internal static class ScriptExporter
    {
        /// <summary>
        /// Writes a C# script string to a file
        /// </summary>
        /// <param name="csCodeText"></param>
        /// <param name="savePath"></param>
        /// <param name="namespace"></param>
        /// <returns>
        /// Returns class name
        /// </returns>
        internal static async UniTask<string> SaveAsFileAsync(string csCodeText, string savePath, string className = null)
        {
            if (string.IsNullOrWhiteSpace(csCodeText)) return null;

            // remove code blocks if present
            csCodeText = csCodeText.Trim();
            if (csCodeText.StartsWith("```csharp", StringComparison.OrdinalIgnoreCase))
            {
                csCodeText = csCodeText.Substring(9).Trim();
            }
            if (csCodeText.EndsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                csCodeText = csCodeText.Substring(0, csCodeText.Length - 3).Trim();
            }

            className ??= ExtractClassName(csCodeText);
            if (string.IsNullOrWhiteSpace(className))
            {
                Debug.LogWarning("Could not detect class name.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(savePath)) return null;

            // 결정: savePath가 파일인가? 디렉토리인가?
            string fullPath;
            if (savePath.EndsWith(".cs"))
            {
                // savePath가 파일이라면 그대로 사용
                fullPath = Path.IsPathRooted(savePath)
                    ? savePath
                    : Path.Combine(Application.dataPath, savePath);
            }
            else
            {
                // 디렉토리일 경우 className.cs 붙임
                string directory = Path.IsPathRooted(savePath)
                    ? savePath
                    : Path.Combine(Application.dataPath, savePath);

                fullPath = Path.Combine(directory, $"{className}.cs");
            }

            fullPath = fullPath.FixDoubleAssets();

            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            try
            {
                await File.WriteAllTextAsync(fullPath, csCodeText);
                AssetDatabase.Refresh();
                return fullPath;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save script: {e.Message}");
                return null;
            }
        }

        internal static bool ValidateScript(string csCodeText, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(csCodeText))
            {
                errorMessage = "C# code text is empty.";
                return false;
            }

            // Check for basic C# syntax errors
            if (!csCodeText.Contains("class") || !csCodeText.Contains("{") || !csCodeText.Contains("}"))
            {
                errorMessage = "C# code does not contain a valid class definition.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }


        /// <summary>
        /// Extracts the class name from a C# script string
        /// </summary>
        /// <param name="csCodeText"></param>
        /// <returns>
        /// Extracted class name
        /// </returns>
        internal static string ExtractClassName(string csCodeText)
        {
            string[] lines = csCodeText.Split('\n');
            string className = string.Empty;

            foreach (string line in lines)
            {
                if (line.Contains("class"))
                {
                    // Split the line into parts
                    string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int classIndex = Array.IndexOf(parts, "class"); // "class" index
                    if (classIndex != -1 && classIndex < parts.Length - 1) // "class" is not the last word in the line
                    {
                        className = parts[classIndex + 1]; // "class" is followed by the class name
                        break;
                    }
                }
            }

            return className;
        }

        /// <summary>
        /// Inserts a namespace into a C# script string
        /// </summary>
        /// <param name="csCodeText"></param>
        /// <returns>
        /// C# script string with namespace inserted
        /// </returns>
        private static string InsertNamespace(string csCodeText, string @namespace)
        {
            // Check if the namespace is already present
            if (csCodeText.Contains($"namespace {@namespace}"))
            {
                Debug.LogWarning($"Namespace {@namespace} already exists.");
                return csCodeText;
            }

            // Define the pattern to match the start of a class, interface, struct, or enum
            string pattern = @"\b(internal|private|internal|abstract|sealed|class|interface|struct|enum)\s+";

            // Match the pattern
            Match match = Regex.Match(csCodeText, pattern, RegexOptions.Multiline);

            if (match.Success)
            {
                // Split the string into two parts: before and after the match
                string beforeNamespace = csCodeText.Substring(0, match.Index);
                string afterNamespace = csCodeText.Substring(match.Index);
                string indentedAfterNamespace = Regex.Replace(afterNamespace, @"^", "    ", RegexOptions.Multiline);

                // Insert the namespace
                string result = beforeNamespace +
                                $"namespace {@namespace}\n{{\n" +
                                indentedAfterNamespace +
                                "\n}";

                return result;
            }

            // If the pattern is not found, return the original string
            return csCodeText;
        }
    }
}
