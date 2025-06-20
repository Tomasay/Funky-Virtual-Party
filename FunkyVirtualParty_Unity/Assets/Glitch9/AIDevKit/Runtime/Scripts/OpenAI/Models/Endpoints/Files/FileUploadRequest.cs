using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using UnityEngine;
// ReSharper disable All

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Upload a file that can be used across various endpoints.
    /// The size of all the files uploaded by one organization can be up to 100 GB.
    /// The size of individual files can be a maximum of 512 MB
    /// or 2 million tokens for Assistants.
    /// See the Assistants Tools guide to learn more about the types of files supported.
    /// The Fine-tuning API only supports .jsonl files.
    ///
    /// Please contact us if you need to increase these storage limits.
    /// https://help.openai.com/en/
    /// </summary>
    /// <remarks>
    /// POST https://api.openai.com/v1/files
    /// </remarks>
    public class FileUploadRequest : AIRequest
    {
        /// <summary>
        /// [Required] The File object (not file name) to be uploaded.
        /// </summary>
        [JsonProperty("file")] public IFile File { get; set; }

        /// <summary>
        /// [Required] The intended purpose of the uploaded file. 
        /// Use "fine-tune" for Fine-tuning and "Assistants" for Assistants and Messages.
        /// This allows us to validate the format of the uploaded file is correct for fine-tuning.
        /// </summary>
        /// <remarks>
        /// This has to be  <see cref="string"/> because the request has to be converted to Form Request. <see cref="IO.RESTApi.RequestExtensions"/>
        /// </remarks>
        [JsonProperty("purpose")] public UploadPurpose Purpose { get; set; }

        public class Builder : ModelRequestBuilder<Builder, FileUploadRequest>
        {
            public Builder SetFile(IFile file, UploadPurpose purpose)
            {
                _req.File = file;
                _req.Purpose = purpose;
                return this;
            }

            public override FileUploadRequest Build([CallerFilePath] string sender = "")
            {
                return base.Build(MIMEType.MultipartForm, sender);
            }
        }
    }
}