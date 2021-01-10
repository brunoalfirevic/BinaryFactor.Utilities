// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;

    public static class GuidUtilities
    {
        public static Guid CreateFromVariant1ByteArray(byte[] bytes)
        {
            return new Guid(
                (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3],
                (short)((bytes[4] << 8) | bytes[5]),
                (short)((bytes[6] << 8) | bytes[7]),
                bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15]);
        }
    }
}
