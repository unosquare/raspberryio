namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interfaces a SPI Bus containing the 2 SPI channels.
    /// </summary>
    public interface ISpiBus
    {
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

    }
}
