namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using System.Linq;

    public sealed partial class InfraredSensor
    {
        /// <summary>
        /// Represents event arguments for when a receiver buffer is ready to be decoded.
        /// </summary>
        /// <seealso cref="EventArgs" />
        public class InfraredSensorDataEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InfraredSensorDataEventArgs" /> class.
            /// </summary>
            /// <param name="pulses">The pulses.</param>
            /// <param name="flushReason">The state.</param>
            internal InfraredSensorDataEventArgs(InfraredPulse[] pulses, ReceiverFlushReason flushReason)
            {
                Pulses = pulses;
                FlushReason = flushReason;
                TrainDurationUsecs = pulses.Sum(s => s.DurationUsecs);
            }

            /// <summary>
            /// Gets the array fo IR pulses.
            /// </summary>
            public InfraredPulse[] Pulses { get; }

            /// <summary>
            /// Gets the state of the receiver that triggered the event.
            /// </summary>
            public ReceiverFlushReason FlushReason { get; }

            /// <summary>
            /// Gets the pulse train duration in microseconds.
            /// </summary>
            public long TrainDurationUsecs { get; }
        }
    }
}
