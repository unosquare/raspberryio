namespace Unosquare.RaspberryIO.Native
{
    using System.Runtime.InteropServices;

    public partial class WiringPi
    {
        #region WiringPi - Shift Library

        /// <summary>
        /// This shifts an 8-bit data value in with the data appearing on the dPin and the clock being sent out on the cPin. 
        /// Order is either LSBFIRST or MSBFIRST. The data is sampled after the cPin goes high. 
        /// (So cPin high, sample data, cPin low, repeat for 8 bits) The 8-bit value is returned by the function.
        /// </summary>
        /// <param name="dPin">The d pin.</param>
        /// <param name="cPin">The c pin.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(shiftIn), SetLastError = true)]
        public static extern byte shiftIn(byte dPin, byte cPin, byte order);

        /// <summary>
        /// The shifts an 8-bit data value val out with the data being sent out on dPin and the clock being sent out on the cPin. 
        /// order is as above. Data is clocked out on the rising or falling edge – ie. dPin is set, then cPin is taken high then low 
        /// – repeated for the 8 bits.
        /// </summary>
        /// <param name="dPin">The d pin.</param>
        /// <param name="cPin">The c pin.</param>
        /// <param name="order">The order.</param>
        /// <param name="val">The value.</param>
        [DllImport(WiringPiLibrary, EntryPoint = nameof(shiftOut), SetLastError = true)]
        public static extern void shiftOut(byte dPin, byte cPin, byte order, byte val);

        #endregion

    }
}
