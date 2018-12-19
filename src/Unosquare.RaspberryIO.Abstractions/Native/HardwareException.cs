namespace Unosquare.RaspberryIO.Abstractions.Native
{
    using System;
    using System.Runtime.InteropServices;
    using Swan;

    /// <summary>
    /// Represents a low-level exception, typically thrown when return codes from a
    /// low-level operation is non-zero or in some cases when it is less than zero.
    /// </summary>
    /// <seealso cref="Exception" />
    public class HardwareException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="component">The component.</param>
        public HardwareException(int errorCode, string component)
            : base($"A hardware exception occurred. Error Code: {errorCode}")
        {
            ExtendedMessage = null;

            try
            {
                ExtendedMessage = Standard.Strerror(errorCode);
            }
            catch
            {
                $"Could not retrieve native error description using {nameof(Standard.Strerror)}".Error(nameof(HardwareException));
            }

            ErrorCode = errorCode;
            Component = component;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public string Component { get; }

        /// <summary>
        /// Gets the extended message (could be null).
        /// </summary>
        /// <value>
        /// The extended message.
        /// </value>
        public string ExtendedMessage { get; }

        /// <summary>
        /// Throws a new instance of a hardware error by retrieving the last error number (errno).
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <exception cref="HardwareException">When an error thrown by an API call occurs.</exception>
        public static void Throw(string className, string methodName) => throw new HardwareException(Marshal.GetLastWin32Error(), $"{className}.{methodName}");

        /// <inheritdoc />
        public override string ToString() => $"{GetType()}{(string.IsNullOrWhiteSpace(Component) ? string.Empty : $" on {Component}")}: ({ErrorCode}) - {Message}";
    }
}
