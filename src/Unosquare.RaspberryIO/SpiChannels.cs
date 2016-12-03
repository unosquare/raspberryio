namespace Unosquare.RaspberryIO
{
    using System;

    public class SpiChannels
    {
        private static SpiChannels m_Instance = null;

        /// <summary>
        /// Gets the singleton's instance.
        /// </summary>
        internal static SpiChannels Instance
        {
            get
            {
                lock (Pi.SyncLock)
                {
                    if (m_Instance == null)
                        m_Instance = new SpiChannels();

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SpiChannels"/> class from being created.
        /// </summary>
        private SpiChannels() { }

        #region SPI Access

        /// <summary>
        /// Gets or sets the channel 0 frequency.
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
        public SpiBus Channel0
        {
            get
            {
                if (Channel0Frequency == 0)
                    throw new InvalidOperationException($"Set the {nameof(Channel0Frequency)} between {SpiBus.MinFrequency} and {SpiBus.MaxFrequency} before using the SPI bus.");

                return SpiBus.Retrieve(SpiChannel.Channel0, Channel0Frequency);
            }
        }

        /// <summary>
        /// Gets or sets the channel 1 frequency.
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
        public SpiBus Channel1
        {
            get
            {
                if (Channel1Frequency == 0)
                    throw new InvalidOperationException($"Set the {nameof(Channel1Frequency)} between {SpiBus.MinFrequency} and {SpiBus.MaxFrequency} before using the SPI bus.");

                return SpiBus.Retrieve(SpiChannel.Channel1, Channel1Frequency);
            }
        }

        #endregion
    }
}
