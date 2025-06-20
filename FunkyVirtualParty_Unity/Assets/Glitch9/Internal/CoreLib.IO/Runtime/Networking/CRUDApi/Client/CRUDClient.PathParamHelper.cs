using System;
using System.Collections.Generic;

namespace Glitch9.IO.Networking.RESTApi
{
    public abstract partial class CRUDClient<TSelf> where TSelf : CRUDClient<TSelf>
    {
        internal static class PathParamHelper
        {
            internal static (TRequest, IPathParam[]) ConfigurePathParameters<TRequest>(
                CRUDServiceBase<TSelf> service,
                TRequest request,
                IPathParam[] pathParams)
                where TRequest : RESTRequest
            {
                try
                {
                    ThrowIf.ArgumentIsNull(
                        (service, $"Service"),
                        (service.client, $"Client-{typeof(TSelf).Name}"),
                        (request, typeof(TRequest).Name));

                    string apiName = service.client.Name;
                    ThrowIf.IsNullOrEmpty(apiName, "API Name");
                    List<IPathParam> newPathParams = new(pathParams);

                    CRUDParam apiKeyParam = service.client.apiKeyParam;

                    if (apiKeyParam != null && apiKeyParam.Type != ParamType.Unset)
                    {
                        string apiKey = service.CustomApiKey ?? apiKeyParam.GetValue();
                        if (string.IsNullOrEmpty(apiKey)) throw new NoApiKeyException(apiName);

                        if (apiKeyParam.Type == ParamType.Header)
                        {
                            string headerName = apiKeyParam.HeaderName ?? RESTHeader.kDefaultAuthHeaderName;
                            string headerFormat = apiKeyParam.HeaderFormat ?? RESTHeader.kDefaultAuthHeaderFormat;
                            string authHeaderValue = string.Format(headerFormat, apiKey);

                            request.AddHeader(new(headerName, authHeaderValue));
                        }
                        else if (apiKeyParam.Type == ParamType.Query)
                        {
                            string key = apiKeyParam.QueryKey;
                            if (string.IsNullOrEmpty(key)) throw new NoApiKeyQueryKeyException(apiName);
                            newPathParams.Add(PathParam.Query(key, apiKey));
                        }
                    }

                    bool versionQueryParamSet = false;
                    CRUDParam betaVersionParam = service.client.betaVersionParam;

                    if (service.IsBetaService)
                    {
                        if (betaVersionParam != null && betaVersionParam.Type != ParamType.Unset)
                        {
                            string betaVersion = betaVersionParam.GetValue();
                            if (string.IsNullOrEmpty(betaVersion)) throw new NoBetaVersionException(apiName);

                            if (betaVersionParam.Type == ParamType.Header)
                            {
                                if (!service.CustomBetaHeaders.IsNullOrEmpty())
                                {
                                    foreach (RESTHeader header in service.CustomBetaHeaders)
                                    {
                                        request.AddHeader(header);
                                    }
                                }
                                else
                                {
                                    string headerName = betaVersionParam.HeaderName ?? throw new NoBetaHeaderException(apiName);
                                    request.AddHeader(new(headerName, betaVersion));
                                }
                            }
                            else if (betaVersionParam.Type == ParamType.Query)
                            {
                                newPathParams.Add(PathParam.Version(betaVersion));
                                versionQueryParamSet = true;
                            }
                        }
                    }

                    if (!versionQueryParamSet)
                    {
                        CRUDParam versionParam = service.client.versionParam;

                        if (versionParam != null && versionParam.Type == ParamType.Query)
                        {
                            if (string.IsNullOrEmpty(service.client.Version)) throw new NoVersionException(apiName);
                            newPathParams.Add(PathParam.Version(service.client.Version));
                        }
                    }

                    if (!service.client.additionalHeaders.IsNullOrEmpty())
                    {
                        foreach (RESTHeader header in service.client.additionalHeaders)
                        {
                            request.AddHeader(header);
                        }
                    }

                    return (request, newPathParams.ToArray());
                }
                catch (Exception e)
                {
                    service.client.Logger.Error($"Error in AutoParamHelper: {e.Message}");
                    return (request, pathParams);
                }
            }
        }
    }
}