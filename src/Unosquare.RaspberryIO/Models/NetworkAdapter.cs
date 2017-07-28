namespace Unosquare.RaspberryIO.Models
{
    using System.Net;

    /// <summary>
    /// Represents a Network Adapter
    /// </summary>
    public class NetworkAdapter
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the i PV4.
        /// </summary>
        public IPAddress IPv4 { get; set; }

        /// <summary>
        /// Gets or sets the i PV6.
        /// </summary>
        public IPAddress IPv6 { get; set; }

        /// <summary>
        /// Gets or sets the name of the access point.
        /// </summary>
        public string AccessPointName { get; set; }

        /// <summary>
        /// Gets or sets the mac address.
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is wireless.
        /// </summary>
        public bool IsWireless { get; set; }
    }
}
