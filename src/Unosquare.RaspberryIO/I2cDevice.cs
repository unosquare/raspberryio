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
        /// Gets the file descriptor.
        /// </summary>
        /// <value>
        /// The file descriptor.
        /// </value>
        public int FileDescriptor { get; private set; }


        /// <summary>
        /// Reads from the specified file descriptor
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CRead(FileDescriptor);
            }
        }

        /// <summary>
        /// Writes to the specified file descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int Write(int data)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CWrite(FileDescriptor, data);
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int WriteRegisterByte(int register, byte data)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CWriteReg8(FileDescriptor, register, data);
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns></returns>
        public int ReadRegisterByte(int register)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CReadReg8(FileDescriptor, register);
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int WriteRegisterWord(int register, UInt16 data)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CWriteReg16(FileDescriptor, register, data);
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns></returns>
        public int ReadRegisterWord(int register)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CReadReg16(FileDescriptor, register);
            }
        }
    }
}
