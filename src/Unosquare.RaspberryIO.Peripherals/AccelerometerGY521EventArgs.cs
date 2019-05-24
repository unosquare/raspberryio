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
        internal AccelerometerGY521EventArgs(
            Point3d gyro,
            Point3d accel)
        {
            Gyro = gyro;
            Accel = accel;
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
        /// Rotation along x axis.
        /// </summary>
        public Point3d Rotation { get; }

        /// <summary>
        /// Formats AccelerometerArgs.
        /// </summary>
        /// <returns> String containing accelerometer data. </returns>
        public override string ToString() =>
            "\nGyroscope data\n" +
            "   Acceleration\n" +
                $"   X: {Gyro.X}\n" +
                $"   Y: {Gyro.Y}\n" +
                $"   Z: {Gyro.Z}\n\n" +
            "Accelerometer data\n" +
            "   Acceleration\n" +
                $"   X: {Accel.X}\n" +
                $"   Y: {Accel.Y}\n" +
                $"   Z: {Accel.Z}\n\n";
    }
}
