namespace Unosquare.RaspberryIO
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Represents a singleton of the Raspberry Pi GPIO controller 
    /// as an IReadOnlyCollection of GpioPins
    /// Low level operations are accomplished by using the Wiring Pi library.
    /// Use the Instance property to access the singleton's instance
    /// </summary>
    public sealed class GpioController : IReadOnlyCollection<GpioPin>
    {
        #region Static Declarations

        private static GpioController m_Instance = null;
        
        #endregion

        #region Private Declarations

        private const string WiringPiCodesEnvironmentVariable = "WIRINGPI_CODES";
        private readonly ReadOnlyCollection<GpioPin> PinCollection = null;
        private readonly Dictionary<WiringPiPin, GpioPin> RegisteredPins = new Dictionary<WiringPiPin, GpioPin>();

        #endregion

        #region Singleton Implementation

        /// <summary>
        /// Provides access to the (singleton) GPIO Controller pins and functionality
        /// It automatically initializes the underlying library and populates the pins upon first access.
        /// This property is thread-safe
        /// </summary>
        internal static GpioController Instance
        {
            get
            {
                lock (Pi.SyncLock)
                {
                    return m_Instance ?? (m_Instance = new GpioController());
                }
            }
        }

        /// <summary>
        /// Determines if the underlying GPIO controller has been initialized properly.
        /// </summary>
        /// <value>
        /// <c>true</c> if the controller is properly initialized; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInitialized { get { lock (Pi.SyncLock) { return Mode != ControllerMode.NotInitialized; } } }

        /// <summary>
        /// Gets or sets the initialization mode.
        /// </summary>
        private static ControllerMode Mode { get; set; } = ControllerMode.NotInitialized;

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

            RegisterPin(GpioPin.Pin00.Value);
            RegisterPin(GpioPin.Pin01.Value);
            RegisterPin(GpioPin.Pin02.Value);
            RegisterPin(GpioPin.Pin03.Value);
            RegisterPin(GpioPin.Pin04.Value);
            RegisterPin(GpioPin.Pin05.Value);
            RegisterPin(GpioPin.Pin06.Value);
            RegisterPin(GpioPin.Pin07.Value);
            RegisterPin(GpioPin.Pin08.Value);
            RegisterPin(GpioPin.Pin09.Value);
            RegisterPin(GpioPin.Pin10.Value);
            RegisterPin(GpioPin.Pin11.Value);
            RegisterPin(GpioPin.Pin12.Value);
            RegisterPin(GpioPin.Pin13.Value);
            RegisterPin(GpioPin.Pin14.Value);
            RegisterPin(GpioPin.Pin15.Value);
            RegisterPin(GpioPin.Pin16.Value);
            RegisterPin(GpioPin.Pin17.Value);
            RegisterPin(GpioPin.Pin18.Value);
            RegisterPin(GpioPin.Pin19.Value);
            RegisterPin(GpioPin.Pin20.Value);
            RegisterPin(GpioPin.Pin21.Value);
            RegisterPin(GpioPin.Pin22.Value);
            RegisterPin(GpioPin.Pin23.Value);
            RegisterPin(GpioPin.Pin24.Value);
            RegisterPin(GpioPin.Pin25.Value);
            RegisterPin(GpioPin.Pin26.Value);
            RegisterPin(GpioPin.Pin27.Value);
            RegisterPin(GpioPin.Pin28.Value);
            RegisterPin(GpioPin.Pin29.Value);
            RegisterPin(GpioPin.Pin30.Value);
            RegisterPin(GpioPin.Pin31.Value);

            #endregion

            PinCollection = new ReadOnlyCollection<GpioPin>(RegisteredPins.Values.ToArray());
        }

        /// <summary>
        /// Short-hand method of registering pins
        /// </summary>
        /// <param name="pin">The pin.</param>
        private void RegisterPin(GpioPin pin)
        {
            if (RegisteredPins.ContainsKey(pin.WiringPiPinNumber) == false)
                RegisteredPins[pin.WiringPiPinNumber] = pin;
            else
                throw new InvalidOperationException($"Pin {pin.WiringPiPinNumber} has been registered");
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
                throw new PlatformNotSupportedException($"This library does not support the platform {Environment.OSVersion}");

            lock (Pi.SyncLock)
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
        public ReadOnlyCollection<GpioPin> Pins => PinCollection;

        /// <summary>
        /// Gets the <see cref="GpioPin"/> with the specified pin number.
        /// </summary>
        /// <value>
        /// The <see cref="GpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number.</param>
        /// <returns></returns>
        public GpioPin this[WiringPiPin pinNumber] => RegisteredPins[pinNumber];

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

        #region Individual Pin Properties

        /// <summary>
        /// Provides direct access to the Pin 00.
        /// </summary>
        public GpioPin Pin00 => GpioPin.Pin00.Value;

        /// <summary>
        /// Provides direct access to the Pin 01.
        /// </summary>
        public GpioPin Pin01 => GpioPin.Pin01.Value;

        /// <summary>
        /// Provides direct access to the Pin 02.
        /// </summary>
        public GpioPin Pin02 => GpioPin.Pin02.Value;

        /// <summary>
        /// Provides direct access to the Pin 03.
        /// </summary>
        public GpioPin Pin03 => GpioPin.Pin03.Value;

        /// <summary>
        /// Provides direct access to the Pin 04.
        /// </summary>
        public GpioPin Pin04 => GpioPin.Pin04.Value;

        /// <summary>
        /// Provides direct access to the Pin 05.
        /// </summary>
        public GpioPin Pin05 => GpioPin.Pin05.Value;

        /// <summary>
        /// Provides direct access to the Pin 06.
        /// </summary>
        public GpioPin Pin06 => GpioPin.Pin06.Value;

        /// <summary>
        /// Provides direct access to the Pin 07.
        /// </summary>
        public GpioPin Pin07 => GpioPin.Pin07.Value;

        /// <summary>
        /// Provides direct access to the Pin 08.
        /// </summary>
        public GpioPin Pin08 => GpioPin.Pin08.Value;

        /// <summary>
        /// Provides direct access to the Pin 09.
        /// </summary>
        public GpioPin Pin09 => GpioPin.Pin09.Value;

        /// <summary>
        /// Provides direct access to the Pin 10.
        /// </summary>
        public GpioPin Pin10 => GpioPin.Pin10.Value;

        /// <summary>
        /// Provides direct access to the Pin 11.
        /// </summary>
        public GpioPin Pin11 => GpioPin.Pin11.Value;

        /// <summary>
        /// Provides direct access to the Pin 12.
        /// </summary>
        public GpioPin Pin12 => GpioPin.Pin12.Value;

        /// <summary>
        /// Provides direct access to the Pin 13.
        /// </summary>
        public GpioPin Pin13 => GpioPin.Pin13.Value;

        /// <summary>
        /// Provides direct access to the Pin 14.
        /// </summary>
        public GpioPin Pin14 => GpioPin.Pin14.Value;

        /// <summary>
        /// Provides direct access to the Pin 15.
        /// </summary>
        public GpioPin Pin15 => GpioPin.Pin15.Value;

        /// <summary>
        /// Provides direct access to the Pin 16.
        /// </summary>
        public GpioPin Pin16 => GpioPin.Pin16.Value;

        /// <summary>
        /// Provides direct access to the Pin 17.
        /// </summary>
        public GpioPin Pin17 => GpioPin.Pin17.Value;

        /// <summary>
        /// Provides direct access to the Pin 18.
        /// </summary>
        public GpioPin Pin18 => GpioPin.Pin18.Value;

        /// <summary>
        /// Provides direct access to the Pin 19.
        /// </summary>
        public GpioPin Pin19 => GpioPin.Pin19.Value;

        /// <summary>
        /// Provides direct access to the Pin 20.
        /// </summary>
        public GpioPin Pin20 => GpioPin.Pin20.Value;

        /// <summary>
        /// Provides direct access to the Pin 21.
        /// </summary>
        public GpioPin Pin21 => GpioPin.Pin21.Value;

        /// <summary>
        /// Provides direct access to the Pin 22.
        /// </summary>
        public GpioPin Pin22 => GpioPin.Pin22.Value;

        /// <summary>
        /// Provides direct access to the Pin 23.
        /// </summary>
        public GpioPin Pin23 => GpioPin.Pin23.Value;

        /// <summary>
        /// Provides direct access to the Pin 24.
        /// </summary>
        public GpioPin Pin24 => GpioPin.Pin24.Value;

        /// <summary>
        /// Provides direct access to the Pin 25.
        /// </summary>
        public GpioPin Pin25 => GpioPin.Pin25.Value;

        /// <summary>
        /// Provides direct access to the Pin 26.
        /// </summary>
        public GpioPin Pin26 => GpioPin.Pin26.Value;

        /// <summary>
        /// Provides direct access to the Pin 27.
        /// </summary>
        public GpioPin Pin27 => GpioPin.Pin27.Value;

        /// <summary>
        /// Provides direct access to the Pin 28.
        /// </summary>
        public GpioPin Pin28 => GpioPin.Pin28.Value;

        /// <summary>
        /// Provides direct access to the Pin 29.
        /// </summary>
        public GpioPin Pin29 => GpioPin.Pin29.Value;

        /// <summary>
        /// Provides direct access to the Pin 30.
        /// </summary>
        public GpioPin Pin30 => GpioPin.Pin30.Value;

        /// <summary>
        /// Provides direct access to the Pin 31.
        /// </summary>
        public GpioPin Pin31 => GpioPin.Pin31.Value;

        #endregion

        #region Pin Group Methods (Read, Write, Pad Drive)

        /// <summary>
        /// This sets the “strength” of the pad drivers for a particular group of pins. 
        /// There are 3 groups of pins and the drive strength is from 0 to 7. 
        /// Do not use this unless you know what you are doing.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="value">The value.</param>
        public void SetPadDrive(int group, int value)
        {
            lock (Pi.SyncLock)
            {
                Interop.setPadDrive(group, value);
            }
        }

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins. 
        /// It’s the fastest way to set all 8 bits at once to a particular value, 
        /// although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.InvalidOperationException">PinMode</exception>
        public void WriteByte(byte value)
        {
            lock (Pi.SyncLock)
            {
                if (this.Skip(0).Take(8).Any(p => p.PinMode != GpioPinDriveMode.Output))
                    throw new InvalidOperationException($"All firts 8 pins (0 to 7) need their {nameof(GpioPin.PinMode)} to be set to {GpioPinDriveMode.Output}");

                Interop.digitalWriteByte(value);
            }
        }

        /// <summary>
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins. 
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// Please note this function is undocumented and unsopported
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">PinMode</exception>
        public byte ReadByte()
        {
            lock (Pi.SyncLock)
            {
                if (this.Skip(0).Take(8).Any(p => p.PinMode != GpioPinDriveMode.Input))
                    throw new InvalidOperationException($"All firts 8 pins (0 to 7) need their {nameof(GpioPin.PinMode)} to be set to {GpioPinDriveMode.Input}");

                return (byte)Interop.digitalReadByte();
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
        public int Count => PinCollection.Count;

        #endregion

    }
}
