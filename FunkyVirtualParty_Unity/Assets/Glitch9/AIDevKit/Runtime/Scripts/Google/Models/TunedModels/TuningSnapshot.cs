using System;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class TuningSnapshot
    {
        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonProperty("epoch")]
        public int Epoch { get; set; }

        [JsonProperty("mean_score")]
        public float MeanScore { get; set; }

        [JsonProperty("compute_time")]
        public DateTime? ComputeTime { get; set; }
    }
}