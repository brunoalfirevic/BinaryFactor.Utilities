using System;

namespace BinaryFactor.Utilities
{
    public static class UniqueIdentifierHelper
    {
        public static string Generate(string prefix = null) => Guid.NewGuid().ToValidIdentifierString(prefix);
    }
}
