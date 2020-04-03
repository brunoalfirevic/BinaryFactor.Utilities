// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

using System;
using System.Numerics;

namespace BinaryFactor.Utilities
{
    public static class ConvertEx
    {
        public static T ChangeType<T>(object value, IFormatProvider formatProvider = null, bool convertDbNullToNull = false)
        {
            return (T) ChangeType(value, typeof(T), formatProvider, convertDbNullToNull);
        }

        public static object ChangeType(object value, Type targetType, IFormatProvider formatProvider = null, bool convertDbNullToNull = false)
        {
            if (convertDbNullToNull && value is DBNull)
                value = null;

            if (value == null)
            {
                if (!targetType.CanBeNull())
                    throw new InvalidOperationException($"Null value cannot be converted to {targetType.FullName}");

                return null;
            }

            targetType = targetType.UnwrapPossibleNullableType();

            if (value.GetType().Is(targetType))
                return value;

            if (targetType.IsEnum)
            {
                if (value is string s)
                    return Enum.Parse(targetType, s, ignoreCase: true);

                if (value.GetType().IsFiniteWholeNumber())
                    return Enum.ToObject(targetType, value);
            }

            if (targetType == typeof(BigInteger))
            {
                if (value is string s)
                    return BigInteger.Parse(s, formatProvider);

                if (value is Enum e)
                {
                    var enumValue = ConvertEnumToUnderlyingType(e);
                    return ConvertNumberToBigInt(enumValue);
                }

                if (value is byte[] byteArray)
                    return new BigInteger(byteArray);

                if (value.GetType().IsFiniteNumber())
                    return ConvertNumberToBigInt(value);
            }

            if (value is BigInteger bigInt)
            {
                if (targetType == typeof(string))
                    return bigInt.ToString(formatProvider);

                if (targetType.IsEnum)
                {
                    var numberValue = ConvertBigIntToNumber(bigInt, Enum.GetUnderlyingType(targetType));
                    return Enum.ToObject(targetType, numberValue);
                }

                if (targetType == typeof(byte[]))
                    return bigInt.ToByteArray();

                if (targetType.IsFiniteNumber())
                    return ConvertBigIntToNumber(bigInt, targetType);
            }

            return Convert.ChangeType(value, targetType, formatProvider);
        }

        private static BigInteger ConvertNumberToBigInt(object number)
        {
            return Type.GetTypeCode(number.GetType()) switch
            {
                TypeCode.Int32 => new BigInteger((int) number),
                TypeCode.UInt32 => new BigInteger((uint) number),
                TypeCode.Int64 => new BigInteger((long) number),
                TypeCode.UInt64 => new BigInteger((ulong) number),
                TypeCode.Int16 => new BigInteger((short) number),
                TypeCode.UInt16 => new BigInteger((ushort) number),
                TypeCode.Byte => new BigInteger((byte) number),
                TypeCode.SByte => new BigInteger((sbyte) number),
                TypeCode.Decimal => new BigInteger((decimal) number),
                TypeCode.Single => new BigInteger((float) number),
                TypeCode.Double => new BigInteger((double) number),

                _ => throw new ArgumentException(),
            };
        }

        private static object ConvertBigIntToNumber(BigInteger bigInteger, Type targetType)
        {
            return Type.GetTypeCode(targetType) switch
            {
                TypeCode.Int32 => (int) bigInteger,
                TypeCode.UInt32 => (uint) bigInteger,
                TypeCode.Int64 => (long) bigInteger,
                TypeCode.UInt64 => (ulong) bigInteger,
                TypeCode.Int16 => (short) bigInteger,
                TypeCode.UInt16 => (ushort) bigInteger,
                TypeCode.Byte => (byte) bigInteger,
                TypeCode.SByte => (sbyte) bigInteger,
                TypeCode.Decimal => (decimal) bigInteger,
                TypeCode.Single => (float) bigInteger,
                TypeCode.Double => (double) bigInteger,

                _ => throw new ArgumentException(),
            };
        }

        private static object ConvertEnumToUnderlyingType(Enum @enum)
        {
            return @enum.GetTypeCode() switch
            {
                TypeCode.Int32 => (int) (object) @enum,
                TypeCode.UInt32 => (uint) (object) @enum,
                TypeCode.Int64 => (long) (object) @enum,
                TypeCode.UInt64 => (ulong) (object) @enum,
                TypeCode.Int16 => (short) (object) @enum,
                TypeCode.UInt16 => (ushort) (object) @enum,
                TypeCode.Byte => (byte) (object) @enum,
                TypeCode.SByte => (sbyte) (object) @enum,
                TypeCode.Decimal => (decimal) (object) @enum,
                TypeCode.Single => (float) (object) @enum,
                TypeCode.Double => (double) (object) @enum,

                _ => throw new ArgumentException(),
            };
        }
    }
}
