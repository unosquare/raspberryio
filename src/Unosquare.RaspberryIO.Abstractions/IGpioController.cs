namespace Unosquare.RaspberryIO.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for Raspberry Pi GPIO controller.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{IGpioPin}" />
    public interface IGpioController : IReadOnlyCollection<IGpioPin>
    {
        /// <summary>
        /// Gets the <see cref="IGpioPin"/> with the specified BCM pin number.
        /// </summary>
        /// <value>
        /// The <see cref="IGpioPin"/>.
        /// </value>
        /// <param name="bcmPinNumber">The BCM pin number.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        IGpioPin this[int bcmPinNumber] { get; }
    }
}
