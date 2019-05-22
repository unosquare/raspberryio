namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions.Native;

    /// <summary>
    /// Implements settings for GY-521 accelerometer peripheral.
    /// </summary>
    public sealed class AccelerometerGY521 : IDisposable
    {
        private const int PwrMgmt1 = 0x6b;
        private const int PwrMgmt2 = 0x6c;
        private const int Address = 0x68;
        private static readonly int[] AllowedPinNumbers = { 1, 3, 5, 6 };
        private static readonly TimeSpan ReadInterval = TimeSpan.FromSeconds(1);
        private static readonly long BitPulseMidMicroseconds = 50; // (26 ... 50)µs for false; (51 ... 76)µs for true

        private readonly IGpioPin DataPin;
        private readonly Thread ReadWorker;

        static AccelerometerGY521()
        {
            ValidPins = new ReadOnlyCollection<IGpioPin>(
                Pi.Gpio
                    .Where(g => g.Header == GpioHeader.P1 &&
                                AllowedPinNumbers.Contains(g.PhysicalPinNumber))
                    .ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521"/> class.
        /// </summary>
        /// <param name="dataPin"> Data pin. </param>
        public AccelerometerGY521(IGpioPin dataPin)
        {
            if (ValidPins.Contains(dataPin) == false)
                throw new ArgumentException($"{nameof(dataPin)}, {dataPin} is not available to service this driver.");

            DataPin = dataPin;
            ReadWorker = new Thread(PerformContinuousReads);
        }

        /// <summary>
        /// Collection of valid pins to read data from.
        /// </summary>
        public static ReadOnlyCollection<IGpioPin> ValidPins { get; }

        /// <summary>
        /// Check if sensor is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Get distance between two points.
        /// </summary>
        /// <param name="a"> Point A. </param>
        /// <param name="b"> Point B. </param>
        /// <returns> Distance between two points. </returns>
        public static double Distance(double a, double b) =>
            Math.Sqrt((a * a) + (b * b));

        /// <summary>
        /// Get rotation on x axis.
        /// </summary>
        /// <param name="x"> Point X. </param>
        /// <param name="y"> Point Y. </param>
        /// <param name="z"> Point Z. </param>
        /// <returns> Rotation on degrees along X axis. </returns>
        public static double GetRotationX(double x, double y, double z)
        {
            double radians = Math.Atan2(y, Distance(x, z));
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Get rotation on y axis.
        /// </summary>
        /// <param name="x"> Point X. </param>
        /// <param name="y"> Point Y. </param>
        /// <param name="z"> Point Z. </param>
        /// <returns> Rotation on degrees along Y axis. </returns>
        public static double GetRotationY(double x, double y, double z)
        {
            double radians = Math.Atan2(x, Distance(y, z));
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Starts the sensor reading.
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            ReadWorker.Start();
        }

        /// <summary>
        /// Abort sensor reading.
        /// </summary>
        public void Dispose()
        {
            if (IsRunning == false)
                return;
            StopContinuousReads();
        }

        /// <summary>
        /// Performs a continuous read with the sensor.
        /// </summary>
        private void PerformContinuousReads()
        {
        }

        private AccelerometerGY521EventArgs RetrieveSensorData()
        {
            //return new AccelerometerGY521EventArgs();
        }

        private void StopContinuousReads() =>
            IsRunning = false;
    }

    /// <summary>
    /// Encapsulates data obtained by accelerometer's sensor.
    /// </summary>
    public sealed class AccelerometerGY521EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521EventArgs"/> class.
        /// Builds object of Accelerometer args.
        /// </summary>
        /// <param name="gyro_x"> Gyroscope X factor. </param>
        /// <param name="gyro_y"> Gyroscope Y factor. </param>
        /// <param name="gyro_z"> Gyroscope Z factor. </param>
        /// <param name="gyro_sx"> Gyroscope X factor scale. </param>
        /// <param name="gyro_sy"> Gyroscope Y factor scale. </param>
        /// <param name="gyro_sz"> Gyroscope Z factor scale. </param>
        /// <param name="accel_x"> Accelerometer X factor. </param>
        /// <param name="accel_y"> Accelerometer Y factor. </param>
        /// <param name="accel_z"> Accelerometer Z factor. </param>
        /// <param name="accel_sx"> Accelerometer X factor scale. </param>
        /// <param name="accel_sy"> Accelerometer Y factor scale. </param>
        /// <param name="accel_sz"> Accelerometer Z factor scale. </param>
        /// <param name="rot_x"> Rotation X factor. </param>
        /// <param name="rot_y"> Rotation Y factor. </param>
        internal AccelerometerGY521EventArgs(
            double gyro_x,
            double gyro_y,
            double gyro_z,
            double gyro_sx,
            double gyro_sy,
            double gyro_sz,
            double accel_x,
            double accel_y,
            double accel_z,
            double accel_sx,
            double accel_sy,
            double accel_sz,
            double rot_x,
            double rot_y)
        {
            GyroX = gyro_x;
            GyroY = gyro_y;
            GyroZ = gyro_z;
            GyroScaleX = gyro_sx;
            GyroScaleY = gyro_sy;
            GyroScaleZ = gyro_sz;

            AccelX = accel_x;
            AccelY = accel_y;
            AccelZ = accel_z;
            AccelScaleX = accel_sx;
            AccelScaleY = accel_sy;
            AccelScaleZ = accel_sz;

            RotationX = rot_x;
            RotationY = rot_y;
        }

        /// <summary>
        /// Gyroscope's X factor.
        /// </summary>
        public double GyroX { get; }

        /// <summary>
        /// Gyroscope's Y factor.
        /// </summary>
        public double GyroY { get; }

        /// <summary>
        /// Gyroscope's Z factor.
        /// </summary>
        public double GyroZ { get; }

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
        public double AccelX { get; }

        /// <summary>
        /// Acceleration's X factor.
        /// </summary>
        public double AccelY { get; }

        /// <summary>
        /// Acceleration's X factor.
        /// </summary>
        public double AccelZ { get; }

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
        /// Formats accelrometerargs.
        /// </summary>
        /// <returns> String containing accelerometer args data. </returns>
        public override string ToString() =>
            "Gyroscope data\n" +
            "  Acceleration\t\t Scale\t\n\n" +
                $"X: -{ GyroX }\t\t-{ GyroScaleX }\n" +
                $"Y: -{ GyroY }\t\t-{ GyroScaleY }\n" +
                $"Z: -{ GyroZ }\t\t-{ GyroScaleZ }\n" +
            "Accelerometer data\n" +
            "  Acceleration\t\t Scale\t\n\n" +
                $"X: -{ AccelX }\t\t-{ AccelScaleX }\n" +
                $"Y: -{ AccelY }\t\t-{ AccelScaleY }\n" +
                $"Z: -{ AccelZ }\t\t-{ AccelScaleZ }\n" +
            "Rotation data\n" +
            "  Acceleration\n\n" +
                $"X: -{ RotationX }\n" +
                $"Y: -{ RotationY }\n";
    }
}
