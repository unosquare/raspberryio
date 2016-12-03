namespace Unosquare.RaspberryIO
{
    using System;

    /// <summary>
    /// A simple wrapper for the I2c bus on the Raspberry Pi
    /// </summary>
    public class I2cBus
    {
        static private I2cBus m_Instance = null;

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
        /// This initialises the I2C system with your given device identifier. 
        /// The ID is the I2C number of the device and you can use the i2cdetect program to find this out. 
        /// wiringPiI2CSetup() will work out which revision Raspberry Pi you have and open the appropriate device in /dev.
        /// The return value is the standard Linux filehandle, or -1 if any error – in which case, you can consult errno as usual.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        public int SetupFileDescriptor(int deviceId)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CSetup(deviceId);
            }

        }


        /// <summary>
        /// Reads from the specified file descriptor
        /// </summary>
        /// <param name="fileDescriptor">The descriptor.</param>
        /// <returns></returns>
        public int Read(int fileDescriptor)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CRead(fileDescriptor);
            }
        }

        /// <summary>
        /// Writes to the specified file descriptor.
        /// </summary>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int Write(int fileDescriptor, int data)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CWrite(fileDescriptor, data);
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="Int32">The int32.</param>
        /// <param name="register">The register.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int WriteRegisterByte(int fileDescriptor, int, int register, byte data)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CWriteReg8(fileDescriptor, register, data);
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="register">The register.</param>
        /// <returns></returns>
        public int ReadRegisterByte(int fileDescriptor, int register)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CReadReg8(fileDescriptor, register);
            }
        }

        /// <summary>
        /// These write an 8 or 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="Int32">The int32.</param>
        /// <param name="register">The register.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int WriteRegisterWord(int fileDescriptor, int, int register, UInt16 data)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CWriteReg16(fileDescriptor, register, data);
            }
        }

        /// <summary>
        /// These read an 8 or 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="register">The register.</param>
        /// <returns></returns>
        public int ReadRegisterWord(int fileDescriptor, int register)
        {
            lock (Pi.SyncLock)
            {
                return Interop.wiringPiI2CReadReg16(fileDescriptor, register);
            }
        }

    }
}
