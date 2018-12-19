namespace Unosquare.RaspberryIO.Computer
{
    using Abstractions;
    using Abstractions.Native;
    using Swan.Abstractions;
    using Swan.Components;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// http://raspberry-pi-guide.readthedocs.io/en/latest/system.html.
    /// </summary>
    public sealed class SystemInfo : SingletonBase<SystemInfo>
    {
        private const string CpuInfoFilePath = "/proc/cpuinfo";
        private const string MemInfoFilePath = "/proc/meminfo";
        private const string UptimeFilePath = "/proc/uptime";

        /// <summary>
        /// Prevents a default instance of the <see cref="SystemInfo"/> class from being created.
        /// </summary>
        /// <exception cref="NotSupportedException">Could not initialize the GPIO controller.</exception>
        private SystemInfo()
        {
            #region Obtain and format a property dictionary

            var properties =
                typeof(SystemInfo).GetTypeInfo()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(
                        p =>
                            p.CanWrite && p.CanRead &&
                            (p.PropertyType == typeof(string) || p.PropertyType == typeof(string[])))
                    .ToArray();
            var propDictionary = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var prop in properties)
            {
                propDictionary[prop.Name.Replace(" ", string.Empty).ToLowerInvariant().Trim()] = prop;
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

                    var propertyKey = lineParts[0].Trim().Replace(" ", string.Empty);
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

                    var memKb = lineParts[1].ToLowerInvariant().Trim().Replace("kb", string.Empty).Trim();

                    if (int.TryParse(memKb, out var parsedMem))
                    {
                        InstalledRam = parsedMem * 1024;
                        break;
                    }
                }
            }

            #endregion

            #region Board Version and Form Factor

            var hasSysInfo = DependencyContainer.Current.CanResolve<ISystemInfo>();
            try
            {
                if (string.IsNullOrWhiteSpace(Revision) == false &&
                    int.TryParse(
                        Revision.ToUpperInvariant(),
                        NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture,
                        out var boardVersion))
                {
                    RaspberryPiVersion = PiVersion.Unknown;
                    if (Enum.GetValues(typeof(PiVersion)).Cast<int>().Contains(boardVersion))
                    {
                        RaspberryPiVersion = (PiVersion)boardVersion;
                    }
                }

                if (hasSysInfo)
                    BoardRevision = (int)DependencyContainer.Current.Resolve<ISystemInfo>().BoardRevision;
            }
            catch
            {
                /* Ignore */
            }

            #endregion

            #region Version Information

            if (hasSysInfo)
                LibraryVersion = DependencyContainer.Current.Resolve<ISystemInfo>().LibraryVersion;

            #endregion

            #region Extract OS Info

            try
            {
                Standard.Uname(out var unameInfo);

                OperatingSystem = new OsInfo
                {
                    DomainName = unameInfo.DomainName,
                    Machine = unameInfo.Machine,
                    NodeName = unameInfo.NodeName,
                    Release = unameInfo.Release,
                    SysName = unameInfo.SysName,
                    Version = unameInfo.Version,
                };
            }
            catch
            {
                OperatingSystem = new OsInfo();
            }

            #endregion
        }

        /// <summary>
        /// Gets the library version.
        /// </summary>
        public Version LibraryVersion { get; }

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
        /// Gets the board revision (1 or 2).
        /// </summary>
        /// <value>
        /// The wiring pi board revision.
        /// </value>
        public int BoardRevision { get; }

        /// <summary>
        /// Gets the number of processor cores.
        /// </summary>
        public int ProcessorCount => int.TryParse(Processor, out var outIndex) ? outIndex + 1 : 0;

        /// <summary>
        /// Gets the installed ram in bytes.
        /// </summary>
        public int InstalledRam { get; }

        /// <summary>
        /// Gets a value indicating whether this CPU is little endian.
        /// </summary>
        public bool IsLittleEndian => BitConverter.IsLittleEndian;

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
        /// Gets the system up-time (in seconds).
        /// </summary>
        public double Uptime
        {
            get
            {
                try
                {
                    if (File.Exists(UptimeFilePath) == false) return 0;
                    var parts = File.ReadAllText(UptimeFilePath).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 1 && float.TryParse(parts[0], out var result))
                        return result;
                }
                catch
                {
                    /* Ignore */
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the uptime in TimeSpan.
        /// </summary>
        public TimeSpan UptimeTimeSpan => TimeSpan.FromSeconds(Uptime);

        /// <summary>
        /// Placeholder for processor index.
        /// </summary>
        private string Processor { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var properties = typeof(SystemInfo).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && (
                                p.PropertyType == typeof(string) ||
                                p.PropertyType == typeof(string[]) ||
                                p.PropertyType == typeof(int) ||
                                p.PropertyType == typeof(bool) ||
                                p.PropertyType == typeof(TimeSpan)))
                .ToArray();

            var propertyValues2 = new List<string>
            {
                "System Information",
                $"\t{nameof(LibraryVersion),-22}: {LibraryVersion}",
                $"\t{nameof(RaspberryPiVersion),-22}: {RaspberryPiVersion}",
            };

            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(string[]))
                {
                    propertyValues2.Add($"\t{property.Name,-22}: {property.GetValue(this)}");
                }
                else if (property.GetValue(this) is string[] allValues)
                {
                    var concatValues = string.Join(" ", allValues);
                    propertyValues2.Add($"\t{property.Name,-22}: {concatValues}");
                }
            }

            return string.Join(Environment.NewLine, propertyValues2.ToArray());
        }
    }
}