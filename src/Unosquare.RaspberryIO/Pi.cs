namespace Unosquare.RaspberryIO
{
    using Abstractions;
    using Camera;
    using Computer;
    using Swan.Components;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Our main character. Provides access to the Raspberry Pi's GPIO, system and board information and Camera.
    /// </summary>
    public static class Pi
    {
        private const string MissingDependenciesMessage = "You need to load a valid assembly (WiringPi or PiGPIO)";
        private static readonly object SyncLock = new object();

        /// <summary>
        /// Initializes static members of the <see cref="Pi" /> class.
        /// </summary>
        static Pi()
        {
            lock (SyncLock)
            {
                Info = SystemInfo.Instance;
                Camera = CameraController.Instance;
                PiDisplay = DsiDisplay.Instance;
            }
        }

        /// <summary>
        /// Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
        /// </summary>
        public static IGpioController Gpio
        {
            get
            {
                if (!DependencyContainer.Current.CanResolve<IGpioController>())
                    throw new InvalidOperationException(MissingDependenciesMessage);

                return DependencyContainer.Current.Resolve<IGpioController>();
            }
        }

        /// <summary>
        /// Provides information on this Raspberry Pi's CPU and form factor.
        /// </summary>
        public static SystemInfo Info { get; }

        /// <summary>
        /// Provides access to the 2-channel SPI bus.
        /// </summary>
        public static ISpiBus Spi
        {
            get
            {
                if (!DependencyContainer.Current.CanResolve<ISpiBus>())
                    throw new InvalidOperationException(MissingDependenciesMessage);

                return DependencyContainer.Current.Resolve<ISpiBus>();
            }
        }

        /// <summary>
        /// Provides access to the functionality of the i2c bus.
        /// </summary>
        public static II2CBus I2C
        {
            get
            {
                if (!DependencyContainer.Current.CanResolve<II2CBus>())
                    throw new InvalidOperationException(MissingDependenciesMessage);

                return DependencyContainer.Current.Resolve<II2CBus>();
            }
        }

        /// <summary>
        /// Provides access to timing functionallity.
        /// </summary>
        public static ITiming Timing
        {
            get
            {
                if (!DependencyContainer.Current.CanResolve<ITiming>())
                    throw new InvalidOperationException(MissingDependenciesMessage);

                return DependencyContainer.Current.Resolve<ITiming>();
            }
        }

        /// <summary>
        /// Provides access to the official Raspberry Pi Camera.
        /// </summary>
        public static CameraController Camera { get; }

        /// <summary>
        /// Provides access to the official Raspberry Pi 7-inch DSI Display.
        /// </summary>
        public static DsiDisplay PiDisplay { get; }

        /// <summary>
        /// Restarts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static async Task<ProcessResult> RestartAsync() => await ProcessRunner.GetProcessResultAsync("reboot", null, null);

        /// <summary>
        /// Restarts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static ProcessResult Restart() => RestartAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Halts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static async Task<ProcessResult> ShutdownAsync() => await ProcessRunner.GetProcessResultAsync("halt", null, null);

        /// <summary>
        /// Halts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static ProcessResult Shutdown() => ShutdownAsync().GetAwaiter().GetResult();
    }
}
