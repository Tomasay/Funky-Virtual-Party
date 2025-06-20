using UnityEngine;

namespace Glitch9.AIDevKit
{
    public class Transcript : IGeneratedResult
    {
        public static implicit operator string(Transcript transcript) => transcript.Text;
        public virtual SystemLanguage Language { get; set; }
        public virtual string Text { get; set; }
        public int Count { get; } = 1; // always 1
        public Usage Usage => usage ??= UsageFactory.PerCharacter(Text?.Length ?? 0);
        private Usage usage;

        internal static Transcript Translation(string text) => new()
        {
            Text = text,
            Language = SystemLanguage.English
        };

        public override string ToString() => Text;
    }
}