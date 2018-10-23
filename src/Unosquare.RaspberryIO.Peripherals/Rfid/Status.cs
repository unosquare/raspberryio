namespace Unosquare.RaspberryIO.Peripherals
{
    public partial class RFIDControllerMfrc522
    {
        /// <summary>
        /// Enumerates the different statuses.
        /// </summary>
        public enum Status : byte
        {
            /// <summary>
            /// All ok status
            /// </summary>
            AllOk = 0,

            /// <summary>
            /// The no tag error status
            /// </summary>
            NoTagError = 1,

            /// <summary>
            /// The error status
            /// </summary>
            Error = 2,
        }
    }
}