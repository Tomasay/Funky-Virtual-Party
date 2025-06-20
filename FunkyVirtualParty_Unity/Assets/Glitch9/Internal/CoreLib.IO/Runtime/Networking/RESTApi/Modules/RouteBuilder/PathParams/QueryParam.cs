namespace Glitch9.IO.Networking.RESTApi
{
    public class QueryParam : IPathParam
    {
        public string key;
        public string value;

        public QueryParam(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public QueryParam(string key, int value)
        {
            this.key = key;
            this.value = value.ToString();
        }

        public QueryParam(string key, bool value)
        {
            this.key = key;
            this.value = value ? "true" : "false";
        }

        public override string ToString()
        {
            return $"?{key}={value}";
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value);
        }
    }
}