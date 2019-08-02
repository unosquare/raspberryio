namespace Unosquare.RaspberryIO.Abstractions
{

    /// <summary>
    /// Defines the fields contained in one of the items of bluetooth Errors.
    /// </summary>
    public class BluetoothErrorItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothErrorItem" /> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public BluetoothErrorItem(BluetoothErrorCode errorCode, string message = null)
        {
            ErrorCode = errorCode;
            Message = message ?? errorCode.ToString();
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public BluetoothErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }
    }
}
