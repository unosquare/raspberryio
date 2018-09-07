namespace Unosquare.RaspberryIO.Computer
{
    /// <summary>
    /// Represents a wireless network information.
    /// </summary>
    public class WirelessNetworkInfo
    {
        /// <summary>
        /// Gets the ESSID of the Wireless network.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the network quality.
        /// </summary>
        public string Quality { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is encrypted.
        /// </summary>
        public bool IsEncrypted { get; internal set; }
    }
}
