// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System.IO;
    using System.Text;

    public static class EncodingExtensions
    {
        public static string GetStringWithoutPreamble(this Encoding encoding, byte[] bytes)
        {
            using var streamReader = new StreamReader(new MemoryStream(bytes), encoding);
            return streamReader.ReadToEnd();
        }
    }
}
