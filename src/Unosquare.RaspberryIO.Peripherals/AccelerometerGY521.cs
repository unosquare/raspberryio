namespace Unosquare.RaspberryIO.Peripherals
{
    using Unosquare.RaspberryIO.Abstractions;

    /// <summary>
    /// Implements settings for GY-521 accelerometer peripheral.
    /// </summary>
    public class AccelerometerGY521
    {
        private const byte PwrMgmt1 = 0x6b;
        private const byte PwrAddress = 0x68;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerGY521"/> class.
        /// </summary>
        /// <param name="device"> i2C device. </param>
        public AccelerometerGY521(II2CDevice device)
        {
            Device = device;
            Device.WriteAddressByte(PwrAddress, PwrMgmt1);
        }

        /// <summary>
        /// Retrieves the I2C Device.
        /// </summary>
        public II2CDevice Device { get; }

        /// <summary>
        /// Retrieve the data capted by the sensor.
        /// </summary>
        /// <returns> Data calculated by GY-521 accelerometer. </returns>
        public AccelerometerGY521EventArgs RetrieveSensorData()
        {
            var gyro = new Point3d(Device.ReadAddressWord(0x43), Device.ReadAddressWord(0x45), Device.ReadAddressWord(0x47));
            var accel = new Point3d(Device.ReadAddressWord(0x3b), Device.ReadAddressWord(0x3d), Device.ReadAddressWord(0x3f));

            return new AccelerometerGY521EventArgs(gyro, accel);
        }
    }
}
