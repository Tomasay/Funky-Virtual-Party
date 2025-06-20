using System;

namespace Glitch9.AIDevKit
{
    public class ModelFilter : IEquatable<ModelFilter>, IAIDevKitAssetFilter<Model>
    {
        public Api Api { get; set; }
        public ModelFeature? Feature { get; set; }

        public bool IsEmpty => (Api == Api.All || Api == Api.All) && Feature == null;

        public bool Matches(Model data)
        {
            if (Api != Api.All && data.Api != Api) return false;
            if (Feature != null && !data.Feature.HasFlag(Feature)) return false;
            return true;
        }

        public static ModelFilter LLM(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.TextGeneration };
        public static ModelFilter IMG(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.ImageGeneration };
        public static ModelFilter TTS(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.SpeechGeneration };
        public static ModelFilter STT(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.SpeechRecognition };
        public static ModelFilter EMB(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.TextEmbedding };
        public static ModelFilter MOD(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.Moderation };
        public static ModelFilter SFX(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.SoundFXGeneration };
        public static ModelFilter RTM(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.Realtime };
        public static ModelFilter VCM(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.VoiceChanger };
        public static ModelFilter VID(Api api = Api.All) => new() { Api = api, Feature = ModelFeature.VideoGeneration };


        // Dictionary key를 위한 동등성 비교 구현
        public override bool Equals(object obj) => Equals(obj as ModelFilter);

        public bool Equals(ModelFilter other)
        {
            if (other == null) return false;
            return Api == other.Api && Feature == other.Feature;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Api.GetHashCodeOrDefault();
                hash = hash * 23 + Feature.GetHashCodeOrDefault();
                return hash;
            }
        }
    }
}