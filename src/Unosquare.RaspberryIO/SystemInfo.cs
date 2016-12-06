namespace Unosquare.RaspberryIO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// http://raspberry-pi-guide.readthedocs.io/en/latest/system.html
    /// </summary>
    public sealed class SystemInfo
    {
        private const string CpuInfoFilePath = "/proc/cpuinfo";
        private const string MemInfoFilePath = "/proc/meminfo";

        private static SystemInfo m_Instance = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="SystemInfo"/> class from being created.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Could not initialize the GPIO controller</exception>
        private SystemInfo()
        {
            var controller = GpioController.Instance;
            if (GpioController.IsInitialized == false)
                throw new NotSupportedException("Could not initialize the GPIO controller");

            #region Obtain and format a property dictionary

            var properties = typeof(SystemInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CanWrite && p.CanRead && (p.PropertyType == typeof(string) || p.PropertyType == typeof(string[])))
                .ToArray();
            var propDictionary = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var prop in properties)
            {
                propDictionary[prop.Name.Replace(" ", "").ToLowerInvariant().Trim()] = prop;
            }

            #endregion

            #region Extract CPU information

            var cpuInfoLines = File.ReadAllLines(CpuInfoFilePath);

            foreach (var line in cpuInfoLines)
            {
                var lineParts = line.Split(new char[] { ':' }, 2);
                if (lineParts.Length != 2)
                    continue;

                var propertyKey = lineParts[0].Trim().Replace(" ", "");
                var propertyStringValue = lineParts[1].Trim();

                if (propDictionary.ContainsKey(propertyKey))
                {
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

            var memInfoLines = File.ReadAllLines(MemInfoFilePath);
            foreach (var line in memInfoLines)
            {
                var lineParts = line.Split(new char[] { ':' }, 2);
                if (lineParts.Length != 2)
                    continue;

                if (lineParts[0].ToLowerInvariant().Trim().Equals("memtotal") == false)
                    continue;

                var memKb = lineParts[1].ToLowerInvariant().Trim().Replace("kb", "").Trim();
                var parsedMem = 0;
                if (int.TryParse(memKb, out parsedMem))
                {
                    InstalledRam = parsedMem * 1024;
                    break;
                }

            }

            #endregion

            #region Board Version and Form Factor

            var boardVersion = 0;
            if (string.IsNullOrWhiteSpace(Revision) == false &&
                int.TryParse(
                    Revision.ToUpperInvariant(),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out boardVersion))
            {
                RaspberryPiVersion = RaspberryPiVersion.Unknown;
                if (Enum.GetValues(typeof(RaspberryPiVersion)).Cast<int>().Contains(boardVersion))
                {
                    RaspberryPiVersion = (RaspberryPiVersion)boardVersion;
                }
            }

            WiringPiBoardRevision = Interop.piBoardRev();

            #endregion

            #region Version Information

            {
                var libParts = Interop.WiringPiLibrary.Split('.');
                var major = int.Parse(libParts[libParts.Length - 2]);
                var minor = int.Parse(libParts[libParts.Length - 1]);
                var version = new Version(major, minor);
                WiringPiVersion = version;
            }

            #endregion

        }

        /// <summary>
        /// Provides access to the (singleton) Info
        /// This property is thread-safe
        /// </summary>
        internal static SystemInfo Instance
        {
            get
            {
                lock (Pi.SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new SystemInfo();
                    }

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Gets the wiring pi library version.
        /// </summary>
        public Version WiringPiVersion { get; }

        /// <summary>
        /// Gets the Raspberry Pi version.
        /// </summary>
        public RaspberryPiVersion RaspberryPiVersion { get; }

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
                var outIndex = 0;
                if (int.TryParse(Processor, out outIndex))
                {
                    return outIndex + 1;
                }
                else
                {
                    return 0;
                }
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
                    p.PropertyType == typeof(bool)
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
                    properyValues.Add($"\t{property.Name, -22}: {property.GetValue(this)}");
                }
                else
                {
                    var concatValues = string.Join(" ", property.GetValue(this) as string[]);
                    properyValues.Add($"\t{property.Name,-22}: {concatValues}");
                }
            }
            
            return string.Join(Environment.NewLine, properyValues.ToArray());
        }
    }
}
