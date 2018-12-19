namespace Unosquare.RaspberryIO.Peripherals
{
    public sealed partial class InfraredSensor
    {
        /// <summary>
        /// Represents data of an infrared pulse.
        /// </summary>
        public class InfraredPulse
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InfraredPulse"/> class.
            /// </summary>
            /// <param name="value">if set to <c>true</c> [value].</param>
            /// <param name="durationUsecs">The duration usecs.</param>
            internal InfraredPulse(bool value, long durationUsecs)
            {
                Value = value;
                DurationUsecs = durationUsecs;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="InfraredPulse"/> class from being created.
            /// </summary>
            private InfraredPulse()
            {
                // placeholder
            }

            /// <summary>
            /// Gets the signal value.
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Gets the duration microseconds.
            /// </summary>
            public long DurationUsecs { get; }
        }
    }
}
