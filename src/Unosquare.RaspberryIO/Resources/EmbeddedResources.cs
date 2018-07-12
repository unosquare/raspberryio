namespace Unosquare.RaspberryIO.Resources
{
    using Native;
    using Swan;
    using System;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// Provides access to embedded assembly files
    /// </summary>
    internal static class EmbeddedResources
    {
        /// <summary>
        /// Initializes static members of the <see cref="EmbeddedResources"/> class.
        /// </summary>
        static EmbeddedResources()
        {
            ResourceNames =
                new ReadOnlyCollection<string>(typeof(EmbeddedResources).Assembly().GetManifestResourceNames());
        }

        /// <summary>
        /// Gets the resource names.
        /// </summary>
        /// <value>
        /// The resource names.
        /// </value>
        public static ReadOnlyCollection<string> ResourceNames { get; }

        /// <summary>
        /// Extracts all the file resources to the specified base path.
        /// </summary>
        public static void ExtractAll()
        {
            var basePath = Runtime.EntryAssemblyDirectory;
            var executablePermissions = Standard.StringToInteger("0777", IntPtr.Zero, 8);

            foreach (var resourceName in ResourceNames)
            {
                var filename = resourceName.Substring($"{typeof(EmbeddedResources).Namespace}.".Length);
                var targetPath = Path.Combine(basePath, filename);
                if (File.Exists(targetPath)) return;

                using (var stream = typeof(EmbeddedResources).Assembly()
                    .GetManifestResourceStream($"{typeof(EmbeddedResources).Namespace}.{filename}"))
                {
                    using (var outputStream = File.OpenWrite(targetPath))
                    {
                        stream?.CopyTo(outputStream);
                    }

                    try
                    {
                        Standard.Chmod(targetPath, (uint)executablePermissions);
                    }
                    catch
                    {
                        /* Ignore */
                    }
                }
            }
        }
    }
}