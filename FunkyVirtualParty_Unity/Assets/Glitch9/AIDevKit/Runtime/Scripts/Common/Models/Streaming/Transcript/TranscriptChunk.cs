namespace Glitch9.AIDevKit
{
    public class TranscriptChunk : AIDevKitChunk<TranscriptStreamEvent>
    {
        public override bool IsDone => Value?.Type == "transcript.text.done";
        public override Usage Usage => Value?.Usage;

        internal static TranscriptChunk Create(TranscriptStreamEvent streamEvent)
        {
            return new TranscriptChunk
            {
                Value = streamEvent,
            };
        }

        internal static TranscriptChunk Error(string error)
        {
            return new TranscriptChunk
            {
                ErrorMessage = error ?? "Unknown error",
            };
        }
    }
}