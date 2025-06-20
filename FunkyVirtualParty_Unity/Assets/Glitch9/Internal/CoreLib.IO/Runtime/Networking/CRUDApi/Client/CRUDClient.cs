using Cysharp.Threading.Tasks;
using System;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// REST API client for performing CRUD operations.
    /// </summary>
    public abstract partial class CRUDClient<TSelf> : RESTClient
        where TSelf : CRUDClient<TSelf>
    {
        /// <summary>
        /// The name of the API.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The version of the API.
        /// </summary>
        public string Version => versionParam?.GetValue();

        /// <summary>
        /// The beta version of the API if available.
        /// </summary>
        public string BetaVersion => betaVersionParam?.GetValue();

        /// <summary>
        /// Delegate for handling exceptions that occur during an API request.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        public delegate void ExceptionHandler(string endpoint, Exception exception);

        /// <summary>
        /// Event invoked when an error occurs during an API request.
        /// </summary>
        public ExceptionHandler OnException { get; set; }

        /// <summary>
        /// Special logger for logging CRUD operations.
        /// </summary>
        public CRUDLogger CRUDLogger => (CRUDLogger)Logger;

        public string BaseUrl { get; }

        // Fields 
        private readonly CRUDParam apiKeyParam;
        private readonly CRUDParam versionParam;
        private readonly CRUDParam betaVersionParam;
        private readonly RESTHeader[] additionalHeaders;
        public string GetApiKey() => apiKeyParam?.GetValue();

        // Constructors
        protected CRUDClient(CRUDClientSettings clientSettings) : base(clientSettings)
        {
            if (clientSettings == null) throw new ArgumentNullException(nameof(clientSettings));
            if (string.IsNullOrEmpty(clientSettings.Name)) throw new ArgumentException("API name must be set.", nameof(clientSettings));
            if (string.IsNullOrEmpty(clientSettings.BaseURL)) throw new ArgumentException("Base URL must be set.", nameof(clientSettings));

            Name = clientSettings.Name;
            BaseUrl = clientSettings.BaseURL;
            additionalHeaders = clientSettings.AdditionalHeaders;

            apiKeyParam = clientSettings.ApiKey;
            versionParam = clientSettings.Version;
            betaVersionParam = clientSettings.BetaVersion;
        }


        // Override Methods -------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Parses the error res from the API.
        /// Override this method to customize the error message parsing logic.
        /// </summary>
        /// <param name="errorJson"></param>
        /// <returns>The error message.</returns>
        protected abstract string FormatErrorMessage(string errorJson);

        /// <summary>
        /// Override this method to handle the status of a <see cref="CRUDMethod.Delete"/> operation.
        /// </summary>
        /// <typeparam name="TResBody"></typeparam>
        /// <param name="res"></param>
        /// <returns></returns>
        protected abstract bool IsDeletedPredicate(RESTResponse res);

        // CRUD Operations
        private async UniTask<TResBody> POSTCreateAsyncINTERNAL<TReqBody, TResBody>(RESTRequest<TReqBody> req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            Type reqType = req.GetType();
            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Create(reqType);

            RESTResponse<TResBody> res = await POSTAsync<TReqBody, TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Create, res);
        }

        private async UniTask<RESTResponse> POSTCreateAsyncINTERNAL<TReqBody>(RESTRequest<TReqBody> req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            Type reqType = req.GetType();
            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Create(reqType);

            RESTResponse res = await POSTAsync(req);
            ThrowIf.ResultIsNull(res);

            return res;
        }

        private async UniTask<TResBody> POSTCreateAsyncINTERNAL<TResBody>(RESTRequest req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            Type reqType = req.GetType();
            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Create(reqType);

            RESTResponse<TResBody> res = await POSTAsync<TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Create, res);
        }

        private async UniTask<TResBody> POSTUpdateAsyncINTERNAL<TReqBody, TResBody>(RESTRequest<TReqBody> req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Update(req.GetType());
            RESTResponse<TResBody> res = await POSTAsync<TReqBody, TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Update, res);
        }

        private async UniTask<TResBody> POSTUpdateAsyncINTERNAL<TResBody>(RESTRequest req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Update(typeof(TResBody));
            RESTResponse<TResBody> res = await POSTAsync<TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Update, res);
        }

        private async UniTask<TResBody> PATCHUpdateAsyncINTERNAL<TResBody>(RESTRequest req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Update(typeof(TResBody));
            RESTResponse<TResBody> res = await PATCHAsync<TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Update, res);
        }

        private async UniTask<TResBody> GETRetrieveAsyncINTERNAL<TResBody>(RESTRequest req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Retrieve(typeof(TResBody));
            RESTResponse<TResBody> res = await GETAsync<TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Retrieve, res);
        }

        private async UniTask<bool> DELETEDeleteAsyncINTERNAL<TResBody>(RESTRequest req, string endpoint)
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Delete(typeof(TResBody));
            RESTResponse<TResBody> res = await DELETEAsync<TResBody>(req);
            ThrowIf.ResultIsNull(res);

            return IsDeletedPredicate(res);
        }

        private async UniTask<QueryResponse<TResBody>> GETListAsyncINTERNAL<TQuery, TResBody>(RESTRequest<TQuery> req, string endpoint)
           where TQuery : Query
        {
            ThrowIf.ArgumentIsNull(req);
            ThrowIf.EndpointIsNull(endpoint);
            req.Endpoint = endpoint;

            if (!req.IgnoreLogs && LogLevel.RequestDetails()) CRUDLogger.Query(typeof(TResBody));
            RESTResponse<QueryResponse<TResBody>> res = await GETAsync<QueryResponse<TResBody>>(req);
            ThrowIf.ResultIsNull(res);

            return FinalizeResponse(CRUDMethod.Query, res);
        }

        // --- Utility Methods --------------------------------------------------------------------------------------------------------------

        // Handle ress from API calls, converting them to the appropriate type.
        private TResBody FinalizeResponse<TResBody>(CRUDMethod method, RESTResponse<TResBody> res)
        {
            if (res.IsSuccess) return res.Body;
            HandleErrorLogging(method, res);
            return res.Body;
        }

        private QueryResponse<TResBody> FinalizeResponse<TResBody>(CRUDMethod method, RESTResponse<QueryResponse<TResBody>> res)
        {
            if (res.IsSuccess) return res.Body;
            HandleErrorLogging(method, res);
            return res.Body;
        }

        private void HandleErrorLogging(CRUDMethod method, RESTResponse res)
        {
            string failReason = res.ErrorMessage;

            if (string.IsNullOrEmpty(failReason))
            {
                failReason = "No fail reason provided.";
            }
            else
            {
                failReason = FormatErrorMessage(failReason);
            }

            string failMessage = $"<color=cyan>[{LastRequest}({method})]</color> Error: {failReason}";
            CRUDLogger.Error(failMessage);
        }

        // Handle exceptions from GenerativeAI API calls, potentially parsing error messages.
        public void HandleException(Exception exception)
        {
            //string exceptionMessage = exception.Message; 
            OnException?.Invoke(LastEndpoint, exception);
        }
    }
}