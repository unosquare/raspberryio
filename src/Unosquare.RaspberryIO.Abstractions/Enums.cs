namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Defines the SPI channel numbers.
    /// </summary>
    public enum SpiChannelNumber
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
    /// Defines the GPIO Pin values 0 for low, 1 for High.
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
        Low = 0,
    }

    /// <summary>
    /// The GPIO pin resistor mode. This is used on input pins so that their
    /// lines are not floating.
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
    /// Defines the different drive modes of a GPIO pin.
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
        GpioClock = 3,

        /// <summary>
        /// The alt0 operating mode
        /// </summary>
        Alt0 = 4,

        /// <summary>
        /// The alt1 operating mode
        /// </summary>
        Alt1 = 5,

        /// <summary>
        /// The alt2 operating mode
        /// </summary>
        Alt2 = 6,

        /// <summary>
        /// The alt3 operating mode
        /// </summary>
        Alt3 = 7,
    }

    /// <summary>
    /// Defines the different threading locking keys.
    /// </summary>
    public enum ThreadLockKey
    {
        /// <summary>
        /// The lock 0
        /// </summary>
        Lock0 = 0,

        /// <summary>
        /// The lock 1
        /// </summary>
        Lock1 = 1,

        /// <summary>
        /// The lock 2
        /// </summary>
        Lock2 = 2,

        /// <summary>
        /// The lock 3
        /// </summary>
        Lock3 = 3,
    }

    /// <summary>
    /// Defines the different edge detection modes  for pin interrupts.
    /// </summary>
    public enum EdgeDetection
    {
        /// <summary>
        /// Falling Edge
        /// </summary>
        FallingEdge,

        /// <summary>
        /// Rising edge
        /// </summary>
        RisingEdge,

        /// <summary>
        /// Both, falling and rising edges
        /// </summary>
        FallingAndRisingEdge,
    }

    /// <summary>
    /// The hardware revision of the board.
    /// </summary>
    public enum BoardRevision
    {
        /// <summary>
        /// Revision 1 (the early Model A and B's).
        /// </summary>
        Rev1,

        /// <summary>
        /// Revision 2 (everything else - it covers the B, B+ and CM).
        /// </summary>
        Rev2,
    }

    public enum BcmPin
    {
        /// <summary>
        /// The Gpio00
        /// </summary>
        Gpio00 = 0,

        /// <summary>
        /// The Gpio01
        /// </summary>
        Gpio01 = 1,

        /// <summary>
        /// The Gpio02
        /// </summary>
        Gpio02 = 2,

        /// <summary>
        /// The Gpio03
        /// </summary>
        Gpio03 = 3,

        /// <summary>
        /// The Gpio04
        /// </summary>
        Gpio04 = 4,

        /// <summary>
        /// The Gpio05
        /// </summary>
        Gpio05 = 5,

        /// <summary>
        /// The Gpio06
        /// </summary>
        Gpio06 = 6,

        /// <summary>
        /// The Gpio07
        /// </summary>
        Gpio07 = 7,

        /// <summary>
        /// The Gpio08
        /// </summary>
        Gpio08 = 8,

        /// <summary>
        /// The Gpio09
        /// </summary>
        Gpio09 = 9,

        /// <summary>
        /// The Gpio10
        /// </summary>
        Gpio10 = 10,

        /// <summary>
        /// The Gpio11
        /// </summary>
        Gpio11 = 11,

        /// <summary>
        /// The Gpio12
        /// </summary>
        Gpio12 = 12,

        /// <summary>
        /// The Gpio13
        /// </summary>
        Gpio13 = 13,

        /// <summary>
        /// The Gpio14
        /// </summary>
        Gpio14 = 14,

        /// <summary>
        /// The Gpio15
        /// </summary>
        Gpio15 = 15,

        /// <summary>
        /// The Gpio16
        /// </summary>
        Gpio16 = 16,

        /// <summary>
        /// The Gpio17
        /// </summary>
        Gpio17 = 17,

        /// <summary>
        /// The Gpio18
        /// </summary>
        Gpio18 = 18,

        /// <summary>
        /// The Gpio19
        /// </summary>
        Gpio19 = 19,

        /// <summary>
        /// The Gpio20
        /// </summary>
        Gpio20 = 20,

        /// <summary>
        /// The Gpio21
        /// </summary>
        Gpio21 = 21,

        /// <summary>
        /// The Gpio22
        /// </summary>
        Gpio22 = 22,

        /// <summary>
        /// The Gpio23
        /// </summary>
        Gpio23 = 23,

        /// <summary>
        /// The Gpio24
        /// </summary>
        Gpio24 = 24,

        /// <summary>
        /// The Gpio25
        /// </summary>
        Gpio25 = 25,

        /// <summary>
        /// The Gpio26
        /// </summary>
        Gpio26 = 26,

        /// <summary>
        /// The Gpio27
        /// </summary>
        Gpio27 = 27,

        /// <summary>
        /// The Gpio28
        /// </summary>
        Gpio28 = 28,

        /// <summary>
        /// The Gpio29
        /// </summary>
        Gpio29 = 29,

        /// <summary>
        /// The Gpio30
        /// </summary>
        Gpio30 = 30,

        /// <summary>
        /// The Gpio31
        /// </summary>
        Gpio31 = 31,
    }
}
