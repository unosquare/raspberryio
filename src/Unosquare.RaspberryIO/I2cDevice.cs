namespace Unosquare.RaspberryIO
{
    using System;

    /// <summary>
    /// Represents a device on the I2C Bus
    /// </summary>
    public class I2cDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2cDevice"/> class.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="fileDescriptor">The file descriptor.</param>
        internal I2cDevice(int deviceId, int fileDescriptor)
        {
            DeviceId = deviceId;
            FileDescriptor = fileDescriptor;
        }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public int DeviceId { get; private set; }

        /// <summary>
        /// Gets the standard POSIX file descriptor.
        /// </summary>
        /// <value>
        /// The file descriptor.
        /// </value>
        public int FileDescriptor { get; }

        /// <summary>
        /// Reads a byte from the specified file descriptor
        /// </summary>
        /// <returns></returns>
        public byte Read()
        {
            lock (Pi.SyncLock)
            {
                var result = Interop.wiringPiI2CRead(FileDescriptor);
                if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(Read));
                return (byte)result;
            }
        }

        /// <summary>
        /// Reads a buffer of the specified length, one byte at a time
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public byte[] Read(int length)
        {
            lock (Pi.SyncLock)
            {
                var result = 0;
                var buffer = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    result = Interop.wiringPiI2CRead(FileDescriptor);
                    if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(Read));
                    buffer[i] = (byte)result;
                }

                return buffer;
            }
        }

        /// <summary>
        /// Writes a byte of data the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte data)
        {
            lock (Pi.SyncLock)
            {
                var result = Interop.wiringPiI2CWrite(FileDescriptor, data);
                if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(Write));
            }
        }

        /// <summary>
        /// Writes a set of bytes to the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte[] data)
        {
            var result = 0;
            lock (Pi.SyncLock)
            {
                foreach (var b in data)
                {
                    Interop.wiringPiI2CWrite(FileDescriptor, b);
                    if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(Write));
                }
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <param name="data">The data.</param>
        public void WriteAddressByte(int address, byte data)
        {
            lock (Pi.SyncLock)
            {
                var result = Interop.wiringPiI2CWriteReg8(FileDescriptor, address, data);
                if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(WriteAddressByte));
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <param name="data">The data.</param>
        public void WriteAddressWord(int address, ushort data)
        {
            lock (Pi.SyncLock)
            {
                var result = Interop.wiringPiI2CWriteReg16(FileDescriptor, address, data);
                if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(WriteAddressWord));
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <returns></returns>
        public byte ReadAddressByte(int address)
        {
            lock (Pi.SyncLock)
            {
                var result = Interop.wiringPiI2CReadReg8(FileDescriptor, address);
                if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(ReadAddressByte));

                return (byte)result;
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <returns></returns>
        public ushort ReadAddressWord(int address)
        {
            lock (Pi.SyncLock)
            {
                var result = Interop.wiringPiI2CReadReg16(FileDescriptor, address);
                if (result < 0) HardwareException.Throw(nameof(I2cDevice), nameof(ReadAddressWord));

                return Convert.ToUInt16(result);
            }
        }
    }
}
