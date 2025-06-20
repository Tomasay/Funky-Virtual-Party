using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Glitch9
{
    /// <summary>
    /// Validate objects and throw exceptions when a condition is not met.
    /// </summary>
    public class ThrowIf
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the value is null.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ArgumentIsNull([NotNull] object value, string paramName)
        {
            if (value == null) throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the value is null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ArgumentIsNull<T>([NotNull] T value)
        {
            if (value != null) return;
            string paramName = typeof(T).Name;
            throw new ArgumentNullException(paramName);
        }

        public static void ArgumentIsNull(params (object value, string name)[] values)
        {
            foreach ((object value, string name) pair in values)
            {
                if (pair.value == null)
                {
                    throw new ArgumentNullException(pair.name);
                }
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the string is null or empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void IsNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value)) throw new EmptyStringException(paramName);
        }

        public static void IsNullOrEmpty<T>(ICollection<T> value, string paramName)
        {
            if (value == null || value.Count == 0) throw new EmptyStringException(paramName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the string is null or whitespace.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void IsNullOrWhitespace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new EmptyStringException(paramName);
        }

        public static void IsDefault<T>(T value, string paramName)
        {
            if (value.Equals(default(T))) throw new ArgumentNullException(paramName);
        }

        public static void IsLessThanZero(int value, string paramName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(paramName);
        }

        public static void IsLessThanZero(float value, string paramName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(paramName);
        }

        public static void IsLessThanZero(double value, string paramName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(paramName);
        }

        public static void IsLessThanZero(decimal value, string paramName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(paramName);
        }

        public static void IsLessThanZero(long value, string paramName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(paramName);
        }


        private const string kEndpoint = "Endpoint";
        public static void EndpointIsNull([NotNull] string value, string endpointName = kEndpoint)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(endpointName);
        }

        public static void ResultIsNull<T>(T result)
        {
            if (result == null) throw new InvalidOperationException($"Result: '{typeof(T).Name}' is null.");
        }

        private const string kDefaultCurrencyName = "Funds";
        public static void InsufficientCurrency(float balance, float amount, string currencyName = kDefaultCurrencyName)
        {
            if (balance < amount)
            {
                string message = $"Insufficient {currencyName.ToLower()}.";
                throw new InvalidOperationException(message);
            }
        }

        private const string kDefaultCollectionName = "Collection";
        private const string kDefaultArrayName = "Array";

        public static void CollectionIsNullOrEmpty<T>(ICollection<T> collection, string collectionName = kDefaultCollectionName)
        {
            if (collection == null || collection.Count == 0) throw new ArgumentNullException(collectionName);
        }

        public static void ListIsNullOrEmpty<T>(IList<T> array, string arrayName = kDefaultArrayName)
        {
            if (array == null || array.Count == 0) throw new ArgumentNullException(arrayName);
        }
    }
}