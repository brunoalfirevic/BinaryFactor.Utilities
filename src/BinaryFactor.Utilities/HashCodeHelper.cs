using System;
using System.Collections.Generic;

namespace BinaryFactor.Utilities
{
    public static class HashCodeHelper
    {
        public static int Combine<T>(IEnumerable<T> values)
        {
            unchecked
            {
                var hash = Combine(17, RandomSeed);

                foreach (var value in values)
                    hash = Combine(hash, value?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static int CombineUnordered<T>(IEnumerable<T> values)
        {
            var hash = RandomSeed;

            foreach (var item in values)
                hash = CombineUnordered(hash, item?.GetHashCode() ?? 0);

            return hash;
        }

        private static int Combine(int h1, int h2) => h1 * 23 + h2;

        private static int CombineUnordered(int h1, int h2) => h1 ^ h2;

        private static readonly int RandomSeed = Guid.NewGuid().GetHashCode();
    }
}
