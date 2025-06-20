using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Administration
{
    public partial class AdministrationService
    {
        public ProjectService Project { get; }

        public AdministrationService(OpenAI client)
        {
            Project = new ProjectService(client);
        }
    }
}