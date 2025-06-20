using Glitch9.Collections;
using Glitch9.Reflection;
using System;
using System.Collections.Generic;

namespace Glitch9
{
    public static class AttributeCache<T> where T : Attribute
    {
        private static readonly ThreadSafeMap<object, T> kTypeAttributeCache = new(ReflectionUtil.GetAttribute<T>);

        public static T Get(object type)
        {
            return kTypeAttributeCache.Get(type);
        }

        public static T Get<TType>()
        {
            Type type = typeof(TType);
            return kTypeAttributeCache.Get(type);
        }

        public static Dictionary<object, T> GetDictionary()
        {
            return kTypeAttributeCache.GetDictionary();
        }
    }
}