using System;

namespace Glitch9.Collections
{
    [Serializable]
    public class Database<TData> : ReferencedDictionary<string, TData>
        where TData : class, IData, new()
    {
    }
}