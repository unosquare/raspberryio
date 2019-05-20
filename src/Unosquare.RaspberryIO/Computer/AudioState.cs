namespace Unosquare.RaspberryIO.Computer
{
    /// <summary>
    /// Manage the volume of any sound device.
    /// </summary>
    public struct AudioState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioState"/> struct.
        /// </summary>
        /// <param name="cardNumber"> Card number. </param>
        /// <param name="deviceName"> Controller name. </param>
        /// <param name="level"> Initial Volume level on percentage (%). </param>
        /// <param name="decibels"> Volume in decibels. </param>
        /// <param name="isMute"> Device is muted or sounding. </param>
        public AudioState(int cardNumber, string deviceName, int level, float decibels, bool isMute)
        {
            CardNumber = cardNumber;
            DeviceName = deviceName;
            Level = level;
            Decibels = decibels;
            IsMute = isMute;
        }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public int CardNumber { get; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public string DeviceName { get; }

        /// <summary>
        /// Volume level for the audio device, can be converted to decibels (db).
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public float Decibels { get; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public bool IsMute { get; }

        /// <summary>
        /// Prints out the audio state.
        /// </summary>
        /// <returns> Audio state info. </returns>
        public override string ToString() => 
            "Device information: \n" +
                $">> Name: {DeviceName}\n" +
                $">> Card number: {CardNumber}\n" +
                $">> Volume (%): {Level}%\n" +
                $">> Volume (dB): {Decibels:0.00}dB\n" +
                $">> [{(IsMute ? "Off" : "On")}]\n\n";
    }
}