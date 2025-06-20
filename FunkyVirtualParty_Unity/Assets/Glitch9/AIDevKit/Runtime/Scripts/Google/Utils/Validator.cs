namespace Glitch9.AIDevKit.Google
{
    public class Validator
    {
        public static void CheckResponse(GenerateContentResponse response, bool stream)
        {
            if (response.PromptFeedback?.BlockReason != null)
            {
                throw new BlockedPromptException(response.PromptFeedback);
            }

            // if (!stream && response.Candidates[0].FinishReason != StopReason.None &&
            //     response.Candidates[0].FinishReason != StopReason.Stop &&
            //     response.Candidates[0].FinishReason != StopReason.MaxTokens)
            // {
            //     throw new StopCompletionException(response.Candidates[0]);
            // }
        }
    }
}