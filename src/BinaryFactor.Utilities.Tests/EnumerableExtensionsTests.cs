// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Tests
{
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
    }
}
