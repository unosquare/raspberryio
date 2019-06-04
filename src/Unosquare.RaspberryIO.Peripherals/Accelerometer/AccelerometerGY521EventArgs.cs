namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    /// <summary>
    /// GY521 sensor measurements.
    /// </summary>
    public sealed class AccelerometerGY521EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521EventArgs"/> class.
        /// </summary>
        /// <param name="gyro">Gyroscope measurement.</param>
        /// <param name="accel">Accelerometer  measurement.</param>
        /// <param name="temperature">Temperature measurement.</param>
        internal AccelerometerGY521EventArgs(Point3D gyro, Point3D accel, double temperature)
        {
            Gyro = gyro;
            Accel = accel;
            Temperature = temperature;
        }

        /// <summary>
        /// Gyroscope's X factor.
        /// </summary>
        public Point3D Gyro { get; }

        /// <summary>
        /// Acceleration's X factor.
        /// </summary>
        public Point3D Accel { get; }

        /// <summary>
        /// Temperature measured by the sensor in celsius degrees.
        /// </summary>
        public double Temperature { get; }
    }
}
