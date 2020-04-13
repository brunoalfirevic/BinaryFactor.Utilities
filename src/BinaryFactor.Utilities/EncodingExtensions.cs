using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryFactor.Utilities
{
    public static class EncodingExtensions
    {
        public static string GetStringWithoutPreamble(this Encoding encoding, byte[] bytes)
        {
            using var streamReader = new StreamReader(new MemoryStream(bytes), encoding);
            return streamReader.ReadToEnd();
        }
    }
}
