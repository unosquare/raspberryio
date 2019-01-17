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
        private const string MissingDependenciesMessage = "You need to load a valid assembly (WiringPi or PiGPIO).";
        private static readonly object SyncLock = new object();
        private static bool _isInit;

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
        /// Provides information on this Raspberry Pi's CPU and form factor.
        /// </summary>
        public static SystemInfo Info { get; }

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
        /// Provides access to timing functionallity.
        /// </summary>
        public static ITiming Timing =>
            ResolveDependency<ITiming>();

        /// <summary>
        /// Provides access to threading functionallity.
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

        /// <summary>
        /// Initializes an Abstractions implementation.
        /// </summary>
        /// <typeparam name="T">An implementation of <see cref="IBootstrap"/>.</typeparam>
        public static void Init<T>()
            where T : IBootstrap
        {
            if (_isInit)
                return;

            lock(SyncLock)
            {
                if (!_isInit)
                {
                    Activator.CreateInstance<T>().Bootstrap();
                    _isInit = true;
                }
            }
        }

        private static T ResolveDependency<T>()
            where T : class
        {
            if (!_isInit)
                throw new InvalidOperationException($"You must first initialize {nameof(Pi)} referencing a valid {nameof(IBootstrap)} implementation.");

            if (!DependencyContainer.Current.CanResolve<T>())
                throw new InvalidOperationException(MissingDependenciesMessage);

            return DependencyContainer.Current.Resolve<T>();
        }
    }
}
