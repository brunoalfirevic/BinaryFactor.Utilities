// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;

    public static class HashCodeHelper
    {
        public static int Combine<T>(IEnumerable<T> values)
        {
            unchecked
            {
                var hash = CombineHashes(17, RandomSeed);

                foreach (var value in values)
                    hash = CombineHashes(hash, value?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static int CombineUnordered<T>(IEnumerable<T> values)
        {
            var hash = RandomSeed;

            foreach (var item in values)
                hash = CombineHashesUnordered(hash, item?.GetHashCode() ?? 0);

            return hash;
        }

        private static int CombineHashes(int h1, int h2) => h1 * 23 + h2;

        private static int CombineHashesUnordered(int h1, int h2) => h1 ^ h2;

        private static readonly int RandomSeed = Guid.NewGuid().GetHashCode();
    }
}
