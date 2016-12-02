using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    internal static class Utilities
    {

        private static readonly object SyncLock = new object();
        private static bool? m_IsLinuxOS = new Nullable<bool>();
        private static bool? m_IsRunningAsRoot = new Nullable<bool>();

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

        static public int WiringPiToBcmPinNumber(this int wiringPiPinNumber)
        {
            lock (SyncLock)
            {
                return Interop.wpiPinToGpio(wiringPiPinNumber);
            }
        }

        static public int HaderToBcmPinNumber(this int headerPinNumber)
        {
            lock (SyncLock)
            {
                return Interop.physPinToGpio(headerPinNumber);
            }
        }
    }
}
