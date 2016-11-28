using System;

namespace Unosquare.RaspberryIO
{
    /// <summary>
    /// http://raspberry-pi-guide.readthedocs.io/en/latest/system.html
    /// </summary>
    public class SystemInfo
    {

        internal SystemInfo()
        {
            
        }

        public string Processor { get; set; }
        public string BogoMips { get; set; }
        public string[] Features { get; set; }
        public string CpuImplementer { get; set; }
        public string CpuArchitecture { get; set; }
        public string CpuVariant { get; set; }
        public string CpuPart { get; set; }
        public string CpuRevision { get; set; }
        public string Hardware { get; set; }
        public string Revision { get; set; }
        public string SerialNumber { get; set; }
        public RaspberryPiVersion RaspberryPiVersion { get; set; }
        public int WiringPiBoardRevision { get; set; }
        public int InstalledRam { get; set; }
        public bool IsLittleEndian { get { return BitConverter.IsLittleEndian; } }
    }
}
