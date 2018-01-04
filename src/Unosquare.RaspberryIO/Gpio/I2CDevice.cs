namespace Unosquare.RaspberryIO.Gpio
{
    using System;
    using System.Threading.Tasks;
    using Native;

    /// <summary>
    /// Represents a device on the I2C Bus
    /// </summary>
    public class I2CDevice
    {
        private readonly object SyncLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="I2CDevice"/> class.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="fileDescriptor">The file descriptor.</param>
        internal I2CDevice(int deviceId, int fileDescriptor)
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
        public int DeviceId { get; }

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
        /// <returns>The byte from device</returns>
        public byte Read()
        {
            lock (SyncLock)
            {
                var result = WiringPi.wiringPiI2CRead(FileDescriptor);
                if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(Read));
                return (byte)result;
            }
        }

        /// <summary>
        /// Reads a byte from the specified file descriptor
        /// </summary>
        /// <returns>The byte from device</returns>
        public async Task<byte> ReadAsync()
        {
            return await Task.Run(() => { return Read(); });
        }

        /// <summary>
        /// Reads a buffer of the specified length, one byte at a time
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The byte array from device</returns>
        public byte[] Read(int length)
        {
            lock (SyncLock)
            {
                var buffer = new byte[length];
                for (var i = 0; i < length; i++)
                {
                    var result = WiringPi.wiringPiI2CRead(FileDescriptor);
                    if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(Read));
                    buffer[i] = (byte)result;
                }

                return buffer;
            }
        }

        /// <summary>
        /// Reads a buffer of the specified length, one byte at a time
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The byte array from device</returns>
        public async Task<byte[]> ReadAsync(int length)
        {
            return await Task.Run(() => { return Read(length); });
        }

        /// <summary>
        /// Writes a byte of data the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte data)
        {
            lock (SyncLock)
            {
                var result = WiringPi.wiringPiI2CWrite(FileDescriptor, data);
                if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(Write));
            }
        }

        /// <summary>
        /// Writes a byte of data the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The awaitable task</returns>
        public async Task WriteAsync(byte data)
        {
            await Task.Run(() => { Write(data); });
        }

        /// <summary>
        /// Writes a set of bytes to the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte[] data)
        {
            var result = 0;
            lock (SyncLock)
            {
                foreach (var b in data)
                {
                    WiringPi.wiringPiI2CWrite(FileDescriptor, b);
                    if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(Write));
                }
            }
        }

        /// <summary>
        /// Writes a set of bytes to the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The awaitable task</returns>
        public async Task WriteAsync(byte[] data)
        {
            await Task.Run(() => { Write(data); });
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <param name="data">The data.</param>
        public void WriteAddressByte(int address, byte data)
        {
            lock (SyncLock)
            {
                var result = WiringPi.wiringPiI2CWriteReg8(FileDescriptor, address, data);
                if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(WriteAddressByte));
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <param name="data">The data.</param>
        public void WriteAddressWord(int address, ushort data)
        {
            lock (SyncLock)
            {
                var result = WiringPi.wiringPiI2CWriteReg16(FileDescriptor, address, data);
                if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(WriteAddressWord));
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <returns>The address byte from device</returns>
        public byte ReadAddressByte(int address)
        {
            lock (SyncLock)
            {
                var result = WiringPi.wiringPiI2CReadReg8(FileDescriptor, address);
                if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(ReadAddressByte));

                return (byte)result;
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="address">The register.</param>
        /// <returns>The address word from device</returns>
        public ushort ReadAddressWord(int address)
        {
            lock (SyncLock)
            {
                var result = WiringPi.wiringPiI2CReadReg16(FileDescriptor, address);
                if (result < 0) HardwareException.Throw(nameof(I2CDevice), nameof(ReadAddressWord));

                return Convert.ToUInt16(result);
            }
        }
    }
}
