namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    public partial class WiringPi
    {
        #region WiringPi - SPI Library Calls

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>Unknown</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSPIGetFd", SetLastError = true)]
        public static extern int WiringPiSPIGetFd(int channel);

        /// <summary>
        /// This performs a simultaneous write/read transaction over the selected SPI bus. Data that was in your buffer is overwritten by data returned from the SPI bus.
        /// That’s all there is in the helper library. It is possible to do simple read and writes over the SPI bus using the standard read() and write() system calls though –
        /// write() may be better to use for sending data to chains of shift registers, or those LED strings where you send RGB triplets of data.
        /// Devices such as A/D and D/A converters usually need to perform a concurrent write/read transaction to work.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="data">The data.</param>
        /// <param name="len">The length.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSPIDataRW", SetLastError = true)]
        public static extern int WiringPiSPIDataRW(int channel, byte[] data, int len);

        /// <summary>
        /// This function is undocumented
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Unkown</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSPISetupMode", SetLastError = true)]
        public static extern int WiringPiSPISetupMode(int channel, int speed, int mode);

        /// <summary>
        /// This is the way to initialize a channel (The Pi has 2 channels; 0 and 1). The speed parameter is an integer
        /// in the range 500,000 through 32,000,000 and represents the SPI clock speed in Hz.
        /// The returned value is the Linux file-descriptor for the device, or -1 on error. If an error has happened, you may use the standard errno global variable to see why.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="speed">The speed.</param>
        /// <returns>The Linux file descriptor for the device or -1 for error</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "wiringPiSPISetup", SetLastError = true)]
        public static extern int WiringPiSPISetup(int channel, int speed);

        #endregion
    }
}
