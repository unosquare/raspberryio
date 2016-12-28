namespace Unosquare.RaspberryIO.Gpio
{
    using System;

    public partial class GpioPin
    {
        #region Static Pin Definitions

        internal static readonly Lazy<GpioPin> Pin08 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin08, 3)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.I2CSDA },
            Name = "BCM 2 (I2C Data)"
        });

        internal static readonly Lazy<GpioPin> Pin09 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin09, 5)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.I2CSCL },
            Name = "BCM 3 (I2C Clock)"
        });

        internal static readonly Lazy<GpioPin> Pin07 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin07, 7)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.GPCLK },
            Name = "BCM 4"
        });

        internal static readonly Lazy<GpioPin> Pin00 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin00, 11)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.UARTRTS },
            Name = "BCM 17"
        });

        internal static readonly Lazy<GpioPin> Pin02 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin02, 13)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 27"
        });

        internal static readonly Lazy<GpioPin> Pin03 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin03, 15)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 22"
        });

        internal static readonly Lazy<GpioPin> Pin12 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin12, 19)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPIMOSI },
            Name = "BCM 10"
        });

        internal static readonly Lazy<GpioPin> Pin13 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin13, 21)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPIMISO },
            Name = "BCM 9"
        });

        internal static readonly Lazy<GpioPin> Pin14 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin14, 23)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPICLK },
            Name = "BCM 11"
        });

        internal static readonly Lazy<GpioPin> Pin30 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin30, 27)
        {
            Capabilities = new[] { PinCapability.I2CSDA },
            Name = "BCM 0 (HAT EEPROM i2c Data)"
        });

        internal static readonly Lazy<GpioPin> Pin31 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin31, 28)
        {
            Capabilities = new[] { PinCapability.I2CSCL },
            Name = "BCM 1 (HAT EEPROM i2c Clock)"
        });

        internal static readonly Lazy<GpioPin> Pin11 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin11, 26)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPICS },
            Name = "BCM 7 (SPI Chip Select 1)"
        });

        internal static readonly Lazy<GpioPin> Pin10 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin10, 24)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPICS },
            Name = "BCM 8 (SPI Chip Select 0)"
        });

        internal static readonly Lazy<GpioPin> Pin06 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin06, 22)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 25"
        });
        internal static readonly Lazy<GpioPin> Pin05 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin05, 18)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 24"
        });

        internal static readonly Lazy<GpioPin> Pin04 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin04, 16)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 23"
        });

        internal static readonly Lazy<GpioPin> Pin01 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin01, 12)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.PWM },
            Name = "BCM 18 (PWM0)"
        });

        internal static readonly Lazy<GpioPin> Pin16 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin16, 10)
        {
            Capabilities = new[] { PinCapability.UARTRXD },
            Name = "BCM 15 (UART Receive)"
        });

        internal static readonly Lazy<GpioPin> Pin15 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin15, 8)
        {
            Capabilities = new[] { PinCapability.UARTTXD },
            Name = "BCM 14 (UART Transmit)"
        });

        internal static readonly Lazy<GpioPin> Pin21 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin21, 29)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 5"
        });

        internal static readonly Lazy<GpioPin> Pin22 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin22, 31)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 6"
        });

        internal static readonly Lazy<GpioPin> Pin23 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin23, 33)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.PWM },
            Name = "BCM 13"
        });

        internal static readonly Lazy<GpioPin> Pin24 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin24, 35)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPIMISO },
            Name = "BCM 19 (SPI Master-In)"
        });

        internal static readonly Lazy<GpioPin> Pin25 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin25, 37)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 26"
        });

        internal static readonly Lazy<GpioPin> Pin29 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin29, 40)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPICLK },
            Name = "BCM 21 (SPI Clock)"
        });

        internal static readonly Lazy<GpioPin> Pin28 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin28, 38)
        {
            Capabilities = new[] { PinCapability.GP, PinCapability.SPIMOSI },
            Name = "BCM 20 (SPI Master-Out)"
        });

        internal static readonly Lazy<GpioPin> Pin27 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin27, 36)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 16"
        });
        internal static readonly Lazy<GpioPin> Pin26 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin26, 32)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 12"
        });

        internal static readonly Lazy<GpioPin> Pin17 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin17, 3)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 28"
        });

        internal static readonly Lazy<GpioPin> Pin18 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin18, 4)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 29"
        });

        internal static readonly Lazy<GpioPin> Pin19 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin19, 5)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 30"
        });

        internal static readonly Lazy<GpioPin> Pin20 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin20, 6)
        {
            Capabilities = new[] { PinCapability.GP },
            Name = "BCM 31"
        });

        #endregion
    }
}
