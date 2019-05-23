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

            GyroScaleX = Gyro.X / 131;
            GyroScaleY = Gyro.Y / 131;
            GyroScaleZ = Gyro.Z / 131;

            AccelScaleX = Accel.X / 16384.0;
            AccelScaleY = Accel.Y / 16384.0;
            AccelScaleZ = Accel.Z / 16384.0;

            RotationX = AccelScaleX.GetRotationX(AccelScaleY, AccelScaleZ);
            RotationY = AccelScaleX.GetRotationY(AccelScaleY, AccelScaleZ);
        }

        /// <summary>
        /// Gyroscope's X factor.
        /// </summary>
        public Point3d Gyro { get; }

        /// <summary>
        /// Gyroscope's X scale.
        /// </summary>
        public double GyroScaleX { get; }

        /// <summary>
        /// Gyroscope's X scale.
        /// </summary>
        public double GyroScaleY { get; }

        /// <summary>
        /// Gyroscope's X scale.
        /// </summary>
        public double GyroScaleZ { get; }

        /// <summary>
        /// Acceleration's X factor.
        /// </summary>
        public Point3d Accel { get; }

        /// <summary>
        /// Acceleration's X scale.
        /// </summary>
        public double AccelScaleX { get; }

        /// <summary>
        /// Acceleration's X scale.
        /// </summary>
        public double AccelScaleY { get; }

        /// <summary>
        /// Acceleration's X scale.
        /// </summary>
        public double AccelScaleZ { get; }

        /// <summary>
        /// Rotation along x axis.
        /// </summary>
        public double RotationX { get; }

        /// <summary>
        /// Rotation along Y axis.
        /// </summary>
        public double RotationY { get; }

        /// <summary>
        /// Formats AccelerometerArgs.
        /// </summary>
        /// <returns> String containing accelerometer data. </returns>
        public override string ToString() =>
            "Gyroscope data\n" +
            "  Acceleration\t\t Scale\t\n\n" +
                $"X: -{Gyro.X}\t\t-{GyroScaleX}\n" +
                $"Y: -{Gyro.Y}\t\t-{GyroScaleY}\n" +
                $"Z: -{Gyro.Z}\t\t-{GyroScaleZ}\n" +
            "Accelerometer data\n" +
            "  Acceleration\t\t Scale\t\n\n" +
                $"X: -{Accel.X}\t\t-{AccelScaleX}\n" +
                $"Y: -{Accel.Y}\t\t-{AccelScaleY}\n" +
                $"Z: -{Accel.Z}\t\t-{AccelScaleZ}\n" +
            "Rotation data\n" +
            "  Acceleration\n\n" +
                $"X: -{RotationX}\n" +
                $"Y: -{RotationY}\n";
    }
}
