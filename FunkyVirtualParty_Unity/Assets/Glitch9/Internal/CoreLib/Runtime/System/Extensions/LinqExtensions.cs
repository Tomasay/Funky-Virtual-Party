using System;
using System.Collections;
using System.Collections.Generic;

namespace Glitch9.Utilities
{
    /// <summary>Various LinQ extensions.</summary>
    public static class LinqExtensions
    {
        /// <summary>Calls an action on each item before yielding them.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="action">The action to call for each item.</param>
        public static IEnumerable<T> Examine<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
            {
                action(obj);
                yield return obj;
            }
        }

        /// <summary>Perform an action on each item.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to perform.</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
                action(obj);
            return source;
        }

        /// <summary>Perform an action on each item.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to perform.</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int num = 0;
            foreach (T obj in source)
                action(obj, num++);
            return source;
        }

        /// <summary>Convert each item in the collection.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="converter">Func to convert the items.</param>
        public static IEnumerable<T> Convert<T>(this IEnumerable source, Func<object, T> converter)
        {
            foreach (object obj in source)
                yield return converter(obj);
        }

        /// <summary>Add an item to the beginning of a collection.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="prepend">Func to create the item to prepend.</param>
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, Func<T> prepend)
        {
            yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>Add an item to the beginning of a collection.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="prepend">The item to prepend.</param>
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, T prepend)
        {
            yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add a collection to the beginning of another collection.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="prepend">The collection to prepend.</param>
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, IEnumerable<T> prepend)
        {
            foreach (T obj in prepend)
                yield return obj;
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">Func to create the item to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
            this IEnumerable<T> source,
            bool condition,
            Func<T> prepend)
        {
            if (condition)
                yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">The item to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
            this IEnumerable<T> source,
            bool condition,
            T prepend)
        {
            if (condition)
                yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add a collection to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">The collection to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
            this IEnumerable<T> source,
            bool condition,
            IEnumerable<T> prepend)
        {
            if (condition)
            {
                foreach (T obj in prepend)
                    yield return obj;
            }
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">Func to create the item to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
            this IEnumerable<T> source,
            Func<bool> condition,
            Func<T> prepend)
        {
            if (condition())
                yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">The item to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
          this IEnumerable<T> source,
          Func<bool> condition,
          T prepend)
        {
            if (condition())
                yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add a collection to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">The collection to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
          this IEnumerable<T> source,
          Func<bool> condition,
          IEnumerable<T> prepend)
        {
            if (condition())
            {
                foreach (T obj in prepend)
                    yield return obj;
            }
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">Func to create the item to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
          this IEnumerable<T> source,
          Func<IEnumerable<T>, bool> condition,
          Func<T> prepend)
        {
            if (condition(source))
                yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">The item to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
          this IEnumerable<T> source,
          Func<IEnumerable<T>, bool> condition,
          T prepend)
        {
            if (condition(source))
                yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>
        /// Add a collection to the beginning of another collection, if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="prepend">The collection to prepend.</param>
        public static IEnumerable<T> PrependIf<T>(
            this IEnumerable<T> source,
            Func<IEnumerable<T>, bool> condition,
            IEnumerable<T> prepend)
        {
            if (condition(source))
            {
                foreach (T obj in prepend)
                    yield return obj;
            }
            foreach (T obj in source)
                yield return obj;
        }

        /// <summary>Add an item to the end of a collection.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="append">Func to create the item to append.</param>
        public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, Func<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            yield return append();
        }

        /// <summary>Add an item to the end of a collection.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="append">The item to append.</param>
        public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, T append)
        {
            foreach (T obj in source)
                yield return obj;
            yield return append;
        }

        /// <summary>Add a collection to the end of another collection.</summary>
        /// <param name="source">The collection.</param>
        /// <param name="append">The collection to append.</param>
        public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, IEnumerable<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            foreach (T obj in append)
                yield return obj;
        }

        /// <summary>
        /// Add an item to the end of a collection if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="append">Func to create the item to append.</param>
        public static IEnumerable<T> AppendIf<T>(
          this IEnumerable<T> source,
          bool condition,
          Func<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition)
                yield return append();
        }

        /// <summary>
        /// Add an item to the end of a collection if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="append">The item to append.</param>
        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, bool condition, T append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition)
                yield return append;
        }

        /// <summary>
        /// Add a collection to the end of another collection if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="append">The collection to append.</param>
        public static IEnumerable<T> AppendIf<T>(
          this IEnumerable<T> source,
          bool condition,
          IEnumerable<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition)
            {
                foreach (T obj in append)
                    yield return obj;
            }
        }

        /// <summary>
        /// Add an item to the end of a collection if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="append">Func to create the item to append.</param>
        public static IEnumerable<T> AppendIf<T>(
          this IEnumerable<T> source,
          Func<bool> condition,
          Func<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition())
                yield return append();
        }

        /// <summary>
        /// Add an item to the end of a collection if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="append">The item to append.</param>
        public static IEnumerable<T> AppendIf<T>(
            this IEnumerable<T> source,
            Func<bool> condition,
            T append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition())
                yield return append;
        }

        /// <summary>
        /// Add a collection to the end of another collection if a condition is met.
        /// </summary>
        /// <param name="source">The collection.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="append">The collection to append.</param>
        public static IEnumerable<T> AppendIf<T>(
            this IEnumerable<T> source,
            Func<bool> condition,
            IEnumerable<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition())
            {
                foreach (T obj in append)
                    yield return obj;
            }
        }

        /// <summary>
        /// Returns and casts only the items of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="source">The collection.</param>
        public static IEnumerable<T> FilterCast<T>(this IEnumerable source)
        {
            foreach (object obj1 in source)
            {
                if (obj1 is T obj2)
                    yield return obj2;
            }
        }
    }
}
