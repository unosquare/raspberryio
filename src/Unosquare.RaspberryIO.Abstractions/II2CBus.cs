namespace Unosquare.RaspberryIO.Abstractions
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interfaces the I2c bus on the Raspberry Pi.
    /// </summary>
    public interface II2CBus
    {
        /// <summary>
        /// Gets the registered devices as a read only collection.
        /// </summary>
        ReadOnlyCollection<II2CDevice> Devices { get; }

        /// <summary>
        /// Gets the <see cref="II2CDevice"/> with the specified device identifier.
        /// </summary>
        /// <value>
        /// The <see cref="II2CDevice"/>.
        /// </value>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>A reference to an I2C device.</returns>
        II2CDevice this[int deviceId] { get; }

        /// <summary>
        /// Gets the device by identifier.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device reference.</returns>
        II2CDevice GetDeviceById(int deviceId);

        /// <summary>
        /// Adds a device to the bus by its Id. If the device is already registered it simply returns the existing device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device reference.</returns>
        II2CDevice AddDevice(int deviceId);
    }
}
