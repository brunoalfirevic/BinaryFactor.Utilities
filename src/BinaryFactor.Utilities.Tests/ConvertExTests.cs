// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities.Tests
{
    using System;
    using System.Numerics;
    using Shouldly;
    using BinaryFactor.Utilities;

    public class ConvertExTests
    {
        public void TestVariousConversions()
        {
            ConvertEx.ChangeType<ushort?>(null).ShouldBeNull();
            ConvertEx.ChangeType<int>(100L).ShouldBe(100);
            ConvertEx.ChangeType<int>(new long?(100L)).ShouldBe(100);
        }

        public void TestEnumConversions()
        {
            ConvertEx.ChangeType<LongEnum?>("First").ShouldBe(LongEnum.First);
            ConvertEx.ChangeType<LongEnum?>((short) 2).ShouldBe(LongEnum.Second);
            ConvertEx.ChangeType<ShortEnum?>(2L).ShouldBe(ShortEnum.Second);
            ConvertEx.ChangeType<LongEnum?>(null).ShouldBeNull();
            ConvertEx.ChangeType<int>(LongEnum.First).ShouldBe(1);
        }

        public void TestBigIntegerConversions()
        {
            ConvertEx.ChangeType<int>((BigInteger) 100).ShouldBe(100);
            ConvertEx.ChangeType<int?>((BigInteger) 100).ShouldBe(100);
            ConvertEx.ChangeType<string>((BigInteger) 100).ShouldBe("100");
            ConvertEx.ChangeType<LongEnum?>((BigInteger) 2).ShouldBe(LongEnum.Second);

            ConvertEx.ChangeType<BigInteger?>(null).ShouldBeNull();
            ConvertEx.ChangeType<BigInteger>((BigInteger) 100).ShouldBe(new BigInteger(100));
            ConvertEx.ChangeType<BigInteger>(LongEnum.First).ShouldBe(new BigInteger(1));
            ConvertEx.ChangeType<BigInteger>(100).ShouldBe(new BigInteger(100));
        }

        public void TestGuidToBigIntConversion()
        {
            for (var i = 0; i < 1000; i++)
            {
                var guid = Guid.NewGuid();

                var bigInt = ConvertEx.ChangeType<BigInteger>(guid);
                bigInt.Sign.ShouldBe(1);

                var convertedGuid = ConvertEx.ChangeType<Guid>(bigInt);
                convertedGuid.ShouldBe(guid);
            }
        }

        public void TestCustomAlphabetConversionOfGuid()
        {
            var alphabet = "0123456789abcdef";
            var guid = Guid.Parse("6ca4f0f8-2508-4bac-b8f1-5d1e3da2247a");

            var str = ConvertEx.ToCustomAlphabetString(guid, alphabet, isBigEndian: true);

            str.ShouldBe("6ca4f0f825084bacb8f15d1e3da2247a");

            var decoded = ConvertEx.FromCustomAlphabetStringToGuid(str, alphabet, isBigEndian: true);
            decoded.ShouldBe(guid);
        }

        public void TestCustomAlphabetConversionUsingStandardDigits()
        {
            var alphabet = "0123456789";
            var bigInt = BigInteger.Parse("144413047667920780076747050081956537466");

            var str = ConvertEx.ToCustomAlphabetString(bigInt, alphabet);
            str.ShouldBe("144413047667920780076747050081956537466");

            var decoded = ConvertEx.FromCustomAlphabetString(str, alphabet);
            decoded.ShouldBe(bigInt);
        }

        public void TestCustomAlphabetConversionOfZeroLengthArray()
        {
            var alphabet = "0123456789";

            var str = ConvertEx.ToCustomAlphabetString(new byte[0], alphabet);
            str.ShouldBe("");

            var decoded = ConvertEx.FromCustomAlphabetStringToBytes(str, alphabet);
            decoded.ShouldBe(new byte[0]);
        }

        public void TestCustomAlphabetConversionOfArrayOfZeros()
        {
            var alphabet = "01";
            var bytes = new byte[7];

            var str = ConvertEx.ToCustomAlphabetString(bytes, alphabet);
            str.ShouldBe("0000000");

            var decoded = ConvertEx.FromCustomAlphabetStringToBytes(str, alphabet);
            decoded.ShouldBe(bytes);
        }

        public void TestCustomAlphabetConversionOfArrayWithTrailingZeros()
        {
            var alphabet = "01";
            var bytes = new byte[] { 5, 3, 0 };

            var str = ConvertEx.ToCustomAlphabetString(bytes, alphabet);
            str.ShouldBe("01100000101");

            var decoded = ConvertEx.FromCustomAlphabetStringToBytes(str, alphabet);
            decoded.ShouldBe(bytes);

            var bytesBigEndian = new byte[] { 0, 3, 5 };

            var strBigEndian = ConvertEx.ToCustomAlphabetString(bytesBigEndian, alphabet, isBigEndian: true);
            strBigEndian.ShouldBe("01100000101");

            var decodedBigEndian = ConvertEx.FromCustomAlphabetStringToBytes(strBigEndian, alphabet, isBigEndian: true);
            decodedBigEndian.ShouldBe(bytesBigEndian);
        }

        enum LongEnum : ulong
        {
            First = 1L,
            Second = 2L
        }

        enum ShortEnum : short
        {
            First = 1,
            Second = 2
        }
    }
}
