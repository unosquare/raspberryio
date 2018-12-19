namespace Unosquare.RaspberryIO.Abstractions
{
    using System;

    /// <summary>
    /// Interface for GPIO Pin on a RaspberryPi board.
    /// </summary>
    public interface IGpioPin
    {
        /// <summary>
        /// Gets the <see cref="Abstractions.BcmPin"/>.
        /// </summary>
        /// <value>
        /// The pin number.
        /// </value>
        BcmPin BcmPin { get; }

        /// <summary>
        /// Gets the BCM chip (hardware) pin number.
        /// </summary>
        /// <value>
        /// The pin number.
        /// </value>
        int BcmPinNumber { get; }

        /// <summary>
        /// Gets the physical (header) pin number.
        /// </summary>
        int PhysicalPinNumber { get; }

        /// <summary>
        /// Gets the pin's header (physical board) location.
        /// </summary>
        GpioHeader Header { get; }

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

        /// <summary>
        /// Registers the interrupt callback on the pin. Pin mode has to be set to Input.
        /// </summary>
        /// <param name="edgeDetection">The edge detection.</param>
        /// <param name="callback">The callback function. This function is called whenever
        /// the interrupt occurs.</param>
        void RegisterInterruptCallback(EdgeDetection edgeDetection, Action callback);

        /// <summary>
        /// Registers the interrupt callback on the pin. Pin mode has to be set to Input.
        /// </summary>
        /// <param name="edgeDetection">The edge detection.</param>
        /// <param name="callback">The callback function. This function is called whenever the interrupt occurs.
        /// The function is passed the GPIO, the current level, and the current tick
        /// (The number of microseconds since boot).</param>
        void RegisterInterruptCallback(EdgeDetection edgeDetection, Action<int, int, uint> callback);
    }
}
