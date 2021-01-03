// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    #nullable enable

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    #pragma warning disable CS0436 // Type conflicts with imported type
    using NotNullIfNotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
    #pragma warning restore CS0436 // Type conflicts with imported type

    public interface IOptional
    {
        bool HasValue { get; }

        object Value { get; }

        Type UnderlyingType { get; }
    }

    public static class Optional
    {
        public static Optional<T> Of<T>(T? value)
            where T : notnull
        {
            return value == null
                ? new Optional<T>()
                : new Optional<T>(value);
        }

        public static Optional<T> Of<T>(T? value)
            where T : struct
        {
            return value == null
                ? new Optional<T>()
                : new Optional<T>(value.Value);
        }

        public static Optional<T> Empty<T>() where T : notnull
        {
            return default;
        }
    }

    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public struct Optional<T> : IOptional, IEquatable<Optional<T>>, IComparable<Optional<T>>, IComparable
        where T: notnull
    {
        private readonly T? value;

        internal Optional(T? value)
        {
            this.value = value;
            HasValue = value != null;
        }

        public bool HasValue { get; }

        public T Value => HasValue ? this.value! : throw new InvalidOperationException();

        object IOptional.Value => Value;

        public Type UnderlyingType => typeof(T);

        public T? GetValueOrDefault() => HasValue ? this.value! : default;

        [return: NotNullIfNotNull("defaultValue")]
        public T? GetValueOrDefault(T? defaultValue) => HasValue ? this.value! : defaultValue;

        public T GetValueOrInvoke(Func<T> defaultValueGetter) => HasValue ? this.value! : defaultValueGetter();

        public Optional<T> Or(Optional<T> other) => HasValue ? this : other;

        public Optional<T> Or(T? other) => HasValue ? this : Optional.Of(other);

        public Optional<T> OrInvoke(Func<Optional<T>> otherGetter) => HasValue ? this : otherGetter();

        public Optional<T> OrInvoke(Func<T?> otherGetter) => HasValue ? this : Optional.Of(otherGetter());

        public override string ToString() => HasValue ? $"{nameof(Optional)}({Value})" : $"{nameof(Optional)}.{nameof(Optional.Empty)}";

        public override int GetHashCode() => this.value?.GetHashCode() ?? 0;

        public override bool Equals(object obj) => obj is Optional<T> optional && Equals(optional);

        public bool Equals(Optional<T> other)
        {
            if (!HasValue && !other.HasValue)
                return true;

            if (HasValue != other.HasValue)
                return false;

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public int CompareTo(object obj) => obj is Optional<T> optional ? CompareTo(optional) : 1;

        public int CompareTo(Optional<T> other)
        {
            if (!HasValue && !other.HasValue)
                return 0;

            if (HasValue && !other.HasValue)
                return 1;

            if (!HasValue && other.HasValue)
                return -1;

            return Comparer<T>.Default.Compare(this.value!, other.value!);
        }

        public static implicit operator Optional<T>(T? value) => Optional.Of(value);

        public static explicit operator T(Optional<T> optional) => optional.HasValue ? optional.value! : throw new InvalidCastException();

        public static bool operator >(Optional<T> left, Optional<T> right) => left.CompareTo(right) > 0;
        public static bool operator >(Optional<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value!, right) > 0;
        public static bool operator >(T left, Optional<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value!) > 0;

        public static bool operator <(Optional<T> left, Optional<T> right) => left.CompareTo(right) < 0;
        public static bool operator <(Optional<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value!, right) < 0;
        public static bool operator <(T left, Optional<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value!) < 0;

        public static bool operator >=(Optional<T> left, Optional<T> right) => left.CompareTo(right) >= 0;
        public static bool operator >=(Optional<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value!, right) >= 0;
        public static bool operator >=(T left, Optional<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value!) >= 0;

        public static bool operator <=(Optional<T> left, Optional<T> right) => left.CompareTo(right) <= 0;
        public static bool operator <=(Optional<T> left, T right) => left.HasValue && Comparer<T>.Default.Compare(left.value!, right) <= 0;
        public static bool operator <=(T left, Optional<T> right) => right.HasValue && Comparer<T>.Default.Compare(left, right.value!) <= 0;

        public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);
        public static bool operator ==(Optional<T> left, T right) => left.HasValue && EqualityComparer<T>.Default.Equals(left.value!, right);
        public static bool operator ==(T left, Optional<T> right) => right.HasValue && EqualityComparer<T>.Default.Equals(left, right.value!);

        public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);
        public static bool operator !=(Optional<T> left, T right) => !(left.HasValue && EqualityComparer<T>.Default.Equals(left.value!, right));
        public static bool operator !=(T left, Optional<T> right) => !(right.HasValue && EqualityComparer<T>.Default.Equals(left, right.value!));

        public Optional<T> Where(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return HasValue && predicate(this.value!) ? this : Optional.Empty<T>();
        }

        public Optional<TResult> Select<TResult>(Func<T, TResult?> selector)
            where TResult: notnull
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return HasValue ? Optional.Of(selector(this.value!)) : Optional.Empty<TResult>();
        }

        public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> selector)
            where TResult : notnull
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return HasValue ? selector(this.value!) : Optional.Empty<TResult>();
        }

        public Optional<TResult> SelectMany<TOther, TResult>(Func<T, Optional<TOther>> otherSelector, Func<T, TOther, TResult?> resultSelector)
            where TOther : notnull
            where TResult : notnull
        {
            if (otherSelector == null)
                throw new ArgumentNullException(nameof(otherSelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return SelectMany(value => otherSelector(value).Select(other => resultSelector(value, other)));
        }

        public Optional<TResult> Select<TResult>(Func<T, TResult?> selector)
            where TResult : struct
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return HasValue ? Optional.Of(selector(this.value!)) : Optional.Empty<TResult>();
        }

        public Optional<TResult> SelectMany<TOther, TResult>(Func<T, Optional<TOther>> otherSelector, Func<T, TOther, TResult?> resultSelector)
            where TOther : notnull
            where TResult : struct
        {
            if (otherSelector == null)
                throw new ArgumentNullException(nameof(otherSelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return SelectMany(value => otherSelector(value).Select(other => resultSelector(value, other)));
        }
    }

    public static class OptionalExtensions
    {
        public static T? GetValueOrNull<T>(this Optional<T> optional)
            where T : struct
        {
            return optional.HasValue ? optional.Value : null;
        }

        public static T? GetValueOrDefault<T>(this Optional<T> optional, T? defaultValue)
          where T : struct
        {
            return optional.HasValue ? optional.Value : defaultValue;
        }

        public static T? GetValueOrInvoke<T>(this Optional<T> optional, Func<T?> defaultValueGetter)
            where T : struct
        {
            return optional.HasValue ? optional.Value : defaultValueGetter();
        }

        public static Optional<T> Or<T>(this Optional<T> optional, T? other)
            where T : struct
        {
            return optional.HasValue ? optional : Optional.Of(other);
        }

        public static Optional<T> OrInvoke<T>(this Optional<T> optional, Func<T?> otherGetter)
           where T : struct
        {
            return optional.HasValue ? optional : Optional.Of(otherGetter());
        }
    }

    #nullable restore
}
