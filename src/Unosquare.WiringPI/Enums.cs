namespace Unosquare.WiringPI
{
    using System;

    /// <summary>
    /// Defines all the available Wiring Pi Pin Numbers.
    /// </summary>
    public enum WiringPiPin
    {
        /// <summary>
        /// Unknown WiringPi pin.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// WiringPi pin 0.
        /// </summary>
        Pin00 = 0,

        /// <summary>
        /// WiringPi pin 1.
        /// </summary>
        Pin01 = 1,

        /// <summary>
        /// WiringPi pin 2.
        /// </summary>
        Pin02 = 2,

        /// <summary>
        /// WiringPi pin 3.
        /// </summary>
        Pin03 = 3,

        /// <summary>
        /// WiringPi pin 4.
        /// </summary>
        Pin04 = 4,

        /// <summary>
        /// WiringPi pin 5.
        /// </summary>
        Pin05 = 5,

        /// <summary>
        /// WiringPi pin 6.
        /// </summary>
        Pin06 = 6,

        /// <summary>
        /// WiringPi pin 7.
        /// </summary>
        Pin07 = 7,

        /// <summary>
        /// WiringPi pin 8.
        /// </summary>
        Pin08 = 8,

        /// <summary>
        /// WiringPi pin 9.
        /// </summary>
        Pin09 = 9,

        /// <summary>
        /// WiringPi pin 10.
        /// </summary>
        Pin10 = 10,

        /// <summary>
        /// WiringPi pin 11.
        /// </summary>
        Pin11 = 11,

        /// <summary>
        /// WiringPi pin 12.
        /// </summary>
        Pin12 = 12,

        /// <summary>
        /// WiringPi pin 13.
        /// </summary>
        Pin13 = 13,

        /// <summary>
        /// WiringPi pin 14.
        /// </summary>
        Pin14 = 14,

        /// <summary>
        /// WiringPi pin 15.
        /// </summary>
        Pin15 = 15,

        /// <summary>
        /// WiringPi pin 16.
        /// </summary>
        Pin16 = 16,

        /// <summary>
        /// WiringPi pin 17.
        /// </summary>
        Pin17 = 17,

        /// <summary>
        /// WiringPi pin 18.
        /// </summary>
        Pin18 = 18,

        /// <summary>
        /// WiringPi pin 19.
        /// </summary>
        Pin19 = 19,

        /// <summary>
        /// WiringPi pin 20.
        /// </summary>
        Pin20 = 20,

        /// <summary>
        /// WiringPi pin 21.
        /// </summary>
        Pin21 = 21,

        /// <summary>
        /// WiringPi pin 22.
        /// </summary>
        Pin22 = 22,

        /// <summary>
        /// WiringPi pin 23.
        /// </summary>
        Pin23 = 23,

        /// <summary>
        /// WiringPi pin 24.
        /// </summary>
        Pin24 = 24,

        /// <summary>
        /// WiringPi pin 25.
        /// </summary>
        Pin25 = 25,

        /// <summary>
        /// WiringPi pin 26.
        /// </summary>
        Pin26 = 26,

        /// <summary>
        /// WiringPi pin 27.
        /// </summary>
        Pin27 = 27,

        /// <summary>
        /// WiringPi pin 28.
        /// </summary>
        Pin28 = 28,

        /// <summary>
        /// WiringPi pin 29.
        /// </summary>
        Pin29 = 29,

        /// <summary>
        /// WiringPi pin 30.
        /// </summary>
        Pin30 = 30,

        /// <summary>
        /// WiringPi pin 31.
        /// </summary>
        Pin31 = 31,
    }

    /// <summary>
    /// Defines the different pin capabilities.
    /// </summary>
    [Flags]
    public enum PinCapability
    {
        /// <summary>
        /// General Purpose capability: Digital and Analog Read/Write
        /// </summary>
        GP = 0x01,

        /// <summary>
        /// General Purpose Clock (not PWM)
        /// </summary>
        GPCLK = 0x02,

        /// <summary>
        /// i2c data channel
        /// </summary>
        I2CSDA = 0x04,

        /// <summary>
        /// i2c clock channel
        /// </summary>
        I2CSCL = 0x08,

        /// <summary>
        /// SPI Master Out, Slave In channel
        /// </summary>
        SPIMOSI = 0x10,

        /// <summary>
        /// SPI Master In, Slave Out channel
        /// </summary>
        SPIMISO = 0x20,

        /// <summary>
        /// SPI Clock channel
        /// </summary>
        SPICLK = 0x40,

        /// <summary>
        /// SPI Chip Select Channel
        /// </summary>
        SPICS = 0x80,

        /// <summary>
        /// UART Request to Send Channel
        /// </summary>
        UARTRTS = 0x100,

        /// <summary>
        /// UART Transmit Channel
        /// </summary>
        UARTTXD = 0x200,

        /// <summary>
        /// UART Receive Channel
        /// </summary>
        UARTRXD = 0x400,

        /// <summary>
        /// Hardware Pule Width Modulation
        /// </summary>
        PWM = 0x800,
    }

    /// <summary>
    /// The PWM mode.
    /// </summary>
    public enum PwmMode
    {
        /// <summary>
        /// PWM pulses are sent using mark-sign patterns (old school)
        /// </summary>
        MarkSign = 0,

        /// <summary>
        /// PWM pulses are sent as a balanced signal (default, newer mode)
        /// </summary>
        Balanced = 1,
    }

    /// <summary>
    /// Defines GPIO controller initialization modes.
    /// </summary>
    internal enum ControllerMode
    {
        /// <summary>
        /// The not initialized
        /// </summary>
        NotInitialized,

        /// <summary>
        /// The direct with wiring pi pins
        /// </summary>
        DirectWithWiringPiPins,

        /// <summary>
        /// The direct with BCM pins
        /// </summary>
        DirectWithBcmPins,

        /// <summary>
        /// The direct with header pins
        /// </summary>
        DirectWithHeaderPins,

        /// <summary>
        /// The file stream with hardware pins
        /// </summary>
        FileStreamWithHardwarePins,
    }
}
