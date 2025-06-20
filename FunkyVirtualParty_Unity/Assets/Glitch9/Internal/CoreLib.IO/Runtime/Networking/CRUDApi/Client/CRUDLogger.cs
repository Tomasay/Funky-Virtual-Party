using System.Reflection;

namespace Glitch9.IO.Networking.RESTApi
{
    public class CRUDLogger : RESTLogger
    {
        public CRUDLogger(string tag, RESTLogLevel logLevel) : base(tag, logLevel) { }
        public void Create(MemberInfo type) => LogINTERNAL(CRUDMethod.Create.GetMessage(), type);
        public void Update(MemberInfo type) => LogINTERNAL(CRUDMethod.Update.GetMessage(), type);
        public void Retrieve(MemberInfo type) => LogINTERNAL(CRUDMethod.Retrieve.GetMessage(), type);
        public void Delete(MemberInfo type) => LogINTERNAL(CRUDMethod.Delete.GetMessage(), type);
        public void Query(MemberInfo type) => LogINTERNAL(CRUDMethod.Query.GetMessage(), type);
        private void LogINTERNAL(string action, MemberInfo type) => Info($"{action} {type.Name}.");
    }
}