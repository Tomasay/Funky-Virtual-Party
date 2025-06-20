using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Base class for CRUD services, providing common properties and methods.
    /// If the service requires something that does not fit <see cref="CRUDService{TClient, TData, TRequest, Tquery}"/>,
    /// inherit from this class and implement the required methods.
    /// </summary>
    /// <typeparam name="TClient">The REST client type, which must inherit from <see cref="CRUDClient{TClient}"/>.</typeparam>
    public abstract class CRUDServiceBase<TClient> where TClient : CRUDClient<TClient>
    {
        internal readonly TClient client;
        internal readonly bool IsBetaService;
        internal readonly string CustomApiKey;
        internal readonly RESTHeader[] CustomBetaHeaders;

        protected CRUDServiceBase(TClient client, params RESTHeader[] betaHeaders)
        {
            this.client = client;
            CustomBetaHeaders = betaHeaders;
            IsBetaService = !betaHeaders.IsNullOrEmpty();
        }

        protected CRUDServiceBase(TClient client, string customApiKey, params RESTHeader[] betaHeaders)
        {
            this.client = client;
            CustomBetaHeaders = betaHeaders;
            IsBetaService = !betaHeaders.IsNullOrEmpty();
            CustomApiKey = customApiKey;
        }

        protected CRUDServiceBase(TClient client, bool betaService = false, string customApiKey = null)
        {
            this.client = client;
            IsBetaService = betaService;
            CustomApiKey = customApiKey;
        }
    }

    /// <summary>
    /// <c>CRUDService</c> is a generic base class for performing standard CRUD operations against a RESTful API.
    /// It provides methods to create, retrieve, update, delete, and list resources on the server using a specified endpoint.
    /// 
    /// If a specific operation is not needed, use <see cref="object"/> as a placeholder for the corresponding generic type.
    /// For example, if query options are not required, use <c>object</c> for <typeparamref name="Tquery"/>.
    /// </summary>
    /// <typeparam name="TClient">
    /// The REST client type, which must inherit from <see cref="CRUDClient{TClient}"/>.
    /// </typeparam>
    /// <typeparam name="TData">
    /// The type representing the resource data returned from the API.
    /// </typeparam>
    /// <typeparam name="TRequest">
    /// The type used when sending create or update requests to the API.
    /// Use <see cref="object"/> if creation and update operations are not supported.
    /// </typeparam>
    /// <typeparam name="Tquery">
    /// The type used for specifying query parameters when listing resources.
    /// Use <see cref="object"/> if listing is not supported.
    /// </typeparam> 
    public class CRUDService<TClient, TData, TRequest, Tquery> : CRUDServiceBase<TClient>
        where TClient : CRUDClient<TClient>
        where Tquery : Query
    {
        protected readonly string _endpoint;
        protected readonly string _endpointWithId;

        protected CRUDService(string endpoint, TClient client, params RESTHeader[] betaHeaders) : base(client, betaHeaders)
        {
            _endpoint = endpoint;
            _endpointWithId = $"{endpoint}/{{0}}";
        }

        protected CRUDService(string endpoint, TClient client, string customApiKey, params RESTHeader[] betaHeaders) : base(client, customApiKey, betaHeaders)
        {
            _endpoint = endpoint;
            _endpointWithId = $"{endpoint}/{{0}}";
        }

        protected CRUDService(string endpoint, TClient client, bool betaService = false, string customApiKey = null) : base(client, betaService, customApiKey)
        {
            _endpoint = endpoint;
            _endpointWithId = $"{endpoint}/{{0}}";
        }

        /// <summary>
        /// Creates a new resource on the server using the specified request body.
        /// </summary>
        /// <param name="req">The data to send in the creation request.</param>
        /// <param name="options">Optional request options.</param>
        /// <returns>The created resource.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TRequest"/> is set to <see cref="object"/>.</exception>
        public UniTask<TData> CreateAsync(TRequest req, RequestOptions options = null)
        {
            ThrowIfTRequestIsObject(CRUDMethod.Create);
            return client.POSTCreateAsync<TRequest, TData>(_endpoint, this, req, options);
        }

        /// <summary>
        /// Retrieves a resource from the server by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the resource to retrieve.</param>
        /// <param name="options">Optional request options.</param>
        /// <returns>The retrieved resource.</returns>
        public UniTask<TData> RetrieveAsync(string id, RequestOptions options = null)
        {
            return client.GETRetrieveAsync<TData>(_endpointWithId, this, options, PathParam.ID(id));
        }

        /// <summary>
        /// Updates an existing resource on the server using the specified identifier and request body.
        /// </summary>
        /// <param name="id">The identifier of the resource to update.</param>
        /// <param name="req">The data to send in the update request.</param>
        /// <param name="options">Optional request options.</param>
        /// <returns>The updated resource.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TRequest"/> is set to <see cref="object"/>.</exception>
        public UniTask<TData> UpdateAsync(string id, TRequest req, RequestOptions options = null)
        {
            ThrowIfTRequestIsObject(CRUDMethod.Update);
            return client.POSTUpdateAsync<TRequest, TData>(_endpointWithId, this, req, options, PathParam.ID(id));
        }

        /// <summary>
        /// Partially updates an existing resource on the server using the specified identifier and update masks.
        /// </summary>
        /// <param name="id">The identifier of the resource to update.</param>
        /// <param name="updateMasks">The update masks specifying which fields to update.</param>
        /// <param name="options">Optional request options.</param>
        /// <returns>The updated resource.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TRequest"/> is set to <see cref="object"/>.</exception>
        public UniTask<TData> UpdateAsync(string id, IEnumerable<UpdateMask> updateMasks, RequestOptions options = null)
        {
            return client.PATCHUpdateAsync<TData>(_endpointWithId, this, options, updateMasks.ToPathParams(PathParam.ID(id)));
        }

        /// <summary>
        /// Deletes a resource from the server using the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the resource to delete.</param>
        /// <param name="options">Optional request options.</param>
        /// <returns><c>true</c> if the resource was deleted successfully; otherwise, <c>false</c>.</returns>
        public UniTask<bool> DeleteAsync(string id, RequestOptions options = null)
        {
            return client.DELETEDeleteAsync<TData>(_endpointWithId, this, options, PathParam.ID(id));
        }

        /// <summary>
        /// Retrieves a list of resources from the server, optionally filtered or paginated using query options.
        /// </summary>
        /// <param name="query">Optional query options for filtering, sorting, or pagination.</param>
        /// <param name="options">Optional request options.</param>
        /// <returns>A list of resources wrapped in a <see cref="QueryResponse{TData}"/>.</returns>
        public UniTask<QueryResponse<TData>> ListAsync(Tquery query = null, RequestOptions options = null)
        {
            ThrowIfTqueryIsObject();
            return client.GETListAsync<Tquery, TData>(_endpoint, this, query, options);
        }

        private void ThrowIfTRequestIsObject(CRUDMethod method)
        {
            if (typeof(TRequest) == typeof(object))
                throw new InvalidOperationException($"This service does not support {method} operation.");
        }

        private void ThrowIfTqueryIsObject()
        {
            if (typeof(Tquery) == typeof(object))
                throw new InvalidOperationException($"This service does not support listing operation.");
        }
    }
}