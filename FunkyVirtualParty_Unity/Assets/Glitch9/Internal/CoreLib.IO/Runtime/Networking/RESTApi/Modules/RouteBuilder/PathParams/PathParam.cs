using System;

namespace Glitch9.IO.Networking.RESTApi
{
    public interface IPathParam
    {
        public bool IsValid();
    }

    public static class PathParam
    {
        public static IPathParam Query(string key, string value)
        {
            return new QueryParam(key, value);
        }

        public static IPathParam SSE()
        {
            return new QueryParam("alt", "sse");
        }

        public static IPathParam Force(bool value)
        {
            return new QueryParam("force", value);
        }

        public static IPathParam ID(params string[] ids)
        {
            if (ids == null || ids.Length == 0) throw new ArgumentException("Path parameter IDs cannot be null or empty.");
            foreach (var id in ids) if (string.IsNullOrEmpty(id)) throw new ArgumentException("Path parameter ID cannot be null or empty.");
            return new IdParam(ids);
        }

        public static IPathParam Method(string method)
        {
            return new MethodParam(method);
        }

        public static IPathParam Version(string version)
        {
            return new VersionParam(version);
        }

        public static IPathParam Child(string childPath)
        {
            return new ChildParam(childPath);
        }
    }
}