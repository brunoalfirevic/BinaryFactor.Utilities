// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public interface IOptionalNullable
    {
        bool HasValue { get; }

        object Value { get; }

        Type UnderlyingType { get; }
    }

    public static class OptionalNullable
    {
        public static OptionalNullable<T> Of<T>(T value)
        {
            return new OptionalNullable<T>(value);
        }

        public static OptionalNullable<T> Empty<T>()
        {
            return default;
        }
    }

    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public struct OptionalNullable<T> : IOptionalNullable, IEquatable<OptionalNullable<T>>, IComparable<OptionalNullable<T>>, IComparable
    {
        private readonly T value;

        public OptionalNullable(T value)
        {
            this.value = value;
            HasValue = true;
        }

        public bool HasValue { get; }

        public T Value => HasValue ? this.value : throw new InvalidOperationException();

        object IOptionalNullable.Value => Value;

        public Type UnderlyingType => typeof(T);

        public T GetValueOrDefault() => HasValue ? this.value : default;

        public T GetValueOrDefault(T defaultValue) => HasValue ? this.value : defaultValue;

        public T GetValueOrInvoke(Func<T> defaultValueGetter) => HasValue ? this.value : defaultValueGetter();

        public OptionalNullable<T> Or(OptionalNullable<T> other) => HasValue ? this : other;

        public OptionalNullable<T> Or(T other) => HasValue ? this : OptionalNullable.Of(other);

        public OptionalNullable<T> OrInvoke(Func<OptionalNullable<T>> otherGetter) => HasValue ? this : otherGetter();

        public OptionalNullable<T> OrInvoke(Func<T> otherGetter) => HasValue ? this : OptionalNullable.Of(otherGetter());

        public override string ToString() => HasValue ? $"{nameof(OptionalNullable)}({Value})" : $"{nameof(OptionalNullable)}.{nameof(OptionalNullable.Empty)}";

        public override int GetHashCode() => this.value?.GetHashCode() ?? 0;

        public override bool Equals(object obj) => obj is OptionalNullable<T> optional && Equals(optional);

        public bool Equals(OptionalNullable<T> other)
        {
            if (!HasValue && !other.HasValue)
                return true;

            if (HasValue != other.HasValue)
                return false;

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public int CompareTo(object obj) => obj is OptionalNullable<T> optional ? CompareTo(optional) : 1;

        public int CompareTo(OptionalNullable<T> other)
        {
            if (!HasValue && !other.HasValue)
                return 0;

            if (HasValue && !other.HasValue)
                return 1;

            if (!HasValue && other.HasValue)
                return -1;

            return Comparer<T>.Default.Compare(this.value, other.value);
        }

        public static implicit operator OptionalNullable<T>(T value) => new OptionalNullable<T>(value);

        public static explicit operator T(OptionalNullable<T> optional) => optional.HasValue ? optional.Value : throw new InvalidCastException();

        public static bool operator >(OptionalNullable<T> left, OptionalNullable<T> right) => left.CompareTo(right) > 0;
        public static bool operator >(OptionalNullable<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value, right) > 0;
        public static bool operator >(T left, OptionalNullable<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value) > 0;

        public static bool operator <(OptionalNullable<T> left, OptionalNullable<T> right) => left.CompareTo(right) < 0;
        public static bool operator <(OptionalNullable<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value, right) < 0;
        public static bool operator <(T left, OptionalNullable<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value) < 0;

        public static bool operator >=(OptionalNullable<T> left, OptionalNullable<T> right) => left.CompareTo(right) >= 0;
        public static bool operator >=(OptionalNullable<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value, right) >= 0;
        public static bool operator >=(T left, OptionalNullable<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value) >= 0;

        public static bool operator <=(OptionalNullable<T> left, OptionalNullable<T> right) => left.CompareTo(right) <= 0;
        public static bool operator <=(OptionalNullable<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value, right) <= 0;
        public static bool operator <=(T left, OptionalNullable<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value) <= 0;

        public static bool operator ==(OptionalNullable<T> left, OptionalNullable<T> right) => left.Equals(right);
        public static bool operator ==(OptionalNullable<T> left, T right) => left.HasValue && EqualityComparer<T>.Default.Equals(left.value, right);
        public static bool operator ==(T left, OptionalNullable<T> right) => right.HasValue && EqualityComparer<T>.Default.Equals(left, right.value);

        public static bool operator !=(OptionalNullable<T> left, OptionalNullable<T> right) => !left.Equals(right);
        public static bool operator !=(OptionalNullable<T> left, T right) => !(left.HasValue && EqualityComparer<T>.Default.Equals(left.value, right));
        public static bool operator !=(T left, OptionalNullable<T> right) => !(right.HasValue && EqualityComparer<T>.Default.Equals(left, right.value));

        public OptionalNullable<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return HasValue ? OptionalNullable.Of(selector(this.value)) : OptionalNullable.Empty<TResult>();
        }

        public OptionalNullable<TResult> SelectMany<TResult>(Func<T, OptionalNullable<TResult>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return HasValue ? selector(this.value) : OptionalNullable.Empty<TResult>();
        }

        public OptionalNullable<TResult> SelectMany<TOther, TResult>(Func<T, OptionalNullable<TOther>> otherSelector, Func<T, TOther, TResult> resultSelector)
        {
            if (otherSelector == null)
                throw new ArgumentNullException(nameof(otherSelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return SelectMany(value => otherSelector(value).Select(other => resultSelector(value, other)));
        }

        public OptionalNullable<T> Where(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return HasValue && predicate(this.value) ? this : OptionalNullable.Empty<T>();
        }
    }

    public static class OptionalNullableExtensions
    {
        public static T? GetValueOrNull<T>(this OptionalNullable<T> optional)
            where T : struct
        {
            return optional.HasValue ? optional.Value : null;
        }

        public static T? GetValueOrDefault<T>(this OptionalNullable<T> optional, T? defaultValue)
            where T : struct
        {
            return optional.HasValue ? optional.Value : defaultValue;
        }

        public static T? GetValueOrInvoke<T>(this OptionalNullable<T> optional, Func<T?> defaultValueGetter)
            where T : struct
        {
            return optional.HasValue ? optional.Value : defaultValueGetter();
        }

        public static OptionalNullable<T?> Or<T>(this OptionalNullable<T> optional, OptionalNullable<T?> other)
            where T : struct
        {
            return optional.HasValue ? OptionalNullable.Of<T?>(optional.Value) : other;
        }

        public static OptionalNullable<T?> Or<T>(this OptionalNullable<T> optional, T? other)
            where T : struct
        {
            return optional.HasValue ? OptionalNullable.Of<T?>(optional.Value) : OptionalNullable.Of(other);
        }

        public static OptionalNullable<T?> OrInvoke<T>(this OptionalNullable<T> optional, Func<OptionalNullable<T?>> otherGetter)
            where T : struct
        {
            return optional.HasValue ? OptionalNullable.Of<T?>(optional.Value) : otherGetter();
        }

        public static OptionalNullable<T?> OrInvoke<T>(this OptionalNullable<T> optional, Func<T?> otherGetter)
            where T : struct
        {
            return optional.HasValue ? OptionalNullable.Of<T?>(optional.Value) : OptionalNullable.Of(otherGetter());
        }
    }
}
