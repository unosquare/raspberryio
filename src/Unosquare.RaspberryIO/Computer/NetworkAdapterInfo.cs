namespace Unosquare.RaspberryIO.Computer
{
    using System.Net;

    /// <summary>
    /// Represents a Network Adapter.
    /// </summary>
    public class NetworkAdapterInfo
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the IP V4 address.
        /// </summary>
        public IPAddress IPv4 { get; internal set; }

        /// <summary>
        /// Gets the IP V6 address.
        /// </summary>
        public IPAddress IPv6 { get; internal set; }

        /// <summary>
        /// Gets the name of the access point.
        /// </summary>
        public string AccessPointName { get; internal set; }

        /// <summary>
        /// Gets the MAC (Physical) address.
        /// </summary>
        public string MacAddress { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is wireless.
        /// </summary>
        public bool IsWireless { get; internal set; }
    }
}
