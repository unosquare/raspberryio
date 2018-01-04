namespace Unosquare.RaspberryIO
{
    using Camera;
    using Computer;
    using Gpio;
    using Native;
    using System.Threading.Tasks;
    using Unosquare.Swan.Components;

    /// <summary>
    /// Our main character. Provides access to the Raspberry Pi's GPIO, system and board information and Camera
    /// </summary>
    public static class Pi
    {
        internal static string LoggerSource = typeof(Pi).Namespace;
        private static readonly object SyncLock = new object();

        /// <summary>
        /// Initializes static members of the <see cref="Pi" /> class.
        /// </summary>
        static Pi()
        {
            lock (SyncLock)
            {
                // Extraction of embedded resources
                Resources.EmbeddedResources.ExtractAll();

                // Instance assignments
                Gpio = GpioController.Instance;
                Info = SystemInfo.Instance;
                Timing = Timing.Instance;
                Spi = SpiBus.Instance;
                I2C = I2CBus.Instance;
                Camera = CameraController.Instance;
                PiDisplay = DsiDisplay.Instance;
            }
        }

        /// <summary>
        /// Restarts the Pi. Must be running as SU
        /// </summary>
        /// <returns>The process result</returns>
        public static async Task<ProcessResult> RestartAsync()
        {
            return await ProcessRunner.GetProcessResultAsync("reboot");
        }

        /// <summary>
        /// Restarts the Pi. Must be running as SU
        /// </summary>
        /// <returns>The process result</returns>
        public static ProcessResult Restart()
        {
            return RestartAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Halts the Pi. Must be running as SU
        /// </summary>
        /// <returns>The process result</returns>
        public static async Task<ProcessResult> ShutdownAsync()
        {
            return await ProcessRunner.GetProcessResultAsync("halt");
        }

        /// <summary>
        /// Halts the Pi. Must be running as SU
        /// </summary>
        /// <returns>The process result</returns>
        public static ProcessResult Shutdown()
        {
            return ShutdownAsync().GetAwaiter().GetResult();
        }

        #region Components

        /// <summary>
        /// Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
        /// </summary>
        public static GpioController Gpio { get; }

        /// <summary>
        /// Provides information on this Raspberry Pi's CPU and form factor.
        /// </summary>
        public static SystemInfo Info { get; }

        /// <summary>
        /// Provides access to The PI's Timing and threading API
        /// </summary>
        public static Timing Timing { get; }

        /// <summary>
        /// Provides access to the 2-channel SPI bus
        /// </summary>
        public static SpiBus Spi { get; }

        /// <summary>
        /// Provides access to the functionality of the i2c bus.
        /// </summary>
        public static I2CBus I2C { get; }

        /// <summary>
        /// Provides access to the official Raspberry Pi Camera
        /// </summary>
        public static CameraController Camera { get; }

        /// <summary>
        /// Provides access to the official Raspberry Pi 7-inch DSI Display
        /// </summary>
        public static DsiDisplay PiDisplay { get; }

        #endregion

    }
}
