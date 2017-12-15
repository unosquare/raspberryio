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
        /// The P5 connector (auxiliary, not commenly used)
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
    /// Defines the different pin capabilities
    /// </summary>
    public enum PinCapability
    {
        /// <summary>
        /// General Purpose capability: Digitala nd Analog Read/Write
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
        NotInitialized,
        DirectWithWiringPiPins,
        DirectWithBcmPins,
        DirectWithHeaderPins,
        FileStreamWithHardwarePins,
    }
}