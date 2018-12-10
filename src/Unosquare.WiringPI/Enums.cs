namespace Unosquare.WiringPI
{
    using System;

    /// <summary>
    /// Defines all the available Wiring Pi Pin Numbers.
    /// </summary>
    public enum WiringPiPin
    {
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The pin00
        /// </summary>
        Pin00 = 0,

        /// <summary>
        /// The pin01
        /// </summary>
        Pin01 = 1,

        /// <summary>
        /// The pin02
        /// </summary>
        Pin02 = 2,

        /// <summary>
        /// The pin03
        /// </summary>
        Pin03 = 3,

        /// <summary>
        /// The pin04
        /// </summary>
        Pin04 = 4,

        /// <summary>
        /// The pin05
        /// </summary>
        Pin05 = 5,

        /// <summary>
        /// The pin06
        /// </summary>
        Pin06 = 6,

        /// <summary>
        /// The pin07
        /// </summary>
        Pin07 = 7,

        /// <summary>
        /// The pin08
        /// </summary>
        Pin08 = 8,

        /// <summary>
        /// The pin09
        /// </summary>
        Pin09 = 9,

        /// <summary>
        /// The pin10
        /// </summary>
        Pin10 = 10,

        /// <summary>
        /// The pin11
        /// </summary>
        Pin11 = 11,

        /// <summary>
        /// The pin12
        /// </summary>
        Pin12 = 12,

        /// <summary>
        /// The pin13
        /// </summary>
        Pin13 = 13,

        /// <summary>
        /// The pin14
        /// </summary>
        Pin14 = 14,

        /// <summary>
        /// The pin15
        /// </summary>
        Pin15 = 15,

        /// <summary>
        /// The pin16
        /// </summary>
        Pin16 = 16,

        /// <summary>
        /// The pin17
        /// </summary>
        Pin17 = 17,

        /// <summary>
        /// The pin18
        /// </summary>
        Pin18 = 18,

        /// <summary>
        /// The pin19
        /// </summary>
        Pin19 = 19,

        /// <summary>
        /// The pin20
        /// </summary>
        Pin20 = 20,

        /// <summary>
        /// The pin21
        /// </summary>
        Pin21 = 21,

        /// <summary>
        /// The pin22
        /// </summary>
        Pin22 = 22,

        /// <summary>
        /// The pin23
        /// </summary>
        Pin23 = 23,

        /// <summary>
        /// The pin24
        /// </summary>
        Pin24 = 24,

        /// <summary>
        /// The pin25
        /// </summary>
        Pin25 = 25,

        /// <summary>
        /// The pin26
        /// </summary>
        Pin26 = 26,

        /// <summary>
        /// The pin27
        /// </summary>
        Pin27 = 27,

        /// <summary>
        /// The pin28
        /// </summary>
        Pin28 = 28,

        /// <summary>
        /// The pin29
        /// </summary>
        Pin29 = 29,

        /// <summary>
        /// The pin30
        /// </summary>
        Pin30 = 30,

        /// <summary>
        /// The pin31
        /// </summary>
        Pin31 = 31,
    }

    /// <summary>
    /// Defines the Header connectors available.
    /// </summary>
    public enum GpioHeader
    {
        /// <summary>
        /// Not defined
        /// </summary>
        None,

        /// <summary>
        /// The P1 connector (main connector)
        /// </summary>
        P1,

        /// <summary>
        /// The P5 connector (auxiliary, not commonly used)
        /// </summary>
        P5,
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
    /// Enumerates the different pins on the P1 Header.
    /// Enumeration values correspond to the physical pin number.
    /// </summary>
    public enum P1
    {
        /// <summary>
        /// Header P1 Pin 3. GPIO 0 for rev1 or GPIO 2 for rev2.
        /// </summary>
        Pin03 = 3,

        /// <summary>
        /// Header P1 Pin 5. GPIO 1 for rev1 or GPIO 3 for rev2.
        /// </summary>
        Pin05 = 5,

        /// <summary>
        /// Header P1 Pin 7. GPIO 04.
        /// </summary>
        Pin07 = 7,

        /// <summary>
        /// Header P1 Pin 11. GPIO 17.
        /// </summary>
        Pin11 = 11,

        /// <summary>
        /// Header P1 Pin 13. GPIO 21 for rev1 or GPIO 27 for rev2.
        /// </summary>
        Pin13 = 13,

        /// <summary>
        /// Header P1 Pin 15. GPIO 22.
        /// </summary>
        Pin15 = 15,

        /// <summary>
        /// Header P1 Pin 19. GPIO 10.
        /// </summary>
        Pin19 = 19,

        /// <summary>
        /// Header P1 Pin 21. GPIO 9.
        /// </summary>
        Pin21 = 21,

        /// <summary>
        /// Header P1 Pin 23. GPIO 11.
        /// </summary>
        Pin23 = 23,

        /// <summary>
        /// Header P1 Pin 27. GPIO 0.
        /// </summary>
        Pin27 = 27,

        /// <summary>
        /// Header P1 Pin 29. GPIO 5.
        /// </summary>
        Pin29 = 29,

        /// <summary>
        /// Header P1 Pin 31. GPIO 6.
        /// </summary>
        Pin31 = 31,

        /// <summary>
        /// Header P1 Pin 33. GPIO 13.
        /// </summary>
        Pin33 = 33,

        /// <summary>
        /// Header P1 Pin 35. GPIO 19.
        /// </summary>
        Pin35 = 35,

        /// <summary>
        /// Header P1 Pin 37. GPIO 26.
        /// </summary>
        Pin37 = 37,

        /// <summary>
        /// Header P1 Pin 8. GPIO 14.
        /// </summary>
        Pin08 = 8,

        /// <summary>
        /// Header P1 Pin 10. GPIO 15.
        /// </summary>
        Pin10 = 10,

        /// <summary>
        /// Header P1 Pin 12. GPIO 18.
        /// </summary>
        Pin12 = 12,

        /// <summary>
        /// Header P1 Pin 16. GPIO 23.
        /// </summary>
        Pin16 = 16,

        /// <summary>
        /// Header P1 Pin 18. GPIO 24.
        /// </summary>
        Pin18 = 18,

        /// <summary>
        /// Header P1 Pin 22. GPIO 25.
        /// </summary>
        Pin22 = 22,

        /// <summary>
        /// Header P1 Pin 24. GPIO 8.
        /// </summary>
        Pin24 = 24,

        /// <summary>
        /// Header P1 Pin 26. GPIO 7.
        /// </summary>
        Pin26 = 26,

        /// <summary>
        /// Header P1 Pin 28. GPIO 1.
        /// </summary>
        Pin28 = 28,

        /// <summary>
        /// Header P1 Pin 32. GPIO 12.
        /// </summary>
        Pin32 = 32,

        /// <summary>
        /// Header P1 Pin 36. GPIO 16.
        /// </summary>
        Pin36 = 36,

        /// <summary>
        /// Header P1 Pin 38. GPIO 20.
        /// </summary>
        Pin38 = 38,

        /// <summary>
        /// Header P1 Pin 40. GPIO 21.
        /// </summary>
        Pin40 = 40,
    }

    /// <summary>
    /// Enumerates the different pins on the P5 Header
    /// as commonly referenced by Raspberry Pi documentation.
    /// Enumeration values correspond to the physical pin number.
    /// </summary>
    public enum P5
    {
        /// <summary>
        /// Header P5 Pin 3, GPIO 28
        /// </summary>
        Pin03 = 3,

        /// <summary>
        /// Header P5 Pin 4, GPIO 29
        /// </summary>
        Pin04 = 4,

        /// <summary>
        /// Header P5 Pin 5, GPIO 30
        /// </summary>
        Pin05 = 5,

        /// <summary>
        /// Header P5 Pin 6, GPIO 31
        /// </summary>
        Pin06 = 6,
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
