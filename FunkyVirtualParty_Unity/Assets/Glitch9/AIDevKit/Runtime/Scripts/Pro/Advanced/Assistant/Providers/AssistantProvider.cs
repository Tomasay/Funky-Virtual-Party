using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using System.Linq;
using System.Text;
using OpenAIClient = Glitch9.AIDevKit.OpenAI.OpenAI;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class AssistantProvider : AssistantProviderBase<Assistant>
    {
        internal AssistantProvider(AssistantController controller, AssistantLogger logger) : base(controller, logger) { }
        protected override async UniTask<Assistant> CreateInternalAsync(params object[] args)
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.Append(Controller.Instructions);
                if (Controller.MaxRequestLength != -1) sb.Append($" Limit your response to {Controller.MaxRequestLength} characters.");

                AssistantRequest.Builder builder = new AssistantRequest.Builder()
                    .SetName(Controller.AssistantName)
                    .SetSender(Controller.AssistantName)
                    .SetModel(Controller.Model)
                    .SetDescription(Controller.Description)
                    .SetInstructions(sb.ToString())
                    .SetTemperature(Controller.Temperature)
                    .SetTopP(Controller.TopP);

                if (Controller.Tools != null) builder.SetTools(Controller.Tools);
                if (Controller.ToolResources != null) builder.SetToolResources(Controller.ToolResources);
                if (Controller.Metadata != null) builder.SetMetadata(Controller.Metadata);
                if (Controller.ResponseFormat != null) builder.SetResponseFormat(Controller.ResponseFormat.Value);

                return await OpenAIClient.DefaultInstance.Beta.Assistants.CreateAsync(builder.Build());
            }
        }

        protected override async UniTask<Assistant> RetrieveInternalAsync(string id, params object[] args)
        {
            bool fetchAssistantList = true;
            Assistant newAssistant = null;

            if (!string.IsNullOrEmpty(id))
            {
                _logger.Info("Previous assistant id found. Trying to retrieve the assistant.");
                newAssistant = await Controller.Client.Beta.Assistants.RetrieveAsync(id);
                if (newAssistant != null) return newAssistant;

                if (fetchAssistantList) _logger.Info("Failed to retrieve the previous assistant. Fetching the assistant list.");
                else _logger.Info("Failed to retrieve the previous assistant. Creating a new assistant.");

                await UniTask.Delay(AssistantController.MIN_INTERNAL_OPERATION_MILLIS);
            }
            else
            {
                if (fetchAssistantList) _logger.Info("AssistantObject id doesn't exist. Fetching the assistant list.");
                else _logger.Info("AssistantObject id doesn't exist. Creating a new assistant.");
            }

            if (fetchAssistantList)
            {
                QueryResponse<Assistant> queryResponse = await Controller.Client.Beta.Assistants.ListAsync(new(100));
                Assistant[] assistants = queryResponse?.Data;
                if (assistants.IsNotNullOrEmpty()) newAssistant = FindMatchingAssistant(assistants);
                if (newAssistant != null) return newAssistant;
                await UniTask.Delay(AssistantController.MIN_INTERNAL_OPERATION_MILLIS);
                _logger.Info("Failed to find a matching assistant from the list. Creating a new assistant.");
            }
            else
            {
                _logger.Info("Fetching assistant list is disabled. Creating a new assistant.");
            }

            return null;
        }

        protected override async UniTask<Assistant> UpdateInternalAsync(string id, params object[] args)
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.Append(Controller.Instructions);
                if (Controller.MaxRequestLength != -1) sb.Append($" Limit your response to {Controller.MaxRequestLength} characters.");

                AssistantRequest.Builder builder = new AssistantRequest.Builder()
                    .SetName(Controller.AssistantName)
                    .SetModel(Controller.Model)
                    .SetDescription(Controller.Description)
                    .SetTemperature(Controller.Temperature)
                    .SetTopP(Controller.TopP)
                    .SetInstructions(sb.ToString());

                if (Controller.Tools != null) builder.SetTools(Controller.Tools);
                if (Controller.ToolResources != null) builder.SetToolResources(Controller.ToolResources);
                if (Controller.Metadata != null) builder.SetMetadata(Controller.Metadata);
                if (Controller.ResponseFormat != null) builder.SetResponseFormat(Controller.ResponseFormat.Value);

                return await OpenAIClient.DefaultInstance.Beta.Assistants.UpdateAsync(id, builder.Build());
            }
        }

        protected override async UniTask<Assistant[]> ListInternalAsync(params object[] args)
        {
            QueryResponse<Assistant> queryResponse = await Controller.Client.Beta.Assistants.ListAsync(new(100));
            return queryResponse?.Data;
        }

        protected override async UniTask<bool> DeleteInternalAsync(string id, params object[] args)
        {
            return await OpenAIClient.DefaultInstance.Beta.Assistants.DeleteAsync(id);
        }

        /// <summary>
        /// OpenAI saves your assistant to their server.
        /// So try retrieving the assistant from the server first instead of creating a new one.
        /// </summary>
        /// <param name="assistants">Array of existing assistants.</param>
        /// <returns>The matching assistant object if found; otherwise, null.</returns>
        protected virtual Assistant FindMatchingAssistant(Assistant[] assistants)
        {
            return assistants.FirstOrDefault(a => a.Name == Controller.AssistantName);
        }
    }
}