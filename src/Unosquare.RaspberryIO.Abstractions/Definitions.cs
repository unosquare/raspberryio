namespace Unosquare.RaspberryIO.Abstractions
{
    public static class Definitions
    {
        private static readonly int[] Gpio2PhysR1 =
        {
            3, 5, -1, -1, 7, -1, -1, 26, 24, 21, 19, 23, -1, -1, 8, 10, -1, 11, 12, -1, -1, 13, 15, 16, 18, 22, -1, -1, -1, -1, -1, -1,
        };

        private static readonly int[] Gpio2PhysR2 =
        {
            27, 28, 3, 5, 7, 29, 31, 26, 24, 21, 19, 23, 32, 33, 8, 10, 36, 11, 12, 35, 38, 40, 15, 16, 18, 22, 37, 13, // P1
            3, 4, 5, 6, // P5
        };

        public static int GetPhysicalPin(BoardRevision rev, BcmPin bcmPin) =>
            rev == BoardRevision.Rev1 ? Gpio2PhysR1[(int)bcmPin] : Gpio2PhysR2[(int)bcmPin];
    }
}
