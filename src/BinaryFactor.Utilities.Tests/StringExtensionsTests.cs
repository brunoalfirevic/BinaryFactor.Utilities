// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Tests
{
    using System;
    using System.Text.RegularExpressions;
    using Shouldly;
    using BinaryFactor.Utilities;

    public class StringExtensionsTests
    {
        public void TestSplitStringByRegex()
        {
            var str = "abc12efg3hij1";
            var regex = new Regex("(?<separator>\\d)");

            str
                .SplitByRegex(regex, "separator", StringSplitOptions.RemoveEmptyEntries)
                .ShouldBe(new[] { "abc", "efg", "hij" });

            str
                .SplitByRegex(regex, "separator")
                .ShouldBe(new[] { "abc", "", "efg", "hij", "" });
        }
    }
}
