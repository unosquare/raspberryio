namespace Unosquare.RaspberryIO
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// A simple wrapper for the I2c bus on the Raspberry Pi
    /// </summary>
    public class I2cBus
    {
        // TODO: It would be nice to integrate i2c device detection. 

        static private I2cBus m_Instance = null;
        static private readonly Dictionary<int, I2cDevice> m_Devices = new Dictionary<int, I2cDevice>();


        /// <summary>
        /// Gets the instance of this singleton.
        /// </summary>
        static internal I2cBus Instance
        {
            get
            {
                lock (Pi.SyncLock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new I2cBus();
                    }

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="I2cBus"/> class from being created.
        /// </summary>
        private I2cBus() { }

        /// <summary>
        /// Gets the registered devices as a read only collection.
        /// </summary>
        public ReadOnlyCollection<I2cDevice> Devices
        {
            get { return new ReadOnlyCollection<I2cDevice>(m_Devices.Values.ToArray()); }
        }

        /// <summary>
        /// Gets the device by identifier.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        public I2cDevice GetDeviceById(int deviceId)
        {
            lock (Pi.SyncLock)
            {
                return m_Devices[deviceId];
            }
        }

        /// <summary>
        /// Adds a device to the bus by its Id. If the device is already registered it siply returns the existing device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public I2cDevice AddDevice(int deviceId)
        {
            if (m_Devices.ContainsKey(deviceId))
                return m_Devices[deviceId];

            var fileDescriptor = SetupFileDescriptor(deviceId);
            if (fileDescriptor < 0)
                throw new KeyNotFoundException($"Device with id {deviceId} could not be registered with the I2C bus. Error Code: {fileDescriptor}.");

            lock (Pi.SyncLock)
            {
                var device = new I2cDevice(deviceId, fileDescriptor);
                m_Devices[deviceId] = device;
                return device;
            }
        }

        /// <summary>
        /// This initialises the I2C system with your given device identifier. 
        /// The ID is the I2C number of the device and you can use the i2cdetect program to find this out. 
        /// wiringPiI2CSetup() will work out which revision Raspberry Pi you have and open the appropriate device in /dev.
        /// The return value is the standard Linux filehandle, or -1 if any error – in which case, you can consult errno as usual.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        private static int SetupFileDescriptor(int deviceId)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CSetup(deviceId);
            }

        }

    }
}
