namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    /// <summary>
    /// Represents the sensor data that was read.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class UltrasonicReadEventArgs : EventArgs
    {
        internal UltrasonicReadEventArgs(double distance)
        {
            Distance = distance;
        }

        private UltrasonicReadEventArgs(bool isValid)
            : this(UltrasonicHcsr04.NoObstacleDistance)
        {
            IsValid = isValid;
        }

        /// <summary>
        /// Returns true if the reading was valid.
        /// </summary>
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether any obstacle was detected.
        /// </summary>
        public bool HasObstacles =>
            Distance != UltrasonicHcsr04.NoObstacleDistance;

        /// <summary>
        /// Gets the actual distance to the obstacle in centimeters, or <see cref="UltrasonicHcsr04.NoObstacleDistance"/> if no obstacle was detected.
        /// </summary>
        public double Distance { get; }

        /// <summary>
        /// Gets the actual distance to the obstacle in inches, or <see cref="UltrasonicHcsr04.NoObstacleDistance"/> if no obstacle was detected.
        /// </summary>
        public double DistanceInch =>
            Distance * 2.54;

        internal static UltrasonicReadEventArgs CreateInvalidReading() =>
            new UltrasonicReadEventArgs(false);
    }
}
