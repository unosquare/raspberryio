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
        /// <param name="errorCode">The error code.</param>
        public BluetoothErrorException(string message, BluetoothErrorCode errorCode = BluetoothErrorCode.Generic)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public BluetoothErrorCode ErrorCode { get; }

        /// <inheritdoc />
        public override string Message => $"Error Code: {ErrorCode}\r\n{base.Message}";

        /// <summary>
        /// To the Bluetooth error item.
        /// </summary>
        /// <returns>An Bluetooth Error Item.</returns>
        public BluetoothErrorItem ToABluetoothErrorItem() => new BluetoothErrorItem(ErrorCode, base.Message);
    }
}
