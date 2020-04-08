// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TopologicalSortExtensions
    {
        public static IList<T> TopologicallyOrder<T>(this IEnumerable<T> vertices, IEnumerable<(T, T)> edges, IEqualityComparer<T> comparer = null)
        {
            return vertices.TopologicallyConnect(edges, comparer).WithoutCycles();
        }

        public static IList<T> TopologicallyOrderDesc<T>(this IEnumerable<T> vertices, IEnumerable<(T, T)> edges, IEqualityComparer<T> comparer = null)
        {
            return vertices.TopologicallyConnectDesc(edges, comparer).WithoutCycles();
        }

        public static IList<T> TopologicallyOrderByDesc<T>(this IEnumerable<T> vertices, Func<T, T, bool> comesBefore, IEqualityComparer<T> comparer = null)
        {
            return vertices.TopologicallyOrderBy((left, right) => comesBefore(right, left), comparer);
        }

        public static IList<T> TopologicallyOrderBy<T>(this IEnumerable<T> vertices, Func<T, T, bool> comesBefore, IEqualityComparer<T> comparer = null)
        {
            return vertices.TopologicallyConnectBy(comesBefore, comparer).WithoutCycles();
        }

        public static IList<IList<T>> TopologicallyConnectByDesc<T>(this IEnumerable<T> vertices, Func<T, T, bool> comesBefore, IEqualityComparer<T> comparer = null)
        {
            return vertices.TopologicallyConnectBy((left, right) => comesBefore(right, left), comparer);
        }

        public static IList<IList<T>> TopologicallyConnectBy<T>(this IEnumerable<T> vertices, Func<T, T, bool> comesBefore, IEqualityComparer<T> comparer = null)
        {
            var verticesArray = vertices.ToArray();

            IEnumerable<T> Edges(T v)
            {
                for(var i = 0; i < verticesArray.Length; i++)
                {
                    var w = verticesArray[i];
                    if (comesBefore(w, v))
                        yield return w;
                }
            }

            return vertices.TopologicallyConnect(Edges, comparer);
        }

        public static IList<IList<T>> TopologicallyConnect<T>(this IEnumerable<T> vertices, IEnumerable<(T, T)> edges, IEqualityComparer<T> comparer = null)
        {
            var edgesLookup = edges.ToLookup(edge => edge.Item2, edge => edge.Item1, comparer ?? EqualityComparer<T>.Default);

            return vertices.TopologicallyConnect(edge => edgesLookup[edge], comparer);
        }

        public static IList<IList<T>> TopologicallyConnectDesc<T>(this IEnumerable<T> vertices, IEnumerable<(T, T)> edges, IEqualityComparer<T> comparer = null)
        {
            var edgesLookup = edges.ToLookup(edge => edge.Item1, edge => edge.Item2, comparer ?? EqualityComparer<T>.Default);

            return vertices.TopologicallyConnect(edge => edgesLookup[edge], comparer);
        }

        private static IList<IList<T>> TopologicallyConnect<T>(this IEnumerable<T> vertices, Func<T, IEnumerable<T>> edges, IEqualityComparer<T> comparer = null)
        {
            return GabowTopologicallyConnect(vertices, edges, comparer);
            // return TarjanTopologicallyConnect(vertices, edges, comparer);
        }

        private static IList<IList<T>> TarjanTopologicallyConnect<T>(this IEnumerable<T> vertices, Func<T, IEnumerable<T>> edges, IEqualityComparer<T> comparer)
        {
            comparer ??= EqualityComparer<T>.Default;

            var result = new List<IList<T>>();

            var indices = new Dictionary<T, int>(comparer);
            var lowLink = new Dictionary<T, int>(comparer);
            var stack = new Stack<T>();
            var index = 0;

            void StronglyConnect(T v)
            {
                indices[v] = index;
                lowLink[v] = index;
                stack.Push(v);
                index++;

                foreach (var w in edges(v))
                {
                    if (!indices.ContainsKey(w))
                    {
                        StronglyConnect(w);
                        lowLink[v] = Math.Min(lowLink[v], lowLink[w]);
                    }
                    else if (stack.Contains(w, comparer))
                    {
                        lowLink[v] = Math.Min(lowLink[v], indices[w]);
                    }
                }

                if (lowLink[v] == indices[v])
                {
                    var stronglyConnectedComponent = new List<T>();

                    T w;
                    do
                    {
                        w = stack.Pop();
                        stronglyConnectedComponent.Add(w);
                    } while (!comparer.Equals(v, w));

                    result.Add(stronglyConnectedComponent);
                }
            }

            foreach (var v in vertices)
            {
                if (!indices.ContainsKey(v))
                    StronglyConnect(v);
            }

            return result;
        }

        private static IList<IList<T>> GabowTopologicallyConnect<T>(this IEnumerable<T> vertices, Func<T, IEnumerable<T>> edges, IEqualityComparer<T> comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;

            var result = new List<IList<T>>();

            var s = new Stack<T>();
            var p = new Stack<T>();
            var preorder = new Dictionary<T, int>(comparer);
            var assigned = new HashSet<T>(comparer);
            var c = 0;

            void Search(T v)
            {
                preorder[v] = c++;
                s.Push(v);
                p.Push(v);

                foreach (var w in edges(v))
                {
                    if (!preorder.ContainsKey(w))
                    {
                        Search(w);
                    }
                    else if (!assigned.Contains(w))
                    {
                        while(preorder[p.Peek()] > preorder[w])
                            p.Pop();
                    }
                }

                if (p.Count > 0 && comparer.Equals(v, p.Peek()))
                {
                    var stronglyConnectedComponent = new List<T>();

                    T w;
                    do
                    {
                        w = s.Pop();
                        stronglyConnectedComponent.Add(w);
                        assigned.Add(w);
                    } while (!comparer.Equals(v, w));

                    p.Pop();
                    result.Add(stronglyConnectedComponent);
                }
            }

            foreach (var v in vertices)
            {
                if (!preorder.ContainsKey(v))
                    Search(v);
            }

            return result;
        }

        private static IList<T> WithoutCycles<T>(this IList<IList<T>> stronglyConnectedComponents)
        {
            return stronglyConnectedComponents
                .Select(scc =>
                    scc.Count == 1
                        ? scc[0]
                        : throw new InvalidOperationException("Cycle found while trying to topologically sort the collection"))
                .ToList();
        }
    }
}
