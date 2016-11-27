using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public static class Utilities
    {

        private static readonly object SyncLock = new object();
        private const string CpuInfoFilePath = "/proc/cpuinfo";

        private static bool? m_IsLinuxOS = new Nullable<bool>();
        private static bool? m_IsRunningAsRoot = new Nullable<bool>();
        private static BoardInfo m_BoardInformation = null;


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

        static public BoardInfo BoardInformation
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_BoardInformation == null)
                    {
                        var result = new BoardInfo();
                        var cpuInfoLines = File.ReadAllLines(CpuInfoFilePath);
                        var properties = typeof(BoardInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(p => p.CanWrite && p.CanRead && (p.PropertyType == typeof(string) || p.PropertyType == typeof(string[])))
                            .ToArray();
                        var propDictionary = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

                        foreach (var prop in properties)
                        {
                            propDictionary[prop.Name.Replace(" ", "").ToLowerInvariant().Trim()] = prop;
                        }

                        foreach (var line in cpuInfoLines)
                        {
                            var lineParts = line.Split(new char[] { ':' }, 2);
                            if (lineParts.Length != 2)
                                continue;

                            var propertyKey = lineParts[0].Trim();
                            var propertyStringValue = lineParts[1].Trim();

                            if (propDictionary.ContainsKey(propertyKey))
                            {
                                var property = propDictionary[propertyKey];

                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(result, propertyStringValue);
                                }
                                else if (property.PropertyType == typeof(string[]))
                                {
                                    var propertyArrayAvalue = propertyStringValue.Split(' ');
                                    property.SetValue(result, propertyArrayAvalue);
                                }

                            }
                        }

                        // TODO: RAM and RPi revision and WiringPi board revision
                        var boardVersion = 0;
                        if (string.IsNullOrWhiteSpace(result.Revision) == false && 
                            int.TryParse(
                                result.Revision.ToUpperInvariant(), 
                                NumberStyles.HexNumber,
                                CultureInfo.InvariantCulture, 
                                out boardVersion))
                        {
                            result.RaspberryPiVersion = RaspberryPiVersion.Unknown;
                            if (Enum.GetValues(typeof(RaspberryPiVersion)).Cast<int>().Contains(boardVersion))
                            {
                                result.RaspberryPiVersion = (RaspberryPiVersion)boardVersion;
                            }
                        }

                        result.WirinPiBoardRevision = Interop.piBoardRev;


                        m_BoardInformation = result;
                    }

                    return m_BoardInformation;
                }
                


            }
        }
    }
}
