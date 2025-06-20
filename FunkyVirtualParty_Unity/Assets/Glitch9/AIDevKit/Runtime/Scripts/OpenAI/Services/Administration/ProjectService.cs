using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Administration
{
    public partial class AdministrationService
    {
        public class ProjectService : CRUDServiceBase<OpenAI>
        {
            private const string kEndpoint = "{ver}/organization/projects";
            private const string kEndpointWithId = "{ver}/organization/projects/{0}";
            private const string kEndpointArchive = "{ver}/organization/projects/{0}/archive";
            public ProjectService(OpenAI client) : base(client, customApiKey: OpenAISettings.GetAdminApiKey()) { }

            public async UniTask<Project> CreateAsync(string projectName, RequestOptions options = null)
            {
                ThrowIfAdminKeyNotSet();
                NameModel req = new(projectName);
                return await client.POSTCreateAsync<NameModel, Project>(kEndpoint, this, req, options);
            }

            public async UniTask<Project> RetrieveAsync(string projectId, RequestOptions options = null)
            {
                ThrowIfAdminKeyNotSet();
                return await client.GETRetrieveAsync<Project>(kEndpointWithId, this, options, PathParam.ID(projectId));
            }

            public async UniTask<Project> ModifyAsync(string projectId, string projectName, RequestOptions options = null)
            {
                ThrowIfAdminKeyNotSet();
                NameModel req = new(projectName);
                return await client.POSTUpdateAsync<NameModel, Project>(kEndpointWithId, this, req, options, PathParam.ID(projectId));
            }

            public async UniTask<Project> ArchiveAsync(string projectId, RequestOptions options = null)
            {
                ThrowIfAdminKeyNotSet();
                return await client.POSTUpdateAsync<Project>(kEndpointArchive, this, options, PathParam.ID(projectId));
            }

            public async UniTask<QueryResponse<Project>> ListAsync(CursorQuery query = null, RequestOptions options = null)
            {
                ThrowIfAdminKeyNotSet();
                return await client.GETListAsync<CursorQuery, Project>(kEndpoint, this, query, options);
            }

            private void ThrowIfAdminKeyNotSet()
            {
                if (string.IsNullOrWhiteSpace(CustomApiKey))
                    throw new InvalidOperationException("OpenAI Admin API key is not set.");
            }
        }
    }
}
