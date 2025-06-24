
namespace Glitch9.AIDevKit
{
    public class TranscriptStreamHandler : AIDevKitStreamHandler<TranscriptStreamHandler, TranscriptChunk, TranscriptStreamEvent, GENTranscriptTask>
    {
        internal override TranscriptStreamEvent BuildFinalResult()
        {
            return _lastChunk?.Value;
        }

        internal override void ProcessChunk(TranscriptChunk chunk)
        {
            // do nothing
        }
    }
}