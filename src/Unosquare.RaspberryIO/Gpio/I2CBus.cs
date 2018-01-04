namespace Unosquare.RaspberryIO.Gpio
{
    using Native;
    using Swan.Abstractions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// A simple wrapper for the I2c bus on the Raspberry Pi
    /// </summary>
    public class I2CBus : SingletonBase<I2CBus>
    {
        // TODO: It would be nice to integrate i2c device detection.
        private static readonly object SyncRoot = new object();
        private readonly Dictionary<int, I2CDevice> m_Devices = new Dictionary<int, I2CDevice>();

        /// <summary>
        /// Prevents a default instance of the <see cref="I2CBus"/> class from being created.
        /// </summary>
        private I2CBus()
        {
            // placeholder
        }

        /// <summary>
        /// Gets the registered devices as a read only collection.
        /// </summary>
        public ReadOnlyCollection<I2CDevice> Devices => new ReadOnlyCollection<I2CDevice>(m_Devices.Values.ToArray());

        /// <summary>
        /// Gets the device by identifier.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device reference</returns>
        public I2CDevice GetDeviceById(int deviceId)
        {
            lock (SyncRoot)
            {
                return m_Devices[deviceId];
            }
        }

        /// <summary>
        /// Adds a device to the bus by its Id. If the device is already registered it simply returns the existing device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device reference</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">When the device file descriptor is not found</exception>
        public I2CDevice AddDevice(int deviceId)
        {
            lock (SyncRoot)
            {
                if (m_Devices.ContainsKey(deviceId))
                    return m_Devices[deviceId];

                var fileDescriptor = SetupFileDescriptor(deviceId);
                if (fileDescriptor < 0)
                    throw new KeyNotFoundException($"Device with id {deviceId} could not be registered with the I2C bus. Error Code: {fileDescriptor}.");

                var device = new I2CDevice(deviceId, fileDescriptor);
                m_Devices[deviceId] = device;
                return device;
            }            
        }

        /// <summary>
        /// This initializes the I2C system with your given device identifier. 
        /// The ID is the I2C number of the device and you can use the i2cdetect program to find this out. 
        /// wiringPiI2CSetup() will work out which revision Raspberry Pi you have and open the appropriate device in /dev.
        /// The return value is the standard Linux filehandle, or -1 if any error – in which case, you can consult errno as usual.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The Linux file handle</returns>
        private static int SetupFileDescriptor(int deviceId)
        {
            lock (SyncRoot)
            {
                return WiringPi.wiringPiI2CSetup(deviceId);
            }
        }
    }
}
