namespace Unosquare.RaspberryIO
{
    using Abstractions;
    using Camera;
    using Computer;
    using Swan;
    using Swan.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Our main character. Provides access to the Raspberry Pi's GPIO, system and board information and Camera.
    /// </summary>
    public static class Pi
    {
        private const string MissingDependenciesMessage = "You need to load a valid assembly (WiringPi or PiGPIO).";
        private static readonly object SyncLock = new object();
        private static bool _isInit;
        private static SystemInfo _info;

        /// <summary>
        /// Initializes static members of the <see cref="Pi" /> class.
        /// </summary>
        static Pi()
        {
            lock (SyncLock)
            {
                Camera = CameraController.Instance;
                PiDisplay = DsiDisplay.Instance;
                Audio = AudioSettings.Instance;
                Bluetooth = Bluetooth.Instance;
            }
        }

        /// <summary>
        /// Provides information on this Raspberry Pi's CPU and form factor.
        /// </summary>
        public static SystemInfo Info => _info ??= SystemInfo.Instance;

        /// <summary>
        /// Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
        /// </summary>
        public static IGpioController Gpio =>
            ResolveDependency<IGpioController>();

        /// <summary>
        /// Provides access to the 2-channel SPI bus.
        /// </summary>
        public static ISpiBus Spi =>
            ResolveDependency<ISpiBus>();

        /// <summary>
        /// Provides access to the functionality of the i2c bus.
        /// </summary>
        public static II2CBus I2C =>
            ResolveDependency<II2CBus>();

        /// <summary>
        /// Provides access to timing functionality.
        /// </summary>
        public static ITiming Timing =>
            ResolveDependency<ITiming>();

        /// <summary>
        /// Provides access to threading functionality.
        /// </summary>
        public static IThreading Threading =>
            ResolveDependency<IThreading>();

        /// <summary>
        /// Provides access to the official Raspberry Pi Camera.
        /// </summary>
        public static CameraController Camera { get; }

        /// <summary>
        /// Provides access to the official Raspberry Pi 7-inch DSI Display.
        /// </summary>
        public static DsiDisplay PiDisplay { get; }

        /// <summary>
        /// Provides access to Raspberry Pi ALSA sound card driver.
        /// </summary>
        public static AudioSettings Audio { get; }

        /// <summary>
        /// Provides access to Raspberry Pi Bluetooth driver.
        /// </summary>
        public static Bluetooth Bluetooth { get;  }

        /// <summary>
        /// Restarts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static Task<ProcessResult> RestartAsync() => ProcessRunner.GetProcessResultAsync("reboot", null, null);

        /// <summary>
        /// Restarts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static ProcessResult Restart() => RestartAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Halts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static Task<ProcessResult> ShutdownAsync() => ProcessRunner.GetProcessResultAsync("halt", null, null);

        /// <summary>
        /// Halts the Pi. Must be running as SU.
        /// </summary>
        /// <returns>The process result.</returns>
        public static ProcessResult Shutdown() => ShutdownAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Initializes an Abstractions implementation.
        /// </summary>
        /// <typeparam name="T">An implementation of <see cref="IBootstrap"/>.</typeparam>
        public static void Init<T>()
            where T : IBootstrap
        {
            lock (SyncLock)
            {
                if (_isInit) return;

                Activator.CreateInstance<T>().Bootstrap();
                _isInit = true;
            }
        }

        private static T ResolveDependency<T>()
            where T : class
        {
            if (!_isInit)
                throw new InvalidOperationException($"You must first initialize {nameof(Pi)} referencing a valid {nameof(IBootstrap)} implementation.");

            return DependencyContainer.Current.CanResolve<T>()
                ? DependencyContainer.Current.Resolve<T>()
                : throw new InvalidOperationException(MissingDependenciesMessage);
        }
    }
}
