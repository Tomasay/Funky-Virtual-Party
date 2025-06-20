using UnityEngine.UIElements;

namespace Glitch9.Editor
{
    internal static class USSStyles
    {
        private const string kMarkerFileName = "corelib_editor_styles";
        private static readonly StyleSheetCache _cache = StyleSheetCache.WithMarkerFile(kMarkerFileName);
        internal static StyleSheet Get(string fileName) => _cache.Get(fileName);
    }
}