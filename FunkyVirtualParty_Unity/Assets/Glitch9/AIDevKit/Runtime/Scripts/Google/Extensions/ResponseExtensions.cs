namespace Glitch9.AIDevKit.Google
{
    public static class ResponseExtensions
    {
        public static string GetOutputText(this GenerateContentResponse res)
        {
            return res?.Candidates?[0].Content?.Parts?[0].Text;
        }

        public static GenerateContentRequest AddContent(this GenerateContentRequest req, params Content[] contents)
        {
            req?.Contents.AddRange(contents);
            return req;
        }
    }
}
