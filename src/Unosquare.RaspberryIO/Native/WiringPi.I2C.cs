namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    public partial class WiringPi
    {
#pragma warning disable SA1300
        #region WiringPi - I2C Library Calls

        /// <summary>
        /// Simple device read. Some devices present data when you read them without having to do any register transactions.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CRead), SetLastError = true)]
        public static extern int wiringPiI2CRead(int fd);

        /// <summary>
        /// These read an 8-bit value from the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CReadReg8), SetLastError = true)]
        public static extern int wiringPiI2CReadReg8(int fd, int reg);

        /// <summary>
        /// These read a 16-bit value from the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CReadReg16), SetLastError = true)]
        public static extern int wiringPiI2CReadReg16(int fd, int reg);

        /// <summary>
        /// Simple device write. Some devices accept data this way without needing to access any internal registers.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CWrite), SetLastError = true)]
        public static extern int wiringPiI2CWrite(int fd, int data);

        /// <summary>
        /// These write an 8-bit data value into the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CWriteReg8), SetLastError = true)]
        public static extern int wiringPiI2CWriteReg8(int fd, int reg, int data);

        /// <summary>
        /// These write a 16-bit data value into the device register indicated.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="reg">The reg.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CWriteReg16), SetLastError = true)]
        public static extern int wiringPiI2CWriteReg16(int fd, int reg, int data);

        /// <summary>
        /// This initialises the I2C system with your given device identifier. 
        /// The ID is the I2C number of the device and you can use the i2cdetect program to find this out. wiringPiI2CSetup() 
        /// will work out which revision Raspberry Pi you have and open the appropriate device in /dev.
        /// The return value is the standard Linux filehandle, or -1 if any error – in which case, you can consult errno as usual.
        /// E.g. the popular MCP23017 GPIO expander is usually device Id 0x20, so this is the number you would pass into wiringPiI2CSetup().
        /// </summary>
        /// <param name="devId">The dev identifier.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiI2CSetup), SetLastError = true)]
        public static extern int wiringPiI2CSetup(int devId);

        #endregion
#pragma warning restore SA1300
    }
}
