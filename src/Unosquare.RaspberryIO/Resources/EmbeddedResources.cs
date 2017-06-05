namespace Unosquare.RaspberryIO.Resources
{
    using Swan;
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using Native;

    /// <summary>
    /// Provides access to embedded assembly files
    /// </summary>
    internal static class EmbeddedResources
    {
        /// <summary>
        /// Initializes the <see cref="EmbeddedResources"/> class.
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
        /// <param name="basePath">The base path.</param>
        public static void ExtractAll(string basePath = null)
        {
            if (string.IsNullOrWhiteSpace(basePath))
                basePath = Runtime.EntryAssemblyDirectory;

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
                        var executablePermissions = Standard.strtol("0777", IntPtr.Zero, 8);
                        Standard.chmod(targetPath, (uint) executablePermissions);
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