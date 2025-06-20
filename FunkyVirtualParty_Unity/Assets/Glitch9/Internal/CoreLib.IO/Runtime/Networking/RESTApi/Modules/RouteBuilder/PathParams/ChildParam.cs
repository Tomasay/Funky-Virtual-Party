namespace Glitch9.IO.Networking.RESTApi
{
    public class ChildParam : IPathParam
    {
        public string childPath;

        public ChildParam(string childPath)
        {
            this.childPath = childPath;
        }
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(childPath);
        }
    }
}