namespace Glitch9.IO.Networking.RESTApi
{
    public abstract class ParamResolver
    {
        public abstract (RESTRequest req, string endpoint) Resolve(RESTRequest req, string endpoint);
    }

    public class PathParamResolver : ParamResolver
    {
        private readonly string _pathParamTemplate;
        public PathParamResolver(string pathParamTemplate)
        {
            _pathParamTemplate = pathParamTemplate;
        }

        public override (RESTRequest req, string endpoint) Resolve(RESTRequest req, string endpoint)
        {
            throw new System.NotImplementedException();
        }
    }
}