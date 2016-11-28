namespace Unosquare.RaspberryIO
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Our main character. Represents a singleton of the Raspberry Pi GPIO controller 
    /// as an IReadOnlyCollection of GpioPins
    /// Low level operations are accomplished by using the Wiring Pi library.
    /// Use the Instance property to access the singleton's instance
    /// </summary>
    public sealed class GpioController : IReadOnlyCollection<GpioPin>
    {
        #region Static Declarations

        static private GpioController m_Instance = null;
        static private readonly ManualResetEventSlim OperationDone = new ManualResetEventSlim(true);
        static private readonly object SyncLock = new object();

        #endregion

        #region Private Declarations

        private const string WiringPiCodesEnvironmentVariable = "WIRINGPI_CODES";
        private ReadOnlyCollection<GpioPin> PinCollection = null;
        private readonly Dictionary<WiringPiPin, GpioPin> RegisteredPins = new Dictionary<WiringPiPin, GpioPin>();

        #endregion

        #region Singleton Implementation

        /// <summary>
        /// Provides access to the (singleton) GPIO Controller pins and functionality
        /// It automatically initializes the underlying library and populates the pins upon first access.
        /// This property is thread-safe
        /// </summary>
        static public GpioController Instance
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new GpioController();
                    }

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Determines if the underlyng GPIO controller has been initialized properly.
        /// </summary>
        /// <value>
        /// <c>true</c> if the controller is properly initialized; otherwise, <c>false</c>.
        /// </value>
        static public bool IsInitialized { get { lock (SyncLock) { return Mode != ControllerMode.NotInitialized; } } }

        /// <summary>
        /// Gets or sets the initialization mode.
        /// </summary>
        static private ControllerMode Mode { get; set; } = ControllerMode.NotInitialized;

        /// <summary>
        /// Gets the Raspberry Pi board information.
        /// </summary>
        static public SystemInfo System { get { return Utilities.BoardInformation; } }

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Prevents a default instance of the <see cref="GpioController"/> class from being created.
        /// It in turn initializes the controller and registers the pin -- in that order.
        /// </summary>
        /// <exception cref="System.SystemException">Unable to initialize the GPIO controller.</exception>
        private GpioController()
        {
            if (m_Instance != null)
                return;

            if (PinCollection != null)
                return;

            if (IsInitialized == false)
            {
                var initResult = Initialize(ControllerMode.DirectWithWiringPiPins);
                if (initResult == false)
                    throw new SystemException("Unable to initialize the GPIO controller.");
            }

            #region Pin Registration (32 WiringPi Pins)

            RegisterPin(GpioPin.Pin00);
            RegisterPin(GpioPin.Pin01);
            RegisterPin(GpioPin.Pin02);
            RegisterPin(GpioPin.Pin03);
            RegisterPin(GpioPin.Pin04);
            RegisterPin(GpioPin.Pin05);
            RegisterPin(GpioPin.Pin06);
            RegisterPin(GpioPin.Pin07);
            RegisterPin(GpioPin.Pin08);
            RegisterPin(GpioPin.Pin09);
            RegisterPin(GpioPin.Pin10);
            RegisterPin(GpioPin.Pin11);
            RegisterPin(GpioPin.Pin12);
            RegisterPin(GpioPin.Pin13);
            RegisterPin(GpioPin.Pin14);
            RegisterPin(GpioPin.Pin15);
            RegisterPin(GpioPin.Pin16);
            RegisterPin(GpioPin.Pin17);
            RegisterPin(GpioPin.Pin18);
            RegisterPin(GpioPin.Pin19);
            RegisterPin(GpioPin.Pin20);
            RegisterPin(GpioPin.Pin21);
            RegisterPin(GpioPin.Pin22);
            RegisterPin(GpioPin.Pin23);
            RegisterPin(GpioPin.Pin24);
            RegisterPin(GpioPin.Pin25);
            RegisterPin(GpioPin.Pin26);
            RegisterPin(GpioPin.Pin27);
            RegisterPin(GpioPin.Pin28);
            RegisterPin(GpioPin.Pin29);
            RegisterPin(GpioPin.Pin30);
            RegisterPin(GpioPin.Pin31);

            #endregion

            PinCollection = new ReadOnlyCollection<GpioPin>(RegisteredPins.Values.ToArray());
        }

        /// <summary>
        /// Short-hand method of registerning pins
        /// </summary>
        /// <param name="pin">The pin.</param>
        private void RegisterPin(GpioPin pin)
        {
            RegisteredPins[pin.PinNumber] = pin;
        }

        /// <summary>
        /// Initializes the controller given the initialization mode and pin numbering scheme
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        /// <exception cref="System.PlatformNotSupportedException">
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Initialize</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private bool Initialize(ControllerMode mode)
        {
            if (Utilities.IsLinuxOS == false)
                throw new PlatformNotSupportedException($"This library does not support the platform {Environment.OSVersion.ToString()}");

            lock (SyncLock)
            {
                if (IsInitialized)
                    throw new InvalidOperationException($"Cannot call {nameof(Initialize)} more than once.");

                Environment.SetEnvironmentVariable(WiringPiCodesEnvironmentVariable, "1", EnvironmentVariableTarget.Process);

                var result = -1;

                switch (mode)
                {
                    case ControllerMode.DirectWithWiringPiPins:
                        {
                            if (Utilities.IsRunningAsRoot == false)
                                throw new PlatformNotSupportedException($"This program must be started with root privileges for mode '{mode}'");

                            result = Interop.wiringPiSetup();
                            break;
                        }
                    case ControllerMode.DirectWithBcmPins:
                        {
                            if (Utilities.IsRunningAsRoot == false)
                                throw new PlatformNotSupportedException($"This program must be started with root privileges for mode '{mode}'");

                            result = Interop.wiringPiSetupGpio();
                            break;
                        }
                    case ControllerMode.DirectWithHeaderPins:
                        {
                            if (Utilities.IsRunningAsRoot == false)
                                throw new PlatformNotSupportedException($"This program must be started with root privileges for mode '{mode}'");

                            result = Interop.wiringPiSetupPhys();
                            break;
                        }
                    case ControllerMode.FileStreamWithHardwarePins:
                        {
                            result = Interop.wiringPiSetupSys();
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException($"'{mode}' is not a valid initialization mode.");
                        }
                }

                Mode = result == 0 ? mode : ControllerMode.NotInitialized;
                return IsInitialized;
            }
        }

        #endregion

        #region Pin Addressing Methods

        /// <summary>
        /// Gets a red-only collection of all registered pins.
        /// </summary>
        public ReadOnlyCollection<GpioPin> Pins { get { return PinCollection; } }

        /// <summary>
        /// Gets the <see cref="GpioPin"/> with the specified pin number.
        /// </summary>
        /// <value>
        /// The <see cref="GpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number.</param>
        /// <returns></returns>
        public GpioPin this[WiringPiPin pinNumber]
        {
            get { return RegisteredPins[pinNumber]; }
        }

        /// <summary>
        /// Gets the <see cref="GpioPin"/> with the specified pin number.
        /// </summary>
        /// <value>
        /// The <see cref="GpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number.</param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public GpioPin this[int pinNumber]
        {
            get
            {
                if (Enum.IsDefined(typeof(WiringPiPin), pinNumber) == false)
                    throw new IndexOutOfRangeException($"Pin {pinNumber} is not registered in the GPIO controller.");

                return RegisteredPins[(WiringPiPin)pinNumber];
            }
        }

        #endregion

        #region IReadOnlyCollection Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<GpioPin> GetEnumerator() { return PinCollection.GetEnumerator(); }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() { return PinCollection.GetEnumerator(); }

        /// <summary>
        /// Gets the number of registered pins in the controller.
        /// </summary>
        public int Count { get { return PinCollection.Count; } }

        #endregion

    }
}
