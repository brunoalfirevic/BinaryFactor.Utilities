// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class AssemblyExtensions
    {
        public static string GetEmbeddedResourceTextByPath(this Assembly assembly, string path)
        {
            var embeddedResourceName = GetEmbeddedResourceName(assembly, path);
            return GetEmbeddedResourceText(assembly, embeddedResourceName);
        }

        public static byte[] GetEmbeddedResourceBytesByPath(this Assembly assembly, string path)
        {
            var embeddedResourceName = GetEmbeddedResourceName(assembly, path);
            return GetEmbeddedResourceBytes(assembly, embeddedResourceName);
        }

        private static string GetEmbeddedResourceName(Assembly assembly, string path)
        {
            return GetEmbeddedResourceName(assembly.GetName().Name, path);
        }

        private static string GetEmbeddedResourceName(string @namespace, string path)
        {
            var name_parts = path
                .Replace("\\", ".").Replace("/", ".").Replace(" ", "_")
                .Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => char.IsDigit(part[0]) ? "_" + part : part);

            return @namespace + "." + string.Join(".", name_parts);
        }

        private static string GetEmbeddedResourceText(Assembly assembly, string embeddedResource)
        {
            var bytes = GetEmbeddedResourceBytes(assembly, embeddedResource);

            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] GetEmbeddedResourceBytes(Assembly assembly, string embeddedResource)
        {
            using var resourceStream = assembly.GetManifestResourceStream(embeddedResource);

            if (resourceStream == null)
            {
                throw new InvalidOperationException(string.Format("Embedded resource {0} does not exist", embeddedResource));
            }

            using var resourceReader = new BinaryReader(resourceStream);

            return resourceReader.ReadBytes((int)resourceStream.Length);
        }
    }
}
