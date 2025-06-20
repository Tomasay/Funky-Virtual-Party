using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Glitch9.Reflection
{
    public static class ReflectionUtil
    {
        public static T CreateInstance<T>(params object[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                {
                    T instance = Activator.CreateInstance<T>();
                    return instance;
                }
                else
                {
                    Type type = typeof(T);
                    T instance = (T)Activator.CreateInstance(type, args);
                    return instance;
                }
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                return default;
            }
        }

        public static string TypeNameWithNamespace(Type type)
        {
            return string.IsNullOrEmpty(type.Namespace) ? type.Name : $"{type.Namespace}.{type.Name}";
        }

        public static IEnumerable<Type> GetSubclassTypes(Type parent, bool excludeAbstract = true, bool excludeInterface = true, params string[] excludes)
        {
            Type type = parent;
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => type.IsAssignableFrom(p));
            if (excludeAbstract) types = types.Where(p => !p.IsAbstract);
            if (excludeInterface) types = types.Where(p => !p.IsInterface);
            if (excludes != null && excludes.Length > 0)
            {
                types = types.Where(p =>
                {
                    bool isExcluded = false;
                    foreach (string exclude in excludes)
                    {
                        if (p.Name.Search(exclude))
                        {
                            isExcluded = true;
                            break;
                        }
                    }

                    return !isExcluded;
                });
            }

            return types;
        }

        public static IEnumerable<Type> GetSubclassTypesWithAttribute<TAttribute>(Type parent)
        {
            Type type = parent;
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => type.IsAssignableFrom(p) && p.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
            return types;
        }


        public static List<T> InstantiateSubclasses<T>(Action<T> onGet = null, params string[] excludes) where T : class
        {
            List<T> list = new();
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetAllTypes())
                {
                    if (type.IsAbstract || type.IsInterface) continue;
                    if (excludes != null && excludes.Length > 0)
                    {
                        bool isExcluded = false;
                        foreach (string exclude in excludes)
                        {
                            if (type.Name.Search(exclude))
                            {
                                isExcluded = true;
                                break;
                            }
                        }

                        if (isExcluded) continue;
                    }

                    if (type.IsSubclassOf(typeof(T)))
                    {
                        T instance = (T)Activator.CreateInstance(type);
                        list.Add(instance);
                        onGet?.Invoke(instance);
                    }
                }
            }

            return list;
        }

        public static Type FindEnumType(this string enumName)
        {
            Type enumType = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == enumName)
                    {
                        enumType = type;
                        break;
                    }
                }
            }

            if (enumType == null)
            {
                Debug.LogError($"ApiEnumDE {enumName} not found");
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Error", $"ApiEnumDE {enumName} not found", "OK");
#endif
            }

            return enumType;
        }

        public static string[] FindEnumNames(this string enumName)
        {
            Type enumType = FindEnumType(enumName);
            if (enumType == null) return null;

            // find the enum values
            string[] enumValues = Enum.GetNames(enumType);
            if (enumValues.Length == 0)
            {
                Debug.LogError($"ApiEnumDE {enumName} has no values");
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Error", $"ApiEnumDE {enumName} has no values", "OK");
#endif
                return null;
            }

            return enumValues;
        }

        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            ThrowIf.ArgumentIsNull(propertyInfo, nameof(propertyInfo));

            MethodInfo m = propertyInfo.GetGetMethod(true);
            if (m != null && m.IsVirtual)
            {
                return true;
            }

            m = propertyInfo.GetSetMethod(true);
            if (m != null && m.IsVirtual)
            {
                return true;
            }

            return false;
        }

        public static MethodInfo GetBaseDefinition(this PropertyInfo propertyInfo)
        {
            ThrowIf.ArgumentIsNull(propertyInfo, nameof(propertyInfo));

            MethodInfo m = propertyInfo.GetGetMethod(true);
            if (m != null)
            {
                return m.GetBaseDefinition();
            }

            return propertyInfo.GetSetMethod(true)?.GetBaseDefinition();
        }

        public static bool IsPublic(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            if (getMethod != null && getMethod.IsPublic)
            {
                return true;
            }
            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod != null && setMethod.IsPublic)
            {
                return true;
            }

            return false;
        }

        public static bool IsNullable(Type t)
        {
            ThrowIf.ArgumentIsNull(t, nameof(t));

            if (t.IsValueType)
            {
                return IsNullableType(t);
            }

            return true;
        }

        public static bool IsNullableType(Type t)
        {
            ThrowIf.ArgumentIsNull(t, nameof(t));

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type EnsureNotNullableType(Type t)
        {
            return (IsNullableType(t))
                ? Nullable.GetUnderlyingType(t)!
                : t;
        }

        public static Type EnsureNotByRefType(Type t)
        {
            return (t.IsByRef && t.HasElementType)
                ? t.GetElementType()!
                : t;
        }

        public static bool IsGenericDefinition(Type type, Type genericInterfaceDefinition)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            Type t = type.GetGenericTypeDefinition();
            return (t == genericInterfaceDefinition);
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition)
        {
            return ImplementsGenericDefinition(type, genericInterfaceDefinition, out _);
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, [NotNullWhen(true)] out Type implementingType)
        {
            ThrowIf.ArgumentIsNull(type, nameof(type));
            ThrowIf.ArgumentIsNull(genericInterfaceDefinition, nameof(genericInterfaceDefinition));

            if (!genericInterfaceDefinition.IsInterface || !genericInterfaceDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatArg(genericInterfaceDefinition));
            }

            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (Type i in type.GetInterfaces())
            {
                if (i.IsGenericType)
                {
                    Type interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = i;
                        return true;
                    }
                }
            }

            implementingType = null;
            return false;
        }


        /// <summary>
        /// Gets the type of the typed collection's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed collection's items.</returns>
        public static Type GetCollectionItemType(Type type)
        {
            ThrowIf.ArgumentIsNull(type, nameof(type));

            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (ImplementsGenericDefinition(type, typeof(IEnumerable<>), out Type genericListType))
            {
                if (genericListType!.IsGenericTypeDefinition)
                {
                    throw new Exception("Type {0} is not a collection.".FormatArg(type));
                }

                return genericListType!.GetGenericArguments()[0];
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            throw new Exception("Type {0} is not a collection.".FormatArg(type));
        }

        /// <summary>
        /// Gets the member's underlying type.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The underlying type of the member.</returns>
        public static Type GetMemberUnderlyingType(MemberInfo member)
        {
            ThrowIf.ArgumentIsNull(member, nameof(member));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType!;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                default:
                    throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo, EventInfo or MethodInfo", nameof(member));
            }
        }

        /// <summary>
        /// Determines whether the property is an indexed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the property is an indexed property; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIndexedProperty(PropertyInfo property)
        {
            ThrowIf.ArgumentIsNull(property, nameof(property));

            return (property.GetIndexParameters().Length > 0);
        }

        /// <summary>
        /// Gets the member's value on the object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target object.</param>
        /// <returns>The member's value on the object.</returns>
        public static object GetMemberValue(MemberInfo member, object target)
        {
            ThrowIf.ArgumentIsNull(member, nameof(member));
            ThrowIf.ArgumentIsNull(target, nameof(target));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(target);
                case MemberTypes.Property:
                    try
                    {
                        return ((PropertyInfo)member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException e)
                    {
                        throw new ArgumentException("MemberInfo '{0}' has index parameters".FormatArg(member.Name), e);
                    }
                default:
                    throw new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo".FormatArg(member.Name), nameof(member));
            }
        }

        /// <summary>
        /// Sets the member's value on the target object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetMemberValue(MemberInfo member, object target, object value)
        {
            ThrowIf.ArgumentIsNull(member, nameof(member));
            ThrowIf.ArgumentIsNull(target, nameof(target));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(target, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(target, value, null);
                    break;
                default:
                    throw new ArgumentException("MemberInfo '{0}' must be of type FieldInfo or PropertyInfo".FormatArg(member.Name), nameof(member));
            }
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be read.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be read.</param>
        /// /// <param name="nonPublic">if set to <c>true</c> then allow the member to be gotten non-publicly.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = (FieldInfo)member;

                    if (nonPublic)
                    {
                        return true;
                    }
                    else if (fieldInfo.IsPublic)
                    {
                        return true;
                    }
                    return false;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = (PropertyInfo)member;

                    if (!propertyInfo.CanRead)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    return (propertyInfo.GetGetMethod(nonPublic) != null);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be set.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be set.</param>
        /// <param name="nonPublic">if set to <c>true</c> then allow the member to be set non-publicly.</param>
        /// <param name="canSetReadOnly">if set to <c>true</c> then allow the member to be set if read-only.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = (FieldInfo)member;

                    if (fieldInfo.IsLiteral)
                    {
                        return false;
                    }
                    if (fieldInfo.IsInitOnly && !canSetReadOnly)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    if (fieldInfo.IsPublic)
                    {
                        return true;
                    }
                    return false;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = (PropertyInfo)member;

                    if (!propertyInfo.CanWrite)
                    {
                        return false;
                    }
                    if (nonPublic)
                    {
                        return true;
                    }
                    return (propertyInfo.GetSetMethod(nonPublic) != null);
                default:
                    return false;
            }
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> targetMembers = new();

            targetMembers.AddRange(GetFields(type, bindingAttr));
            targetMembers.AddRange(GetProperties(type, bindingAttr));

            // for some reason .NET returns multiple members when overriding a generic member on a base class
            // http://social.msdn.microsoft.com/Forums/en-US/b5abbfee-e292-4a64-8907-4e3f0fb90cd9/reflection-overriden-abstract-generic-properties?forum=netfxbcl
            // filter members to only return the override on the topmost class
            // update: I think this is fixed in .NET 3.5 SP1 - leave this in for now...
            List<MemberInfo> distinctMembers = new(targetMembers.Count);

            foreach (IGrouping<string, MemberInfo> groupedMember in targetMembers.GroupBy(m => m.Name))
            {
                int count = groupedMember.Count();

                if (count == 1)
                {
                    distinctMembers.Add(groupedMember.First());
                }
                else
                {
                    List<MemberInfo> resolvedMembers = new();
                    foreach (MemberInfo memberInfo in groupedMember)
                    {
                        // this is a bit hacky
                        // if the hiding property is hiding a base property and it is virtual
                        // then this ensures the derived property gets used
                        if (resolvedMembers.Count == 0)
                        {
                            resolvedMembers.Add(memberInfo);
                        }
                        else if (!IsOverridenGenericMember(memberInfo, bindingAttr) || memberInfo.Name == "Item")
                        {
                            // two members with the same name were declared on a type
                            // this can be done via IL emit, e.g. Moq
                            if (resolvedMembers.Any(m => m.DeclaringType == memberInfo.DeclaringType))
                            {
                                continue;
                            }

                            resolvedMembers.Add(memberInfo);
                        }
                    }

                    distinctMembers.AddRange(resolvedMembers);
                }
            }

            return distinctMembers;
        }

        private static bool IsOverridenGenericMember(MemberInfo memberInfo, BindingFlags bindingAttr)
        {
            if (memberInfo.MemberType != MemberTypes.Property)
            {
                return false;
            }

            PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
            if (!IsVirtual(propertyInfo))
            {
                return false;
            }

            Type declaringType = propertyInfo.DeclaringType!;
            if (!declaringType.IsGenericType)
            {
                return false;
            }
            Type genericTypeDefinition = declaringType.GetGenericTypeDefinition();
            if (genericTypeDefinition == null)
            {
                return false;
            }
            MemberInfo[] members = genericTypeDefinition.GetMember(propertyInfo.Name, bindingAttr);
            if (members.Length == 0)
            {
                return false;
            }
            Type memberUnderlyingType = GetMemberUnderlyingType(members[0]);
            if (!memberUnderlyingType.IsGenericParameter)
            {
                return false;
            }

            return true;
        }

        public static T GetAttribute<T>(object attributeProvider) where T : Attribute
        {
            return GetAttribute<T>(attributeProvider, true);
        }

        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            T[] attributes = GetAttributes<T>(attributeProvider, inherit);

            return attributes?.FirstOrDefault();
        }

        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            Attribute[] a = GetAttributes(attributeProvider, typeof(T), inherit);

            if (a is T[] attributes)
            {
                return attributes;
            }

            return a.Cast<T>().ToArray();
        }

        public static Attribute[] GetAttributes(object attributeProvider, Type attributeType, bool inherit)
        {
            ThrowIf.ArgumentIsNull(attributeProvider, nameof(attributeProvider));

            object provider = attributeProvider;

            // http://hyperthink.net/blog/getcustomattributes-gotcha/
            // ICustomAttributeProvider doesn't do inheritance

            switch (provider)
            {
                case Type t:
                    object[] array = attributeType != null ? t.GetCustomAttributes(attributeType, inherit) : t.GetCustomAttributes(inherit);
                    Attribute[] attributes = array.Cast<Attribute>().ToArray();
                    return attributes;
                case Assembly a:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(a, attributeType) : Attribute.GetCustomAttributes(a);
                case MemberInfo mi:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(mi, attributeType, inherit) : Attribute.GetCustomAttributes(mi, inherit);
                case Module m:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(m, attributeType, inherit) : Attribute.GetCustomAttributes(m, inherit);
                case ParameterInfo p:
                    return (attributeType != null) ? Attribute.GetCustomAttributes(p, attributeType, inherit) : Attribute.GetCustomAttributes(p, inherit);
                default:
                    ICustomAttributeProvider customAttributeProvider = (ICustomAttributeProvider)attributeProvider;
                    object[] result = (attributeType != null) ? customAttributeProvider.GetCustomAttributes(attributeType, inherit) : customAttributeProvider.GetCustomAttributes(inherit);
                    return (Attribute[])result;
            }
        }

        public static MemberInfo GetMemberInfoFromType(Type targetType, MemberInfo memberInfo)
        {
            const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = (PropertyInfo)memberInfo;

                    Type[] types = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();

                    return targetType.GetProperty(propertyInfo.Name, bindingAttr, null, propertyInfo.PropertyType, types, null);
                default:
                    return targetType.GetMember(memberInfo.Name, memberInfo.MemberType, bindingAttr).SingleOrDefault();
            }
        }

        public static IEnumerable<FieldInfo> GetFields(Type targetType, BindingFlags bindingAttr)
        {
            ThrowIf.ArgumentIsNull(targetType, nameof(targetType));

            List<MemberInfo> fieldInfos = new(targetType.GetFields(bindingAttr));
#if !PORTABLE
            // Type.GetFields doesn't return inherited private fields
            // manually find private fields from base class
            GetChildPrivateFields(fieldInfos, targetType, bindingAttr);
#endif

            return fieldInfos.Cast<FieldInfo>();
        }

#if !PORTABLE
        private static void GetChildPrivateFields(IList<MemberInfo> initialFields, Type type, BindingFlags bindingAttr)
        {
            Type targetType = type;

            // fix weirdness with private FieldInfos only being returned for the current Type
            // find base type fields and add them to result
            if ((bindingAttr & BindingFlags.NonPublic) != 0)
            {
                // modify flags to not search for public fields
                BindingFlags nonPublicBindingAttr = bindingAttr.RemoveFlag(BindingFlags.Public);

                while ((targetType = targetType.BaseType) != null)
                {
                    // filter out protected fields
                    IEnumerable<FieldInfo> childPrivateFields =
                        targetType.GetFields(nonPublicBindingAttr).Where(f => f.IsPrivate);

                    initialFields.AddRange(childPrivateFields);
                }
            }
        }
#endif

        public static IEnumerable<PropertyInfo> GetProperties(Type targetType, BindingFlags bindingAttr)
        {
            ThrowIf.ArgumentIsNull(targetType, nameof(targetType));

            List<PropertyInfo> propertyInfos = new(targetType.GetProperties(bindingAttr));

            // GetProperties on an interface doesn't return properties from its interfaces
            if (targetType.IsInterface)
            {
                foreach (Type i in targetType.GetInterfaces())
                {
                    propertyInfos.AddRange(i.GetProperties(bindingAttr));
                }
            }

            GetChildPrivateProperties(propertyInfos, targetType, bindingAttr);

            // a base class private getter/setter will be inaccessible unless the property was gotten from the base class
            for (int i = 0; i < propertyInfos.Count; i++)
            {
                PropertyInfo member = propertyInfos[i];
                if (member.DeclaringType != targetType)
                {
                    PropertyInfo declaredMember = (PropertyInfo)GetMemberInfoFromType(member.DeclaringType!, member)!;
                    propertyInfos[i] = declaredMember;
                }
            }

            return propertyInfos;
        }

        public static BindingFlags RemoveFlag(this BindingFlags bindingAttr, BindingFlags flag)
        {
            return ((bindingAttr & flag) == flag)
                ? bindingAttr ^ flag
                : bindingAttr;
        }

        private static void GetChildPrivateProperties(IList<PropertyInfo> initialProperties, Type type, BindingFlags bindingAttr)
        {
            // fix weirdness with private PropertyInfos only being returned for the current Type
            // find base type properties and add them to result

            // also find base properties that have been hidden by subtype properties with the same name

            Type targetType = type;
            while ((targetType = targetType.BaseType) != null)
            {
                foreach (PropertyInfo propertyInfo in targetType.GetProperties(bindingAttr))
                {
                    PropertyInfo subTypeProperty = propertyInfo;

                    if (!subTypeProperty.IsVirtual())
                    {
                        if (!IsPublic(subTypeProperty))
                        {
                            // have to test on name rather than reference because instances are different
                            // depending on the type that GetProperties was called on
                            int index = initialProperties.IndexOf(p => p.Name == subTypeProperty.Name);
                            if (index == -1)
                            {
                                initialProperties.Add(subTypeProperty);
                            }
                            else
                            {
                                PropertyInfo childProperty = initialProperties[index];
                                // don't replace public child with private base
                                if (!IsPublic(childProperty))
                                {
                                    // replace nonpublic properties for a child, but gotten from
                                    // the parent with the one from the child
                                    // the property gotten from the child will have access to private getter/setter
                                    initialProperties[index] = subTypeProperty;
                                }
                            }
                        }
                        else
                        {
                            int index = initialProperties.IndexOf(p => p.Name == subTypeProperty.Name
                                                                       && p.DeclaringType == subTypeProperty.DeclaringType);

                            if (index == -1)
                            {
                                initialProperties.Add(subTypeProperty);
                            }
                        }
                    }
                    else
                    {
                        Type subTypePropertyDeclaringType = subTypeProperty.GetBaseDefinition()?.DeclaringType ?? subTypeProperty.DeclaringType!;

                        int index = initialProperties.IndexOf(p => p.Name == subTypeProperty.Name
                                                                   && p.IsVirtual()
                                                                   && (p.GetBaseDefinition()?.DeclaringType ?? p.DeclaringType!).IsAssignableFrom(subTypePropertyDeclaringType));

                        // don't add a virtual property that has an override
                        if (index == -1)
                        {
                            initialProperties.Add(subTypeProperty);
                        }
                    }
                }
            }
        }
    }
}




