// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities.Tests
{
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
