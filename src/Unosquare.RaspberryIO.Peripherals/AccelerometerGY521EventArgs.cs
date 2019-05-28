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
        /// <param name="gyro_sens"> Gyroscope sensitivity scale. </param>
        /// <param name="accl_sens"> Accelerometer sensitivity scale. </param>
        internal AccelerometerGY521EventArgs(Point3d gyro, Point3d accel, double gyro_sens, double accl_sens)
        {
            Gyro = gyro;
            Accel = accel;

            GyroScale = Gyro / gyro_sens;
            AccelScale = Accel / accl_sens;

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
        /// Rotation along x axis.
        /// </summary>
        public Point3d Rotation { get; }

        /// <summary>
        /// Formats AccelerometerArgs.
        /// </summary>
        /// <returns> String containing accelerometer data. </returns>
        public override string ToString() =>
            "\nGyroscope data\n" +
                "       Orientation             Scale\n" +
                $"  X: {Gyro.X}                 {GyroScale.X}\n" +
                $"  Y: {Gyro.Y}                 {GyroScale.Y}\n" +
                $"  Z: {Gyro.Z}                 {GyroScale.Z}\n\n" +
            "Accelerometer data\n" +
                "       Acceleration            Scale\n" +
                $"  X: {Accel.X}                {AccelScale.X}\n" +
                $"  Y: {Accel.Y}                {AccelScale.Y}\n" +
                $"  Z: {Accel.Z}                {AccelScale.Z}\n\n" +
            "Rotation data\n" +
                "       Rotation\n" +
                $"  X: {Rotation.X}\n" +
                $"  Y: {Rotation.Y}\n\n";
    }
}
