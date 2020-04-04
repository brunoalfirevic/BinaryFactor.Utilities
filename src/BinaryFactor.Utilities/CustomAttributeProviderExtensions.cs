// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class CustomAttributeProviderExtensions
    {
        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit = false)
            where TAttribute : class
        {
            return attributeProvider.GetAttribute<TAttribute>(inherit) != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit = false)
            where TAttribute : class
        {
            return attributeProvider.GetAttributes<TAttribute>(inherit).FirstOrDefault();
        }

        public static IList<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit = false)
            where TAttribute : class
        {
            var attributes = attributeProvider.GetCustomAttributes(inherit);

            return attributes.OfType<TAttribute>().ToList();
        }
    }
}
