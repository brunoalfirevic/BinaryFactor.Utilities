using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace BinaryFactor.Utilities
{
    public static class TypeExtensions
    {
        public static bool Is<T>(this Type type)
        {
            return type.Is(typeof(T));
        }

        public static bool Is(this Type type, Type testType)
        {
            return testType.IsAssignableFrom(type);
        }

        public static bool IsFiniteWholeNumber(this Type type)
        {
            type = type.UnwrapPossibleNullableType();

            return type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(byte) || type == typeof(sbyte);
        }

        public static bool IsFiniteNonIntegralNumber(this Type type)
        {
            type = type.UnwrapPossibleNullableType();

            return type == typeof(decimal) ||
                   type == typeof(float) ||
                   type == typeof(double);
        }

        public static bool IsFiniteNumber(this Type type)
        {
            return type.IsFiniteWholeNumber() || type.IsFiniteNonIntegralNumber();
        }

        public static bool IsWholeNumber(this Type type)
        {
            return type.IsFiniteWholeNumber() || type.UnwrapPossibleNullableType() == typeof(BigInteger);
        }

        public static bool IsNumber(this Type type)
        {
            return type.IsFiniteNumber() || type.UnwrapPossibleNullableType() == typeof(BigInteger);
        }

        public static object CreateDefaultValue(this Type type)
        {
            if (type.IsValueType && !type.IsNullableValueType())
                return Activator.CreateInstance(type);

            return null;
        }

        public static bool IsNullableValueType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type UnwrapPossibleNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static Type UnwrapPossibleTaskType(this Type type)
        {
            if (type == typeof(Task))
                return typeof(void);

            if (type.IsAssignableToGenericType(typeof(Task<>)))
                return type.GetGenericArguments(typeof(Task<>)).Single();

            return type;
        }

        public static bool CanBeNull(this Type type)
        {
            return !type.IsValueType || type.IsNullableValueType();
        }

        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(string) && type.Is<IEnumerable>();
        }

        public static bool IsGenericEnumerable(this Type type)
        {
            return type != typeof(string) && type.IsAssignableToGenericType(typeof(IEnumerable<>));
        }

        public static Type GetEnumerableItemType(this Type enumerableType)
        {
            return enumerableType.GetGenericArguments(typeof(IEnumerable<>)).Single();
        }

        public static bool IsAssignableToGenericType(this Type type, Type genericType)
        {
            var interfaceTypes = type.GetInterfaces();
            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = type.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        public static IList<Type> GetGenericArguments(this Type type, Type genericTypeDefinition)
        {
            if (genericTypeDefinition.IsInterface)
            {
                var interfaces = type.GetInterfaces().ToList();
                if (type.IsInterface)
                    interfaces.Add(type);

                return interfaces
                    .Single(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition)
                    .GetGenericArguments();
            }
            else
            {
                var hierarhcy = type.Hierarchy();
                foreach (var hierarchyType in hierarhcy)
                {
                    if (hierarchyType.IsGenericType && hierarchyType.GetGenericTypeDefinition() == genericTypeDefinition)
                        return hierarchyType.GetGenericArguments();
                }
            }

            throw new ArgumentException();
        }

        public static IList<Type> Hierarchy(this Type type)
        {
            var result = new List<Type>();
            while (type != null)
            {
                result.Add(type);
                type = type.BaseType;
            }

            return result;
        }
    }
}
