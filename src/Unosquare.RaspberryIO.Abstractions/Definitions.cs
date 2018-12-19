namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Represents Definitions for GPIO information.
    /// </summary>
    public static class Definitions
    {
        private static readonly int[] GpioToPhysR1 =
        {
            3, 5, -1, -1, 7, -1, -1, 26, 24, 21, 19, 23, -1, -1, 8, 10, -1, 11, 12, -1, -1, 13, 15, 16, 18, 22, -1, -1, -1, -1, -1, -1,
        };

        private static readonly int[] GpioToPhysR2 =
        {
            27, 28, 3, 5, 7, 29, 31, 26, 24, 21, 19, 23, 32, 33, 8, 10, 36, 11, 12, 35, 38, 40, 15, 16, 18, 22, 37, 13, // P1
            3, 4, 5, 6, // P5
        };

        /// <summary>
        /// BCMs to physical pin number.
        /// </summary>
        /// <param name="rev">The rev.</param>
        /// <param name="bcmPin">The BCM pin.</param>
        /// <returns>The physical pin number.</returns>
        public static int BcmToPhysicalPinNumber(BoardRevision rev, BcmPin bcmPin) =>
            rev == BoardRevision.Rev1 ? GpioToPhysR1[(int)bcmPin] : GpioToPhysR2[(int)bcmPin];
    }
}
