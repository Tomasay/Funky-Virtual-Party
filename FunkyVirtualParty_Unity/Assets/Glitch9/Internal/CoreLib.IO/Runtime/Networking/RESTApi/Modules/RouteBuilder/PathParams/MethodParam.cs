namespace Glitch9.IO.Networking.RESTApi
{
    public class MethodParam : IPathParam
    {
        public string method;

        public MethodParam(string method)
        {
            this.method = method;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(method);
        }

        public override string ToString()
        {
            return $":{method}";
        }
    }
}