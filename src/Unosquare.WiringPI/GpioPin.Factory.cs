﻿namespace Unosquare.WiringPI
{
    using System;

    public partial class GpioPin
    {
        #region Static Pin Definitions

        internal static readonly Lazy<GpioPin> Pin08 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin08, 3)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSDA,
            Name = "BCM 2 (SDA)",
        });

        internal static readonly Lazy<GpioPin> Pin09 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin09, 5)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSCL,
            Name = "BCM 3 (SCL)",
        });

        internal static readonly Lazy<GpioPin> Pin07 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin07, 7)
        {
            Capabilities = PinCapability.GP | PinCapability.GPCLK,
            Name = "BCM 4 (GPCLK0)",
        });

        internal static readonly Lazy<GpioPin> Pin00 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin00, 11)
        {
            Capabilities = PinCapability.GP | PinCapability.UARTRTS,
            Name = "BCM 17",
        });

        internal static readonly Lazy<GpioPin> Pin02 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin02, 13)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 27",
        });

        internal static readonly Lazy<GpioPin> Pin03 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin03, 15)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 22",
        });

        internal static readonly Lazy<GpioPin> Pin12 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin12, 19)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMOSI,
            Name = "BCM 10 (MOSI)",
        });

        internal static readonly Lazy<GpioPin> Pin13 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin13, 21)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMISO,
            Name = "BCM 9 (MISO)",
        });

        internal static readonly Lazy<GpioPin> Pin14 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin14, 23)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICLK,
            Name = "BCM 11 (SCLCK)",
        });

        internal static readonly Lazy<GpioPin> Pin30 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin30, 27)
        {
            Capabilities = PinCapability.I2CSDA,
            Name = "BCM 0 (ID_SD)",
        });

        internal static readonly Lazy<GpioPin> Pin31 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin31, 28)
        {
            Capabilities = PinCapability.I2CSCL,
            Name = "BCM 1 (ID_SC)",
        });

        internal static readonly Lazy<GpioPin> Pin11 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin11, 26)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICS,
            Name = "BCM 7 (CE1)",
        });

        internal static readonly Lazy<GpioPin> Pin10 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin10, 24)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICS,
            Name = "BCM 8 (CE0)",
        });

        internal static readonly Lazy<GpioPin> Pin06 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin06, 22)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 25",
        });
        internal static readonly Lazy<GpioPin> Pin05 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin05, 18)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 24",
        });

        internal static readonly Lazy<GpioPin> Pin04 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin04, 16)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 23",
        });

        internal static readonly Lazy<GpioPin> Pin01 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin01, 12)
        {
            Capabilities = PinCapability.GP | PinCapability.PWM,
            Name = "BCM 18 (PWM0)",
        });

        internal static readonly Lazy<GpioPin> Pin16 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin16, 10)
        {
            Capabilities = PinCapability.UARTRXD,
            Name = "BCM 15 (RXD)",
        });

        internal static readonly Lazy<GpioPin> Pin15 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin15, 8)
        {
            Capabilities = PinCapability.UARTTXD,
            Name = "BCM 14 (TXD)",
        });

        internal static readonly Lazy<GpioPin> Pin21 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin21, 29)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 5",
        });

        internal static readonly Lazy<GpioPin> Pin22 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin22, 31)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 6",
        });

        internal static readonly Lazy<GpioPin> Pin23 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin23, 33)
        {
            Capabilities = PinCapability.GP | PinCapability.PWM,
            Name = "BCM 13 (PWM1)",
        });

        internal static readonly Lazy<GpioPin> Pin24 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin24, 35)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMISO,
            Name = "BCM 19 (MISO)",
        });

        internal static readonly Lazy<GpioPin> Pin25 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin25, 37)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 26",
        });

        internal static readonly Lazy<GpioPin> Pin29 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin29, 40)
        {
            Capabilities = PinCapability.GP | PinCapability.SPICLK,
            Name = "BCM 21 (SCLK)",
        });

        internal static readonly Lazy<GpioPin> Pin28 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin28, 38)
        {
            Capabilities = PinCapability.GP | PinCapability.SPIMOSI,
            Name = "BCM 20 (MOSI)",
        });

        internal static readonly Lazy<GpioPin> Pin27 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin27, 36)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 16",
        });
        internal static readonly Lazy<GpioPin> Pin26 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin26, 32)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 12 (PWM0)",
        });

        internal static readonly Lazy<GpioPin> Pin17 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin17, 3)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSDA,
            Name = "BCM 28 (SDA)",
        });

        internal static readonly Lazy<GpioPin> Pin18 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin18, 4)
        {
            Capabilities = PinCapability.GP | PinCapability.I2CSCL,
            Name = "BCM 29 (SCL)",
        });

        internal static readonly Lazy<GpioPin> Pin19 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin19, 5)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 30",
        });

        internal static readonly Lazy<GpioPin> Pin20 = new Lazy<GpioPin>(() => new GpioPin(WiringPiPin.Pin20, 6)
        {
            Capabilities = PinCapability.GP,
            Name = "BCM 31",
        });

        #endregion
    }
}