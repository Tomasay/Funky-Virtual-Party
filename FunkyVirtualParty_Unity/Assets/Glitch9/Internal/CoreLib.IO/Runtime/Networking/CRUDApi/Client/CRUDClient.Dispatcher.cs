using System;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Networking.RESTApi
{
    public abstract partial class CRUDClient<TSelf>
        where TSelf : CRUDClient<TSelf>
    {
        internal async UniTask<TResponse> POSTCreateAsync<TRequestBody, TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            RequestOptions options,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest<TRequestBody> req = new(endpointFormat, body, options);
                (RESTRequest<TRequestBody> req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await POSTCreateAsyncINTERNAL<TRequestBody, TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<TResponse> POSTCreateAsync<TRequestBody, TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            params IPathParam[] pathParams)
        {
            return await POSTCreateAsync<TRequestBody, TResponse>(endpointFormat, service, body, null, pathParams);
        }

        internal async UniTask<RESTResponse> POSTCreateAsync<TRequestBody>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            RequestOptions options,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest<TRequestBody> req = new(endpointFormat, body, options);
                (RESTRequest<TRequestBody> req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await POSTCreateAsyncINTERNAL(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<RESTResponse> POSTCreateAsync<TRequestBody>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            params IPathParam[] pathParams)
        {
            return await POSTCreateAsync(endpointFormat, service, body, null, pathParams);
        }

        internal async UniTask<TResponse> POSTCreateAsync<TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            RequestOptions options = null,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest req = RESTRequest.Temp(endpointFormat, options);
                (RESTRequest req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await POSTCreateAsyncINTERNAL<TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }


        internal async UniTask<TResponse> GETRetrieveAsync<TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            RequestOptions options = null,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest req = RESTRequest.Temp(endpointFormat, options);
                (RESTRequest req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await GETRetrieveAsyncINTERNAL<TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<TResponse> POSTUpdateAsync<TRequestBody, TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            RequestOptions options,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest<TRequestBody> req = new(endpointFormat, body, options);
                (RESTRequest<TRequestBody> req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await POSTUpdateAsyncINTERNAL<TRequestBody, TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<TResponse> POSTUpdateAsync<TRequestBody, TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            params IPathParam[] pathParams)
        {
            return await POSTUpdateAsync<TRequestBody, TResponse>(endpointFormat, service, body, null, pathParams);
        }

        internal async UniTask<TResponse> POSTUpdateAsync<TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            RequestOptions options = null,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest req = RESTRequest.Temp(endpointFormat, options);
                (RESTRequest req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await POSTUpdateAsyncINTERNAL<TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<TResponse> PATCHUpdateAsync<TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            RequestOptions options = null,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest req = RESTRequest.Temp(endpointFormat, options);
                (RESTRequest req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await PATCHUpdateAsyncINTERNAL<TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<bool> DELETEDeleteAsync<TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            RequestOptions options = null,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest req = RESTRequest.Temp(endpointFormat, options);
                (RESTRequest req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await DELETEDeleteAsyncINTERNAL<TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<bool> DELETEDeleteAsync<TRequestBody>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TRequestBody body,
            RequestOptions options = null,
            params IPathParam[] pathParams)
        {
            try
            {
                RESTRequest<TRequestBody> req = new(endpointFormat, body, options);
                (RESTRequest req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await DELETEDeleteAsyncINTERNAL<TRequestBody>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }

        internal async UniTask<QueryResponse<TResponse>> GETListAsync<TQuery, TResponse>(
            string endpointFormat,
            CRUDServiceBase<TSelf> service,
            TQuery query = null,
            RequestOptions options = null,
            params IPathParam[] pathParams) where TQuery : Query
        {
            try
            {
                RESTRequest<TQuery> req = new(endpointFormat, query, options);
                (RESTRequest<TQuery> req, IPathParam[] pathParams) tuple = PathParamHelper.ConfigurePathParameters(service, req, pathParams);

                string endpoint = RouteBuilder.Build(this, endpointFormat, tuple.pathParams);
                return await GETListAsyncINTERNAL<TQuery, TResponse>(tuple.req, endpoint);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default;
            }
        }
    }
}