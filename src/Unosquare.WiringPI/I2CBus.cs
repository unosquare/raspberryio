namespace Unosquare.WiringPI
{
    using Native;
    using RaspberryIO.Abstractions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// A simple wrapper for the I2c bus on the Raspberry Pi.
    /// </summary>
    public class I2CBus : II2CBus
    {
        // TODO: It would be nice to integrate i2c device detection.
        private static readonly object SyncRoot = new object();
        private readonly Dictionary<int, II2CDevice> _devices = new Dictionary<int, II2CDevice>();

        /// <inheritdoc />
        public ReadOnlyCollection<II2CDevice> Devices => new ReadOnlyCollection<II2CDevice>(_devices.Values.ToArray());

        /// <inheritdoc />
        public II2CDevice this[int deviceId] => GetDeviceById(deviceId);

        /// <inheritdoc />
        public II2CDevice GetDeviceById(int deviceId)
        {
            lock (SyncRoot)
            {
                return _devices[deviceId];
            }
        }

        /// <inheritdoc />
        /// <exception cref="KeyNotFoundException">When the device file descriptor is not found.</exception>
        public II2CDevice AddDevice(int deviceId)
        {
            lock (SyncRoot)
            {
                if (_devices.ContainsKey(deviceId))
                    return _devices[deviceId];

                var fileDescriptor = SetupFileDescriptor(deviceId);
                if (fileDescriptor < 0)
                    throw new KeyNotFoundException($"Device with id {deviceId} could not be registered with the I2C bus. Error Code: {fileDescriptor}.");

                var device = new I2CDevice(deviceId, fileDescriptor);
                _devices[deviceId] = device;
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
        /// <returns>The Linux file handle.</returns>
        private static int SetupFileDescriptor(int deviceId)
        {
            lock (SyncRoot)
            {
                return WiringPi.WiringPiI2CSetup(deviceId);
            }
        }
    }
}
