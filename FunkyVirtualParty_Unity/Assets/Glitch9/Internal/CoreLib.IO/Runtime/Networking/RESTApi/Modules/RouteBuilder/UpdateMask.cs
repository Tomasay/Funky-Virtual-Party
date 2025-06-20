using System.Collections.Generic;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// This is a comma-separated list of fully qualified names of fields. Example: "user.displayName,photo".
    /// </summary>
    public struct UpdateMask
    {
        public string key;
        public string value;

        public UpdateMask(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public override readonly string ToString()
        {
            return $"{key},{value}";
        }
    }

    public static class UpdateMaskExtensions
    {
        public static IPathParam[] ToPathParams(this IEnumerable<UpdateMask> updateMasks, params IPathParam[] additionalParams)
        {
            const string QUERY_KEY = "updateMask";

            List<IPathParam> pathParams = new(additionalParams);

            foreach (UpdateMask updateMask in updateMasks)
            {
                pathParams.Add(PathParam.Query(QUERY_KEY, updateMask.ToString()));
            }

            return pathParams.ToArray();
        }
    }
}