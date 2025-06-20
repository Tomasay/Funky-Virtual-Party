using System;
using System.Collections.Generic;
using System.Linq;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Provides methods to manage Firestore document and collection references dynamically.
    /// </summary>
    public static class RouteBuilder
    {
        public const string kVersionParamName = "{ver}";
        private static string AddLastSlash(string text) => text.EndsWith('/') ? text : text + '/';
        private static string RemoveFirstSlash(string text) => text.StartsWith('/') ? text.Substring(1) : text;

        public static string Build<T>(CRUDClient<T> client, string url, params IPathParam[] pathParams) where T : CRUDClient<T>
        {
            // basePath Example: https://generativelanguage.googleapis.com/v1beta/corpora/{0}/documents/{1}
            // basePath without id in the end: https://generativelanguage.googleapis.com/v1beta/corpora/{0}/documents

            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url), "URL cannot be null or empty.");

            string baseUrl = client.BaseUrl;
            ILogger logger = client.Logger;

            List<QueryParam> queryParams = new();
            bool idsAlreadyDefined = false;
            bool methodAlreadyDefined = false;

            baseUrl = AddLastSlash(baseUrl);
            url = RemoveFirstSlash(url);

            if (!pathParams.IsNullOrEmpty())
            {
                foreach (IPathParam pathParam in pathParams)
                {
                    if (pathParam == null) continue;
                    if (!pathParam.IsValid())
                    {
                        logger.Warning($"Invalid path parameter {pathParam} for endpoint {url}. Ignoring.");
                        continue;
                    }

                    if (pathParam is IdParam idParam)
                    {
                        if (idsAlreadyDefined)
                        {
                            logger.Warning($"Ids already defined for endpoint {url}. Ignoring additional ids {string.Join(", ", idParam.ids)}.");
                            continue;
                        }

                        for (int i = 0; i < idParam.ids.Length; i++)
                        {
                            url = url.Replace($"{{{i}}}", idParam.ids[i]);
                        }

                        idsAlreadyDefined = true;
                        continue;
                    }

                    if (pathParam is MethodParam methodParam)
                    {
                        if (methodAlreadyDefined)
                        {
                            logger.Warning($"Method parameter already defined for endpoint {url}. Ignoring additional method parameter {methodParam.method}.");
                            continue;
                        }

                        url += methodParam;
                        methodAlreadyDefined = true;
                        continue;
                    }

                    if (pathParam is QueryParam queryParam)
                    {
                        queryParams.Add(queryParam);
                        continue;
                    }

                    if (pathParam is ChildParam childParam)
                    {
                        url = AddLastSlash(url) + childParam.childPath;
                        continue;
                    }

                    if (pathParam is VersionParam versionParam)
                    {
                        url = url.Replace(kVersionParamName, versionParam.version);
                    }
                }
            }

            if (url.Contains(kVersionParamName))
            {
                throw new ArgumentException($"Version parameter not defined for endpoint {url}.");
            }

            if (queryParams.Count > 0)
            {
                string query = string.Join("&", queryParams.Select(queryParam => $"{queryParam.key}={Uri.EscapeDataString(queryParam.value)}"));
                url = $"{url}?{query}";
            }

            string result = baseUrl + url;
            //Debug.LogError("Endpoint Result: " + result);

            return result;
        }
    }
}
