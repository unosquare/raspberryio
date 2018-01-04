namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    public partial class WiringPi
    {
#pragma warning disable SA1306
        #region WiringPi - SPI Library Calls

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>Unknown</returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPIGetFd), SetLastError = true)]
        public static extern int wiringPiSPIGetFd(int channel);

        /// <summary>
        /// This performs a simultaneous write/read transaction over the selected SPI bus. Data that was in your buffer is overwritten by data returned from the SPI bus.
        /// That’s all there is in the helper library. It is possible to do simple read and writes over the SPI bus using the standard read() and write() system calls though – 
        /// write() may be better to use for sending data to chains of shift registers, or those LED strings where you send RGB triplets of data. 
        /// Devices such as A/D and D/A converters usually need to perform a concurrent write/read transaction to work.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="data">The data.</param>
        /// <param name="len">The length.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPIDataRW), SetLastError = true)]
        public static extern int wiringPiSPIDataRW(int channel, byte[] data, int len);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Unkown</returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPISetupMode), SetLastError = true)]
        public static extern int wiringPiSPISetupMode(int channel, int speed, int mode);

        /// <summary>
        /// This is the way to initialize a channel (The Pi has 2 channels; 0 and 1). The speed parameter is an integer 
        /// in the range 500,000 through 32,000,000 and represents the SPI clock speed in Hz.
        /// The returned value is the Linux file-descriptor for the device, or -1 on error. If an error has happened, you may use the standard errno global variable to see why.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="speed">The speed.</param>
        /// <returns>The Linux file descriptor for the device or -1 for error</returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(wiringPiSPISetup), SetLastError = true)]
        public static extern int wiringPiSPISetup(int channel, int speed);

        #endregion
#pragma warning restore SA1306
    }
}
