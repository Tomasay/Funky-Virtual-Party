using System;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Advanced.Lyria
{
    [Serializable]
    public class MusicGenerationConfig
    {
        [JsonProperty("guidance")] public float? Guidance { get; set; } = null; // [0.0, 6.0], default: 4.0

        [JsonProperty("bpm")] public int? BPM { get; set; } = null; // [60, 200]

        [JsonProperty("density")] public float? Density { get; set; } = null; // [0.0, 1.0]

        [JsonProperty("brightness")] public float? Brightness { get; set; } = null; // [0.0, 1.0]

        [JsonProperty("scale")] public MusicScale? Scale { get; set; } = null; // Enum, previously defined

        [JsonProperty("mute_bass")] public bool? MuteBass { get; set; } = null; // default: false

        [JsonProperty("mute_drums")] public bool? MuteDrums { get; set; } = null; // default: false

        [JsonProperty("only_bass_and_drums")] public bool? OnlyBassAndDrums { get; set; } = null; // default: false

        [JsonProperty("temperature")] public float? Temperature { get; set; } = null; // [0.0, 3.0], default: 1.1

        [JsonProperty("top_k")] public int? TopK { get; set; } = null; // [1, 1000], default: 40

        [JsonProperty("seed")] public int? Seed { get; set; } = null; // 0 ~ 2,147,483,647 (int.MaxValue)
    }
}
