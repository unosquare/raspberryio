namespace Unosquare.RaspberryIO.Computer
{
    using Native;
    using Swan.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// http://raspberry-pi-guide.readthedocs.io/en/latest/system.html
    /// </summary>
    public sealed class SystemInfo : SingletonBase<SystemInfo>
    {
        private const string CpuInfoFilePath = "/proc/cpuinfo";
        private const string MemInfoFilePath = "/proc/meminfo";

        private static bool? m_IsLinuxOS = new bool?();
        private static bool? m_IsRunningAsRoot = new bool?();

        /// <summary>
        /// Prevents a default instance of the <see cref="SystemInfo"/> class from being created.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Could not initialize the GPIO controller</exception>
        private SystemInfo()
        {
            #region Obtain and format a property dictionary

            var properties =
                typeof(SystemInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(
                        p =>
                            p.CanWrite && p.CanRead &&
                            (p.PropertyType == typeof(string) || p.PropertyType == typeof(string[])))
                    .ToArray();
            var propDictionary = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var prop in properties)
            {
                propDictionary[prop.Name.Replace(" ", "").ToLowerInvariant().Trim()] = prop;
            }

            #endregion

            #region Extract CPU information

            if (File.Exists(CpuInfoFilePath))
            {
                var cpuInfoLines = File.ReadAllLines(CpuInfoFilePath);

                foreach (var line in cpuInfoLines)
                {
                    var lineParts = line.Split(new[] { ':' }, 2);
                    if (lineParts.Length != 2)
                        continue;

                    var propertyKey = lineParts[0].Trim().Replace(" ", "");
                    var propertyStringValue = lineParts[1].Trim();

                    if (!propDictionary.ContainsKey(propertyKey)) continue;

                    var property = propDictionary[propertyKey];
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(this, propertyStringValue);
                    }
                    else if (property.PropertyType == typeof(string[]))
                    {
                        var propertyArrayAvalue = propertyStringValue.Split(' ');
                        property.SetValue(this, propertyArrayAvalue);
                    }
                }
            }

            #endregion

            #region Extract Memory Information

            if (File.Exists(MemInfoFilePath))
            {
                var memInfoLines = File.ReadAllLines(MemInfoFilePath);
                foreach (var line in memInfoLines)
                {
                    var lineParts = line.Split(new[] { ':' }, 2);
                    if (lineParts.Length != 2)
                        continue;

                    if (lineParts[0].ToLowerInvariant().Trim().Equals("memtotal") == false)
                        continue;

                    var memKb = lineParts[1].ToLowerInvariant().Trim().Replace("kb", "").Trim();
                    int parsedMem;
                    if (int.TryParse(memKb, out parsedMem))
                    {
                        InstalledRam = parsedMem * 1024;
                        break;
                    }

                }
            }

            #endregion

            #region Board Version and Form Factor

            try
            {
                int boardVersion;
                if (string.IsNullOrWhiteSpace(Revision) == false &&
                    int.TryParse(
                        Revision.ToUpperInvariant(),
                        NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture,
                        out boardVersion))
                {
                    RaspberryPiVersion = PiVersion.Unknown;
                    if (Enum.GetValues(typeof(PiVersion)).Cast<int>().Contains(boardVersion))
                    {
                        RaspberryPiVersion = (PiVersion)boardVersion;
                    }
                }

                WiringPiBoardRevision = WiringPi.piBoardRev();
            }
            catch
            {
                /* Ignore */
            }

            #endregion

            #region Version Information

            {
                var libParts = WiringPi.WiringPiLibrary.Split('.');
                var major = int.Parse(libParts[libParts.Length - 2]);
                var minor = int.Parse(libParts[libParts.Length - 1]);
                var version = new Version(major, minor);
                WiringPiVersion = version;
            }

            #endregion

            #region Extract OS Info

            utsname unameInfo;
            try
            {
                Standard.uname(out unameInfo);
                OperatingSystem = new OsInfo
                {
                    DomainName = unameInfo.domainname,
                    Machine = unameInfo.machine,
                    NodeName = unameInfo.nodename,
                    Release = unameInfo.release,
                    SysName = unameInfo.sysname,
                    Version = unameInfo.version
                };
            }
            catch
            {
                OperatingSystem = new OsInfo();
            }

            #endregion
        }

        /// <summary>
        /// Gets the wiring pi library version.
        /// </summary>
        public Version WiringPiVersion { get; }

        /// <summary>
        /// Gets the OS information.
        /// </summary>
        /// <value>
        /// The os information.
        /// </value>
        public OsInfo OperatingSystem { get; }

        /// <summary>
        /// Gets the Raspberry Pi version.
        /// </summary>
        public PiVersion RaspberryPiVersion { get; }

        /// <summary>
        /// Gets the Wiring Pi board revision (1 or 2).
        /// </summary>
        /// <value>
        /// The wiring pi board revision.
        /// </value>
        public int WiringPiBoardRevision { get; private set; }

        /// <summary>
        /// Gets the number of processor cores.
        /// </summary>
        public int ProcessorCount
        {
            get
            {
                int outIndex;
                if (int.TryParse(Processor, out outIndex))
                {
                    return outIndex + 1;
                }

                return 0;
            }
        }



        /// <summary>
        /// Gets the installed ram in bytes.
        /// </summary>
        public int InstalledRam { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this CPU is little endian.
        /// </summary>
        public bool IsLittleEndian => BitConverter.IsLittleEndian;

        /// <summary>
        /// Placeholder for processor index
        /// </summary>
        private string Processor { get; set; }

        /// <summary>
        /// Gets the CPU model name.
        /// </summary>
        public string ModelName { get; private set; }

        /// <summary>
        /// Gets a list of supported CPU features.
        /// </summary>
        public string[] Features { get; private set; }

        /// <summary>
        /// Gets the CPU implementer hex code.
        /// </summary>
        public string CpuImplementer { get; private set; }

        /// <summary>
        /// Gets the CPU architecture code.
        /// </summary>
        public string CpuArchitecture { get; private set; }

        /// <summary>
        /// Gets the CPU variant code.
        /// </summary>
        public string CpuVariant { get; private set; }

        /// <summary>
        /// Gets the CPU part code.
        /// </summary>
        public string CpuPart { get; private set; }

        /// <summary>
        /// Gets the CPU revision code.
        /// </summary>
        public string CpuRevision { get; private set; }

        /// <summary>
        /// Gets the hardware model number.
        /// </summary>
        public string Hardware { get; private set; }

        /// <summary>
        /// Gets the hardware revision number.
        /// </summary>
        public string Revision { get; private set; }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        public string Serial { get; private set; }

        /// <summary>
        /// Gets the uptime (at seconds).
        /// </summary>
        /// <value>
        /// The uptime.
        /// </value>
        public ulong Uptime
        {
            get
            {
                try
                {
                    utssysinfo sysInfo;
                    if (Standard.sysinfo(out sysInfo) == 0)
                        return sysInfo.uptime;
                }
                catch
                {
                    /* Ignore */
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the uptime timespan.
        /// </summary>
        /// <value>
        /// The uptime time span.
        /// </value>
        public TimeSpan UptimeTimeSpan
        {
            get
            {
                var uptime = Uptime;
                var hours = (uptime / 3600);
                var mins = (uptime / 60) - (hours * 60);

                return new TimeSpan((int)hours, (int)mins, 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current assembly running on a Linux Operating System.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is Linux os; otherwise, <c>false</c>.
        /// </value>
        public bool IsLinuxOS
        {
            get
            {
                lock (SyncRoot)
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
        public bool IsRunningAsRoot
        {
            get
            {
                lock (SyncRoot)
                {
                    if (m_IsRunningAsRoot.HasValue == false)
                    {
                        try
                        {
                            m_IsRunningAsRoot = Standard.getuid() == 0;
                        }
                        catch
                        {
                            m_IsRunningAsRoot = false;
                        }
                        
                    }

                    return m_IsRunningAsRoot.Value;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var properties = typeof(SystemInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && (
                                p.PropertyType == typeof(string) ||
                                p.PropertyType == typeof(string[]) ||
                                p.PropertyType == typeof(int) ||
                                p.PropertyType == typeof(bool) ||
                                p.PropertyType == typeof(TimeSpan)
                            ))
                .ToArray();

            var properyValues = new List<string>
            {
                "System Information",
                $"\t{nameof(WiringPiVersion),-22}: {WiringPiVersion}",
                $"\t{nameof(RaspberryPiVersion),-22}: {RaspberryPiVersion}"
            };

            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(string[]))
                {
                    properyValues.Add($"\t{property.Name,-22}: {property.GetValue(this)}");
                }
                else
                {
                    var allValues = property.GetValue(this) as string[];
                    if (allValues != null)
                    {
                        var concatValues = string.Join(" ", allValues);
                        properyValues.Add($"\t{property.Name,-22}: {concatValues}");
                    }
                }
            }

            return string.Join(Environment.NewLine, properyValues.ToArray());
        }
    }
}