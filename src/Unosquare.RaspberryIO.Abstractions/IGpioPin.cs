namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interface for GPIO Pin on a RaspberryPi board.
    /// </summary>
    public interface IGpioPin
    {
        /// <summary>
        /// Gets the BCM pin number.
        /// </summary>
        /// <value>
        /// The pin number.
        /// </value>
        int PinNumber { get; }

        /// <summary>
        /// Gets or sets the pin operating mode.
        /// </summary>
        /// <value>
        /// The pin mode.
        /// </value>
        GpioPinDriveMode PinMode { get; set; }

        /// <summary>
        /// This sets or gets the pull-up or pull-down resistor mode on the pin, which should be set as an input.
        /// Unlike the Arduino, the BCM2835 has both pull-up an down internal resistors.
        /// The parameter pud should be; PUD_OFF, (no pull up/down), PUD_DOWN (pull to ground) or PUD_UP (pull to 3.3v)
        /// The internal pull up/down resistors have a value of approximately 50KΩ on the Raspberry Pi.
        /// </summary>
        GpioPinResistorPullMode InputPullMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IGpioPin"/> is value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if value; otherwise, <c>false</c>.
        /// </value>
        bool Value { get; set; }
        
        /// <summary>
        /// Reads the digital value on the pin as a boolean value.
        /// </summary>
        /// <returns>The state of the pin.</returns>
        bool Read();
        
        /// <summary>
        /// Writes the specified bit value.
        /// This method performs a digital write.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        void Write(bool value);
        
        /// <summary>
        /// Writes the specified pin value.
        /// This method performs a digital write.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(GpioPinValue value);

        /// <summary>
        /// Wait for specific pin status.
        /// </summary>
        /// <param name="status">status to check.</param>
        /// <param name="timeOutMillisecond">timeout to reach status.</param>
        /// <returns>true/false.</returns>
        bool WaitForValue(GpioPinValue status, int timeOutMillisecond);
    }
}
