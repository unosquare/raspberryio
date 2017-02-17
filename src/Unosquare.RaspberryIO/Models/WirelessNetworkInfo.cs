namespace Unosquare.RaspberryIO.Models
{
    /// <summary>
    /// Represents a wireless network information
    /// </summary>
    public class WirelessNetworkInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the network quality.
        /// </summary> 
        public string Quality { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is encrypted.
        /// </summary>
        public bool IsEncrypted { get; set; }
    }
}
