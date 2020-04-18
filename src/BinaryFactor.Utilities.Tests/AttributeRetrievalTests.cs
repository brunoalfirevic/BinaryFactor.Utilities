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
            type.CustomAttrs().Has<CustomAttr>().ShouldBe(false);
            type.CustomAttrs().Has<CustomAttr>(inherit: true).ShouldBe(true);
            type.CustomAttrs().Has(typeof(CustomAttr).FullName, inherit: true).ShouldBe(true);
            type.CustomAttrs().GetAll(inherit: false).ShouldHaveSingleItem().ShouldBeOfType<CustomAttrForDerived>();

            var method = type.GetMethod(nameof(DerivedClass.Method));
            method.CustomAttrs().Has<CustomAttr>().ShouldBe(false);
            method.CustomAttrs().Has<CustomAttr>(inherit: true).ShouldBe(true);
            method.CustomAttrs().Has(typeof(CustomAttr).FullName, inherit: true).ShouldBe(true);
            method.CustomAttrs().GetAll(inherit: false).ShouldHaveSingleItem().ShouldBeOfType<CustomAttrForDerived>();
            method.ReturnTypeCustomAttributes.CustomAttrs().Has<CustomAttr>().ShouldBeTrue();
            method.ReturnParameter.CustomAttrs().Has<CustomAttr>().ShouldBeTrue();

            var methodWithoutReturnAttributes = type.GetMethod(nameof(DerivedClass.MethodWithoutReturnAttributes));
            methodWithoutReturnAttributes.ReturnTypeCustomAttributes.CustomAttrs().GetAll().ShouldBeEmpty();
            methodWithoutReturnAttributes.ReturnParameter.CustomAttrs().GetAll().ShouldBeEmpty();

            var parameter = method.GetParameters().Single();
            parameter.CustomAttrs().Has<CustomAttr>().ShouldBe(false);
            parameter.CustomAttrs().Has<CustomAttr>(inherit: true).ShouldBe(true);
            parameter.CustomAttrs().Has(typeof(CustomAttr).FullName, inherit: true).ShouldBe(true);
            parameter.CustomAttrs().GetAll(inherit: false).ShouldHaveSingleItem().ShouldBeOfType<CustomAttrForDerived>();
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
            [return: CustomAttr]
            public override void Method([CustomAttrForDerived] int i)
            {
            }

            public void MethodWithoutReturnAttributes()
            {
            }
        }

        class CustomAttr: Attribute { }

        class CustomAttrForDerived : Attribute { }
    }
}
