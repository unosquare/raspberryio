namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Abstractions.Native;

    /// <summary>
    /// Implements settings for GY-521 accelerometer peripheral.
    /// </summary>
    public class AccelerometerGY521 : IDisposable
    {
        private const int PwrMgmt1 = 0x6b;
        private readonly Thread ReadWorker;
        private readonly TimeSpan ReadTime = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521"/> class.
        /// </summary>
        /// <param name="device"> i2C device. </param>
        public AccelerometerGY521(II2CDevice device)
        {
            Device = device;
            Device.WriteAddressByte(0x68, PwrMgmt1);

            ReadWorker = new Thread(Run);
        }

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
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
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
        }

        /// <summary>
        /// Retrieve the data capted by the sensor.
        /// </summary>
        /// <returns> Data calculated by GY-521 accelerometer. </returns>
        public AccelerometerGY521EventArgs RetrieveSensorData()
        {
            var gyro = new Point3d(ReadWord2C(0x43), ReadWord2C(0x45), ReadWord2C(0x47));
            var accel = new Point3d(ReadWord2C(0x3b), ReadWord2C(0x3d), ReadWord2C(0x3f));

            return new AccelerometerGY521EventArgs(gyro, accel);
        }

        private void Run()
        {
            var timer = new HighResolutionTimer();
            var lastElapsedTime = TimeSpan.FromSeconds(0);

            while(IsRunning == true)
            {
                if (lastElapsedTime < ReadTime)
                    Thread.Sleep(ReadTime - lastElapsedTime);

                timer.Start();
                var sensorData = RetrieveSensorData();
                DataAvailable.Invoke(this, sensorData);

                lastElapsedTime = timer.Elapsed;
                if (timer.IsRunning)
                    timer.Stop();

                timer.Reset();
            }
        }

        /// <summary>
        /// Stops the continuous reads.
        /// </summary>
        private void StopContinuousReads() =>
            IsRunning = false;

        /// <summary>
        /// Reads the byte.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>System.Int32.</returns>
        private int ReadByte(int register) =>
            Device.ReadAddressByte(register);

        /// <summary>
        /// Reads the word.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>System.Int32.</returns>
        private int ReadWord(int register)
        {
            int h = Device.ReadAddressByte(register);
            int l = Device.ReadAddressByte(register + 1);
            return (h << 8) + l;
        }

        /// <summary>
        /// Reads the word for i2C.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>System.Int32.</returns>
        private int ReadWord2C(int register)
        {
            int value = Device.ReadAddressWord(register);
            if (value == 0x8000)
                return -((65535 - value) + 1);
            else
                return value;
        }
    }
}
