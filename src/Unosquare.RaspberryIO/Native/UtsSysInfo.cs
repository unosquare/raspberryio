namespace Unosquare.RaspberryIO.Native
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Sysinfo POSIX struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct utssysinfo
    {
        UIntPtr _uptime;             /* Seconds since boot */
        public ulong uptime
        {
            get { return (ulong)_uptime; }
            set { _uptime = new UIntPtr(value); }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ulong[] loads;  /* 1, 5, and 15 minute load averages */
        public ulong totalram;  /* Total usable main memory size */
        public ulong freeram;   /* Available memory size */
        public ulong sharedram; /* Amount of shared memory */
        public ulong bufferram; /* Memory used by buffers */
        public ulong totalswap; /* Total swap space size */
        public ulong freeswap;  /* swap space still available */
        public ushort procs;    /* Number of current processes */
        public ulong totalhigh; /* Total high memory size */
        public ulong freehigh;  /* Available high memory size */
        public uint mem_unit;   /* Memory unit size in bytes */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public char[] _f; /* Padding to 64 bytes */
    }
}
