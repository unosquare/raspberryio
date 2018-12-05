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
        private static int _boardRevision = -1;
        private static object _lock = new object();

        /// <inheritdoc />
        public int BoardRevision => GetBoardRevision();

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

        internal static int GetBoardRevision()
        {
            if (_boardRevision < 0)
            {
                lock (_lock)
                {
                    if (_boardRevision < 0)
                        _boardRevision = WiringPi.PiBoardRev();
                }
            }

            return _boardRevision;
        }
    }
}
