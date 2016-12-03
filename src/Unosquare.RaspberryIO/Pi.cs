namespace Unosquare.RaspberryIO
{
    /// <summary>
    /// Our main character. Provides access to the Raspberry Pi's GPIO, system and board information and Camera
    /// </summary>
    public static class Pi
    {
        static internal readonly object SyncLock = new object();

        static private readonly GpioController m_GpioController;
        static private readonly SystemInfo m_Info;
        static private readonly Timing m_Timing;
        static private readonly SpiChannels m_Spi;
        static private readonly I2cBus m_I2c;

        /// <summary>
        /// Initializes the <see cref="Pi"/> class.
        /// </summary>
        static Pi()
        {
            lock (SyncLock)
            {
                m_GpioController = GpioController.Instance;
                m_Info = SystemInfo.Instance;
                m_Timing = Timing.Instance;
                m_Spi = SpiChannels.Instance;
                m_I2c = I2cBus.Instance;
            }

        }

        #region Components

        /// <summary>
        /// Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
        /// </summary>
        static public GpioController Gpio { get { return m_GpioController; } }

        /// <summary>
        /// Provides information on this Raspberry Pi's CPU and form factor.
        /// </summary>
        static public SystemInfo Info { get { return m_Info; } }

        /// <summary>
        /// Provides access to The PI's Timing and threading API
        /// </summary>
        static public Timing Timing { get { return m_Timing; } }

        /// <summary>
        /// Provides access to the 2-channel SPI bus
        /// </summary>
        static public SpiChannels Spi { get { return m_Spi; } }

        /// <summary>
        /// Provides access to the functionality of the i2c bus.
        /// </summary>
        static public I2cBus I2c { get { return m_I2c; } }

        #endregion

    }
}
