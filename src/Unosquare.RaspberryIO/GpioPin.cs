namespace Unosquare.RaspberryIO
{
    using System;
    using System.Security;

    /// <summary>
    /// Represents a GPIO Pin, its location and its capabilities.
    /// Full pin reference avaliable here:
    /// http://pinout.xyz/pinout/pin31_gpio6 and  http://wiringpi.com/pins/
    /// </summary>
    public sealed class GpioPin
    {
        #region Static Definitions

        static private readonly object SyncLock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioPin"/> class.
        /// </summary>
        /// <param name="wiringPiPinNumber">The wiring pi pin number.</param>
        /// <param name="headerPinNumber">The header pin number.</param>
        private GpioPin(WiringPiPin wiringPiPinNumber, int headerPinNumber)
        {
            PinNumber = wiringPiPinNumber;
            BcmPinNumber = Utilities.WiringPiToBcmPinNumber((int)wiringPiPinNumber);
            HeaderPinNumber = headerPinNumber;
            Header = ((int)PinNumber >= 17 && (int)PinNumber <= 20) ? 
                GpioHeader.P5 : GpioHeader.P1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the WiringPi Pin number
        /// </summary>
        public WiringPiPin PinNumber { get; protected set; }
        /// <summary>
        /// Gets the BCM chip (hardware) pin number.
        /// </summary>
        public int BcmPinNumber { get; protected set; }
        /// <summary>
        /// Gets or the physical header (physical board) pin number.
        /// </summary>
        public int HeaderPinNumber { get; protected set; }
        /// <summary>
        /// Gets the pin's header (physical board) location.
        /// </summary>
        public GpioHeader Header { get; protected set; }
        /// <summary>
        /// Gets the friendly name of the pin.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets the hardware mode capabilities of this pin.
        /// </summary>
        public PinCapability[] Capabilities { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this pin has an exclusive lock by the Lock Owner
        /// Returns false if the pin is not lock for exclusive use 
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked { get; protected set; } = false;

        /// <summary>
        /// Gets the name of the owner of the current exclusive use lock.
        /// Returns null if no locking is in place
        /// </summary>
        /// <value>
        /// The lock owner.
        /// </value>
        public string LockOwner { get; protected set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// Locks the pin for exclusive use of the specified lock owner.
        /// </summary>
        /// <param name="lockOwner">The lock owner.</param>
        /// <exception cref="System.ArgumentException">lockOwner</exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        public void Lock(string lockOwner)
        {
            if (string.IsNullOrWhiteSpace(lockOwner))
                throw new ArgumentException($"Argument '{nameof(lockOwner)}' cannot be null or empty.");

            lock (SyncLock)
            {
                if (IsLocked)
                    throw new SecurityException($"Pin {PinNumber} has already acquired a lock by '{LockOwner}'");

                IsLocked = true;
                LockOwner = lockOwner;
            }
        }

        /// <summary>
        /// Releases the lock from exclusive lock owner use.
        /// </summary>
        public void Release()
        {
            lock (SyncLock)
            {
                IsLocked = false;
                LockOwner = null;
            }
        }

        #endregion

        #region Static Pin Definitions

        static internal readonly GpioPin Pin08 = new GpioPin(WiringPiPin.Pin08, 3)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSDA },
            Name = "BCM 2 (I2C Data)"
        };

        static internal readonly GpioPin Pin09 = new GpioPin(WiringPiPin.Pin09, 5)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSCL },
            Name = "BCM 3 (I2C Clock)"
        };

        static internal readonly GpioPin Pin07 = new GpioPin(WiringPiPin.Pin07, 7)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 4"
        };

        static internal readonly GpioPin Pin00 = new GpioPin(WiringPiPin.Pin00, 11)
        {
            Capabilities = new PinCapability[] { PinCapability.GP, PinCapability.UARTRTS },
            Name = "BCM 17"
        };

        static internal readonly GpioPin Pin02 = new GpioPin(WiringPiPin.Pin02, 13)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 27"
        };

        static internal readonly GpioPin Pin03 = new GpioPin(WiringPiPin.Pin03, 15)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 22"
        };

        static internal readonly GpioPin Pin12 = new GpioPin(WiringPiPin.Pin12, 19)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMOSI },
            Name = "BCM 10"
        };

        static internal readonly GpioPin Pin13 = new GpioPin(WiringPiPin.Pin13, 21)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMISO },
            Name = "BCM 9"
        };

        static internal readonly GpioPin Pin14 = new GpioPin(WiringPiPin.Pin14, 23)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICLK },
            Name = "BCM 11"
        };

        static internal readonly GpioPin Pin30 = new GpioPin(WiringPiPin.Pin30, 27)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSDA },
            Name = "BCM 0 (HAT EEPROM i2c Data)"
        };

        static internal readonly GpioPin Pin31 = new GpioPin(WiringPiPin.Pin31, 28)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSCL },
            Name = "BCM 1 (HAT EEPROM i2c Clock)"
        };

        static internal readonly GpioPin Pin11 = new GpioPin(WiringPiPin.Pin11, 26)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICS },
            Name = "BCM 7 (SPI Chip Select 1)"
        };

        static internal readonly GpioPin Pin10 = new GpioPin(WiringPiPin.Pin10, 24)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICS },
            Name = "BCM 8 (SPI Chip Select 0)"
        };

        static internal readonly GpioPin Pin06 = new GpioPin(WiringPiPin.Pin06, 22)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 25"
        };

        static internal readonly GpioPin Pin05 = new GpioPin(WiringPiPin.Pin05, 18)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 24"
        };

        static internal readonly GpioPin Pin04 = new GpioPin(WiringPiPin.Pin04, 16)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 23"
        };

        static internal readonly GpioPin Pin01 = new GpioPin(WiringPiPin.Pin01, 12)
        {
            Capabilities = new PinCapability[] { PinCapability.GP, PinCapability.PWM },
            Name = "BCM 18 (PWM0)"
        };

        static internal readonly GpioPin Pin16 = new GpioPin(WiringPiPin.Pin16, 10)
        {
            Capabilities = new PinCapability[] { PinCapability.UARTRXD },
            Name = "BCM 15 (UART Receive)"
        };

        static internal readonly GpioPin Pin15 = new GpioPin(WiringPiPin.Pin15, 8)
        {
            Capabilities = new PinCapability[] { PinCapability.UARTTXD },
            Name = "BCM 14 (UART Transmit)"
        };

        static internal readonly GpioPin Pin21 = new GpioPin(WiringPiPin.Pin21, 29)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 5"
        };

        static internal readonly GpioPin Pin22 = new GpioPin(WiringPiPin.Pin22, 31)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 6"
        };

        static internal readonly GpioPin Pin23 = new GpioPin(WiringPiPin.Pin23, 33)
        {
            Capabilities = new PinCapability[] { PinCapability.GP, PinCapability.PWM },
            Name = "BCM 13"
        };

        static internal readonly GpioPin Pin24 = new GpioPin(WiringPiPin.Pin24, 35)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMISO },
            Name = "BCM 19 (SPI Master-In)"
        };

        static internal readonly GpioPin Pin25 = new GpioPin(WiringPiPin.Pin25, 37)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 26"
        };

        static internal readonly GpioPin Pin29 = new GpioPin(WiringPiPin.Pin29, 40)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICLK },
            Name = "BCM 21 (SPI Clock)"
        };

        static internal readonly GpioPin Pin28 = new GpioPin(WiringPiPin.Pin28, 38)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMOSI },
            Name = "BCM 20 (SPI Master-Out)"
        };

        static internal readonly GpioPin Pin27 = new GpioPin(WiringPiPin.Pin27, 36)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 16"
        };

        static internal readonly GpioPin Pin26 = new GpioPin(WiringPiPin.Pin26, 32)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 12"
        };

        static internal readonly GpioPin Pin17 = new GpioPin(WiringPiPin.Pin17, 3)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 28"
        };

        static internal readonly GpioPin Pin18 = new GpioPin(WiringPiPin.Pin18, 4)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 29"
        };

        static internal readonly GpioPin Pin19 = new GpioPin(WiringPiPin.Pin19, 5)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 30"
        };

        static internal readonly GpioPin Pin20 = new GpioPin(WiringPiPin.Pin23, 6)
        {
            Capabilities = new PinCapability[] { PinCapability.GP },
            Name = "BCM 31"
        };

        #endregion
    }
}
