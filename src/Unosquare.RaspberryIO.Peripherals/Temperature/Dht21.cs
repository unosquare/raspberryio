namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;

    /// <summary>
    /// The DHT21 digital relative humidity and temperature sensor.
    /// </summary>
    /// <seealso cref="DhtSensor" />
    public class Dht21 : DhtSensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dht21" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid.</exception>
        protected internal Dht21(IGpioPin dataPin)
            : base(dataPin)
        {
        }

        protected override double DecodeHumidity(byte[] data)
        {
            throw new NotImplementedException();
        }

        protected override double DecodeTemperature(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
