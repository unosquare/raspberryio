namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    /// <summary>
    /// Encapsulates data obtained by accelerometer's sensor.
    /// </summary>
    public sealed class AccelerometerGY521EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521EventArgs"/> class.
        /// Builds object of Accelerometer args.
        /// </summary>
        /// <param name="gyro"> Gyroscope 3D point. </param>
        /// <param name="accel"> Acceleration 3D point. </param>
        /// <param name="temperature"> GY521 temperature output. </param>
        /// <param name="gyroSens"> Gyroscope sensitivity scale. </param>
        /// <param name="acclSens"> Accelerometer sensitivity scale. </param>
        internal AccelerometerGY521EventArgs(Point3d gyro, Point3d accel, double temperature, Point3d gyroSens, Point3d acclSens)
        {
            Gyro = gyro;
            Accel = accel;
            Temperature = temperature;
            GyroScale = gyroSens;
            AccelScale = acclSens;
            Rotation = new Point3d(Extensions.GetRotationX(AccelScale), Extensions.GetRotationY(AccelScale), 0.0);
        }

        /// <summary>
        /// Gyroscope's X factor.
        /// </summary>
        public Point3d Gyro { get; }

        /// <summary>
        /// Gyroscope's X scale.
        /// </summary>
        public Point3d GyroScale { get; }

        /// <summary>
        /// Acceleration's X factor.
        /// </summary>
        public Point3d Accel { get; }

        /// <summary>
        /// Acceleration's X scale.
        /// </summary>
        public Point3d AccelScale { get; }

        /// <summary>
        /// Temperature measured by the sensor.
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        /// Rotation along x axis.
        /// </summary>
        public Point3d Rotation { get; }
    }
}
