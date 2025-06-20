using System.Globalization;

namespace Glitch9.Editor
{
    public class EditorSearchUtil
    {
        private static CompareInfo CompareInfo => _compareInfo ??= CultureInfo.CurrentCulture.CompareInfo;
        private static CompareInfo _compareInfo;
        public static bool Search(string searchString, string searchTarget)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (string.IsNullOrEmpty(searchTarget)) return true;
            return CompareInfo.IndexOf(searchTarget, searchString, CompareOptions.IgnoreCase) >= 0;
        }

        public static bool Search<TEnum>(string searchString, TEnum searchTarget) where TEnum : System.Enum
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (searchTarget == null) return true;
            return CompareInfo.IndexOf(searchTarget.ToString(), searchString, CompareOptions.IgnoreCase) >= 0;
        }

        public static bool Search(string searchString, string[] searchTarget)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            if (searchTarget == null) return true;
            foreach (string target in searchTarget)
            {
                if (CompareInfo.IndexOf(target, searchString, CompareOptions.IgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
