// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities.Tests
{
    using System;
    using System.Linq;
    using Shouldly;
    using BinaryFactor.Utilities;

    public class EnumerableExtensionsTests
    {
        public void TestBatching()
        {
            var collection = new[] { 1, 2, 3, 4, 5 };

            collection
                .Batch(2)
                .ShouldBe(new[]
                {
                    new[] { 1, 2 },
                    new[] { 3, 4 },
                    new[] { 5 }
                });

            collection
                .Batch(5)
                .ShouldBe(new[]
                {
                    new[] { 1, 2, 3, 4, 5 }
                });

            collection
                .Batch(50)
                .ShouldBe(new[]
                {
                    new[] { 1, 2, 3, 4, 5 }
                });
        }

        public void TestGetRange()
        {
            var collection = new[] { 0, 1, 2, 3, 4 };

            collection.GetRange(0, 0).ShouldBe(new int[0]);
            collection.GetRange(0, 3).ShouldBe(new[] { 0, 1, 2 });
            collection.GetRange(1, 1).ShouldBe(new[] { 1 });
            collection.GetRange(4, 0).ShouldBe(new int[0]);
            collection.GetRange(4, 1).ShouldBe(new[] { 4 });
            collection.GetRange(5, 0).ShouldBe(new int[0]);

            Should.Throw<ArgumentException>(() => collection.GetRange(5, 1).ToList());
            Should.Throw<ArgumentException>(() => collection.GetRange(6, 0).ToList());
        }
    }
}
