// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class AttributeRetrievalExtensions
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

        public static bool HasAttribute<TAttribute>(this Enum value)
            where TAttribute : class
        {
            return value.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : class
        {
            return value.GetAttributes<TAttribute>().FirstOrDefault();
        }

        public static IList<TAttribute> GetAttributes<TAttribute>(this Enum value)
            where TAttribute : class
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(false);

            return attributes.OfType<TAttribute>().ToList();
        }
    }
}
