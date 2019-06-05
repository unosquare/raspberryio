namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    /// <summary>
    /// Represents the sensor data that was read.
    /// </summary>
    public sealed class DhtReadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DhtReadEventArgs"/> class.
        /// </summary>
        /// <param name="temperatureCelsius">The temperature celsius.</param>
        /// <param name="humidityPercentage">The humidity percentage.</param>
        internal DhtReadEventArgs(double temperatureCelsius, double humidityPercentage)
        {
            Temperature = temperatureCelsius;
            HumidityPercentage = humidityPercentage;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="DhtReadEventArgs"/> class from being created.
        /// </summary>
        private DhtReadEventArgs()
        {
            // placeholder
        }

        /// <summary>
        /// Returns true if the sensor reading is valid; otherwise, <c>false</c>.
        /// </summary>
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Gets the temperature in celsius degrees.
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        /// Gets the temperature in fahrenheit degrees.
        /// </summary>
        public double TemperatureFahrenheit =>
            (Temperature * (9.0 / 5.0)) + 32.0;

        /// <summary>
        /// Gets the humidity percentage.
        /// </summary>
        public double HumidityPercentage { get; }

        internal static DhtReadEventArgs CreateInvalidReading() =>
            new DhtReadEventArgs { IsValid = false };
    }
}
