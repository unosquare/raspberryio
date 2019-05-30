namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Abstractions.Native;

    /// <summary>
    /// Implements settings for GY-521 accelerometer peripheral.
    /// </summary>
    public sealed class AccelerometerGY521 : IDisposable
    {
        private const int PwrMgmt1 = 0x6b;
        private const int GyroOutputX = 0x43;
        private const int GyroOutputY = 0x45;
        private const int GyroOutputZ = 0x47;
        private const int AcclOutputX = 0x3b;
        private const int AcclOutputY = 0x3d;
        private const int AcclOutputZ = 0x3f;
        private const int TemperatureOutput = 0x41;
        private const int GyroConfig = 0x1b;
        private const int AcclConfig = 0x1c;
        private readonly Thread ReadWorker;
        private readonly TimeSpan ReadTime = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521"/> class.
        /// </summary>
        /// <param name="device"> i2C device. </param>
        /// <param name="gyroSens">The gyroscope sensitivity factor.</param>
        /// <param name="acclSens">The accelerometer sensitivity factor.</param>
        public AccelerometerGY521(II2CDevice device, FSSEL gyroSens, AFSSEL acclSens)
        {
            Device = device;
            var gyroScale = (int)gyroSens << 3;
            var acclScale = (int)acclSens << 3;
            Device.WriteAddressByte(GyroConfig, (byte)gyroScale);
            Device.WriteAddressByte(AcclConfig, (byte)acclScale);
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
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the instance.
        /// </summary>
        public void Start()
        {
            // Reset sensor sleep mode to 0.
            Device.WriteAddressByte(PwrMgmt1, 0);
            IsRunning = true;
            ReadWorker.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Dispose()
        {
            if (IsRunning == false)
                return;
            StopContinuousReads();
            SetSleepMode();
        }

        /// <summary>
        /// Retrieve the data capted by the sensor.
        /// </summary>
        /// <returns> Data calculated by GY-521 accelerometer. </returns>
        public AccelerometerGY521EventArgs RetrieveSensorData()
        {
            // Read registry for gyroscope and accelerometer on the three axis.
            var gyro = new Point3d(ReadWord2C(GyroOutputX), ReadWord2C(GyroOutputY), ReadWord2C(GyroOutputZ));
            var accel = new Point3d(ReadWord2C(AcclOutputX), ReadWord2C(AcclOutputY), ReadWord2C(AcclOutputZ));
            var temperature = (ReadWord2C(TemperatureOutput) / 340.0) + 36.53;
            var gyroSens = gyro / Device.ReadAddressByte(GyroConfig);
            var acclSens = accel / Device.ReadAddressByte(AcclConfig);

            return new AccelerometerGY521EventArgs(gyro, accel, temperature, gyroSens, acclSens);
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
                DataAvailable?.Invoke(this, sensorData);
                lastElapsedTime = timer.Elapsed;
                if (timer.IsRunning)
                    timer.Stop();

                timer.Reset();
            }
        }

        /// <summary>
        /// Sets the sleep mode.
        /// </summary>
        private void SetSleepMode() =>
            Device.WriteAddressByte(PwrMgmt1, 0x40);

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
            return value >= 0x8000 ? -((65535 - value) + 1) : value;
        }
    }
}
