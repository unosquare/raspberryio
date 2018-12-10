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
        Rev1 = 1,

        /// <summary>
        /// Revision 2 (everything else - it covers the B, B+ and CM).
        /// </summary>
        Rev2 = 2,
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
        /// P1 connector (main connector)
        /// </summary>
        P1,

        /// <summary>
        /// P5 connector (auxiliary, not commonly used)
        /// </summary>
        P5,
    }

    /// <summary>
    /// Defines all the BCM Pin numbers available for the user.
    /// </summary>
    public enum BcmPin
    {
        /// <summary>
        /// GPIO 0
        /// </summary>
        Gpio00 = 0,

        /// <summary>
        /// GPIO 1
        /// </summary>
        Gpio01 = 1,

        /// <summary>
        /// GPIO02
        /// </summary>
        Gpio02 = 2,

        /// <summary>
        /// GPIO 3
        /// </summary>
        Gpio03 = 3,

        /// <summary>
        /// GPIO 4
        /// </summary>
        Gpio04 = 4,

        /// <summary>
        /// GPIO 5
        /// </summary>
        Gpio05 = 5,

        /// <summary>
        /// GPIO 6
        /// </summary>
        Gpio06 = 6,

        /// <summary>
        /// GPIO 7
        /// </summary>
        Gpio07 = 7,

        /// <summary>
        /// GPIO 8
        /// </summary>
        Gpio08 = 8,

        /// <summary>
        /// GPIO 9
        /// </summary>
        Gpio09 = 9,

        /// <summary>
        /// GPIO 10
        /// </summary>
        Gpio10 = 10,

        /// <summary>
        /// GPIO 11
        /// </summary>
        Gpio11 = 11,

        /// <summary>
        /// GPIO 12
        /// </summary>
        Gpio12 = 12,

        /// <summary>
        /// GPIO 13
        /// </summary>
        Gpio13 = 13,

        /// <summary>
        /// GPIO 14
        /// </summary>
        Gpio14 = 14,

        /// <summary>
        /// GPIO 15
        /// </summary>
        Gpio15 = 15,

        /// <summary>
        /// GPIO 16
        /// </summary>
        Gpio16 = 16,

        /// <summary>
        /// GPIO 17
        /// </summary>
        Gpio17 = 17,

        /// <summary>
        /// GPIO 18
        /// </summary>
        Gpio18 = 18,

        /// <summary>
        /// GPIO 19
        /// </summary>
        Gpio19 = 19,

        /// <summary>
        /// GPIO 20
        /// </summary>
        Gpio20 = 20,

        /// <summary>
        /// GPIO 21
        /// </summary>
        Gpio21 = 21,

        /// <summary>
        /// GPIO 22
        /// </summary>
        Gpio22 = 22,

        /// <summary>
        /// GPIO 23
        /// </summary>
        Gpio23 = 23,

        /// <summary>
        /// GPIO 24
        /// </summary>
        Gpio24 = 24,

        /// <summary>
        /// GPIO 25
        /// </summary>
        Gpio25 = 25,

        /// <summary>
        /// GPIO 26
        /// </summary>
        Gpio26 = 26,

        /// <summary>
        /// GPIO 27
        /// </summary>
        Gpio27 = 27,

        /// <summary>
        /// GPIO 28
        /// </summary>
        Gpio28 = 28,

        /// <summary>
        /// GPIO 29
        /// </summary>
        Gpio29 = 29,

        /// <summary>
        /// GPIO 30
        /// </summary>
        Gpio30 = 30,

        /// <summary>
        /// GPIO 31
        /// </summary>
        Gpio31 = 31,
    }

    /// <summary>
    /// Enumerates the different pins on the P1 Header.
    /// Enumeration values correspond to the physical pin number.
    /// </summary>
    public enum P1
    {
        /// <summary>
        /// Header P1 Physical Pin 3. GPIO 0 for rev1 or GPIO 2 for rev2.
        /// </summary>
        Pin03 = 3,

        /// <summary>
        /// Header P1 Physical Pin 5. GPIO 1 for rev1 or GPIO 3 for rev2.
        /// </summary>
        Pin05 = 5,

        /// <summary>
        /// Header P1 Physical Pin 7. GPIO 4.
        /// </summary>
        Pin07 = 7,

        /// <summary>
        /// Header P1 Physical Pin 11. GPIO 17.
        /// </summary>
        Pin11 = 11,

        /// <summary>
        /// Header P1 Physical Pin 13. GPIO 21 for rev1 or GPIO 27 for rev2.
        /// </summary>
        Pin13 = 13,

        /// <summary>
        /// Header P1 Physical Pin 15. GPIO 22.
        /// </summary>
        Pin15 = 15,

        /// <summary>
        /// Header P1 Physical Pin 19. GPIO 10.
        /// </summary>
        Pin19 = 19,

        /// <summary>
        /// Header P1 Physical Pin 21. GPIO 9.
        /// </summary>
        Pin21 = 21,

        /// <summary>
        /// Header P1 Physical Pin 23. GPIO 11.
        /// </summary>
        Pin23 = 23,

        /// <summary>
        /// Header P1 Physical Pin 27. GPIO 0.
        /// </summary>
        Pin27 = 27,

        /// <summary>
        /// Header P1 Physical Pin 29. GPIO 5.
        /// </summary>
        Pin29 = 29,

        /// <summary>
        /// Header P1 Physical Pin 31. GPIO 6.
        /// </summary>
        Pin31 = 31,

        /// <summary>
        /// Header P1 Physical Pin 33. GPIO 13.
        /// </summary>
        Pin33 = 33,

        /// <summary>
        /// Header P1 Physical Pin 35. GPIO 19.
        /// </summary>
        Pin35 = 35,

        /// <summary>
        /// Header P1 Physical Pin 37. GPIO 26.
        /// </summary>
        Pin37 = 37,

        /// <summary>
        /// Header P1 Physical Pin 8. GPIO 14.
        /// </summary>
        Pin08 = 8,

        /// <summary>
        /// Header P1 Physical Pin 10. GPIO 15.
        /// </summary>
        Pin10 = 10,

        /// <summary>
        /// Header P1 Physical Pin 12. GPIO 18.
        /// </summary>
        Pin12 = 12,

        /// <summary>
        /// Header P1 Physical Pin 16. GPIO 23.
        /// </summary>
        Pin16 = 16,

        /// <summary>
        /// Header P1 Physical Pin 18. GPIO 24.
        /// </summary>
        Pin18 = 18,

        /// <summary>
        /// Header P1 Physical Pin 22. GPIO 25.
        /// </summary>
        Pin22 = 22,

        /// <summary>
        /// Header P1 Physical Pin 24. GPIO 8.
        /// </summary>
        Pin24 = 24,

        /// <summary>
        /// Header P1 Physical Pin 26. GPIO 7.
        /// </summary>
        Pin26 = 26,

        /// <summary>
        /// Header P1 Physical Pin 28. GPIO 1.
        /// </summary>
        Pin28 = 28,

        /// <summary>
        /// Header P1 Physical Pin 32. GPIO 12.
        /// </summary>
        Pin32 = 32,

        /// <summary>
        /// Header P1 Physical Pin 36. GPIO 16.
        /// </summary>
        Pin36 = 36,

        /// <summary>
        /// Header P1 Physical Pin 38. GPIO 20.
        /// </summary>
        Pin38 = 38,

        /// <summary>
        /// Header P1 Physical Pin 40. GPIO 21.
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
        /// Header P5 Physical Pin 3, GPIO 28.
        /// </summary>
        Pin03 = 3,

        /// <summary>
        /// Header P5 Physical Pin 4, GPIO 29.
        /// </summary>
        Pin04 = 4,

        /// <summary>
        /// Header P5 Physical Pin 5, GPIO 30.
        /// </summary>
        Pin05 = 5,

        /// <summary>
        /// Header P5 Physical Pin 6, GPIO 31.
        /// </summary>
        Pin06 = 6,
    }
}
