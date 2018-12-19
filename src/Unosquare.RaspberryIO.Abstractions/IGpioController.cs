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
        /// Gets the <see cref="IGpioPin"/> with the specified BCM pin.
        /// </summary>
        /// <value>
        /// The <see cref="IGpioPin"/>.
        /// </value>
        /// <param name="bcmPinNumber">The BCM pin number.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        IGpioPin this[int bcmPinNumber] { get; }

        /// <summary>
        /// Gets the <see cref="IGpioPin"/> with the specified BCM pin.
        /// </summary>
        /// <value>
        /// The <see cref="IGpioPin"/>.
        /// </value>
        /// <param name="bcmPin">The BCM pin.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        IGpioPin this[BcmPin bcmPin] { get; }

        /// <summary>
        /// Gets the <see cref="IGpioPin"/> with the specified pin number.
        /// </summary>
        /// <value>
        /// The <see cref="IGpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number in header P1.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        IGpioPin this[P1 pinNumber] { get; }

        /// <summary>
        /// Gets the <see cref="IGpioPin"/> with the specified pin number.
        /// </summary>
        /// <value>
        /// The <see cref="IGpioPin"/>.
        /// </value>
        /// <param name="pinNumber">The pin number in header P5.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        IGpioPin this[P5 pinNumber] { get; }
    }
}
