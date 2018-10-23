namespace Unosquare.WiringPI
{
    using Native;
    using RaspberryIO.Abstractions;

    /// <summary>
    /// Provides access to timing and threading properties and methods.
    /// </summary>
    public class Timing : ITiming
    {
        /// <inheritdoc />
        /// <summary>
        /// This returns a number representing the number of milliseconds since your program
        /// initialized the GPIO controller.
        /// It returns an unsigned 32-bit number which wraps after 49 days.
        /// </summary>
        public uint Milliseconds => WiringPi.Millis();

        /// <inheritdoc />
        /// <summary>
        /// This returns a number representing the number of microseconds since your
        /// program initialized the GPIO controller
        /// It returns an unsigned 32-bit number which wraps after approximately 71 minutes.
        /// </summary>
        public uint Microseconds => WiringPi.Micros();

        /// <inheritdoc cref="ITiming.SleepMilliseconds(uint)" />
        public static void Sleep(uint millis) => WiringPi.Delay(millis);

        /// <inheritdoc />
        public void SleepMilliseconds(uint millis) => Sleep(millis);

        /// <inheritdoc />
        public void SleepMicroseconds(uint micros) => WiringPi.DelayMicroseconds(micros);
    }
}
