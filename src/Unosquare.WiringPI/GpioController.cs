namespace Unosquare.WiringPI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Native;
    using RaspberryIO.Abstractions;
    using Swan;

    /// <summary>
    /// Represents the Raspberry Pi GPIO controller
    /// as an IReadOnlyCollection of GpioPins
    /// Low level operations are accomplished by using the Wiring Pi library.
    /// </summary>
    public sealed class GpioController : IGpioController
    {
        #region Private Declarations

        private const string WiringPiCodesEnvironmentVariable = "WIRINGPI_CODES";
        private static readonly object SyncRoot = new object();
        private readonly ReadOnlyCollection<GpioPin> _pins;

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Initializes static members of the <see cref="GpioController"/> class.
        /// </summary>
        static GpioController()
        {
            var wiringPiEdgeDetection = new Dictionary<EdgeDetection, int>();
            wiringPiEdgeDetection.Add(EdgeDetection.FallingEdge, 21);
            wiringPiEdgeDetection.Add(EdgeDetection.RisingEdge, 1);
            wiringPiEdgeDetection.Add(EdgeDetection.FallingAndRisingEdge, 3);
            WiringPiEdgeDetectionMapping = new ReadOnlyDictionary<EdgeDetection, int>(wiringPiEdgeDetection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioController"/> class.
        /// </summary>
        /// <exception cref="System.Exception">Unable to initialize the GPIO controller.</exception>
        internal GpioController()
        {
            if (_pins != null)
                return;

            if (IsInitialized == false)
            {
                var initResult = Initialize(ControllerMode.DirectWithBcmPins);
                if (initResult == false)
                    throw new Exception("Unable to initialize the GPIO controller.");
            }

            _pins = new ReadOnlyCollection<GpioPin>(
                    new List<GpioPin>()
                    {
                        GpioPin.Pin00.Value,
                        GpioPin.Pin01.Value,
                        GpioPin.Pin02.Value,
                        GpioPin.Pin03.Value,
                        GpioPin.Pin04.Value,
                        GpioPin.Pin05.Value,
                        GpioPin.Pin06.Value,
                        GpioPin.Pin07.Value,
                        GpioPin.Pin08.Value,
                        GpioPin.Pin09.Value,
                        GpioPin.Pin10.Value,
                        GpioPin.Pin11.Value,
                        GpioPin.Pin12.Value,
                        GpioPin.Pin13.Value,
                        GpioPin.Pin14.Value,
                        GpioPin.Pin15.Value,
                        GpioPin.Pin16.Value,
                        GpioPin.Pin17.Value,
                        GpioPin.Pin18.Value,
                        GpioPin.Pin19.Value,
                        GpioPin.Pin20.Value,
                        GpioPin.Pin21.Value,
                        GpioPin.Pin22.Value,
                        GpioPin.Pin23.Value,
                        GpioPin.Pin24.Value,
                        GpioPin.Pin25.Value,
                        GpioPin.Pin26.Value,
                        GpioPin.Pin27.Value,
                        GpioPin.Pin28.Value,
                        GpioPin.Pin29.Value,
                        GpioPin.Pin30.Value,
                        GpioPin.Pin31.Value,
                    });

            var headerP1 = new Dictionary<int, GpioPin>(_pins.Count);
            var headerP5 = new Dictionary<int, GpioPin>(_pins.Count);
            foreach (var pin in _pins)
            {
                if (pin.PhysicalPinNumber < 0)
                    continue;

                var header = pin.Header == GpioHeader.P1 ? headerP1 : headerP5;
                header[pin.PhysicalPinNumber] = pin;
            }

            HeaderP1 = new ReadOnlyDictionary<int, GpioPin>(headerP1);
            HeaderP5 = new ReadOnlyDictionary<int, GpioPin>(headerP5);
        }

        /// <summary>
        /// Determines if the underlying GPIO controller has been initialized properly.
        /// </summary>
        /// <value>
        /// <c>true</c> if the controller is properly initialized; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInitialized
        {
            get
            {
                lock (SyncRoot)
                {
                    return Mode != ControllerMode.NotInitialized;
                }
            }
        }

        /// <summary>
        /// Gets the wiring pi edge detection mapping.
        /// </summary>
        internal static ReadOnlyDictionary<EdgeDetection, int> WiringPiEdgeDetectionMapping { get; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the number of registered pins in the controller.
        /// </summary>
        public int Count => Pins.Count;

        /// <summary>
        /// Gets or sets the initialization mode.
        /// </summary>
        private static ControllerMode Mode { get; set; } = ControllerMode.NotInitialized;

        #endregion

        #region Pin Addressing

        /// <summary>
        /// Gets the PWM base frequency (in Hz).
        /// </summary>
        public int PwmBaseFrequency => 19200000;

        /// <summary>
        /// Gets a red-only collection of all pins.
        /// </summary>
        public ReadOnlyCollection<GpioPin> Pins => _pins;

        /// <summary>
        /// Provides all the pins on Header P1 of the Pi as a lookup by physical header pin number.
        /// This header is the main header and it is the one commonly used.
        /// </summary>
        public ReadOnlyDictionary<int, GpioPin> HeaderP1 { get; }

        /// <summary>
        /// Provides all the pins on Header P5 of the Pi as a lookup by physical header pin number.
        /// This header is the secondary header and it is rarely used.
        /// </summary>
        public ReadOnlyDictionary<int, GpioPin> HeaderP5 { get; }

        #endregion

        #region Individual Pin Properties

        /// <summary>
        /// Provides direct access to Pin known as BCM0.
        /// </summary>
        public GpioPin Pin00 => GpioPin.Pin00.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM1.
        /// </summary>
        public GpioPin Pin01 => GpioPin.Pin01.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM2.
        /// </summary>
        public GpioPin Pin02 => GpioPin.Pin02.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM3.
        /// </summary>
        public GpioPin Pin03 => GpioPin.Pin03.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM4.
        /// </summary>
        public GpioPin Pin04 => GpioPin.Pin04.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM5.
        /// </summary>
        public GpioPin Pin05 => GpioPin.Pin05.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM6.
        /// </summary>
        public GpioPin Pin06 => GpioPin.Pin06.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM7.
        /// </summary>
        public GpioPin Pin07 => GpioPin.Pin07.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM8.
        /// </summary>
        public GpioPin Pin08 => GpioPin.Pin08.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM9.
        /// </summary>
        public GpioPin Pin09 => GpioPin.Pin09.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM10.
        /// </summary>
        public GpioPin Pin10 => GpioPin.Pin10.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM11.
        /// </summary>
        public GpioPin Pin11 => GpioPin.Pin11.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM12.
        /// </summary>
        public GpioPin Pin12 => GpioPin.Pin12.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM13.
        /// </summary>
        public GpioPin Pin13 => GpioPin.Pin13.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM14.
        /// </summary>
        public GpioPin Pin14 => GpioPin.Pin14.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM15.
        /// </summary>
        public GpioPin Pin15 => GpioPin.Pin15.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM16.
        /// </summary>
        public GpioPin Pin16 => GpioPin.Pin16.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM17.
        /// </summary>
        public GpioPin Pin17 => GpioPin.Pin17.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM18.
        /// </summary>
        public GpioPin Pin18 => GpioPin.Pin18.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM19.
        /// </summary>
        public GpioPin Pin19 => GpioPin.Pin19.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM20.
        /// </summary>
        public GpioPin Pin20 => GpioPin.Pin20.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM21.
        /// </summary>
        public GpioPin Pin21 => GpioPin.Pin21.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM22.
        /// </summary>
        public GpioPin Pin22 => GpioPin.Pin22.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM23.
        /// </summary>
        public GpioPin Pin23 => GpioPin.Pin23.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM24.
        /// </summary>
        public GpioPin Pin24 => GpioPin.Pin24.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM25.
        /// </summary>
        public GpioPin Pin25 => GpioPin.Pin25.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM26.
        /// </summary>
        public GpioPin Pin26 => GpioPin.Pin26.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM27.
        /// </summary>
        public GpioPin Pin27 => GpioPin.Pin27.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM28 (available on Header P5).
        /// </summary>
        public GpioPin Pin28 => GpioPin.Pin28.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM29 (available on Header P5).
        /// </summary>
        public GpioPin Pin29 => GpioPin.Pin29.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM30 (available on Header P5).
        /// </summary>
        public GpioPin Pin30 => GpioPin.Pin30.Value;

        /// <summary>
        /// Provides direct access to Pin known as BCM31 (available on Header P5).
        /// </summary>
        public GpioPin Pin31 => GpioPin.Pin31.Value;

        #endregion

        #region Indexers

        /// <inheritdoc />
        public IGpioPin this[BcmPin bcmPin] => Pins[(int)bcmPin];

        /// <inheritdoc />
        public IGpioPin this[int bcmPinNumber]
        {
            get
            {
                if (!Enum.IsDefined(typeof(BcmPin), bcmPinNumber))
                    throw new IndexOutOfRangeException($"Pin {bcmPinNumber} is not registered in the GPIO controller.");

                return Pins[bcmPinNumber];
            }
        }

        /// <inheritdoc />
        public IGpioPin this[P1 pinNumber] => HeaderP1[(int)pinNumber];

        /// <inheritdoc />
        public IGpioPin this[P5 pinNumber] => HeaderP5[(int)pinNumber];

        /// <summary>
        /// Gets the <see cref="GpioPin"/> with the specified Wiring Pi pin number.
        /// </summary>
        /// <value>
        /// The <see cref="GpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        public GpioPin this[WiringPiPin pinNumber]
        {
            get
            {
                if (pinNumber == WiringPiPin.Unknown)
                    throw new InvalidOperationException("You can not get an unknow WiringPi pin.");

                return Pins.First(p => p.WiringPiPinNumber == pinNumber);
            }
        }

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
            lock (SyncRoot)
            {
                WiringPi.SetPadDrive(group, value);
            }
        }

        /// <summary>
        /// This sets the “strength” of the pad drivers for a particular group of pins.
        /// There are 3 groups of pins and the drive strength is from 0 to 7.
        /// Do not use this unless you know what you are doing.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task.</returns>
        public Task SetPadDriveAsync(int group, int value) =>
            Task.Run(() => SetPadDrive(group, value));

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to set all 8 bits at once to a particular value,
        /// although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="InvalidOperationException">PinMode.</exception>
        public void WriteByte(byte value)
        {
            lock (SyncRoot)
            {
                if (this.Skip(0).Take(8).Any(p => p.PinMode != GpioPinDriveMode.Output))
                {
                    throw new InvalidOperationException(
                        $"All first 8 pins (0 to 7) need their {nameof(GpioPin.PinMode)} to be set to {GpioPinDriveMode.Output}");
                }

                WiringPi.DigitalWriteByte(value);
            }
        }

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to set all 8 bits at once to a particular value,
        /// although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task.</returns>
        public Task WriteByteAsync(byte value) =>
            Task.Run(() => WriteByte(value));

        /// <summary>
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// Please note this function is undocumented and unsupported.
        /// </summary>
        /// <returns>A byte from the GPIO.</returns>
        /// <exception cref="InvalidOperationException">PinMode.</exception>
        public byte ReadByte()
        {
            lock (SyncRoot)
            {
                if (this.Skip(0).Take(8).Any(p =>
                    p.PinMode != GpioPinDriveMode.Input && p.PinMode != GpioPinDriveMode.Output))
                {
                    throw new InvalidOperationException(
                        $"All first 8 pins (0 to 7) need their {nameof(GpioPin.PinMode)} to be set to {GpioPinDriveMode.Input} or {GpioPinDriveMode.Output}");
                }

                return (byte)WiringPi.DigitalReadByte();
            }
        }

        /// <summary>
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// Please note this function is undocumented and unsupported.
        /// </summary>
        /// <returns>A byte from the GPIO.</returns>
        public Task<byte> ReadByteAsync() =>
            Task.Run(() => ReadByte());

        #endregion

        #region IReadOnlyCollection Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<GpioPin> GetEnumerator() => Pins.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IGpioPin> IEnumerable<IGpioPin>.GetEnumerator() => Pins.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => Pins.GetEnumerator();

        #endregion

        #region Helper and Init Methods

        /// <summary>
        /// Converts the Wirings Pi pin number to the BCM pin number.
        /// </summary>
        /// <param name="wiringPiPinNumber">The wiring pi pin number.</param>
        /// <returns>The converted pin.</returns>
        internal static int WiringPiToBcmPinNumber(int wiringPiPinNumber)
        {
            lock (SyncRoot)
            {
                return WiringPi.WpiPinToGpio(wiringPiPinNumber);
            }
        }

        /// <summary>
        /// Converts the Physical (Header) pin number to BCM pin number.
        /// </summary>
        /// <param name="headerPinNumber">The header pin number.</param>
        /// <returns>The converted pin.</returns>
        internal static int HaderToBcmPinNumber(int headerPinNumber)
        {
            lock (SyncRoot)
            {
                return WiringPi.PhysPinToGpio(headerPinNumber);
            }
        }

        /// <summary>
        /// Initializes the controller given the initialization mode and pin numbering scheme.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>True when successful.</returns>
        /// <exception cref="PlatformNotSupportedException">
        /// This library does not support the platform.
        /// </exception>
        /// <exception cref="InvalidOperationException">Library was already Initialized.</exception>
        /// <exception cref="ArgumentException">The init mode is invalid.</exception>
        private bool Initialize(ControllerMode mode)
        {
            if (Runtime.OS != Swan.OperatingSystem.Unix)
                throw new PlatformNotSupportedException("This library does not support the platform");

            lock (SyncRoot)
            {
                if (IsInitialized)
                    throw new InvalidOperationException($"Cannot call {nameof(Initialize)} more than once.");

                Environment.SetEnvironmentVariable(WiringPiCodesEnvironmentVariable, "1"); //TODO: EnvironmentVariableTarget.Process);
                int setupResult;

                switch (mode)
                {
                    case ControllerMode.DirectWithWiringPiPins:
                        {
                            setupResult = WiringPi.WiringPiSetup();
                            break;
                        }

                    case ControllerMode.DirectWithBcmPins:
                        {
                            setupResult = WiringPi.WiringPiSetupGpio();
                            break;
                        }

                    case ControllerMode.DirectWithHeaderPins:
                        {
                            setupResult = WiringPi.WiringPiSetupPhys();
                            break;
                        }

                    case ControllerMode.FileStreamWithHardwarePins:
                        {
                            setupResult = WiringPi.WiringPiSetupSys();
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException($"'{mode}' is not a valid initialization mode.");
                        }
                }

                Mode = setupResult == 0 ? mode : ControllerMode.NotInitialized;
                return IsInitialized;
            }
        }
        #endregion

    }
}
