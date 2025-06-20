using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
// ReSharper disable InconsistentNaming

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// A REST client class for handling various types of REST API requests.
    /// </summary>
    public class RESTClient
    {
        // --- Client Properties --------------------------------------------------
        public JsonSerializerSettings JsonSettings => clientSettings.JsonSettings;
        public RESTLogger Logger => clientSettings.Logger;
        public RESTLogLevel LogLevel => Logger.LogLevel;
        public TimeSpan Timeout => clientSettings.Timeout;
        public SSEParser SSEParser => clientSettings.SSEParser;
        public bool AllowBodyWithDELETE => clientSettings.AllowBodyWithDELETE;

        // --- The Settings Object ------------------------------------------------
        protected readonly RESTClientSettings clientSettings;

        // --- Variables ----------------------------------------------------------
        public string LastRequest { get; set; } = "";
        public string LastEndpoint { get; set; } = "";

        /// <summary>
        /// Constructor to initialize RESTClient with optional JSON settings.
        /// </summary>
        /// <param name="jsonSettings">Custom JSON serializer settings.</param>
        /// <param name="sseParser">Custom SSE parser.</param>
        /// <param name="logger">Custom logger.</param>
        public RESTClient(RESTClientSettings clientSettings = null)
            => this.clientSettings = clientSettings ?? new RESTClientSettings();

        /// <summary>
        /// Sends a POST request with a body and no response body.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse> POSTAsync<TReqBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync(request, UnityWebRequest.kHttpVerbPOST, this);

        /// <summary>
        /// Sends a POST request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> POSTAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync<TReqBody, TResBody>(request, UnityWebRequest.kHttpVerbPOST, this);

        public UniTask<RESTResponse<TResBody>> POSTAsync<TResBody>(RESTRequest request)
            => RESTApiV5.SendRequestAsync<TResBody>(request, UnityWebRequest.kHttpVerbPOST, this);

        public UniTask<RESTResponse> POSTAsync(RESTRequest request)
            => RESTApiV5.SendRequestAsync(request, UnityWebRequest.kHttpVerbPOST, this);

        /// <summary>
        /// Sends a PUT request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse> PUTAsync<TReqBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync(request, UnityWebRequest.kHttpVerbPUT, this);

        /// <summary>
        /// Sends a PUT request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> PUTAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync<TReqBody, TResBody>(request, UnityWebRequest.kHttpVerbPUT, this);

        /// <summary>
        /// Sends a GET request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> GETAsync<TResBody>(RESTRequest request)
            => RESTApiV5.SendRequestAsync<TResBody>(request, UnityWebRequest.kHttpVerbGET, this);

        /// <summary>
        /// Sends a GET request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> GETAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync<TReqBody, TResBody>(request, UnityWebRequest.kHttpVerbGET, this);

        /// <summary>
        /// Sends a DELETE request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> DELETEAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request)
             => RESTApiV5.SendRequestAsync<TReqBody, TResBody>(request, UnityWebRequest.kHttpVerbDELETE, this);

        /// <summary>
        /// Sends a DELETE request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse> DELETEAsync<TReqBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync(request, UnityWebRequest.kHttpVerbDELETE, this);

        public UniTask<RESTResponse<TResBody>> DELETEAsync<TResBody>(RESTRequest request)
            => RESTApiV5.SendRequestAsync<TResBody>(request, UnityWebRequest.kHttpVerbDELETE, this);

        public UniTask<RESTResponse> DELETEAsync(RESTRequest request)
            => RESTApiV5.SendRequestAsync(request, UnityWebRequest.kHttpVerbDELETE, this);


        /// <summary>
        /// Sends a HEAD request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse> HEADAsync<TReqBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync(request, UnityWebRequest.kHttpVerbHEAD, this);

        /// <summary>
        /// Sends a HEAD request with a generic request and response type.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> HEADAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync<TReqBody, TResBody>(request, UnityWebRequest.kHttpVerbHEAD, this);

        /// <summary>
        /// Sends a PATCH request with a generic request type and default response and error types.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse> PATCHAsync<TReqBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync(request, RESTApiV5.Config.kPatchMethod, this);

        public UniTask<RESTResponse<TResBody>> PATCHAsync<TResBody>(RESTRequest request)
            => RESTApiV5.SendRequestAsync<TResBody>(request, RESTApiV5.Config.kPatchMethod, this);

        public UniTask<RESTResponse> PATCHAsync(RESTRequest request)
            => RESTApiV5.SendRequestAsync(request, RESTApiV5.Config.kPatchMethod, this);

        /// <summary>
        /// Sends a PATCH request with generic request, response, and error types.
        /// </summary>
        /// <typeparam name="TReqBody">Request body type.</typeparam>
        /// <typeparam name="TResBody">Response body type.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response result.</returns>
        public UniTask<RESTResponse<TResBody>> PATCHAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request)
            => RESTApiV5.SendRequestAsync<TReqBody, TResBody>(request, RESTApiV5.Config.kPatchMethod, this);
    }
}
