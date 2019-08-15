namespace Unosquare.RaspberryIO.Peripherals
{
    using System;

    public sealed partial class LogicProbe
    {
        /// <summary>
        /// Event arguments representing probe data.
        /// </summary>
        /// <seealso cref="EventArgs" />
        public sealed class ProbeDataEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProbeDataEventArgs"/> class.
            /// </summary>
            /// <param name="timestamp">The timestamp.</param>
            /// <param name="value">if set to <c>true</c> [value].</param>
            internal ProbeDataEventArgs(long timestamp, bool value)
                : this()
            {
                Timestamp = timestamp;
                Value = value;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="ProbeDataEventArgs"/> class from being created.
            /// </summary>
            private ProbeDataEventArgs()
            {
                // placeholder
            }

            /// <summary>
            /// Gets the detection timestamp in microseconds.
            /// </summary>
            public long Timestamp { get; }

            /// <summary>
            /// Gets the read value at the given timestamp.
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Returns a <see cref="string" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="string" /> that represents this instance.
            /// </returns>
            public override string ToString() => $"{Timestamp}, {(Value ? "1" : "0")}";
        }
    }
}
