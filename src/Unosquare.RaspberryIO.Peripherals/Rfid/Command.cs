namespace Unosquare.RaspberryIO.Peripherals
{
    public partial class RFIDControllerMfrc522
    {
        /// <summary>
        /// Contains constants for well-known commands.
        /// </summary>
        public enum Command : byte
        {
            /// <summary>
            /// The idle Command
            /// </summary>
            Idle = 0x00,

            /// <summary>
            /// The authenticate Command
            /// </summary>
            Authenticate = 0x0E,

            /// <summary>
            /// The receive Command
            /// </summary>
            Receive = 0x08,

            /// <summary>
            /// The transmit Command
            /// </summary>
            Transmit = 0x04,

            /// <summary>
            /// The transcieve Command
            /// </summary>
            Transcieve = 0x0C,

            /// <summary>
            /// The reset phase Command
            /// </summary>
            ResetPhase = 0x0F,

            /// <summary>
            /// The compute CRC Command
            /// </summary>
            ComputeCrc = 0x03,
        }
    }
}