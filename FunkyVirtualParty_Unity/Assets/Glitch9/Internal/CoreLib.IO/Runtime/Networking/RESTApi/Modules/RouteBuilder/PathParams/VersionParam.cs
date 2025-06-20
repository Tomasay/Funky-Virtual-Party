namespace Glitch9.IO.Networking.RESTApi
{
    public class VersionParam : IPathParam
    {
        public string version;

        public VersionParam(string version)
        {
            this.version = version;
        }

        public override string ToString()
        {
            return version;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(version);
        }
    }
}