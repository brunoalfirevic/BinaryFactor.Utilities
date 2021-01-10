// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
#if NETSTANDARD2_0
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
#else
        public static HashSet<T> ToHashSet<T>(IEnumerable<T> source)
#endif
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new HashSet<T>(source);
        }

#if NETSTANDARD2_0
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
#else
        public static IEnumerable<T> Prepend<T>(IEnumerable<T> source, T element)
#endif
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new[] { element }.Concat(source);
        }

#if NETSTANDARD2_0
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
#else
        public static IEnumerable<T> Append<T>(IEnumerable<T> source, T element)
#endif
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Concat(new[] { element });
        }

#if NETSTANDARD2_0
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
#else
        public static IEnumerable<T> SkipLast<T>(IEnumerable<T> source, int count)
#endif
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var queue = new Queue<T>();
            using var e = source.GetEnumerator();

            while (e.MoveNext())
            {
                if (queue.Count == count)
                {
                    do
                    {
                        yield return queue.Dequeue();
                        queue.Enqueue(e.Current);
                    }
                    while (e.MoveNext());

                    break;
                }
                else
                {
                    queue.Enqueue(e.Current);
                }
            }
        }

        public static IEnumerable<T> SkipLastWhile<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var queue = new Queue<T>();
            using var e = source.GetEnumerator();

            while (e.MoveNext())
            {
                queue.Enqueue(e.Current);

                if (!predicate(e.Current))
                {
                    while (queue.Count > 0)
                        yield return queue.Dequeue();
                }
            }
        }

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.ToList().AsReadOnly();
        }

        public static IReadOnlyList<T> ToReadOnlyListSafe<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return new List<T>().AsReadOnly();

            return source.ToList().AsReadOnly();
        }

        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int index, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            using var e = source.GetEnumerator();

            for (var i = 0; i < index; i++)
            {
                if (!e.MoveNext())
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            for (var i = 0; i < count; i++)
            {
                if (!e.MoveNext())
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

                yield return e.Current;
            }
        }

        public static string JoinBy(this IEnumerable<string> values, string separator)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            return string.Join(separator, values);
        }

        public static string JoinBy(this IEnumerable<string> values, string separator, int index, int count)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (values is string[] strArray)
            {
                if (strArray.Length - index < count)
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

                return string.Join(separator, strArray, index, count);
            }

            var list = values as List<string> ?? values.ToList();

            if (list.Count - index < count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            return list.GetRange(index, count).JoinBy(separator);
        }

        public static string JoinNonEmpty(this IEnumerable<string> values, string separator)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            return values.Where(s => !string.IsNullOrEmpty(s)).JoinBy(separator);
        }

        public static IEnumerable<IList<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Number larger than zero required.");

            var batch = new List<T>(batchSize);

            foreach(var item in source)
            {
                batch.Add(item);

                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }

            if (batch.Count > 0)
                yield return batch;
        }

        public static IList<T> ConcatSafe<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            var result = new List<T>();

            if (source != null)
                result.AddRange(source);

            if (items != null)
                result.AddRange(items);

            return result;
        }

        public static IEnumerable<T> DistinctBy<T, V>(this IEnumerable<T> source, Func<T, V> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return source.GroupBy(selector).Select(g => g.First());
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, int, TKey> keySelector, Func<TSource, int, TElement> elementSelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var i = 0;
            var result = new Dictionary<TKey, TElement>();

            foreach (var item in source)
            {
                var key = keySelector(item, i);
                var element = elementSelector(item, i);

                result.Add(key, element);
                i++;
            }

            return result;
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, int, TKey> keySelector, Func<TSource, int, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var i = 0;
            var result = new Dictionary<TKey, TElement>(comparer);

            foreach (var item in source)
            {
                var key = keySelector(item, i);
                var element = elementSelector(item, i);

                result.Add(key, element);
                i++;
            }

            return result;
        }
    }
}
