using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Ollama.Services
{
    public class ModelService : CRUDServiceBase<Ollama>
    {
        private const string kEndpointCreateModel = "/api/create"; // POST
        private const string kEndpointCopyModel = "/api/copy"; // POST
        private const string kEndpointPullModel = "/api/pull"; // POST
        private const string kEndpointPushModel = "/api/push"; // POST
        private const string kEndpointDeleteModel = "/api/delete"; // DELETE 
        private const string kEndpointListLocalModels = "/api/tags"; // GET
        private const string kEndpointListRunningModels = "/api/ps"; // GET


        public ModelService(Ollama client) : base(client) { }

        /// <summary>
        /// 
        /// Create a model from:
        /// - another model;
        /// - a safetensors directory; or
        /// - a GGUF file.
        /// 
        /// If you are creating a model from a safetensors directory or from a GGUF file, 
        /// you must create a blob for each of the files 
        /// and then use the file name and SHA256 digest associated with each blob in the files field.
        /// 
        /// </summary>
        /// 
        /// <param name="req"></param>
        /// 
        /// <returns>
        /// A stream of JSON objects is returned:
        /// {"status":"reading model metadata"}
        /// {"status":"creating system layer"}
        /// {"status":"using already created layer sha256:22f7f8ef5f4c791c1b03d7eb414399294764d7cc82c7e94aa81a1feb80a983a2"}
        /// {"status":"using already created layer sha256:8c17c2ebb0ea011be9981cc3922db8ca8fa61e828c5d3f44cb6ae342bf80460b"}
        /// {"status":"using already created layer sha256:7c23fb36d80141c4ab8cdbb61ee4790102ebd2bf7aeff414453177d4f2110e5d"}
        /// {"status":"using already created layer sha256:2e0493f67d0c8c9c68a8aeacdf6a38a2151cb3c4c1d42accf296e19810527988"}
        /// {"status":"using already created layer sha256:2759286baa875dc22de5394b4a925701b1896a7e3f8e53275c36f75a877a82c9"}
        /// {"status":"writing layer sha256:df30045fe90f0d750db82a058109cecd6d4de9c90a3d75b19c09e5f64580bb42"}
        /// {"status":"writing layer sha256:f18a68eb09bf925bb1b669490407c1b1251c5db98dc4d3d81f3088498ea55690"}
        /// {"status":"writing manifest"}
        /// {"status":"success"}
        /// </returns>
        public async UniTask CreateModelAsync(CreateModelRequest req, IChatCompletionStreamHandler streamHandler)
        {
            req.StreamHandler = streamHandler.SetFactory(client.CreateChunk);
            await client.POSTCreateAsync<CreateModelRequest, ModelStreamResponse>(kEndpointCreateModel, this, req);
        }

        /// <summary>
        /// List models that are available locally.
        /// </summary>
        /// <returns></returns>
        public async UniTask<ListModelsResponse> ListLocalModelsAsync()
        {
            return await client.GETRetrieveAsync<ListModelsResponse>(kEndpointListLocalModels, this);
        }

        // CopyModelAsync
        // Returns a 200 OK if successful, or a 404 Not Found if the source model doesn't exist.
        public async UniTask CopyModelAsync(CopyModelRequest req)
        {
            await client.POSTCreateAsync<CopyModelRequest, ModelStreamResponse>(kEndpointCopyModel, this, req);
        }

        // DeleteModelAsync
        // Returns a 200 OK if successful, 404 Not Found if the model to be deleted doesn't exist.
        public async UniTask<bool> DeleteModelAsync(DeleteModelRequest req)
        {
            // Implement DELETE with body to the top level RESTClient 
            return await client.DELETEDeleteAsync(kEndpointDeleteModel, this, req);
        }

        // PullModelAsync
        // If stream is not specified, or set to true, a stream of JSON objects is returned
        public async UniTask PullModelAsync(PushPullModelRequest req)
        {
            await client.POSTCreateAsync<PushPullModelRequest, ModelStreamResponse>(kEndpointPullModel, this, req);
        }

        // PushModelAsync
        // If stream is not specified, or set to true, a stream of JSON objects is returned
        public async UniTask PushModelAsync(PushPullModelRequest req)
        {
            await client.POSTCreateAsync<PushPullModelRequest, ModelStreamResponse>(kEndpointPushModel, this, req);
        }

        /// <summary>
        /// List models that are currently loaded into memory.
        /// </summary>
        /// <returns></returns>
        public async UniTask<ListModelsResponse> ListRunningModelsAsync()
        {
            return await client.GETRetrieveAsync<ListModelsResponse>(kEndpointListRunningModels, this);
        }
    }
}