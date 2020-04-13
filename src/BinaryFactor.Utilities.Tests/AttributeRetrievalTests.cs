// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Tests
{
    using System;
    using System.Linq;
    using BinaryFactor.Utilities;
    using Shouldly;

    public class AttributeRetrievalTests
    {
        public void ShouldCorrectlyRetrieveAttributes()
        {
            var type = typeof(DerivedClass);
            type.GetAttributes().Has<CustomAttr>().ShouldBe(false);
            type.GetAttributes().Has<CustomAttr>(inherit: true).ShouldBe(true);
            type.GetAttributes().Has(typeof(CustomAttr).FullName, inherit: true).ShouldBe(true);
            type.GetAttributes().GetAll(inherit: false).ShouldHaveSingleItem().ShouldBeOfType<CustomAttrForDerived>();

            var method = type.GetMethod(nameof(DerivedClass.Method));
            method.GetAttributes().Has<CustomAttr>().ShouldBe(false);
            method.GetAttributes().Has<CustomAttr>(inherit: true).ShouldBe(true);
            method.GetAttributes().Has(typeof(CustomAttr).FullName, inherit: true).ShouldBe(true);
            method.GetAttributes().GetAll(inherit: false).ShouldHaveSingleItem().ShouldBeOfType<CustomAttrForDerived>();

            var parameter = method.GetParameters().Single();
            parameter.GetAttributes().Has<CustomAttr>().ShouldBe(false);
            parameter.GetAttributes().Has<CustomAttr>(inherit: true).ShouldBe(true);
            parameter.GetAttributes().Has(typeof(CustomAttr).FullName, inherit: true).ShouldBe(true);
            parameter.GetAttributes().GetAll(inherit: false).ShouldHaveSingleItem().ShouldBeOfType<CustomAttrForDerived>();
        }

        [CustomAttr]
        class BaseClass
        {
            [CustomAttr]
            public virtual void Method([CustomAttr] int i)
            {
            }
        }

        [CustomAttrForDerived]
        class DerivedClass: BaseClass
        {
            [CustomAttrForDerived]
            public override void Method([CustomAttrForDerived] int i)
            {
            }
        }

        class CustomAttr: Attribute { }

        class CustomAttrForDerived : Attribute { }
    }
}
