namespace Unosquare.RaspberryIO.Gpio
{
    using Swan.Abstractions;

    /// <summary>
    /// The SPI Bus containing the 2 SPI channels
    /// </summary>
    public class SpiBus : SingletonBase<SpiBus>
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="SpiBus"/> class from being created.
        /// </summary>
        private SpiBus()
        {
            // placeholder
        }

        #region SPI Access

        /// <summary>
        /// Gets or sets the channel 0 frequency in Hz.
        /// </summary>
        /// <value>
        /// The channel0 frequency.
        /// </value>
        public int Channel0Frequency { get; set; }

        /// <summary>
        /// Gets the SPI bus on channel 1.
        /// </summary>
        /// <value>
        /// The channel0.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Channel0Frequency</exception>
        public SpiChannel Channel0
        {
            get
            {
                if (Channel0Frequency == 0)
                {
                    Channel0Frequency = SpiChannel.DefaultFrequency;

                    // throw new InvalidOperationException($"Set the {nameof(Channel0Frequency)} between {SpiChannel.MinFrequency} and {SpiChannel.MaxFrequency} before using the SPI bus.");
                }

                return SpiChannel.Retrieve(SpiChannelNumber.Channel0, Channel0Frequency);
            }
        }

        /// <summary>
        /// Gets or sets the channel 1 frequency in Hz
        /// </summary>
        /// <value>
        /// The channel1 frequency.
        /// </value>
        public int Channel1Frequency { get; set; }

        /// <summary>
        /// Gets the SPI bus on channel 1.
        /// </summary>
        /// <value>
        /// The channel1.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Channel1Frequency</exception>
        public SpiChannel Channel1
        {
            get
            {
                if (Channel1Frequency == 0)
                {
                    Channel1Frequency = SpiChannel.DefaultFrequency;

                    // throw new InvalidOperationException($"Set the {nameof(Channel1Frequency)} between {SpiChannel.MinFrequency} and {SpiChannel.MaxFrequency} before using the SPI bus.");
                }

                return SpiChannel.Retrieve(SpiChannelNumber.Channel1, Channel1Frequency);
            }
        }

        #endregion
    }
}
