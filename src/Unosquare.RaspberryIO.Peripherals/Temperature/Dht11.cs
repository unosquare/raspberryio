namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;

    /// <summary>
    /// The DHT11 digital relative humidity and temperature sensor.
    /// </summary>
    /// <seealso cref="DhtSensor" />
    public class Dht11 : DhtSensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dht11" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid.</exception>
        protected internal Dht11(IGpioPin dataPin)
            : base(dataPin)
        {
        }

        /// <inheritdoc />
        protected override double DecodeHumidity(byte[] data) =>
            (data[0] + (data[1] * 0.1)) / 100.0;

        /// <inheritdoc />
        protected override double DecodeTemperature(byte[] data) =>
            data[2] + ((data[3] & 0x0f) * 0.1);
    }
}
