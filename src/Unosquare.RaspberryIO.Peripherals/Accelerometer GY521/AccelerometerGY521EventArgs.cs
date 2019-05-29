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
        /// <param name="gyro_sens"> Gyroscope sensitivity scale. </param>
        /// <param name="accl_sens"> Accelerometer sensitivity scale. </param>
        internal AccelerometerGY521EventArgs(Point3d gyro, Point3d accel, double temperature, double gyro_sens, double accl_sens)
        {
            Gyro = gyro;
            Accel = accel;
            Temperature = (temperature / 340.0) + 36.53;

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
        /// Temperature measured by the sensor.
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        /// Rotation along x axis.
        /// </summary>
        public Point3d Rotation { get; }

        /// <summary>
        /// Formats AccelerometerArgs.
        /// </summary>
        /// <returns> String containing accelerometer data. </returns>
        public override string ToString() =>
            "\n" +
            "Gyroscope data\n" +
                "       Orientation             Scale\n" +
                $"  X:  {Math.Round(Gyro.X, 2)}                    {Math.Round(GyroScale.X, 2)}\n" +
                $"  Y:  {Math.Round(Gyro.Y, 2)}                    {Math.Round(GyroScale.Y, 2)}\n" +
                $"  Z:  {Math.Round(Gyro.Z, 2)}                    {Math.Round(GyroScale.Z, 2)}\n\n" +
            "Accelerometer data\n" +
                "       Acceleration            Scale\n" +
                $"  X:  {Math.Round(Accel.X, 2)}                   {Math.Round(AccelScale.X, 2)}\n" +
                $"  Y:  {Math.Round(Accel.Y, 2)}                   {Math.Round(AccelScale.Y, 2)}\n" +
                $"  Z:  {Math.Round(Accel.Z, 2)}                   {Math.Round(AccelScale.Z, 2)}\n\n" +
            "Temperature data\n" +
                $"  Temperature: {Math.Round(Temperature, 2)}°C\n" +
                $"  Temperature: {Math.Round((Temperature * 1.8) + 32.0, 2)}°F\n\n" +
            "Rotation data\n" +
                "       Rotation\n" +
                $"  X:  {Math.Round(Rotation.X, 2)}°\n" +
                $"  Y:  {Math.Round(Rotation.Y, 2)}°\n\n";
    }
}
