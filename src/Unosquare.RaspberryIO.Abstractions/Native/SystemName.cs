namespace Unosquare.RaspberryIO.Abstractions.Native
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// OS uname structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SystemName
    {
        /// <summary>
        /// System name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string SysName;

        /// <summary>
        /// Node name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string NodeName;

        /// <summary>
        /// Release level.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string Release;

        /// <summary>
        /// Version level.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string Version;

        /// <summary>
        /// Hardware level.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string Machine;

        /// <summary>
        /// Domain name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string DomainName;
    }
}