namespace Unosquare.RaspberryIO.Computer
{
    using System;

    /// <summary>
    /// Manage the volume of any sound device.
    /// </summary>
    public class AudioState
    {
        private int _level;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioState"/> class.
        /// </summary>
        /// <param name="level"> Initial Volume level on percentage (%). </param>
        public AudioState(int level)
        {
            Level = level;
        }

        /// <summary>
        /// Volume level for the audio device, can be converted to decibels (db).
        /// </summary>
        public int Level
        {
            get => _level;
            set
            {
                _level = Math.Min(Math.Max(0, value), 100);
            }
        }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public int CardNumber { get; set; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public float Decibels { get; set; }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public bool IsMute { get; set; }

        /// <summary>
        /// Prints out the audio state.
        /// </summary>
        /// <returns> The data of . </returns>
        public override string ToString()
        {
            return "Device information: \n" +
                ">> Device name: " + DeviceName + "\n" +
                ">> Card number: " + CardNumber + "\n" +
                ">> Volume level (%): " + Level + "%\n" +
                ">> Volume level (dB): " + Decibels + "dB\n" +
                ">> Mute: [" + (IsMute ? "On" : "Off") + "]\n\n";
        }
    }
}