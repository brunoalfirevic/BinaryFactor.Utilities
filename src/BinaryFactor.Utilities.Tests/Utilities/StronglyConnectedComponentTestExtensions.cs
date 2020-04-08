// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities.Tests.Utilities
{
    using Shouldly;
    using System.Collections.Generic;
    using System.Linq;

    static class StronglyConnectedComponentTestExtensions
    {
        public static void ShouldContainComponent(this IList<IList<char>> components, string component)
        {
            components
                .Where(c => MatchComponent(c, component))
                .ShouldHaveSingleItem($"Component '{component}' should exist");
        }

        public static void ShouldHaveComponentsInOrder(this IList<IList<char>> components, string component1, string component2)
        {
            var index1 = components.ToList().FindIndex(c => MatchComponent(c, component1));
            var index2 = components.ToList().FindIndex(c => MatchComponent(c, component2));

            index1.ShouldBeLessThan(index2, $"Component '{component1}' should come before '{component2}'");
        }

        private static bool MatchComponent(IList<char> component1, string component2)
        {
            return component1.Count == component2.Length &&
                   component2.All(character => component1.Contains(character));
        }
    }
}
