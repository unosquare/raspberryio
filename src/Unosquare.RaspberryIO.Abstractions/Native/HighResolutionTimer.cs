namespace Unosquare.RaspberryIO.Abstractions.Native
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides access to a high-resolution, time measuring device.
    /// </summary>
    /// <seealso cref="Stopwatch" />
    public class HighResolutionTimer : Stopwatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionTimer"/> class.
        /// </summary>
        /// <exception cref="NotSupportedException">High-resolution timer not available.</exception>
        public HighResolutionTimer()
        {
            if (!IsHighResolution)
                throw new NotSupportedException("High-resolution timer not available");
        }

        /// <summary>
        /// Gets the numer of microseconds per timer tick.
        /// </summary>
        public static double MicrosecondsPerTick { get; } = 1000000d / Frequency;

        /// <summary>
        /// Gets the elapsed microseconds.
        /// </summary>
        public long ElapsedMicroseconds => (long)(ElapsedTicks * MicrosecondsPerTick);
    }
}
