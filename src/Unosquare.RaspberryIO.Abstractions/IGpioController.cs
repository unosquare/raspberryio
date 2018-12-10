﻿namespace Unosquare.RaspberryIO.Abstractions
{
    using System;
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
        /// <param name="bcmPin">The BCM pin.</param>
        /// <returns>A reference to the GPIO pin.</returns>
        IGpioPin this[BcmPin bcmPin] { get; }
    }
}
