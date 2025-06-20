using Glitch9.AIDevKit.GENTasks;
using Glitch9.IO.Networking.RESTApi;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal static class AssistantUtil
    {
        internal static List<(string eventAsString, string dataAsString)> PairEvents(List<(SSEField field, string result)> sseFields, ILogger logger)
        {
            List<(string eventAsString, string dataAsString)> eventAndData = new();

            string tempEventAsString = null;
            foreach ((SSEField field, string result) in sseFields)
            {
                if (field == SSEField.Event)
                {
                    if (tempEventAsString != null)
                    {
                        eventAndData.Add((tempEventAsString, ""));
                    }

                    tempEventAsString = result.Trim();
                }
                else if (field == SSEField.Data)
                {
                    if (tempEventAsString == null)
                    {
                        logger.Error("SSE data detected, but there is no event field.");
                        continue;
                    }
                    eventAndData.Add((tempEventAsString, result));
                    tempEventAsString = null;
                }
                else
                {
                    logger.Error($"Invalid SSE field detected: {field}");
                }
            }

            return eventAndData;
        }

        internal static PromptRecord CreatePromptRecord(
            Model assistanatModel,
            string assistantName,
            Usage usageFromRun,
            ThreadMessage inputMessage,
            ThreadMessage outputMessage)
        {
            var r = new PromptRecord()
                .InitializeCommon(EndpointType.Assistant, assistanatModel, assistantName, usageFromRun, 1)
                .SetPromptText(inputMessage.ToString())
                .SetOutputTexts(new[] { outputMessage.ToString() });

            return r.SaveToDatabase();
        }
    }
}