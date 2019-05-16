namespace Unosquare.RaspberryIO.Computer
{
    using System;

    /// <summary>
    /// Manage the volume of any sound device.
    /// </summary>
    public class AudioState
    {
        private int level;
        private float decibels;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioState"/> class.
        /// </summary>
        /// <param name="level"> Initial Volume level on percentage (%). </param>
        public AudioState(int level)
        {
            Level = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioState"/> class.
        /// </summary>
        /// <param name="db"> Initial Volume level on decibels (dB). </param>
        public AudioState(float db)
        {
            Decibels = db;
        }

        /// <summary>
        /// Volume level for the audio device, can be converted to decibels (db).
        /// </summary>
        public int Level
        {
            get => level;
            set => level = Math.Min(Math.Max(0, value), 100);
        }

        /// <summary>
        /// Volume level for the audio device, can be converted to decibels (db).
        /// </summary>
        public float Decibels
        {
            get => decibels;
            set => decibels = value;
        }

        /// <summary>
        /// Checks for a muted device, or a 0% volume level.
        /// </summary>
        public bool IsMute { get; set; }
    }
}