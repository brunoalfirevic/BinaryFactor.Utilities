// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

using System;

namespace BinaryFactor.Utilities
{
    public static class UniqueIdentifierHelper
    {
        public static string Generate(string prefix = null) => Guid.NewGuid().ToValidIdentifierString(prefix);
    }
}
