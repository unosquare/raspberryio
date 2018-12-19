namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interfaces a SPI Bus containing the 2 SPI channels.
    /// </summary>
    public interface ISpiBus
    {
        /// <summary>
        /// Gets the default frequency.
        /// </summary>
        /// <value>
        /// The default frequency.
        /// </value>
        int DefaultFrequency { get; }

        /// <summary>
        /// Gets or sets the channel 0 frequency in Hz.
        /// </summary>
        /// <value>
        /// The channel0 frequency.
        /// </value>
        int Channel0Frequency { get; set; }
        
        /// <summary>
        /// Gets or sets the channel 1 frequency in Hz.
        /// </summary>
        /// <value>
        /// The channel1 frequency.
        /// </value>
        int Channel1Frequency { get; set; }
        
        /// <summary>
        /// Gets the SPI bus on channel 0.
        /// </summary>
        /// <value>
        /// The channel0.
        /// </value>
        ISpiChannel Channel0 { get; }

        /// <summary>
        /// Gets the SPI bus on channel 1.
        /// </summary>
        /// <value>
        /// The channel0.
        /// </value>
        ISpiChannel Channel1 { get; }
    }
}
