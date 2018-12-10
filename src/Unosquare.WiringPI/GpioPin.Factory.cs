namespace Unosquare.WiringPI
{
    using RaspberryIO.Abstractions;
    using System;

    public partial class GpioPin
    {
        #region Static Pin Definitions

        internal static readonly Lazy<GpioPin> Pin00 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio00)
        {
            Capabilities = PinCapability.I2CSDA,
            Name = $"BCM 0 {(SystemInfo.GetBoardRevision() == BoardRevision.Rev1 ? "(SDA)" : "(ID_SD)")}",
        });

        internal static readonly Lazy<GpioPin> Pin01 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio01)
        {
            Capabilities = PinCapability.I2CSCL,
            Name = $"BCM 1  {(SystemInfo.GetBoardRevision() == BoardRevision.Rev1 ? "(SCL)" : "(ID_SC)")}",
        });

        internal static readonly Lazy<GpioPin> Pin02 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio02)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSDA,
            Name = "BCM 2 (SDA)",
        });

        internal static readonly Lazy<GpioPin> Pin03 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio03)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSCL,
            Name = "BCM 3 (SCL)",
        });

        internal static readonly Lazy<GpioPin> Pin04 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio04)
        {
            Capabilities = PinCapability.GP | PinCapability.GPCLK,
            Name = "BCM 4 (GPCLK0)",
        });

        internal static readonly Lazy<GpioPin> Pin05 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio05)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 5",
        });

        internal static readonly Lazy<GpioPin> Pin06 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio06)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 6",
        });

        internal static readonly Lazy<GpioPin> Pin07 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio07)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICS,
            Name = "BCM 7 (CE1)",
        });

        internal static readonly Lazy<GpioPin> Pin08 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio08)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICS,
            Name = "BCM 8 (CE0)",
        });

        internal static readonly Lazy<GpioPin> Pin09 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio09)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMISO,
            Name = "BCM 9 (MISO)",
        });

        internal static readonly Lazy<GpioPin> Pin10 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio10)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMOSI,
            Name = "BCM 10 (MOSI)",
        });

        internal static readonly Lazy<GpioPin> Pin11 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio11)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICLK,
            Name = "BCM 11 (SCLCK)",
        });

        internal static readonly Lazy<GpioPin> Pin12 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio12)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 12 (PWM0)",
        });

        internal static readonly Lazy<GpioPin> Pin13 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio13)
        {
            Capabilities = PinCapability.GP | PinCapability.PWM,
            Name = "BCM 13 (PWM1)",
        });

        internal static readonly Lazy<GpioPin> Pin14 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio14)
        {
            Capabilities = PinCapability.UARTTXD,
            Name = "BCM 14 (TXD)",
        });

        internal static readonly Lazy<GpioPin> Pin15 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio15)
        {
            Capabilities = PinCapability.UARTRXD,
            Name = "BCM 15 (RXD)",
        });

        internal static readonly Lazy<GpioPin> Pin16 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio16)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 16",
        });

        internal static readonly Lazy<GpioPin> Pin17 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio17)
        {
            Capabilities = PinCapability.GP | PinCapability.UARTRTS,
            Name = "BCM 17",
        });

        internal static readonly Lazy<GpioPin> Pin18 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio18)
        {
            Capabilities = PinCapability.GP | PinCapability.PWM,
            Name = "BCM 18 (PWM0)",
        });

        internal static readonly Lazy<GpioPin> Pin19 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio19)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMISO,
            Name = "BCM 19 (MISO)",
        });

        internal static readonly Lazy<GpioPin> Pin20 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio20)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMOSI,
            Name = "BCM 20 (MOSI)",
        });

        internal static readonly Lazy<GpioPin> Pin21 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio21)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICLK,
            Name = $"BCM 21{(SystemInfo.GetBoardRevision() == BoardRevision.Rev1 ? string.Empty : " (SCLK)")}",
        });

        internal static readonly Lazy<GpioPin> Pin22 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio22)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 22",
        });

        internal static readonly Lazy<GpioPin> Pin23 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio23)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 23",
        });

        internal static readonly Lazy<GpioPin> Pin24 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio24)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 24",
        });

        internal static readonly Lazy<GpioPin> Pin25 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio25)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 25",
        });

        internal static readonly Lazy<GpioPin> Pin26 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio26)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 26",
        });

        internal static readonly Lazy<GpioPin> Pin27 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio27)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 27",
        });

        internal static readonly Lazy<GpioPin> Pin28 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio28)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSDA,
            Name = "BCM 28 (SDA)",
        });

        internal static readonly Lazy<GpioPin> Pin29 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio29)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSCL,
            Name = "BCM 29 (SCL)",
        });

        internal static readonly Lazy<GpioPin> Pin30 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio30)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 30",
        });

        internal static readonly Lazy<GpioPin> Pin31 = new Lazy<GpioPin>(() => new GpioPin(BcmPin.Gpio31)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 31",
        });

        #endregion
    }
}