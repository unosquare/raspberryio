using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    /// <summary>
    /// Based on
    /// http://pinout.xyz/pinout/pin31_gpio6
    /// and 
    /// http://wiringpi.com/pins/
    /// </summary>
    public class GpioPin
    {

        static private readonly object SyncLock = new object();

        #region Constructor

        protected GpioPin(int wiringPiPinNumber, int headerPinNumber)
        {
            PinNumber = wiringPiPinNumber;
            BcmPinNumber = Utilities.WiringPiToBcmPinNumber(wiringPiPinNumber);
            HeaderPinNumber = headerPinNumber;
            Header = (PinNumber >= 17 && PinNumber <= 20) ? 
                GpioHeader.P5 : GpioHeader.P1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the WiringPi Pin number
        /// </summary>
        public int PinNumber { get; protected set; }
        public int BcmPinNumber { get; protected set; }
        public int HeaderPinNumber { get; protected set; }
        public GpioHeader Header { get; protected set; }
        public string Name { get; protected set; }
        public PinCapability[] Capabilities { get; protected set; }
        public bool IsLocked { get; protected set; } = false;
        public string LockOwner { get; protected set; } = null;

        #endregion

        #region Methods

        public void Lock(string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException($"Argument '{nameof(owner)}' cannot be null or empty.");

            lock (SyncLock)
            {
                if (IsLocked)
                    throw new SecurityException($"Pin {PinNumber} has already acquired a lock by '{LockOwner}'");

                IsLocked = true;
                LockOwner = owner;
            }
        }

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

        static internal readonly GpioPin Pin08 = new GpioPin(8, 3)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSDA },
            Name = "BCM 2 (I2C Data)"
        };

        static internal readonly GpioPin Pin09 = new GpioPin(9, 5)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSCL },
            Name = "BCM 3 (I2C Clock)"
        };

        static internal readonly GpioPin Pin07 = new GpioPin(7, 7)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 4"
        };

        static internal readonly GpioPin Pin00 = new GpioPin(0, 11)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose, PinCapability.UARTRTS },
            Name = "BCM 17"
        };

        static internal readonly GpioPin Pin02 = new GpioPin(2, 13)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 27"
        };

        static internal readonly GpioPin Pin03 = new GpioPin(3, 15)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 22"
        };

        static internal readonly GpioPin Pin12 = new GpioPin(12, 19)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMOSI },
            Name = "BCM 10"
        };

        static internal readonly GpioPin Pin13 = new GpioPin(13, 21)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMISO },
            Name = "BCM 9"
        };

        static internal readonly GpioPin Pin14 = new GpioPin(14, 23)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICLK },
            Name = "BCM 11"
        };

        static internal readonly GpioPin Pin30 = new GpioPin(30, 27)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSDA },
            Name = "BCM 0 (HAT EEPROM i2c Data)"
        };

        static internal readonly GpioPin Pin31 = new GpioPin(31, 28)
        {
            Capabilities = new PinCapability[] { PinCapability.I2CSCL },
            Name = "BCM 1 (HAT EEPROM i2c Clock)"
        };

        static internal readonly GpioPin Pin11 = new GpioPin(11, 26)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICS },
            Name = "BCM 7 (SPI Chip Select 1)"
        };

        static internal readonly GpioPin Pin10 = new GpioPin(10, 24)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICS },
            Name = "BCM 8 (SPI Chip Select 0)"
        };

        static internal readonly GpioPin Pin06 = new GpioPin(6, 22)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 25"
        };

        static internal readonly GpioPin Pin05 = new GpioPin(5, 18)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 24"
        };

        static internal readonly GpioPin Pin04 = new GpioPin(4, 16)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 23"
        };

        static internal readonly GpioPin Pin01 = new GpioPin(1, 12)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose, PinCapability.PWM },
            Name = "BCM 18 (PWM0)"
        };

        static internal readonly GpioPin Pin16 = new GpioPin(16, 10)
        {
            Capabilities = new PinCapability[] { PinCapability.UARTRXD },
            Name = "BCM 15 (UART Receive)"
        };

        static internal readonly GpioPin Pin15 = new GpioPin(15, 8)
        {
            Capabilities = new PinCapability[] { PinCapability.UARTTXD },
            Name = "BCM 14 (UART Transmit)"
        };

        static internal readonly GpioPin Pin21 = new GpioPin(21, 29)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 5"
        };

        static internal readonly GpioPin Pin22 = new GpioPin(22, 31)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 6"
        };

        static internal readonly GpioPin Pin23 = new GpioPin(23, 33)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose, PinCapability.PWM },
            Name = "BCM 13"
        };

        static internal readonly GpioPin Pin24 = new GpioPin(24, 35)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMISO },
            Name = "BCM 19 (SPI Master-In)"
        };

        static internal readonly GpioPin Pin25 = new GpioPin(25, 37)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 26"
        };

        static internal readonly GpioPin Pin29 = new GpioPin(29, 40)
        {
            Capabilities = new PinCapability[] { PinCapability.SPICLK },
            Name = "BCM 21 (SPI Clock)"
        };

        static internal readonly GpioPin Pin28 = new GpioPin(28, 38)
        {
            Capabilities = new PinCapability[] { PinCapability.SPIMOSI },
            Name = "BCM 20 (SPI Master-Out)"
        };

        static internal readonly GpioPin Pin27 = new GpioPin(27, 36)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 16"
        };

        static internal readonly GpioPin Pin26 = new GpioPin(26, 32)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 12"
        };

        static internal readonly GpioPin Pin17 = new GpioPin(17, 3)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 28"
        };

        static internal readonly GpioPin Pin18 = new GpioPin(18, 4)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 29"
        };

        static internal readonly GpioPin Pin19 = new GpioPin(19, 5)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 30"
        };

        static internal readonly GpioPin Pin20 = new GpioPin(23, 6)
        {
            Capabilities = new PinCapability[] { PinCapability.GeneralPurpose },
            Name = "BCM 31"
        };

        #endregion
    }
}
