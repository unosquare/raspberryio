namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interfaces a device on the I2C Bus.
    /// </summary>
    public interface II2CDevice
    {
        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        int DeviceId { get; }

        /// <summary>
        /// Gets the standard POSIX file descriptor.
        /// </summary>
        /// <value>
        /// The file descriptor.
        /// </value>
        int FileDescriptor { get; }
        
        /// <summary>
        /// Reads a byte from the specified file descriptor.
        /// </summary>
        /// <returns>The byte from device.</returns>
        byte Read();

        /// <summary>
        /// Reads a buffer of the specified length, one byte at a time.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The byte array from device.</returns>
        byte[] Read(int length);

        /// <summary>
        /// Writes a byte of data the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        void Write(byte data);

        /// <summary>
        /// Writes a set of bytes to the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        void Write(byte[] data);

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <param name="data">The data.</param>
        void WriteAddressByte(int address, byte data);

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <param name="data">The data.</param>
        void WriteAddressWord(int address, ushort data);

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <returns>The address byte from device.</returns>
        byte ReadAddressByte(int address);

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <returns>The address word from device.</returns>
        ushort ReadAddressWord(int address);
    }
}
