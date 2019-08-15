namespace Unosquare.RaspberryIO.Peripherals
{
    using Abstractions;
    using Swan.Diagnostics;
    using System;
    using System.Threading;

    /// <summary>
    /// Implements settings for GY-521 accelerometer peripheral.
    /// </summary>
    public sealed class AccelerometerGY521 : IDisposable
    {
        private const int PwrMgmt1 = 0x6b;
        private const int TemperatureOutput = 0x41;
        private const int GyroOutputX = 0x43;
        private const int GyroOutputY = 0x45;
        private const int GyroOutputZ = 0x47;
        private const int AcclOutputX = 0x3b;
        private const int AcclOutputY = 0x3d;
        private const int AcclOutputZ = 0x3f;
        private const int GyroConfig = 0x1b;
        private const int AcclConfig = 0x1c;
        private readonly TimeSpan ReadTime = TimeSpan.FromSeconds(0.5);

        private readonly double[] GyroSensitivity = { 131.0, 65.5, 32.8, 16.4 };
        private readonly double[] AccelSensitivity = { 16384.0, 8192.0, 4096.0, 2048.0 };
        private bool _disposedValue; // To detect redundant calls

        private Thread ReadWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521"/> class 
        /// with default values for gyroscope (250 °/s) and accelerometer (2g) scale range.
        /// </summary>
        /// <param name="device">The device.</param>
        public AccelerometerGY521(II2CDevice device)
            : this(device, GfsSel.Fsr250, AfsSel.Fsr2G)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521"/> class.
        /// </summary>
        /// <param name="device">I2C device.</param>
        /// <param name="gyroScale">The gyroscope sensitivity factor.</param>
        /// <param name="accelScale">The accelerometer sensitivity factor.</param>
        public AccelerometerGY521(II2CDevice device, GfsSel gyroScale, AfsSel accelScale)
        {
            Device = device;
            GyroscopeScale = gyroScale;
            AccelerometerScale = accelScale;
            ReadWorker = new Thread(Run);
        }

        /// <summary>
        /// Occurs when [data available].
        /// </summary>
        public event EventHandler<AccelerometerGY521EventArgs> DataAvailable;

        /// <summary>
        /// Retrieves the I2C Device.
        /// </summary>
        public II2CDevice Device { get; }

        /// <summary>
        /// Gets the current gyroscope full scale factor.
        /// </summary>
        public GfsSel GyroscopeScale { get; }

        /// <summary>
        /// Gets the current accelerometer full scale factor.
        /// </summary>
        public AfsSel AccelerometerScale { get; }

        /// <summary>
        /// Gets the gyroscope sensitivity.
        /// </summary>
        public double GyroscopeSensitivity =>
            GyroSensitivity[(int)GyroscopeScale];

        /// <summary>
        /// Gets the accelerometer sensitivity.
        /// </summary>
        public double AccelerometerSensitivity =>
            AccelSensitivity[(int)AccelerometerScale];

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the instance.
        /// </summary>
        public void Start()
        {
            // Write Gyro and Accel full scale configuration
            Device.WriteAddressByte(GyroConfig, (byte)((byte)GyroscopeScale << 3));
            Device.WriteAddressByte(AcclConfig, (byte)((byte)AccelerometerScale << 3));
            
            // Reset sensor sleep mode to 0.
            SetSleepMode(false);

            IsRunning = true;
            ReadWorker.Start();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            var timer = new HighResolutionTimer();
            var lastElapsedTime = TimeSpan.FromSeconds(0);

            while(IsRunning)
            {
                if (lastElapsedTime < ReadTime)
                    Thread.Sleep(ReadTime - lastElapsedTime);

                timer.Start();

                var sensorData = RetrieveSensorData();
                lastElapsedTime = timer.Elapsed;

                if (IsRunning)
                    DataAvailable?.Invoke(this, sensorData);

                timer.Reset();
            }

            SetSleepMode(true);
        }

        /// <summary>
        /// Retrieve the data capted by the sensor.
        /// </summary>
        /// <returns> Data calculated by GY-521 accelerometer. </returns>
        private AccelerometerGY521EventArgs RetrieveSensorData()
        {
            // Read registry for gyroscope and accelerometer on the three axis.
            var rawGyro = new Point3D(ReadWord2C(GyroOutputX), ReadWord2C(GyroOutputY), ReadWord2C(GyroOutputZ));
            var rawAccel = new Point3D(ReadWord2C(AcclOutputX), ReadWord2C(AcclOutputY), ReadWord2C(AcclOutputZ));
            var temperature = (ReadWord2C(TemperatureOutput) / 340.0) + 36.53;
            var gyro = rawGyro / GyroscopeSensitivity;
            var accel = rawAccel / AccelerometerSensitivity;

            return new AccelerometerGY521EventArgs(gyro, accel, temperature);
        }

        /// <summary>
        /// Sets the sleep mode.
        /// </summary>
        /// <param name="sleep">if set to <c>true</c> puts the sensor into sleep mode.</param>
        private void SetSleepMode(bool sleep) =>
            Device.WriteAddressByte(PwrMgmt1, (byte)(sleep ? 0x40 : 0));

        /// <summary>
        /// Stops the continuous reads.
        /// </summary>
        private void StopContinuousReads() =>
            IsRunning = false;

        /// <summary>
        /// Reads the word.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>System.Int32.</returns>
        private int ReadWord(int register)
        {
            var h = Device.ReadAddressByte(register);
            var l = Device.ReadAddressByte(register + 1);
            return (h << 8) + l;
        }

        /// <summary>
        /// Reads the word for i2C.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>System.Int32.</returns>
        private int ReadWord2C(int register)
        {
            var value = ReadWord(register);
            return (value & 0x8000) != 0 ? -1 * (0x10000 - value) : value;
        }

        /// <inheritdoc />
        public void Dispose() =>
            Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                StopContinuousReads();
            }

            ReadWorker = null;
            _disposedValue = true;
        }
    }
}
