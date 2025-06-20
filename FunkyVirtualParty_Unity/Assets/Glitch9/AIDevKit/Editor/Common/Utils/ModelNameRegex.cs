using System.Collections.Generic;
using System.Text;

namespace Glitch9.AIDevKit.Editor
{
    internal static class ModelNameRegex
    {
        // This word can be the second or third word in the family name
        // Note that it could also be a combination of two words, e.g. "Mini Audio", "Mini TTS"
        private static readonly List<string> _partOfFamilyNames = new()
        {
            "Turbo",
            "Mini",
            "Nano",
            "Pro",
            "Search",
            "Audio",
            "Transcribe",
            "TTS",
            "Instruct",
            "Flash",
            "Flash-Lite",
            "Embedding",
        };

        internal static string ResolveInspectorName(string rawInspectorName)
        {
            if (string.IsNullOrEmpty(rawInspectorName)) return string.Empty;

            if (rawInspectorName.StartsWith("Model that performs")) return "Aqa";

            string family = "";
            string model = "";
            string version = "";

            string[] split = rawInspectorName.Split(' ');

            for (int i = 0; i < split.Length; i++)
            {
                string word = split[i].Trim();

                if (i == 0)
                {
                    if (word.Contains(":ft"))
                    {
                        word = word.Replace(":ft", string.Empty).Trim();
                    }

                    family = word;
                    continue;
                }

                if (_partOfFamilyNames.Contains(word))
                {
                    family += " " + word;
                    continue;
                }


                if (i == 1)
                {
                    char firstChar = word[0];
                    char secondChar = word.Length > 1 ? word[1] : ' ';

                    if (char.IsDigit(firstChar) && !char.IsDigit(secondChar))
                    {
                        family += " " + word;
                        continue;
                    }
                }

                version += " " + word;
            }

            if (string.IsNullOrEmpty(version))
            {
                version = "Latest";
            }

            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.Append(family);

                if (!string.IsNullOrEmpty(model))
                {
                    sb.Append("/"); // for inspector grouping
                    sb.Append(model.Trim());
                }

                if (!string.IsNullOrEmpty(version))
                {
                    sb.Append("/"); // for inspector grouping
                    sb.Append(version.Trim());
                }

                return sb.ToString().Trim();
            }
        }
    }
}