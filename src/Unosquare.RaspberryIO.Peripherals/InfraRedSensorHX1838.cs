namespace Unosquare.RaspberryIO.Peripherals
{
    using Gpio;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Unosquare.Swan;

    /// <summary>
    /// Implements a digital infrared sensor using the HX1838 38kHz digital receiver.
    /// It registers an interrupt on the pin and fires data events asynchronously.
    /// Ideas taken from https://github.com/adafruit/IR-Commander/blob/master/ircommander.pde
    /// and https://github.com/z3t0/Arduino-IRremote/blob/master/IRremote.cpp
    /// </summary>
    public sealed class InfraRedSensorHX1838 : IDisposable
    {
        private const bool Mark = false;
        private const bool Space = true;
        private const long MicrosecondsPerTimeUnit = 50;
        private const double MarkExcess = 100;
        private const double GapMicroseconds = 5000;
        private const double GapTimeUnits = GapMicroseconds / MicrosecondsPerTimeUnit;
        private const int SensorBufferLength = 101;

        private readonly Thread ReadThread;
        private ManualResetEvent ReadThreadLockStopped = new ManualResetEvent(true);
        private bool IsDisposed = false; // To detect redundant calls
        private bool IsStopRequested = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfraRedSensorHX1838"/> class.
        /// </summary>
        /// <param name="inputPin">The input pin.</param>
        public InfraRedSensorHX1838(GpioPin inputPin)
        {
            InputPin = inputPin;
            InputPin.PinMode = GpioPinDriveMode.Input;
            InputPin.InputPullMode = GpioPinResistorPullMode.PullUp;

            ReadThread = new Thread(ReadThreadDoWork)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true,
                Name = nameof(InfraRedSensorHX1838)
            };

            ReadThread.Start();
        }

        /// <summary>
        /// Occurs when a single sensor pulse is available.
        /// </summary>
        public event EventHandler<InfraRedSensorPulseEventArgs> PulseAvailable;

        /// <summary>
        /// Occurs when a data buffer is available.
        /// </summary>
        public event EventHandler<InfraRedSensorDataEventArgs> DataAvailable;

        /// <summary>
        /// Enumerates the different receiver state machine states
        /// </summary>
        public enum ReceiverStates
        {
            /// <summary>
            /// The idle state
            /// </summary>
            Idle = 2,

            /// <summary>
            /// The mark state
            /// </summary>
            Mark = 3,

            /// <summary>
            /// The space state
            /// </summary>
            Space = 4,

            /// <summary>
            /// The stop state
            /// </summary>
            Stop = 5,

            /// <summary>
            /// The overflow state
            /// </summary>
            Overflow = 6,
        }

        /// <summary>
        /// Gets the input pin.
        /// </summary>
        public GpioPin InputPin { get; }

        /// <summary>
        /// Gets the state of the receiver state machine.
        /// </summary>
        public ReceiverStates ReceiverState { get; private set; } = ReceiverStates.Idle;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    IsStopRequested = true;
                    ReadThreadLockStopped?.WaitOne();
                    IsStopRequested = false;
                    ReadThreadLockStopped?.Dispose();
                }

                ReadThreadLockStopped = null;
                IsDisposed = true;
            }
        }

        /// <summary>
        /// State machine to handle continuous reads of data
        /// </summary>
        private void ReadThreadDoWork()
        {
            // Signal that the worker is in the running state
            ReadThreadLockStopped.Reset();

            // Setup state variables
            var delayMicroseconds = 0L;
            var timeUnitsCount = default(uint);
            var currentSensorValue = false;
            var pulseBuffer = new List<InfraRedPulse>(SensorBufferLength);
            var pulseTimer = new Native.HighResolutionTimer();
            var trainTimer = new Native.HighResolutionTimer();

            // Get the timer started!
            pulseTimer.Start();
            trainTimer.Start();

            while (IsStopRequested == false)
            {
                pulseTimer.Restart();
                currentSensorValue = InputPin.Read();
                timeUnitsCount += 1; // One more 50 uS tick
                if (pulseBuffer.Count >= SensorBufferLength)
                    ReceiverState = ReceiverStates.Overflow;

                switch (ReceiverState)
                {
                    // In the middle of a gap
                    case ReceiverStates.Idle:
                        {
                            if (currentSensorValue != Mark) break;

                            if (timeUnitsCount < GapTimeUnits)
                            {
                                // Not big enough to be a gap.
                                timeUnitsCount = 0;
                            }
                            else
                            {
                                // Gap just ended; Record duration; Start recording transmission
                                trainTimer.Restart();
                                pulseBuffer.Clear();
                                var pulse = new InfraRedPulse(Mark, timeUnitsCount);
                                pulseBuffer.Add(pulse);
                                OnInfraredSensorPulseAvailable(pulse);
                                ReceiverState = ReceiverStates.Mark;
                            }

                            break;
                        }

                    // Timing Mark
                    case ReceiverStates.Mark:
                        {
                            if (currentSensorValue != Space) break;

                            var pulse = new InfraRedPulse(Space, timeUnitsCount);
                            pulseBuffer.Add(pulse);
                            OnInfraredSensorPulseAvailable(pulse);

                            // Reset for next pulse
                            timeUnitsCount = 0;
                            ReceiverState = ReceiverStates.Space;
                            break;
                        }

                    // Timing Space
                    case ReceiverStates.Space:
                        {
                            if (currentSensorValue == Mark)
                            {
                                var pulse = new InfraRedPulse(Space, timeUnitsCount);
                                pulseBuffer.Add(pulse);
                                OnInfraredSensorPulseAvailable(pulse);

                                // Reset for next pulse
                                timeUnitsCount = 0;
                                ReceiverState = ReceiverStates.Mark;
                            }
                            else if (timeUnitsCount > GapTimeUnits)
                            {
                                // A long Space, indicates gap between codes
                                // Flag the current code as ready for processing
                                // Switch to STOP
                                // Don't reset timer; keep counting Space width
                                ReceiverState = ReceiverStates.Stop;
                            }

                            break;
                        }

                    // Waiting; Measuring Gap
                    case ReceiverStates.Stop:
                        {
                            // Reset gap timer if we have a Mark
                            if (currentSensorValue == Mark)
                                timeUnitsCount = 0;

                            OnInfraredSensorRawDataAvailable(pulseBuffer.ToArray(), ReceiverStates.Stop, trainTimer.ElapsedMicroseconds);
                            pulseBuffer.Clear();
                            ReceiverState = ReceiverStates.Idle;
                            break;
                        }

                    // Flag up a read overflow; Stop the State Machine
                    case ReceiverStates.Overflow:
                        {
                            OnInfraredSensorRawDataAvailable(pulseBuffer.ToArray(), ReceiverStates.Overflow, trainTimer.ElapsedMicroseconds);
                            pulseBuffer.Clear();
                            ReceiverState = ReceiverStates.Idle;
                            break;
                        }
                }

                if (IsStopRequested)
                    break;

                // Compute the amount of microseconds to pause execution
                delayMicroseconds = (MicrosecondsPerTimeUnit - pulseTimer.ElapsedMicroseconds).Clamp(0, MicrosecondsPerTimeUnit);
                if (delayMicroseconds > 0)
                    Pi.Timing.SleepMicroseconds(Convert.ToUInt32(delayMicroseconds));
            }

            pulseTimer.Stop();
            ReadThreadLockStopped.Set();
        }

        /// <summary>
        /// Called when a single infrared sensor pulse becomes available.
        /// </summary>
        /// <param name="pulse">The pulse.</param>
        private void OnInfraredSensorPulseAvailable(InfraRedPulse pulse)
        {
            PulseAvailable?.Invoke(this, new InfraRedSensorPulseEventArgs(pulse));
        }

        /// <summary>
        /// Called when an infrared sensor raw data buffer becomes available.
        /// </summary>
        /// <param name="pulses">The pulses.</param>
        /// <param name="state">The state.</param>
        /// <param name="trainDurationMicroseconds">The train duration in microseconds.</param>
        private void OnInfraredSensorRawDataAvailable(InfraRedPulse[] pulses, ReceiverStates state, long trainDurationMicroseconds)
        {
            DataAvailable?.Invoke(this, new InfraRedSensorDataEventArgs(pulses, state, trainDurationMicroseconds));
        }

        /// <summary>
        /// Represents data of an infrared pulse
        /// </summary>
        public struct InfraRedPulse
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InfraRedPulse" /> struct.
            /// </summary>
            /// <param name="value">if set to <c>true</c> [value].</param>
            /// <param name="timeUnits">The time units.</param>
            internal InfraRedPulse(bool value, long timeUnits)
            {
                Value = value;
                DurationMicroseconds = timeUnits * MicrosecondsPerTimeUnit;
                TimeUnits = timeUnits;
            }

            /// <summary>
            /// Gets the signal value
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Gets the duration microseconds.
            /// </summary>
            public long DurationMicroseconds { get; }

            /// <summary>
            /// Gets the time units.
            /// </summary>
            public long TimeUnits { get; }
        }

        /// <summary>
        /// Represents event arguments for when a receiver buffer is ready to be decoded.
        /// </summary>
        /// <seealso cref="EventArgs" />
        public class InfraRedSensorDataEventArgs : EventArgs
        {
            internal InfraRedSensorDataEventArgs(InfraRedPulse[] pulses, ReceiverStates state, long trainDurationMicroseconds)
            {
                Pulses = pulses;
                State = state;
                TrainDurationMicroseconds = trainDurationMicroseconds;
            }

            /// <summary>
            /// Gets the array fo IR pulses.
            /// </summary>
            public InfraRedPulse[] Pulses { get; }

            /// <summary>
            /// Gets the state of the receiver that triggered the event.
            /// </summary>
            public ReceiverStates State { get; }

            /// <summary>
            /// Gets the pulse train duration in microseconds.
            /// </summary>
            public long TrainDurationMicroseconds { get; }
        }

        /// <summary>
        /// Contains the raw sensor data event arguments
        /// </summary>
        /// <seealso cref="EventArgs" />
        public class InfraRedSensorPulseEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InfraRedSensorPulseEventArgs" /> class.
            /// </summary>
            /// <param name="pulse">The pulse.</param>
            internal InfraRedSensorPulseEventArgs(InfraRedPulse pulse)
                : base()
            {
                Value = pulse.Value;
                DurationMicroseconds = pulse.DurationMicroseconds;
                TimeUnits = pulse.TimeUnits;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="InfraRedSensorPulseEventArgs"/> class from being created.
            /// </summary>
            private InfraRedSensorPulseEventArgs()
                : base()
            {
                // placeholder
            }

            /// <summary>
            /// Gets the signal value
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Gets the duration microseconds.
            /// </summary>
            public long DurationMicroseconds { get; }

            /// <summary>
            /// Gets the time units.
            /// </summary>
            public long TimeUnits { get; }
        }
    }
}
