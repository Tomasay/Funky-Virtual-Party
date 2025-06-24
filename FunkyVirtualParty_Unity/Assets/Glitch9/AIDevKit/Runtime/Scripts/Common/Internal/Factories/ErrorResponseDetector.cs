using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    internal static class ErrorResponseDetector
    {
        internal static bool IsErrorResponse(string sseString, JsonSerializerSettings jsonSettings, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(sseString)) return false;

            const string error = "\"error\":";

            if (sseString.Contains(error))
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponseWrapper>(sseString, jsonSettings);

                    if (errorResponse == null)
                    {
                        errorMessage = $"Failed to parse error response: {sseString}";
                    }
                    else
                    {
                        errorMessage = errorResponse.Error?.Message;
                    }

                    return true;
                }
                catch (JsonException e)
                {
                    errorMessage = $"Failed to parse error response: {e.Message}\n{sseString}";
                    return true;
                }
            }

            return false;
        }
    }
}