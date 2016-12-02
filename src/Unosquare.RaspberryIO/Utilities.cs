namespace Unosquare.RaspberryIO
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Miscellaneous utilities and helper methods.
    /// </summary>
    internal static class Utilities
    {

        private static readonly object SyncLock = new object();
        private static bool? m_IsLinuxOS = new Nullable<bool>();
        private static bool? m_IsRunningAsRoot = new Nullable<bool>();

        /// <summary>
        /// Gets the entry assembly directory (full path).
        /// </summary>
        /// <value>
        /// The entry assembly directory.
        /// </value>
        public static string EntryAssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetEntryAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Extracts the library wiring pi binary to the current working directory.
        /// </summary>
        public static void ExtractLibWiringPi()
        {
            var targetPath = Path.Combine(EntryAssemblyDirectory, Interop.WiringPiLibrary);
            if (File.Exists(targetPath)) return;

            using (var stream = typeof(Utilities).Assembly.GetManifestResourceStream($"{typeof(Utilities).Namespace}.{Interop.WiringPiLibrary}"))
            {
                using (var outputStream = File.OpenWrite(targetPath))
                {
                    stream.CopyTo(outputStream);
                }
            }

        }

        /// <summary>
        /// Gets a value indicating whether the current assembly running on a linux os.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is linux os; otherwise, <c>false</c>.
        /// </value>
        public static bool IsLinuxOS
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_IsLinuxOS.HasValue == false)
                    {
                        m_IsLinuxOS = Environment.OSVersion.Platform == PlatformID.Unix;
                    }

                    return m_IsLinuxOS.Value;
                }

            }
        }

        /// <summary>
        /// Gets a value indicating whether this program is running as Root
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running as root; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRunningAsRoot
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_IsRunningAsRoot.HasValue == false)
                    {
                        m_IsRunningAsRoot = Interop.getuid() == 0;
                    }

                    return m_IsRunningAsRoot.Value;
                }

            }

        }

        /// <summary>
        /// Converts the Wirings Pi pin number to the BCM pin number.
        /// </summary>
        /// <param name="wiringPiPinNumber">The wiring pi pin number.</param>
        /// <returns></returns>
        static public int WiringPiToBcmPinNumber(this int wiringPiPinNumber)
        {
            lock (SyncLock)
            {
                return Interop.wpiPinToGpio(wiringPiPinNumber);
            }
        }

        /// <summary>
        /// Converts the Physical (Hader) pin number to BCM pin number.
        /// </summary>
        /// <param name="headerPinNumber">The header pin number.</param>
        /// <returns></returns>
        static public int HaderToBcmPinNumber(this int headerPinNumber)
        {
            lock (SyncLock)
            {
                return Interop.physPinToGpio(headerPinNumber);
            }
        }
    }
}
