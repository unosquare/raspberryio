namespace Unosquare.WiringPI
{
    using Native;
    using RaspberryIO.Abstractions;
    using System;

    /// <summary>
    /// Represents the WiringPI system info.
    /// </summary>
    /// <seealso cref="ISystemInfo" />
    public class SystemInfo : ISystemInfo
    {
        /// <inheritdoc />
        public int BoardRevision => WiringPi.PiBoardRev();

        /// <inheritdoc />
        public Version LibraryVersion
        {
            get
            {
                var libParts = WiringPi.WiringPiLibrary.Split('.');
                var major = int.Parse(libParts[libParts.Length - 2]);
                var minor = int.Parse(libParts[libParts.Length - 1]);
                return new Version(major, minor);
            }
        }
    }
}
