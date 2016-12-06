namespace Unosquare.RaspberryIO
{
    using System;

    /// <summary>
    /// Represents a low-level exception, typically thrown when return codes from a
    /// low-level operation is non-zero or in some cases whe it is less than zero.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HardwareException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareException" /> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="component">The component.</param>
        public HardwareException(int errorCode, string component)
            : base(Interop.strerror(errorCode))
        {
            ErrorCode = errorCode;
            Component = component;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Gets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public string Component { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public override string ToString()
        {
            return $"{GetType()}{(string.IsNullOrWhiteSpace(Component) ? "" : $" on {Component}")}: ({ErrorCode}) - {Message}";
        }
    }
}
