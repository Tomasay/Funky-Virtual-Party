using System;

namespace Glitch9.IO.Networking.RESTApi
{

    public class NoBetaHeaderException : Exception
    {
        public NoBetaHeaderException(string apiName) : base($"{apiName} API requires a beta header to be set.")
        {
        }
    }

    public class NoVersionException : Exception
    {
        public NoVersionException(string apiName) : base($"{apiName} API requires a version to be set.")
        {
        }
    }

    public class NoBetaVersionException : Exception
    {
        public NoBetaVersionException(string apiName) : base($"{apiName} API requires a beta version to be set.")
        {
        }
    }
}