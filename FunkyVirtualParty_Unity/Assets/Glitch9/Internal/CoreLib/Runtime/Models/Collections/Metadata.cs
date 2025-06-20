using Glitch9.Collections;
using System;
using System.Collections.Generic;

namespace Glitch9
{
    [Serializable]
    public class Metadata : ReferencedDictionary<string, string>
    {
        public Metadata() { }
        public Metadata(Dictionary<string, string> dictionary)
        {
            if (dictionary == null) return;
            foreach (var kvp in dictionary)
            {
                if (kvp.Key == null || kvp.Value == null) continue;
                Add(kvp.Key, kvp.Value);
            }
        }
    }
}