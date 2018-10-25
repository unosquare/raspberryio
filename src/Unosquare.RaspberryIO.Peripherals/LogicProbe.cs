namespace Unosquare.RaspberryIO.Peripherals
{
    using System;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO.Abstractions.Native;

    /// <summary>
    /// A class representing a logic probe that reads high or low digital values when
    /// an edge change is detected. This is not meant for high frequency probing.
    /// The maximum fairly relibale probing frequency is at most 10kHz (100us periods).
    /// </summary>
    public sealed class LogicProbe
    {
        private readonly IGpioPin _inputPin;
        private readonly HighResolutionTimer _timer = new HighResolutionTimer();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicProbe"/> class.
        /// </summary>
        /// <param name="inputPin">The input pin.</param>
        public LogicProbe(IGpioPin inputPin)
        {
            _inputPin = inputPin;
            _inputPin.PinMode = GpioPinDriveMode.Input;
            _inputPin.InputPullMode = GpioPinResistorPullMode.PullUp;

            _inputPin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, () =>
            {
                if (_timer.IsRunning == false)
                    return;

                var value = _inputPin.Read();
                var elapsed = _timer.ElapsedMicroseconds;
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
        public bool IsRunning => _timer.IsRunning;

        /// <summary>
        /// Starts probing.
        /// </summary>
        public void Start() => _timer.Start();

        /// <summary>
        /// Pauses probing.
        /// </summary>
        public void Pause() => _timer.Stop();

        /// <summary>
        /// Stops measurement and resets the timer to zero.
        /// </summary>
        public void Stop() => _timer.Reset();

        /// <summary>
        /// Restarts probing at the 0 timestamp.
        /// </summary>
        public void Restart() => _timer.Restart();

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
