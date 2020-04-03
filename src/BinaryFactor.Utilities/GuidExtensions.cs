// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;

    public static class GuidExtensions
    {
        public static string ToValidIdentifierString(this Guid guid, string prefix = null) => (prefix ?? "g") + guid.ToString("N");
    }
}
