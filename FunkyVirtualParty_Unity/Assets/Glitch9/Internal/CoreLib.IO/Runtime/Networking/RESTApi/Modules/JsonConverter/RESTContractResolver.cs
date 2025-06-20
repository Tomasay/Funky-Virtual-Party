using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Glitch9.IO.Networking.RESTApi
{
    public class RESTContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);

            return props.Where(p =>
            {
                var propInfo = type.GetProperty(p.PropertyName);
                if (propInfo == null)
                    return true; // propInfo 못 찾았으면 포함시켜 (혹은 false로 제외해도 됨)

                var queryAttr = Attribute.IsDefined(propInfo, typeof(QueryParameterAttribute));
                var pathAttr = Attribute.IsDefined(propInfo, typeof(PathParameterAttribute));

                return !queryAttr && !pathAttr;
            }).ToList();
        }
    }
}