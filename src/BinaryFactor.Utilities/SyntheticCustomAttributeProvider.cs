// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class SyntheticCustomAttributeProvider : ICustomAttributeProvider
    {
        private readonly IList<ICustomAttributeProvider> customAttributeProviders;
        private readonly IList<Attribute> customAttributes;
        private readonly IList<Attribute> inheritedCustomAttributes;

        public SyntheticCustomAttributeProvider(params ICustomAttributeProvider[] customAttributeProviders)
            : this(customAttributeProviders, Enumerable.Empty<Attribute>())
        {
        }

        public SyntheticCustomAttributeProvider(params Attribute[] customAtributes)
            : this(Enumerable.Empty<ICustomAttributeProvider>(), customAtributes)
        {
        }

        public SyntheticCustomAttributeProvider(IEnumerable<ICustomAttributeProvider> customAttributeProviders, IEnumerable<Attribute> customAttributes)
            : this(customAttributeProviders, customAttributes, Enumerable.Empty<Attribute>())
        {
        }

        public SyntheticCustomAttributeProvider(IEnumerable<ICustomAttributeProvider> customAttributeProviders, IEnumerable<Attribute> customAttributes, IEnumerable<Attribute> inheritedCustomAttributes)
        {
            this.customAttributeProviders = customAttributeProviders.ToList();
            this.customAttributes = customAttributes.ToList();
            this.inheritedCustomAttributes = inheritedCustomAttributes.ToList();
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return this.customAttributeProviders
                .SelectMany(cap => cap.GetCustomAttributes(inherit))
                .Concat(this.customAttributes)
                .Concat(inherit ? this.inheritedCustomAttributes : Enumerable.Empty<Attribute>())
                .ToArray();
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this.customAttributeProviders
                .SelectMany(cap => cap.GetCustomAttributes(attributeType, inherit))
                .Concat(this.customAttributes.Concat(inherit ? this.inheritedCustomAttributes : Enumerable.Empty<Attribute>()).Where(ca => ca.GetType().Is(attributeType)))
                .ToArray();
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return GetCustomAttributes(attributeType, inherit).Any();
        }
    }
}