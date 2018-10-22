namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    public sealed partial class InfraredSensor
    {
        /// <summary>
        /// Contains the raw sensor data event arguments.
        /// </summary>
        /// <seealso cref="EventArgs" />
        public class InfraredSensorPulseEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InfraredSensorPulseEventArgs" /> class.
            /// </summary>
            /// <param name="pulse">The pulse.</param>
            internal InfraredSensorPulseEventArgs(InfraredPulse pulse)
            {
                Value = pulse.Value;
                DurationUsecs = pulse.DurationUsecs;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="InfraredSensorPulseEventArgs"/> class from being created.
            /// </summary>
            private InfraredSensorPulseEventArgs()
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
