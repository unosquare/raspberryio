namespace Unosquare.RaspberryIO.Abstractions
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Special Exception to transport the BluetoothErorItem.
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
