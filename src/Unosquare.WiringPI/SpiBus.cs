namespace Unosquare.WiringPI
{
    using RaspberryIO.Abstractions;

    /// <summary>
    /// The SPI Bus containing the 2 SPI channels.
    /// </summary>
    public class SpiBus : ISpiBus
    {
        /// <inheritdoc />
        public int Channel0Frequency { get; set; }
        
        /// <inheritdoc />
        public int Channel1Frequency { get; set; }

        /// <summary>
        /// Gets the SPI bus on channel 1.
        /// </summary>
        /// <value>
        /// The channel0.
        /// </value>
        public ISpiChannel Channel0
        {
            get
            {
                if (Channel0Frequency == 0)
                    Channel0Frequency = SpiChannel.DefaultFrequency;

                return SpiChannel.Retrieve(SpiChannelNumber.Channel0, Channel0Frequency);
            }
        }

        /// <summary>
        /// Gets the SPI bus on channel 1.
        /// </summary>
        /// <value>
        /// The channel1.
        /// </value>
        public ISpiChannel Channel1
        {
            get
            {
                if (Channel1Frequency == 0)
                    Channel1Frequency = SpiChannel.DefaultFrequency;

                return SpiChannel.Retrieve(SpiChannelNumber.Channel1, Channel1Frequency);
            }
        }
    }
}
