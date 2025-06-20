using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using System;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Partial Client class of OpenAIClient for Image requests. (Image Creation / Editing / Variation)
    /// Those requests have slightly different formats.
    /// </summary>
    public class ImageService : CRUDServiceBase<OpenAI>
    {
        public const string kBaseUrl = "{ver}/images";
        public const string kGenerationEndpoint = kBaseUrl + "/generations";
        public const string kEditsEndpoint = kBaseUrl + "/edits";
        public const string kVariationsEndpoint = kBaseUrl + "/variations";

        public ImageService(OpenAI client) : base(client) { }


        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public UniTask<GeneratedImage> CreateAsync(ImageCreationRequest req) => CreateAsyncINTERNAL(req);

        /// <summary>
        /// Creates an edited or extended image given an original image and a prompt.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public UniTask<GeneratedImage> EditAsync(ImageEditRequest req) => EditAsyncINTERNAL(req);

        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public UniTask<GeneratedImage> CreateVariationAsync(ImageVariationRequest req) => CreateVariationINTERNAL(req);


        #region Request Handlers

        private async UniTask<GeneratedImage> CreateAsyncINTERNAL(ImageCreationRequest req)
        {
            ValidateRequest(req);

            if (req.Model == null)
            {
                req.Model = OpenAISettings.DefaultIMG;
            }
            else if (req.Model == AIDevKitConfig.ID_GPT_Image_1)
            {
                req.ResponseFormat = null;
            }

            req.OutputPath ??= OutputPathResolver.ResolveOutputFileName(req.Model, MIMEType.PNG);

            QueryResponse<Image> res = await client.POSTCreateAsync<ImageCreationRequest, QueryResponse<Image>>(kGenerationEndpoint, this, req);
            ThrowIf.ListIsNullOrEmpty(res?.Data, nameof(res));

            return await res!.Data.ToGeneratedImageAsync(req.OutputPath, req.ResponseFormat);
        }

        private async UniTask<GeneratedImage> EditAsyncINTERNAL(ImageEditRequest req)
        {
            ValidateRequest(req);

            if (req.Model == null)
            {
                req.Model = OpenAISettings.DefaultIMG;
            }
            else if (req.Model == AIDevKitConfig.ID_GPT_Image_1)
            {
                req.ResponseFormat = null;
            }

            req.OutputPath ??= OutputPathResolver.ResolveOutputFileName(req.Model, MIMEType.PNG);

            QueryResponse<Image> res = await client.POSTCreateAsync<ImageEditRequest, QueryResponse<Image>>(kEditsEndpoint, this, req);
            ThrowIf.ListIsNullOrEmpty(res?.Data, nameof(res));

            return await res!.Data.ToGeneratedImageAsync(req.OutputPath, req.ResponseFormat);
        }

        private async UniTask<GeneratedImage> CreateVariationINTERNAL(ImageVariationRequest req)
        {
            ValidateRequest(req);

            if (req.Model == null)
            {
                req.Model = AIDevKitConfig.ID_DallE2;
            }
            else if (req.Model != AIDevKitConfig.ID_DallE2)
            {
                client.Logger.Warning($"ImageVariationRequest is only supported for Dall-E 2 model. Using Dall-E 2 model instead of {req.Model}.");
                req.Model = AIDevKitConfig.ID_DallE2;
            }

            req.OutputPath ??= OutputPathResolver.ResolveOutputFileName(req.Model, MIMEType.PNG);

            QueryResponse<Image> res = await client.POSTCreateAsync<ImageVariationRequest, QueryResponse<Image>>(kVariationsEndpoint, this, req);
            ThrowIf.ListIsNullOrEmpty(res.Data, nameof(res.Data));

            return await res!.Data.ToGeneratedImageAsync(req.OutputPath, req.ResponseFormat);
        }

        #endregion

        #region Helper Methods

        private void ValidateRequest(ImageCreationRequest req)
        {
            ThrowIfPromptIsNullOrWhitespace(req.Prompt);

        }

        private void ValidateRequest(ImageEditRequest req)
        {
            ThrowIfImageIsNullOrEmpty(req.Image);
            ThrowIfPromptIsNullOrWhitespace(req.Prompt);
        }

        private void ValidateRequest(ImageVariationRequest req)
        {
            if (req.Prompt != null) req.Prompt = null; // Prompt is not allowed for ImageVariationRequest
            ThrowIfImageIsNullOrEmpty(req.Image);
        }

        private void ThrowIfPromptIsNullOrWhitespace(string promptText)
        {
            ThrowIf.IsNullOrWhitespace(promptText, "Prompt");
        }

        private void ThrowIfImageIsNullOrEmpty(File<Texture2D> image)
        {
            if (image == null) throw new ArgumentException("Image (IFile) is null or empty.");
        }

        #endregion
    }
}


