namespace Unosquare.RaspberryIO
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Occurs when an exception is thrown in the <c>Bluetooth</c> component.
    /// </summary>
    public class BluetoothErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothErrorException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BluetoothErrorException(string message)
            : base(message)
        {
        }
    }
}
