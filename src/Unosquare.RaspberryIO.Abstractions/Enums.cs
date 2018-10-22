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
}
