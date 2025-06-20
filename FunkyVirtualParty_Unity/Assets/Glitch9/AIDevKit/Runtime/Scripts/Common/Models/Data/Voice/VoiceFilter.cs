using System;

namespace Glitch9.AIDevKit
{
    public class VoiceFilter : IEquatable<VoiceFilter>, IAIDevKitAssetFilter<Voice>
    {
        public Api Api { get; set; }

        public bool IsEmpty => Api == Api.All || Api == Api.None;

        public bool Matches(Voice data)
        {
            if (Api != Api.All && data.Api != Api) return false;
            return true;
        }

        public static VoiceFilter API(Api api = Api.All) => new() { Api = api };

        // Dictionary key를 위한 동등성 비교 구현
        public override bool Equals(object obj) => Equals(obj as VoiceFilter);

        public bool Equals(VoiceFilter other)
        {
            if (other == null) return false;
            return Api == other.Api;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Api.GetHashCodeOrDefault();
                return hash;
            }
        }
    }
}