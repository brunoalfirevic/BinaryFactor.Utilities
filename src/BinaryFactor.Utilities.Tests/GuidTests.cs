// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

using System;
using Shouldly;

namespace BinaryFactor.Utilities.Tests
{
    public class GuidTests
    {
        public void TestVariant1GuidEncoding()
        {
            var guid = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
            var variant1Bytes = guid.ToVariant1ByteArray();

            variant1Bytes.ShouldBe(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff });

            var decodedGuid = GuidUtilities.CreateFromVariant1ByteArray(variant1Bytes);
            decodedGuid.ShouldBe(guid);
        }
    }
}
