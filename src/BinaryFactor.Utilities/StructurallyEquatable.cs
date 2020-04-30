using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryFactor.Utilities
{
    public abstract class StructurallyEquatable<T> : StructurallyEquatable, IEquatable<T>
        where T: StructurallyEquatable
    {
        public bool Equals(T other) => Equals((object)other);
    }

    public abstract class StructurallyEquatable
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            var objEqualityItems = ((StructurallyEquatable)obj).EqualsBy();
            var thisEqualityItems = EqualsBy();

            return IsSet(objEqualityItems) && IsSet(thisEqualityItems)
                ? objEqualityItems.ToHashSet().SetEquals(thisEqualityItems)
                : Enumerable.SequenceEqual(objEqualityItems, thisEqualityItems);
        }

        public override int GetHashCode()
        {
            var equalityItems = EqualsBy();

            return IsSet(equalityItems)
                ? HashCodeHelper.CombineUnordered(equalityItems)
                : HashCodeHelper.Combine(equalityItems);
        }

        protected abstract IEnumerable<object> EqualsBy();

        private bool IsSet<T>(IEnumerable<T> enumerable) => enumerable.GetType().IsAssignableToGenericType(typeof(ISet<>));
    }
}
