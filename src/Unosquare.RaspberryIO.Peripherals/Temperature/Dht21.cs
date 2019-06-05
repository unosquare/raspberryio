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
            PullDownMicroseconds = 1100;
        }

        /// <inheritdoc />
        protected override double DecodeHumidity(byte[] data) =>
            ((data[0] << 8) | data[1]) * 0.1;

        /// <inheritdoc />
        protected override double DecodeTemperature(byte[] data)
        {
            var temp = (((data[2] & 0x7F) << 8) | data[3]) * 0.1;
            if ((data[2] & 0x80) == 0x80)
                temp *= -1;

            return temp;
        }
    }
}
