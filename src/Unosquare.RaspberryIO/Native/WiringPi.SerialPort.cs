namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    public partial class WiringPi
    {
        #region WiringPi - Serial Port

        /// <summary>
        /// This opens and initialises the serial device and sets the baud rate. It sets the port into “raw” mode (character at a time and no translations),
        /// and sets the read timeout to 10 seconds. The return value is the file descriptor or -1 for any error, in which case errno will be set as appropriate.
        /// The wiringSerial library is intended to provide simplified control – suitable for most applications, however if you need advanced control
        /// – e.g. parity control, modem control lines (via a USB adapter, there are none on the Pi’s on-board UART!) and so on,
        /// then you need to do some of this the old fashioned way.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="baud">The baud.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "serialOpen", SetLastError = true)]
        public static extern int SerialOpen(string device, int baud);

        /// <summary>
        /// Closes the device identified by the file descriptor given.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "serialClose", SetLastError = true)]
        public static extern int SerialClose(int fd);

        /// <summary>
        /// Sends the single byte to the serial device identified by the given file descriptor.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="c">The c.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "serialPutchar", SetLastError = true)]
        public static extern void SerialPutchar(int fd, byte c);

        /// <summary>
        /// Sends the nul-terminated string to the serial device identified by the given file descriptor.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="s">The s.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "serialPuts", SetLastError = true)]
        public static extern void SerialPuts(int fd, string s);

        /// <summary>
        /// Returns the number of characters available for reading, or -1 for any error condition,
        /// in which case errno will be set appropriately.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "serialDataAvail", SetLastError = true)]
        public static extern int SerialDataAvail(int fd);

        /// <summary>
        /// Returns the next character available on the serial device.
        /// This call will block for up to 10 seconds if no data is available (when it will return -1)
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <returns>The result</returns>
        [DllImport(WiringPiLibrary, EntryPoint = "serialGetchar", SetLastError = true)]
        public static extern int SerialGetchar(int fd);

        /// <summary>
        /// This discards all data received, or waiting to be send down the given device.
        /// </summary>
        /// <param name="fd">The fd.</param>
        [DllImport(WiringPiLibrary, EntryPoint = "serialFlush", SetLastError = true)]
        public static extern void SerialFlush(int fd);

        #endregion
    }
}