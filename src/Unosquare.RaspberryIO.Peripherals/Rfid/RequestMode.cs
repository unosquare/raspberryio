namespace Unosquare.RaspberryIO.Peripherals
{
    public partial class RFIDControllerMfrc522
    {
        /// <summary>
        /// Enumerates the available request mode codes.
        /// </summary>
        public enum RequestMode : byte
        {
            /// <summary>
            /// The request idle mode
            /// </summary>
            RequestIdle = 0x26,

            /// <summary>
            /// The request all mode
            /// </summary>
            RequestAll = 0x52,

            /// <summary>
            /// The anti collision mode
            /// </summary>
            AntiCollision = 0x93,

            /// <summary>
            /// The select tag mode
            /// </summary>
            SelectTag = 0x93,

            /// <summary>
            /// The authenticate1 a mode
            /// </summary>
            Authenticate1A = 0x60,

            /// <summary>
            /// The authenticate1 b mode
            /// </summary>
            Authenticate1B = 0x61,

            /// <summary>
            /// The read mode
            /// </summary>
            Read = 0x30,

            /// <summary>
            /// The write mode
            /// </summary>
            Write = 0xA0,

            /// <summary>
            /// The decrement mode
            /// </summary>
            Decrement = 0xC0,

            /// <summary>
            /// The increment mode
            /// </summary>
            Increment = 0xC1,

            /// <summary>
            /// The restore mode
            /// </summary>
            Restore = 0xC2,

            /// <summary>
            /// The transfer mode
            /// </summary>
            Transfer = 0xB0,

            /// <summary>
            /// The halt mode
            /// </summary>
            Halt = 0x50,
        }
    }
}