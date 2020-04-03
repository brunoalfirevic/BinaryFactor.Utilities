// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryFactor.Utilities
{
    public static class LambdaEqualityComparer
    {
        public static LambdaEqualityComparer<T> Create<T>(Func<T, object> compareBySelector, IEqualityComparer<object> comparer = null, bool callSelectorForNullValues = true)
        {
            return new LambdaEqualityComparer<T>(compareBySelector, comparer, callSelectorForNullValues);
        }
    }

    public class LambdaEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
    {
        private readonly Func<T, object> compareBySelector;
        private readonly IEqualityComparer<object> comparer;

        public LambdaEqualityComparer(Func<T, object> compareBySelector, IEqualityComparer<object> comparer = null, bool callSelectorForNullValues = true)
        {
            this.compareBySelector = callSelectorForNullValues
                ? compareBySelector
                : (obj => obj == null ? null : compareBySelector(obj));

            this.comparer = comparer ?? EqualityComparer<object>.Default;
        }

        public bool Equals(T x, T y)
        {
            var compareByX = this.compareBySelector(x);
            var compareByY = this.compareBySelector(y);

            return this.comparer.Equals(compareByX, compareByY);
        }

        public int GetHashCode(T obj)
        {
            return this.compareBySelector(obj)?.GetHashCode() ?? 0;
        }

        public new bool Equals(object x, object y) => Equals((T) x, (T) y);

        public int GetHashCode(object obj) => GetHashCode((T) obj);
    }
}
