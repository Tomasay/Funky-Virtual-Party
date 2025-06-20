using JetBrains.Annotations;
using System;
using System.Linq;

namespace Glitch9
{
    public static class TypeExtensions
    {

        public static bool HasInterface<TInterface>(this Type type)
        {
            return type.GetInterfaces().Any(i => i == typeof(TInterface));
        }

        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).Any();
        }

        public static bool TryGetAttribute<TAttribute>(this Type type, out TAttribute attribute) where TAttribute : Attribute
        {
            attribute = (TAttribute)type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
            return attribute != null;
        }

        public static bool IsNumber(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInteger([CanBeNull] this object value)
        {
            switch (TypeUtil.GetTypeCode(value?.GetType()))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

    }
}