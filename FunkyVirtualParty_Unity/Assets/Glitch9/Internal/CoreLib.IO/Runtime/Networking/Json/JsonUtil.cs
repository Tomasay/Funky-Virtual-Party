namespace Glitch9.IO.Networking.RESTApi
{
    internal static class JsonUtil
    {
        internal static bool IsJsonComplete(string json)
        {
            int depth = 0;
            bool inString = false;
            char lastChar = '\0';

            foreach (char c in json)
            {
                if (c == '"' && lastChar != '\\')
                {
                    inString = !inString;
                }

                if (!inString)
                {
                    if (c == '{') depth++;
                    else if (c == '}') depth--;
                }

                lastChar = c;
            }

            return depth == 0 && !inString;
        }
    }
}