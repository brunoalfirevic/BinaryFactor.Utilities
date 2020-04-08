// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Tests
{
    using System;
    using System.Linq;
    using Shouldly;
    using BinaryFactor.Utilities;
    using BinaryFactor.Utilities.Tests.Utilities;

    public class TopologicalSortExtensionsTests
    {
        public void TestTopologicalSortWithoutCycles()
        {
            var input = "bcade";

            var edges = new[] { "ad", "ab", "bc", "de", "cd"}.Select(s => (s[0], s[1]));

            input
                .TopologicallyOrder(edges)
                .ShouldBe("abcde");

            input
                .TopologicallyOrderBy((o1, o2) => edges.Contains((o1, o2)))
                .ShouldBe("abcde");

            input
                .TopologicallyOrderDesc(edges)
                .ShouldBe("edcba");

            input
                .TopologicallyOrderByDesc((o1, o2) => edges.Contains((o1, o2)))
                .ShouldBe("edcba");
        }

        public void TestTopologicalSortWithCycles()
        {
            var input = "abcdefghijkl";

            var edges = new[]
            {
                "ab", "bc", "be", "bf", "cd", "cg", "dc", "dh", 
                "ea", "ef", "fg", "gf", "hd", "hg", "ii", "kl"
            }.Select(s => (s[0], s[1]));

            var output = input.TopologicallyConnect(edges);

            output.Count.ShouldBe(7);

            output.ShouldContainComponent("abe");
            output.ShouldContainComponent("cdh");
            output.ShouldContainComponent("fg");
            output.ShouldContainComponent("i");
            output.ShouldContainComponent("j");
            output.ShouldContainComponent("k");
            output.ShouldContainComponent("l");

            output.ShouldHaveComponentsInOrder("abe", "fg");
            output.ShouldHaveComponentsInOrder("cdh", "fg");
            output.ShouldHaveComponentsInOrder("abe", "cdh");
            output.ShouldHaveComponentsInOrder("k", "l");
        }
    }
}
