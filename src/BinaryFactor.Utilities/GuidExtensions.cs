// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;

    public static class GuidExtensions
    {
        private static readonly string ValidCaseInsensitiveIdentifierCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly string ValidCaseSensitiveIdentifierCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string ToValidIdentifierString(this Guid guid, string prefix = null, bool caseSensitive = false)
        {
            var alphabet = caseSensitive ? ValidCaseSensitiveIdentifierCharacters : ValidCaseInsensitiveIdentifierCharacters;
            var result = ConvertEx.ToCustomAlphabetString(guid, alphabet);

            if (!string.IsNullOrEmpty(prefix))
                result = prefix + result;
            else if (!char.IsLetter(result[0]))
                result = 'a' + result;

            return result;
        }

        public static byte[] ToVariant1ByteArray(this Guid guid)
        {
            var bytes = guid.ToByteArray();

            (bytes[0], bytes[3]) = (bytes[3], bytes[0]);
            (bytes[1], bytes[2]) = (bytes[2], bytes[1]);

            (bytes[4], bytes[5]) = (bytes[5], bytes[4]);

            (bytes[6], bytes[7]) = (bytes[7], bytes[6]);

            return bytes;
        }
    }
}
