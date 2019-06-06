namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;

    /// <summary>
    /// The DHT22 digital relative humidity and temperature sensor.
    /// </summary>
    /// <seealso cref="DhtSensor" />
    public class Dht22 : Dht21
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dht22" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid.</exception>
        protected internal Dht22(IGpioPin dataPin)
            : base(dataPin)
        {
        }
    }
}
