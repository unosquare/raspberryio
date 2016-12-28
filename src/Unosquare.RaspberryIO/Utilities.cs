namespace Unosquare.RaspberryIO
{
    using Native;
    using Swan;
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Miscellaneous utilities and helper methods.
    /// </summary>
    internal static class Utilities
    {
        private static readonly object SyncLock = new object();
        private static bool? m_IsLinuxOS = new bool?();
        private static bool? m_IsRunningAsRoot = new bool?();
        private const string GpioToolFileName = "gpio.2.32";
        
        /// <summary>
        /// Extracts the library wiring pi binary to the current working directory.
        /// </summary>
        internal static void ExtractLibWiringPi()
        {
            var targetPath = Path.Combine(CurrentApp.EntryAssemblyDirectory, WiringPi.WiringPiLibrary);
            if (File.Exists(targetPath)) return;

            using (var stream = typeof(Utilities).Assembly.GetManifestResourceStream($"{typeof(Utilities).Namespace}.{WiringPi.WiringPiLibrary}"))
            {
                using (var outputStream = File.OpenWrite(targetPath))
                {
                    stream?.CopyTo(outputStream);
                }
            }
        }

        /// <summary>
        /// Extracts the gpio tool.
        /// </summary>
        internal static void ExtractGpioTool()
        {
            var targetPath = Path.Combine(CurrentApp.EntryAssemblyDirectory, GpioToolFileName);
            if (File.Exists(targetPath)) return;

            using (var stream = typeof(Utilities).Assembly.GetManifestResourceStream($"{typeof(Utilities).Namespace}.{GpioToolFileName}"))
            {
                using (var outputStream = File.OpenWrite(targetPath))
                {
                    stream?.CopyTo(outputStream);
                }
                var executablePermissions = Standard.strtol("0777", IntPtr.Zero, 8);
                Standard.chmod(targetPath, (uint)executablePermissions);
            }
        }

        /// <summary>
        /// Creates a GPIO tool process
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public static Process CreateGpioToolProcess(string arguments)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = arguments,
                    CreateNoWindow = true,
                    FileName = Path.Combine(CurrentApp.EntryAssemblyDirectory, GpioToolFileName),
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
        }

        /// <summary>
        /// Gets a value indicating whether the current assembly running on a Linux os.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is Linux os; otherwise, <c>false</c>.
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
                        m_IsRunningAsRoot = Standard.getuid() == 0;
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
        public static int WiringPiToBcmPinNumber(this int wiringPiPinNumber)
        {
            lock (SyncLock)
            {
                return WiringPi.wpiPinToGpio(wiringPiPinNumber);
            }
        }

        /// <summary>
        /// Converts the Physical (Header) pin number to BCM pin number.
        /// </summary>
        /// <param name="headerPinNumber">The header pin number.</param>
        /// <returns></returns>
        public static int HaderToBcmPinNumber(this int headerPinNumber)
        {
            lock (SyncLock)
            {
                return WiringPi.physPinToGpio(headerPinNumber);
            }
        }
    }
}
