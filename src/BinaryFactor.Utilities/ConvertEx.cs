// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Linq;
    using System.Numerics;
    using System.Text;

    public static class ConvertEx
    {
        public static string ToCustomAlphabetString(BigInteger value, string alphabet)
        {
            var result = new StringBuilder();

            do
            {
                value = BigInteger.DivRem(value, alphabet.Length, out var remainder);
                result.Append(alphabet[(int)remainder]);
            } while (value > 0);

            return result.ToString().Reverse();
        }

        public static string ToCustomAlphabetString(byte[] value, string alphabet, bool isBigEndian = false)
        {
            if (isBigEndian)
            {
                value = value.ToArray();
                Array.Reverse(value);
            }

            var trailingBytesWithZeroValue = 0;
            for (var i = value.Length - 1; i >= 0; i--)
            {
                if (value[i] != 0)
                    break;

                trailingBytesWithZeroValue++;
            }

            var leadingZeros = new string(alphabet[0], trailingBytesWithZeroValue);
            if (value.Length == trailingBytesWithZeroValue)
                return leadingZeros;

            var bytesWithSingleTrailingZero = value.Take(value.Length - trailingBytesWithZeroValue).Append((byte) 0).ToArray();
            return leadingZeros + ToCustomAlphabetString(new BigInteger(bytesWithSingleTrailingZero), alphabet);
        }

        public static string ToCustomAlphabetString(Guid value, string alphabet, bool isBigEndian = false)
        {
            return ToCustomAlphabetString(FromGuidToBigInteger(value, isBigEndian), alphabet);
        }

        public static BigInteger FromCustomAlphabetString(string value, string alphabet)
        {
            var alphabetDict = alphabet.ToDictionary((c, index) => c, (c, index) => index);

            var result = BigInteger.Zero;
            foreach (var c in value)
                result = result * alphabet.Length + alphabetDict[c];

            return result;
        }

        public static byte[] FromCustomAlphabetStringToBytes(string value, string alphabet, bool isBigEndian = false)
        {
            var leadingZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != alphabet[0])
                    break;

                leadingZeros++;
            }

            var bytesForLeadingZeros = new byte[leadingZeros];
            if (leadingZeros == value.Length)
                return bytesForLeadingZeros;

            var convertedBytes = FromCustomAlphabetString(value, alphabet).ToByteArray();

            if (isBigEndian)
                Array.Reverse(convertedBytes);

            return isBigEndian
                ? bytesForLeadingZeros.Concat(convertedBytes).ToArray()
                : convertedBytes.Concat(bytesForLeadingZeros).ToArray();
        }

        public static Guid FromCustomAlphabetStringToGuid(string value, string alphabet, bool isBigEndian = false)
        {
            return FromBigIntegerToGuid(FromCustomAlphabetString(value, alphabet), isBigEndian);
        }

        public static Guid FromBigIntegerToGuid(BigInteger value, bool isBigEndian = false)
        {
            var guidBytes = new byte[16];
            var bigIntegerBytes = value.ToByteArray().SkipLastWhile(b => b == 0).ToArray();

            if (bigIntegerBytes.Length > guidBytes.Length)
                throw new ArgumentException("Value is too large to be converted to GUID", nameof(value));

            bigIntegerBytes.CopyTo(guidBytes, 0);

            if (isBigEndian)
                Array.Reverse(guidBytes);

            return GuidUtilities.CreateFromVariant1ByteArray(guidBytes);
        }

        public static BigInteger FromGuidToBigInteger(Guid value, bool isBigEndian = false)
        {
            var guidBytes = value.ToVariant1ByteArray();

            if (isBigEndian)
                Array.Reverse(guidBytes);

            var bytesWithExtraZeroToForcePositiveBigInteger = new byte[guidBytes.Length + 1];
            guidBytes.CopyTo(bytesWithExtraZeroToForcePositiveBigInteger, 0);
            return new BigInteger(bytesWithExtraZeroToForcePositiveBigInteger);
        }

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

            if (targetType == typeof(Guid))
            {
                if (value is string s)
                    return Guid.Parse(s);

                if (value is byte[] byteArray)
                    return new Guid(byteArray);

                if (value is BigInteger bigInteger)
                    return FromBigIntegerToGuid(bigInteger);
            }

            if (value is Guid guid)
            {
                if (targetType == typeof(string))
                    return guid.ToString();

                if (targetType == typeof(byte[]))
                    return guid.ToByteArray();

                if (targetType == typeof(BigInteger))
                    return FromGuidToBigInteger(guid);
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

        private static void Swap(ref byte left, ref byte right)
        {
            var tmp = left;
            left = right;
            right = tmp;
        }
    }
}
