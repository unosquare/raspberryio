namespace Unosquare.RaspberryIO.Gpio
{
    using Computer;
    using Native;
    using Swan;
    using Swan.Abstractions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a singleton of the Raspberry Pi GPIO controller 
    /// as an IReadOnlyCollection of GpioPins
    /// Low level operations are accomplished by using the Wiring Pi library.
    /// Use the Instance property to access the singleton's instance
    /// </summary>
    public sealed class GpioController : SingletonBase<GpioController>, IReadOnlyCollection<GpioPin>
    {
        #region Private Declarations

        private const string WiringPiCodesEnvironmentVariable = "WIRINGPI_CODES";
        private readonly ReadOnlyCollection<GpioPin> _pinCollection;
        private readonly Dictionary<WiringPiPin, GpioPin> _registeredPins = new Dictionary<WiringPiPin, GpioPin>();
        private static readonly object SyncRoot = new object();

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Prevents a default instance of the <see cref="GpioController" /> class from being created.
        /// It in turn initializes the controller and registers the pin -- in that order.
        /// </summary>
        /// <exception cref="System.Exception">Unable to initialize the GPIO controller.</exception>
        private GpioController()
        {
            if (_pinCollection != null)
                return;

            if (IsInitialized == false)
            {
                var initResult = Initialize(ControllerMode.DirectWithWiringPiPins);
                if (initResult == false)
                    throw new Exception("Unable to initialize the GPIO controller.");
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

            _pinCollection = new ReadOnlyCollection<GpioPin>(_registeredPins.Values.ToArray());
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
        /// Gets or sets the initialization mode.
        /// </summary>
        private static ControllerMode Mode { get; set; } = ControllerMode.NotInitialized;

        /// <summary>
        /// Short-hand method of registering pins
        /// </summary>
        /// <param name="pin">The pin.</param>
        private void RegisterPin(GpioPin pin)
        {
            if (_registeredPins.ContainsKey(pin.WiringPiPinNumber) == false)
                _registeredPins[pin.WiringPiPinNumber] = pin;
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
            if (Runtime.OS != Swan.OperatingSystem.Unix)
                throw new PlatformNotSupportedException("This library does not support the platform");

            lock (SyncRoot)
            {
                if (IsInitialized)
                    throw new InvalidOperationException($"Cannot call {nameof(Initialize)} more than once.");

#if NET452
                Environment.SetEnvironmentVariable(WiringPiCodesEnvironmentVariable, "1", EnvironmentVariableTarget.Process);
#else
                Environment.SetEnvironmentVariable(WiringPiCodesEnvironmentVariable, "1");
#endif

                int setpuResult;

                switch (mode)
                {
                    case ControllerMode.DirectWithWiringPiPins:
                    {
                        if (SystemInfo.Instance.IsRunningAsRoot == false)
                        {
                            throw new PlatformNotSupportedException(
                                $"This program must be started with root privileges for mode '{mode}'");
                        }

                        setpuResult = WiringPi.wiringPiSetup();
                        break;
                    }

                    case ControllerMode.DirectWithBcmPins:
                    {
                        if (SystemInfo.Instance.IsRunningAsRoot == false)
                        {
                            throw new PlatformNotSupportedException(
                                $"This program must be started with root privileges for mode '{mode}'");
                        }

                        setpuResult = WiringPi.wiringPiSetupGpio();
                        break;
                    }

                    case ControllerMode.DirectWithHeaderPins:
                    {
                        if (SystemInfo.Instance.IsRunningAsRoot == false)
                        {
                            throw new PlatformNotSupportedException(
                                $"This program must be started with root privileges for mode '{mode}'");
                        }

                        setpuResult = WiringPi.wiringPiSetupPhys();
                        break;
                    }

                    case ControllerMode.FileStreamWithHardwarePins:
                    {
                        setpuResult = WiringPi.wiringPiSetupSys();
                        break;
                    }

                    default:
                    {
                        throw new ArgumentException($"'{mode}' is not a valid initialization mode.");
                    }
                }

                Mode = setpuResult == 0 ? mode : ControllerMode.NotInitialized;
                return IsInitialized;
            }
        }

        #endregion

        #region Pin Addressing Methods

        /// <summary>
        /// Gets a red-only collection of all registered pins.
        /// </summary>
        public ReadOnlyCollection<GpioPin> Pins => _pinCollection;

        /// <summary>
        /// Gets the <see cref="GpioPin"/> with the specified pin number.
        /// </summary>
        /// <value>
        /// The <see cref="GpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number.</param>
        /// <returns></returns>
        public GpioPin this[WiringPiPin pinNumber] => _registeredPins[pinNumber];

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

                return _registeredPins[(WiringPiPin) pinNumber];
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
            lock (SyncRoot)
            {
                WiringPi.setPadDrive(group, value);
            }
        }

        /// <summary>
        /// This sets the “strength” of the pad drivers for a particular group of pins.
        /// There are 3 groups of pins and the drive strength is from 0 to 7.
        /// Do not use this unless you know what you are doing.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task</returns>
        public async Task SetPadDriveAsync(int group, int value)
        {
            await Task.Run(() => { SetPadDrive(group, value); });
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
            lock (SyncRoot)
            {
                if (this.Skip(0).Take(8).Any(p => p.PinMode != GpioPinDriveMode.Output))
                {
                    throw new InvalidOperationException(
                        $"All firts 8 pins (0 to 7) need their {nameof(GpioPin.PinMode)} to be set to {GpioPinDriveMode.Output}");
                }

                WiringPi.digitalWriteByte(value);
            }
        }

        /// <summary>
        /// This writes the 8-bit byte supplied to the first 8 GPIO pins.
        /// It’s the fastest way to set all 8 bits at once to a particular value,
        /// although it still takes two write operations to the Pi’s GPIO hardware.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The awaitable task</returns>
        public async Task WriteByteAsync(byte value)
        {
            await Task.Run(() => { WriteByte(value); });
        }

        /// <summary>
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins. 
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// Please note this function is undocumented and unsopported
        /// </summary>
        /// <returns>A byte from the GPIO</returns>
        /// <exception cref="System.InvalidOperationException">PinMode</exception>
        public byte ReadByte()
        {
            lock (SyncRoot)
            {
                if (this.Skip(0).Take(8).Any(p =>
                    p.PinMode != GpioPinDriveMode.Input && p.PinMode != GpioPinDriveMode.Output))
                {
                    throw new InvalidOperationException(
                        $"All firts 8 pins (0 to 7) need their {nameof(GpioPin.PinMode)} to be set to {GpioPinDriveMode.Input} or {GpioPinDriveMode.Output}");
                }

                return (byte) WiringPi.digitalReadByte();
            }
        }

        /// <summary>
        /// This reads the 8-bit byte supplied to the first 8 GPIO pins. 
        /// It’s the fastest way to get all 8 bits at once to a particular value.
        /// Please note this function is undocumented and unsopported
        /// </summary>
        /// <returns>A byte from the GPIO</returns>
        public async Task<byte> ReadByteAsync()
        {
            return await Task.Run(() => { return ReadByte(); });
        }

        #endregion

        #region IReadOnlyCollection Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<GpioPin> GetEnumerator()
        {
            return _pinCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pinCollection.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of registered pins in the controller.
        /// </summary>
        public int Count => _pinCollection.Count;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts the Wirings Pi pin number to the BCM pin number.
        /// </summary>
        /// <param name="wiringPiPinNumber">The wiring pi pin number.</param>
        /// <returns></returns>
        internal static int WiringPiToBcmPinNumber(int wiringPiPinNumber)
        {
            lock (SyncRoot)
            {
                return WiringPi.wpiPinToGpio(wiringPiPinNumber);
            }
        }

        /// <summary>
        /// Converts the Physical (Header) pin number to BCM pin number.
        /// </summary>
        /// <param name="headerPinNumber">The header pin number.</param>
        /// <returns></returns>
        internal static int HaderToBcmPinNumber(int headerPinNumber)
        {
            lock (SyncRoot)
            {
                return WiringPi.physPinToGpio(headerPinNumber);
            }
        }

        #endregion

    }
}