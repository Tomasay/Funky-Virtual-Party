using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Glitch9
{
    public static class SystemExtensions
    {
        public static void DisposeAll<T>(this List<T> disposables) where T : IDisposable
        {
            if (disposables == null) return;
            foreach (T disposable in disposables) disposable.Dispose();
        }

        public static string ToSenderName(this object sender)
        {
            if (sender == null) return string.Empty;
            if (sender is Type type) return type.Name;
            if (sender is string str) return str;
            return sender.GetType().Name;
        }

        public static int GetHashCodeOrDefault<T>(this T? value) where T : struct
            => value?.GetHashCode() ?? 0;
        public static int GetHashCodeOrDefault(this string value)
            => value?.GetHashCode() ?? 0;
        public static int GetHashCodeOrDefault<T>(this T value) where T : struct
            => value.GetHashCode(); // for non-nullable enums like AIProvider

        /// <summary>
        /// Assembly에서 로드 가능한 모든 Type을 반환하는 안전한 GetTypes 래퍼.
        /// ReflectionTypeLoadException이 발생해도 가능한 Type을 모두 반환한다.
        /// </summary>
        /// <param name="assembly">확인할 Assembly</param>
        /// <returns>가능한 모든 Type 리스트</returns>
        public static Type[] GetAllTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 로드에 실패한 타입 정보를 포함하여 반환 가능한 타입만 필터링
                var validTypes = ex.Types.Where(t => t != null).ToArray();
                //Debug.LogWarning($"Partial load for assembly: {assembly.FullName}");

                // 상세 오류 정보 출력
                // foreach (var loaderException in ex.LoaderExceptions)
                // {
                //     Debug.LogWarning(loaderException.Message);
                // }

                return validTypes;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[Error] Assembly.GetTypes() 실패: {ex.Message}");
                Debug.LogError($"Failed to load types from assembly: {assembly.FullName} - {ex.Message}");
                return Array.Empty<Type>();
            }
        }
    }
}