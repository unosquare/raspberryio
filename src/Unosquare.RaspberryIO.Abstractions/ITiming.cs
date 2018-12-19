namespace Unosquare.RaspberryIO.Abstractions
{
    /// <summary>
    /// Interface for timing methods using interop.
    /// </summary>
    public interface ITiming
    {
        /// <summary>
        /// This returns a number representing the number of milliseconds since system boot.
        /// </summary>
        /// <returns>The milliseconds since system boot.</returns>
        uint Milliseconds { get; }

        /// <summary>
        /// This returns a number representing the number of microseconds since system boot.
        /// </summary>
        /// <returns>The microseconds since system boot.</returns>
        uint Microseconds { get; }

        /// <summary>
        /// This causes program execution to pause for at least how long milliseconds.
        /// Due to the multi-tasking nature of Linux it could be longer.
        /// Note that the maximum delay is an unsigned 32-bit integer or approximately 49 days.
        /// </summary>
        /// <param name="millis">The number of milliseconds to sleep.</param>
        void SleepMilliseconds(uint millis);

        /// <summary>
        /// This causes program execution to pause for at least how long microseconds.
        /// Due to the multi-tasking nature of Linux it could be longer.
        /// Note that the maximum delay is an unsigned 32-bit integer microseconds or approximately 71 minutes.
        /// Delays under 100 microseconds are timed using a hard-coded loop continually polling the system time,
        /// Delays over 100 microseconds are done using the system nanosleep() function –
        /// You may need to consider the implications of very short delays on the overall performance of the system,
        /// especially if using threads.
        /// </summary>
        /// <param name="micros">The number of microseconds to sleep.</param>
        void SleepMicroseconds(uint micros);
    }
}
