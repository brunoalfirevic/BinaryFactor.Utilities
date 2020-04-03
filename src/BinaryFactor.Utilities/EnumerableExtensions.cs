using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryFactor.Utilities
{
    public static class EnumerableExtensions
    {
        #if NETSTANDARD2_0
        
        public static HashSet<T> ToHashSet2<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        #endif

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> enumerable)
        {
            return AsList(enumerable).AsReadOnly();
        }

        public static IReadOnlyList<T> ToReadOnlyListSafe<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return new List<T>().AsReadOnly();

            return AsList(enumerable).AsReadOnly();
        }

        public static IList<T> GetRange<T>(this IEnumerable<T> enumerable, int index, int count)
        {
            return AsList(enumerable).GetRange(index, count);
        }

        public static string JoinBy(this IEnumerable<string> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }

        public static string JoinBy(this IEnumerable<string> enumerable, string separator, int index, int count)
        {
            if (enumerable is string[] strArray)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

                if (count < 0)
                    throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

                if (strArray.Length - index < count)
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

                return string.Join(separator, strArray, index, count);
            }

            return enumerable.GetRange(index, count).JoinBy(separator);
        }

        public static IEnumerable<IList<T>> Batch<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            var batch = new List<T>(batchSize);

            foreach(var item in enumerable)
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
            return source.GroupBy(selector).Select(g => g.First());
        }

        private static List<T> AsList<T>(IEnumerable<T> enumerable)
        {
            return enumerable as List<T> ?? enumerable.ToList();
        }
    }
}
