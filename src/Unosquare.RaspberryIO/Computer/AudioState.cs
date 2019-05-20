namespace Unosquare.RaspberryIO.Computer
{
    /// <summary>
    /// Manage the volume of any sound device.
    /// </summary>
    public readonly struct AudioState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioState"/> struct.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <param name="level">The volume level in percentaje.</param>
        /// <param name="decibels">The volume level in decibels.</param>
        /// <param name="isMute">if set to <c>true</c> the audio is mute.</param>
        public AudioState(int cardNumber, string controlName, int level, float decibels, bool isMute)
        {
            CardNumber = cardNumber;
            ControlName = controlName;
            Level = level;
            Decibels = decibels;
            IsMute = isMute;
        }

        /// <summary>
        /// Gets the card number.
        /// </summary>
        public int CardNumber { get; }

        /// <summary>
        /// Gets the name of the current control.
        /// </summary>
        public string ControlName { get; }

        /// <summary>
        /// Gets the volume level in percentage.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the volume level in decibels.
        /// </summary>
        public float Decibels { get; }

        /// <summary>
        /// Gets a value indicating whether the audio is mute.
        /// </summary>
        public bool IsMute { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the audio state.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents the audio state.
        /// </returns>
        public override string ToString() =>
            "Device information: \n" +
                $">> Name: {ControlName}\n" +
                $">> Card number: {CardNumber}\n" +
                $">> Volume (%): {Level}%\n" +
                $">> Volume (dB): {Decibels:0.00}dB\n" +
                $">> Mute: [{(IsMute ? "Off" : "On")}]\n\n";
    }
}