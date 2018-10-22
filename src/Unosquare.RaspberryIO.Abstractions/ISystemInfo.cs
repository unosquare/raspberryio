namespace Unosquare.RaspberryIO.Abstractions
{
    using System;

    /// <summary>
    /// Interface for system info.
    /// </summary>
    public interface ISystemInfo
    {
        /// <summary>
        /// Gets the board revision (1 or 2).
        /// </summary>
        /// <value>
        /// The wiring pi board revision.
        /// </value
        int BoardRevision { get;  }

        /// <summary>
        /// Gets the library version.
        /// </summary>
        /// <value>
        /// The library version.
        /// </value>
        Version LibraryVersion { get; }
    }
}
