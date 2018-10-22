namespace Unosquare.RaspberryIO.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for Raspberry Pi GPIO controller.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{IGpioPin}" />
    public interface IGpioController : IReadOnlyCollection<IGpioPin>
    {
    }
}
