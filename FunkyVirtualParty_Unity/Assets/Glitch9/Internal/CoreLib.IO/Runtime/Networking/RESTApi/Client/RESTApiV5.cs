using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Utility class for sending REST API requests and handling responses.
    /// This class doesn't have 'Util' suffix since it's more important than <see cref="RESTClient">RESTClient</see> itself.
    /// </summary>
    public static class RESTApiV5
    {
        #region File History

        /// v3.1 업데이트: 2023-12-17 @Munchkin
        /// - UniTask를 사용하도록 변경
        /// - 에러 핸들링 로직 변경
        /// 
        /// v3.2 업데이트: 2024-02-19 @Munchkin
        /// - GetAuthHeaderFieldName() 추가
        /// - Timeout 로직 변경
        /// - 최소 Delay값 확인 추가
        /// 
        /// v3.3 업데이트: 2024-02-28 @Munchkin
        /// - TErr 추가 (에러오브젝트 기능)
        ///
        /// v4.0 업데이트: 2024-05-28 @Munchkin
        /// - 인터넷 연결 체크 함수 이동 (NetworkUtils.cs)
        /// - 더이상 Exception을 catch하지않고 throw만 함
        ///
        /// v4.1 업데이트: 2024-06-05 @Munchkin
        /// - Logger 추가 (RESTLogger.cs)
        /// 
        /// v4.3 업데이트: 2024-06-20 @Munchkin
        /// - TimeSpan Timeout 추가
        /// 
        /// v5.0 업데이트: 2025-04-01 @Munchkin
        /// - Validate() 추가
        /// 
        /// v5.1 업데이트: 2025-04-15 @Munchkin
        /// - PathParameter, QueryParameter 추가
        /// 
        /// v5.2 대규모 래팩토링: 2025-04-25 @Munchkin
        /// - RESTResquest<TBody>의 리팩토링
        /// - Request와 Response의 Body의 유무에 따라 메소드를 나누어 처리하도록 변경 

        #endregion 

        internal static class Config
        {
            internal const int kNetworkCheckIntervalInMillis = 1000;
            internal const int kNetworkCheckTimeoutInMillis = 5000;
            internal const int kMinOperationDelayInMillis = 100;
            internal const int kMinUnityWebRequestDelayInSec = 2;
            internal static readonly RESTLogLevel kDefaultLogLevel = RESTLogLevel.RequestEndpoint | RESTLogLevel.ResponseBody;
            internal static readonly TimeSpan kDefaultTimeout = TimeSpan.FromSeconds(30);
            internal const string kPatchMethod = "PATCH";
        }

        // Sends request with request body and response body.
        // This CAN be a stream response.
        internal static async UniTask<RESTResponse<TResBody>> SendRequestAsync<TReqBody, TResBody>(RESTRequest<TReqBody> request, string method, RESTClient client)
        {
            // Step 1. Validate request ------------------------------------------------------------------------- 
            RESTApiUtil.ValidateRequest(request, client);

            // Step 1.1. Resolve endpoint with Path&Query Parameters ------------------------------------------------  
            string endpoint = RESTApiUtil.FinalizedEndpoint(request, client);
            request.Endpoint = endpoint;
            if (!request.IgnoreLogs) client.Logger.ReqEndpoint(method, endpoint);

            // Step 1.2. Check network connection ---------------------------------------------------------------- 
            await NetworkUtil.CheckNetworkAsync(Config.kNetworkCheckIntervalInMillis, Config.kNetworkCheckTimeoutInMillis);

            // Step 1.3. Create UnityWebRequest --------------------------------------------------------------------- 
            using UnityWebRequest webReq = await UnityWebRequestFactory.CreateAsync(request, method, client) ?? throw new Issue(ExceptionType.InvalidRequest, "UnityWebRequest is null.");

            // Step 2. Send request -----------------------------------------------------------------------------  
            RESTResponse<TResBody> response = await UnityWebRequestHandler
                .SendUnityWebRequestAsync<TResBody>(webReq, request.RetryDelayInSec, request.MaxRetry, client, request.Token);

            // Step 3. Detect content type from the response ------------------------------------------------------  
            string responseContentType = RESTApiUtil.GetResponseContentType(webReq, client, request.IgnoreLogs);

            // Step 4. Handle 'Stream' response (if it's stream) ---------------------------------------------------  
            if (request.IsStreamRequest)
            {
                // If it's a stream, everything is handled within the SendAndProcessRequest method
                if (!request.IgnoreLogs) client.Logger.Stream("Stream has ended.");
                return RESTResponse<TResBody>.StreamDoneT(); // Let the caller know that the stream has ended
            }

            // Step 5. Handle non-stream response ------------------------------------------------------------------------ 
            if (webReq.downloadHandler == null) throw new Issue(ExceptionType.EmptyResponse, "DownloadHandler is null.");
            client.Logger.ResVerbose($"Received response from {endpoint}");

            // Response with response body is always in a text format?

            // 하지만 예외적으로:
            // 이미지나 PDF를 반환하는 경우 application/octet-stream, application/pdf, image/png 등 binary content-type일 수 있음
            // → 이 경우도 "response body"는 있지만 텍스트가 아님

            DataTransferMode returnTransferMode = RESTApiUtil.ResolveDataTransferMode(responseContentType, client);

            if (returnTransferMode == DataTransferMode.Text)
            {
                string textResult = webReq.downloadHandler.text;
                if (string.IsNullOrEmpty(textResult)) throw new Issue(ExceptionType.EmptyResponse, "DownloadHandler text is null or empty.");
                return await TextResponseConverter.ConvertAsync(request, response, textResult, responseContentType, client);
            }

            if (returnTransferMode == DataTransferMode.Binary)
            {
                byte[] binaryResult = webReq.downloadHandler.data;
                if (binaryResult.IsNullOrEmpty()) throw new Issue(ExceptionType.EmptyResponse, "Binary result is null or empty.");
                return await BinaryResponseConverter.ConvertAsync(request, response, binaryResult, responseContentType, client) as RESTResponse<TResBody>;
            }

            throw new Issue(ExceptionType.UnknownError);
        }

        // Sends request with request body and NO response body.
        // This CAN be a stream response.
        internal static async UniTask<RESTResponse> SendRequestAsync<TReqBody>(RESTRequest<TReqBody> request, string method, RESTClient client)
        {
            // Step 1. Validate request ------------------------------------------------------------------------- 
            RESTApiUtil.ValidateRequest(request, client);

            // Step 1.1. Resolve endpoint with Path&Query Parameters ------------------------------------------------  
            string endpoint = RESTApiUtil.FinalizedEndpoint(request, client);
            request.Endpoint = endpoint;
            if (!request.IgnoreLogs) client.Logger.ReqEndpoint(method, endpoint);

            // Step 1.2. Check network connection ---------------------------------------------------------------- 
            await NetworkUtil.CheckNetworkAsync(Config.kNetworkCheckIntervalInMillis, Config.kNetworkCheckTimeoutInMillis);

            // Step 1.3. Create UnityWebRequest --------------------------------------------------------------------- 
            using UnityWebRequest webReq = await UnityWebRequestFactory.CreateAsync(request, method, client) ?? throw new Issue(ExceptionType.InvalidRequest, "UnityWebRequest is null.");

            // Step 2. Send request -----------------------------------------------------------------------------  
            RESTResponse response = await UnityWebRequestHandler
                .SendUnityWebRequestAsync(webReq, request.RetryDelayInSec, request.MaxRetry, client, request.Token);

            // Step 3. Detect content type from the response ------------------------------------------------------  
            string responseContentType = RESTApiUtil.GetResponseContentType(webReq, client, request.IgnoreLogs);

            // Step 4. Handle 'Stream' response (if it's stream) ---------------------------------------------------  
            if (request.IsStreamRequest)
            {
                // If it's a stream, everything is handled within the SendAndProcessRequest method
                if (!request.IgnoreLogs) client.Logger.Stream("Stream has ended.");
                return RESTResponse.StreamDone(); // Let the caller know that the stream has ended
            }

            // Step 5. Handle non-stream response ------------------------------------------------------------------------ 
            if (webReq.downloadHandler == null) throw new Issue(ExceptionType.EmptyResponse, "DownloadHandler is null.");
            if (!request.IgnoreLogs) client.Logger.ResVerbose($"Received response from {endpoint}");

            // If the response has no body and is not a stream,
            // then it typically either has no content at all (e.g., HTTP 204),
            // or it returns a raw binary response (e.g., a file or image).

            //GNDebug.Mark("BOdyText" + webReq.downloadHandler.text);

            byte[] binaryResult = webReq.downloadHandler.data;

            if (!binaryResult.IsNullOrEmpty())
            {
                return await BinaryResponseConverter.ConvertAsync(request, response, binaryResult, responseContentType, client);
            }

            return response; // Return the response object with no body
        }

        // Sends request with NO request body and response body.
        internal static async UniTask<RESTResponse<TResBody>> SendRequestAsync<TResBody>(RESTRequest request, string method, RESTClient client)
        {
            // Step 1. Validate request ------------------------------------------------------------------------- 
            RESTApiUtil.ValidateRequest(request, client);

            // Step 1.1: (Path, Query 처리는 Body 없어도 가능하니까 호출 가능)
            string endpoint = RESTApiUtil.FinalizeEndpoint(request, client);
            request.Endpoint = endpoint;
            if (!request.IgnoreLogs) client.Logger.ReqEndpoint(method, endpoint);

            // Step 1.2: 네트워크 체크
            await NetworkUtil.CheckNetworkAsync(Config.kNetworkCheckIntervalInMillis, Config.kNetworkCheckTimeoutInMillis);

            // Step 1.3: UWR 생성
            using UnityWebRequest webReq = UnityWebRequestFactory.Create(request, method, client) ?? throw new Issue(ExceptionType.InvalidRequest, "UnityWebRequest is null.");

            // Step 2: 요청 전송
            RESTResponse<TResBody> response = await UnityWebRequestHandler
                .SendUnityWebRequestAsync<TResBody>(webReq, request.RetryDelayInSec, request.MaxRetry, client, request.Token);

            // Step 3: Content-Type 확인
            string responseContentType = RESTApiUtil.GetResponseContentType(webReq, client, request.IgnoreLogs);

            // Step 4: (Stream 불가능하므로 생략)

            // Step 5: 응답 처리
            if (webReq.downloadHandler == null) throw new Issue(ExceptionType.EmptyResponse, "DownloadHandler is null.");
            client.Logger.ResVerbose($"Received response from {endpoint}");

            DataTransferMode returnTransferMode = RESTApiUtil.ResolveDataTransferMode(responseContentType, client);

            if (returnTransferMode == DataTransferMode.Text)
            {
                string textResult = webReq.downloadHandler.text;
                if (string.IsNullOrEmpty(textResult)) throw new Issue(ExceptionType.EmptyResponse, "DownloadHandler text is null or empty.");
                return await TextResponseConverter.ConvertAsync(request, response, textResult, responseContentType, client);
            }

            if (returnTransferMode == DataTransferMode.Binary)
            {
                byte[] binaryResult = webReq.downloadHandler.data;
                if (binaryResult.IsNullOrEmpty()) throw new Issue(ExceptionType.EmptyResponse, "Binary result is null or empty.");
                return await BinaryResponseConverter.ConvertAsync(request, response, binaryResult, responseContentType, client) as RESTResponse<TResBody>;
            }

            throw new Issue(ExceptionType.UnknownError);
        }


        // Sends request with NO request body and NO response body.
        // This CANNOT be a stream response. This is usually used for GET, PATCH, DELETE.
        internal static async UniTask<RESTResponse> SendRequestAsync(RESTRequest request, string method, RESTClient client)
        {
            // Step 1. Validate request ------------------------------------------------------------------------- 
            RESTApiUtil.ValidateRequest(request, client);

            string endpoint = RESTApiUtil.FinalizeEndpoint(request, client);
            request.Endpoint = endpoint;
            if (!request.IgnoreLogs) client.Logger.ReqEndpoint(method, endpoint);

            // Step 1.2. Check network connection ---------------------------------------------------------------- 
            await NetworkUtil.CheckNetworkAsync(Config.kNetworkCheckIntervalInMillis, Config.kNetworkCheckTimeoutInMillis);

            // Step 1.3. Create UnityWebRequest --------------------------------------------------------------------- 
            using UnityWebRequest webReq = UnityWebRequestFactory.Create(request, method, client) ?? throw new Issue(ExceptionType.InvalidRequest, "UnityWebRequest is null.");

            // Step 2. Send request -----------------------------------------------------------------------------  
            await UnityWebRequestHandler
                .SendUnityWebRequestAsync(webReq, request.RetryDelayInSec, request.MaxRetry, client, request.Token);

            // Step 3. Detect content type from the response ------------------------------------------------------  
            string responseContentType = RESTApiUtil.GetResponseContentType(webReq, client, request.IgnoreLogs);

            // Step 4. Handle 'Stream' response (if it's stream) ---------------------------------------------------  
            if (request.IsStreamRequest)
            {
                // If it's a stream, everything is handled within the SendAndProcessRequest method
                if (!request.IgnoreLogs) client.Logger.Stream("Stream has ended.");
                return RESTResponse.StreamDone(); // Let the caller know that the stream has ended
            }

            // Step 5. Handle non-stream response ------------------------------------------------------------------------ 
            if (webReq.downloadHandler == null) throw new Issue(ExceptionType.EmptyResponse, "DownloadHandler is null.");
            if (!request.IgnoreLogs) client.Logger.ResVerbose($"Received response from {request.Endpoint}");

            // Response with NO response body and NO request body has no response content at all.
            return new RESTResponse()
            {
                IsSuccess = true,
                IsStreamDone = true,
                OutputPath = request.OutputPath
            };
        }
    }
}