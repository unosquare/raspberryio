namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;

    /// <summary>
    /// The DHT12 digital relative humidity and temperature sensor.
    /// </summary>
    /// <seealso cref="DhtSensor" />
    public class Dht12 : Dht11
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dht12" /> class.
        /// </summary>
        /// <param name="dataPin">The data pin. Must be a GPIO-only pin on the P1 Header of the Pi.</param>
        /// <exception cref="ArgumentException">dataPin When it is invalid.</exception>
        protected internal Dht12(IGpioPin dataPin)
            : base(dataPin)
        {
        }

        /// <inheritdoc />
        protected override double DecodeTemperature(byte[] data)
        {
            var temp = (data[2] & 0x7F) + ((data[3] & 0x0f) * 0.1);
            if ((data[2] & 0x80) == 0x80)
                temp *= -1;

            return temp;
        }
    }
}
