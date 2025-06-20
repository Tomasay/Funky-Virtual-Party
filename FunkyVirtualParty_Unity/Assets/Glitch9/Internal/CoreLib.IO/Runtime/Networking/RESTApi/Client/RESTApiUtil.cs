using System.IO;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking.RESTApi
{
    internal static class RESTApiUtil
    {
        // Step 1. Validate request 
        // Required for all requests
        internal static void ValidateRequest(RESTRequest request, RESTClient client)
        {
            if (request == null) throw new Issue(ExceptionType.InvalidRequest, "Request is null.");
            if (string.IsNullOrEmpty(request.Endpoint)) throw new Issue(ExceptionType.InvalidEndpoint, "Endpoint is null or empty.");

            //if (request.IgnoreLogs) GNDebug.Pink("Ignoring Logs!!!!!!!");

            client.LastRequest = request.GetType().Name;    // Save the name of the last request type
        }

        // Step 1.1. Resolve endpoint with Path&Query Parameters ------------------------------------------------
        // - Newly added on 2025.04.15.
        // - It works with following two new attributes: [PathParameter] and [QueryParameter]
        // - So use reflection to find all the properties of the request class and use the attributes to resolve the endpoint.
        // Required for requests with body (<TReqBody>)
        internal static string FinalizedEndpoint<TReqBody>(RESTRequest<TReqBody> request, RESTClient client)
        {
            string endpoint = EndpointFormatter.FormatEndpoint(request, client.JsonSettings);
            client.LastEndpoint = endpoint;         // Save the last endpoint   
            return endpoint; // Update the request with the resolved endpoint   
        }

        internal static string FinalizeEndpoint(RESTRequest request, RESTClient client)
        {
            client.LastEndpoint = request.Endpoint;         // Save the last endpoint   
            return request.Endpoint; // Update the request with the resolved endpoint   
        }

        internal static DataTransferMode ResolveDataTransferMode(string contentType, RESTClient client)
        {
            if (contentType.Contains("application/json") ||
                    contentType.Contains("text/") ||
                    contentType.Contains("application/problem+json"))
            {
                return DataTransferMode.Text;
            }
            else if (contentType.Contains("application/octet-stream") ||
                     contentType.Contains("audio/") ||
                     contentType.Contains("image/"))
            {
                return DataTransferMode.Binary;
            }

            client.Logger.Warning("Unknown content-type, falling back to Text.");
            return DataTransferMode.Text;
        }

        internal static string ResolveOutputPath(string outputPath, AudioType audioType) => ResolveOutputPath(outputPath, audioType.GetExtension());

        // Outputpath가 존재한다면, 파일을 저장하고 싶다는 의미
        // temp폴더가 아니라 지정된 outputPath에 저장하도록 한다.
        internal static string ResolveOutputPath(string outputPath, string extension)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = Path.Combine(Application.persistentDataPath, Path.GetRandomFileName() + extension);
            }
            else
            {
                if (!Path.IsPathRooted(outputPath))
                    outputPath = Path.Combine(Application.persistentDataPath, outputPath);

                // if (!outputPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                //     outputPath += extension;
                outputPath = Path.ChangeExtension(outputPath, extension);
            }

            // 디렉토리 보장
            string directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return outputPath;
        }

        /*    // Step 3. Detect content type from the response ------------------------------------------------------  
            string responseContentType = webReq.GetResponseHeader("Content-Type")?.ToLowerInvariant();
            if (string.IsNullOrEmpty(responseContentType)) throw new Issue(ExceptionType.EmptyResponse, "Response Content-Type is null or empty.");
            else if (!request.IgnoreLogs) client.Logger.LogResponseContentType(responseContentType);*/

        internal static string GetResponseContentType(UnityWebRequest webReq, RESTClient client, bool ignoreLogs)
        {
            string responseContentType = webReq.GetResponseHeader("Content-Type")?.ToLowerInvariant();
            if (string.IsNullOrEmpty(responseContentType)) throw new Issue(ExceptionType.EmptyResponse, "Response Content-Type is null or empty.");
            else if (!ignoreLogs) client.Logger.ResContentType(responseContentType);
            return responseContentType;
        }
    }
}