using System;

namespace BinaryFactor.Utilities
{
    public static class GuidExtensions
    {
        public static string ToValidIdentifierString(this Guid guid, string prefix = null) => (prefix ?? "g") + guid.ToString("N");
    }
}
