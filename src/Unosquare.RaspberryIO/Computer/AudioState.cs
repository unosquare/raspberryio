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
        /// <param name="controlName"> Control name. </param>
        /// <param name="level"> Initial Volume level on percentage (%). </param>
        /// <param name="decibels"> Volume in decibels. </param>
        /// <param name="isMute"> Device is muted or sounding. </param>
        public AudioState(int cardNumber, string controlName, int level, float decibels, bool isMute)
        {
            CardNumber = cardNumber;
            ControlName = controlName;
            Level = level;
            Decibels = decibels;
            IsMute = isMute;
        }

        /// <summary>
        /// Card number.
        /// </summary>
        public int CardNumber { get; }

        /// <summary>
        /// Control name.
        /// </summary>
        public string ControlName { get; }

        /// <summary>
        /// Volume level for the audio device, can be converted to decibels (db).
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Volume level in percentage.
        /// </summary>
        public float Decibels { get; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public bool IsMute { get; }
        
        /// <summary>
        /// Presents audio state info in format.
        /// </summary>
        /// <returns> String containing audio state info. </returns>
        public override string ToString() => 
            "Device information: \n" +
                $">> Name: {ControlName}\n" +
                $">> Card number: {CardNumber}\n" +
                $">> Volume (%): {Level}%\n" +
                $">> Volume (dB): {Decibels:0.00}dB\n" +
                $">> Mute: [{(IsMute ? "Off" : "On")}]\n\n";
    }
}