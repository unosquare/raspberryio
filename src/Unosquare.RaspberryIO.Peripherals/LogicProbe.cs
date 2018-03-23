namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using Gpio;
    using Native;

    /// <summary>
    /// A class representing a logic probe that reads high or low digital values when
    /// an edge change is detected. This is not meant for high frequency probing.
    /// The maximum fairly relibale probing frequency is at most 10kHz (100us periods).
    /// </summary>
    public sealed class LogicProbe
    {
        private readonly GpioPin InputPin = null;
        private readonly HighResolutionTimer Timer = new HighResolutionTimer();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicProbe"/> class.
        /// </summary>
        /// <param name="inputPin">The input pin.</param>
        public LogicProbe(GpioPin inputPin)
        {
            InputPin = inputPin;
            InputPin.PinMode = GpioPinDriveMode.Input;
            InputPin.InputPullMode = GpioPinResistorPullMode.PullUp;

            InputPin.RegisterInterruptCallback(EdgeDetection.RisingAndFallingEdges, () =>
            {
                if (Timer.IsRunning == false)
                    return;

                var value = InputPin.Read();
                var elapsed = Timer.ElapsedMicroseconds;
                var data = new ProbeDataEventArgs(elapsed, value);
                ProbeDataAvailable?.Invoke(this, data);
            });
        }

        /// <summary>
        /// Occurs when probe data becomes available.
        /// </summary>
        public event EventHandler<ProbeDataEventArgs> ProbeDataAvailable;

        /// <summary>
        /// Gets a value indicating whether the probe is running.
        /// </summary>
        public bool IsRunning => Timer.IsRunning;

        /// <summary>
        /// Starts probing.
        /// </summary>
        public void Start() => Timer.Start();

        /// <summary>
        /// Pauses probing.
        /// </summary>
        public void Pause() => Timer.Stop();

        /// <summary>
        /// Stops measurement and resets the timer to zero.
        /// </summary>
        public void Stop() => Timer.Reset();

        /// <summary>
        /// Restarts probing at the 0 timestamp
        /// </summary>
        public void Restart() => Timer.Restart();

        /// <summary>
        /// Event arguments representing probe data
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
                : base()
            {
                // placeholder
            }

            /// <summary>
            /// Gets the detection timestamp in microseconds.
            /// </summary>
            public long Timestamp { get; }

            /// <summary>
            /// Gets the read value at the given timestamp
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Returns a <see cref="string" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="string" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return $"{Timestamp}, {(Value ? "1" : "0")}";
            }
        }
    }
}
