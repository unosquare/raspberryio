namespace Unosquare.RaspberryIO.Gpio
{
    /// <summary>
    /// Defines the different drive modes of a GPIO pin
    /// </summary>
    public enum GpioPinDriveMode
    {
        /// <summary>
        /// Input drive mode (perform reads)
        /// </summary>
        Input = 0,

        /// <summary>
        /// Output drive mode (perform writes)
        /// </summary>
        Output = 1,

        /// <summary>
        /// PWM output mode (only certain pins support this -- 2 of them at the moment)
        /// </summary>
        PwmOutput = 2,

        /// <summary>
        /// GPIO Clock output mode (only a pin supports this at this time)
        /// </summary>
        GpioClock = 3
    }

    /// <summary>
    /// The GPIO pin resistor mode. This is used on input pins so that their
    /// lines are not floating
    /// </summary>
    public enum GpioPinResistorPullMode
    {
        /// <summary>
        /// Pull resistor not active. Line floating
        /// </summary>
        Off = 0,

        /// <summary>
        /// Pull resistor sets a default value of 0 on no-connects
        /// </summary>
        PullDown = 1,

        /// <summary>
        /// Pull resistor sets a default value of 1 on no-connects
        /// </summary>
        PullUp = 2,
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
    /// Defines the different edge detection modes  for pin interrupts
    /// </summary>
    public enum EdgeDetection
    {
        /// <summary>
        /// Assumes edge detection was already setup externally
        /// </summary>
        ExternalSetup = 0,

        /// <summary>
        /// Falling Edge
        /// </summary>
        FallingEdge = 1,

        /// <summary>
        /// Rising edge
        /// </summary>
        RisingEdge = 2,

        /// <summary>
        /// Both, rising and falling edges
        /// </summary>
        RisingAndFallingEdges = 3
    }

    /// <summary>
    /// Defines the GPIO Pin values 0 for low, 1 for High
    /// </summary>
    public enum GpioPinValue
    {
        /// <summary>
        /// Digital high
        /// </summary>
        High = 1,

        /// <summary>
        /// Digital low
        /// </summary>
        Low = 0
    }

    /// <summary>
    /// Defines the Header connectors available
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
    /// Defines all the available Wiring Pi Pin Numbers
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
    /// Enumerates the different pins on the P1 Header
    /// as commonly referenced by Raspberry Pi Documentation.
    /// Enumeration values correspond to the physical pin number.
    /// </summary>
    public enum P1
    {
        /// <summary>
        /// Header P1, GPIO Pin 02
        /// </summary>
        Gpio02 = 3,

        /// <summary>
        /// Header P1, GPIO Pin 03
        /// </summary>
        Gpio03 = 5,

        /// <summary>
        /// Header P1, GPIO Pin 04
        /// </summary>
        Gpio04 = 7,

        /// <summary>
        /// Header P1, GPIO Pin 17
        /// </summary>
        Gpio17 = 11,

        /// <summary>
        /// Header P1, GPIO Pin 27
        /// </summary>
        Gpio27 = 13,

        /// <summary>
        /// Header P1, GPIO Pin 22
        /// </summary>
        Gpio22 = 15,

        /// <summary>
        /// Header P1, GPIO Pin 10
        /// </summary>
        Gpio10 = 19,

        /// <summary>
        /// Header P1, GPIO Pin 09
        /// </summary>
        Gpio09 = 21,

        /// <summary>
        /// Header P1, GPIO Pin 11
        /// </summary>
        Gpio11 = 23,

        /// <summary>
        /// Header P1, GPIO Pin 05
        /// </summary>
        Gpio05 = 29,

        /// <summary>
        /// Header P1, GPIO Pin 06
        /// </summary>
        Gpio06 = 31,

        /// <summary>
        /// Header P1, GPIO Pin 13
        /// </summary>
        Gpio13 = 33,

        /// <summary>
        /// Header P1, GPIO Pin 19
        /// </summary>
        Gpio19 = 35,

        /// <summary>
        /// Header P1, GPIO Pin 26
        /// </summary>
        Gpio26 = 37,

        /// <summary>
        /// Header P1, GPIO Pin 14
        /// </summary>
        Gpio14 = 8,

        /// <summary>
        /// Header P1, GPIO Pin 15
        /// </summary>
        Gpio15 = 10,

        /// <summary>
        /// Header P1, GPIO Pin 18
        /// </summary>
        Gpio18 = 12,

        /// <summary>
        /// Header P1, GPIO Pin 23
        /// </summary>
        Gpio23 = 16,

        /// <summary>
        /// Header P1, GPIO Pin 24
        /// </summary>
        Gpio24 = 18,

        /// <summary>
        /// Header P1, GPIO Pin 25
        /// </summary>
        Gpio25 = 22,

        /// <summary>
        /// Header P1, GPIO Pin 08
        /// </summary>
        Gpio08 = 24,

        /// <summary>
        /// Header P1, GPIO Pin 07
        /// </summary>
        Gpio07 = 26,

        /// <summary>
        /// Header P1, GPIO Pin 12
        /// </summary>
        Gpio12 = 32,

        /// <summary>
        /// Header P1, GPIO Pin 16
        /// </summary>
        Gpio16 = 36,

        /// <summary>
        /// Header P1, GPIO Pin 20
        /// </summary>
        Gpio20 = 38,

        /// <summary>
        /// Header P1, GPIO Pin 21
        /// </summary>
        Gpio21 = 40
    }

    /// <summary>
    /// Enumerates the different pins on the P5 Header
    /// as commonly referenced by Raspberry Pi documentation.
    /// Enumeration values correspond to the physical pin number.
    /// </summary>
    public enum P5
    {
        /// <summary>
        /// Header P5, GPIO Pin 28
        /// </summary>
        Gpio28 = 3,

        /// <summary>
        /// Header P5, GPIO Pin 29
        /// </summary>
        Gpio29 = 4,

        /// <summary>
        /// Header P5, GPIO Pin 30
        /// </summary>
        Gpio30 = 5,

        /// <summary>
        /// Header P5, GPIO Pin 31
        /// </summary>
        Gpio31 = 6
    }

    /// <summary>
    /// Defines the different pin capabilities
    /// </summary>
    public enum PinCapability
    {
        /// <summary>
        /// General Purpose capability: Digital and Analog Read/Write
        /// </summary>
        GP,

        /// <summary>
        /// General Purpose Clock (not PWM)
        /// </summary>
        GPCLK,

        /// <summary>
        /// i2c data channel
        /// </summary>
        I2CSDA,

        /// <summary>
        /// i2c clock channel
        /// </summary>
        I2CSCL,

        /// <summary>
        /// SPI Master Out, Slave In channel
        /// </summary>
        SPIMOSI,

        /// <summary>
        /// SPI Master In, Slave Out channel
        /// </summary>
        SPIMISO,

        /// <summary>
        /// SPI Clock channel
        /// </summary>
        SPICLK,

        /// <summary>
        /// SPI Chip Select Channel
        /// </summary>
        SPICS,

        /// <summary>
        /// UART Request to Send Channel
        /// </summary>
        UARTRTS,

        /// <summary>
        /// UART Transmit Channel
        /// </summary>
        UARTTXD,

        /// <summary>
        /// UART Receive Channel
        /// </summary>
        UARTRXD,

        /// <summary>
        /// Hardware Pule Width Modulation
        /// </summary>
        PWM
    }

    /// <summary>
    /// Defines the SPI channel numbers
    /// </summary>
    internal enum SpiChannelNumber
    {
        /// <summary>
        /// The channel 0
        /// </summary>
        Channel0 = 0,

        /// <summary>
        /// The channel 1
        /// </summary>
        Channel1 = 1,
    }

    /// <summary>
    /// Defines GPIO controller initialization modes
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