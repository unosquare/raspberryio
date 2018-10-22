namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interface for GPIO Pin on a RaspberryPi board.
    /// </summary>
    public interface IGpioPin
    {
        /// <summary>
        /// Gets the pin number.
        /// </summary>
        /// <value>
        /// The pin number.
        /// </value>
        int PinNumber { get; }

        /// <summary>
        /// Gets the pin mode.
        /// </summary>
        /// <value>
        /// The pin mode.
        /// </value>
        GpioPinDriveMode PinMode { get; }

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
        /// Wait for specific pin status.
        /// </summary>
        /// <param name="status">status to check.</param>
        /// <param name="timeOutMillisecond">timeout to reach status.</param>
        /// <returns>true/false.</returns>
        bool WaitForValue(GpioPinValue status, int timeOutMillisecond);
    }
}
