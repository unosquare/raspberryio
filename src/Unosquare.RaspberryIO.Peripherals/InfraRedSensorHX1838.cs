namespace Unosquare.RaspberryIO.Peripherals
{
    using Gpio;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
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
        private const long UsecsPerTimeUnit = 50;
        private const double GapUsecs = 5000;
        private const double GapTimeUnits = GapUsecs / UsecsPerTimeUnit;
        private const int SensorBufferLength = 128;

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
        /// Creates a string representing all the pulses passed to this method.
        /// </summary>
        /// <param name="pulses">The pulses.</param>
        /// <param name="groupSize">Number of pulse data to output per line.</param>
        /// <returns>A string representing the pulses</returns>
        public static string DebugPulses(InfraRedPulse[] pulses, int groupSize = 4)
        {
            var builder = new StringBuilder(pulses.Length * 24);
            builder.AppendLine();

            for (var i = 0; i < pulses.Length; i += groupSize)
            {
                var p = pulses[i];
                for (var offset = 0; offset < groupSize; offset++)
                {
                    if (i + offset >= pulses.Length)
                        continue;

                    p = pulses[i + offset];
                    builder.Append($" {(p.Value ? "S" : "M")} {p.DurationUsecs,7} | ");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

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
            var delayUsecs = 0L;
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

                                // The first pulse will be a gap
                                var pulse = new InfraRedPulse(Mark, timeUnitsCount);
                                pulseBuffer.Add(pulse);
                                OnInfraredSensorPulseAvailable(pulse);
                                timeUnitsCount = 0;
                                ReceiverState = ReceiverStates.Mark;
                            }

                            break;
                        }

                    // Timing Mark
                    case ReceiverStates.Mark:
                        {
                            // Mark ended; Record time
                            if (currentSensorValue == Space)
                            {
                                var pulse = new InfraRedPulse(Space, timeUnitsCount);
                                pulseBuffer.Add(pulse);
                                OnInfraredSensorPulseAvailable(pulse);

                                // Reset for next pulse
                                timeUnitsCount = 0;
                                ReceiverState = ReceiverStates.Space;
                            }

                            break;
                        }

                    // Timing Space
                    case ReceiverStates.Space:
                        {
                            // Space just ended; Record time
                            if (currentSensorValue == Mark)
                            {
                                var pulse = new InfraRedPulse(Mark, timeUnitsCount);
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
                delayUsecs = (UsecsPerTimeUnit - pulseTimer.ElapsedMicroseconds).Clamp(0, UsecsPerTimeUnit);
                if (delayUsecs > 0)
                    Pi.Timing.SleepMicroseconds(Convert.ToUInt32(delayUsecs));
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
        /// <param name="trainDurationUsecs">The train duration in microseconds.</param>
        private void OnInfraredSensorRawDataAvailable(InfraRedPulse[] pulses, ReceiverStates state, long trainDurationUsecs)
        {
            DataAvailable?.Invoke(this, new InfraRedSensorDataEventArgs(pulses, state, trainDurationUsecs));
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
                DurationUsecs = timeUnits * UsecsPerTimeUnit;
                TimeUnits = timeUnits;
            }

            /// <summary>
            /// Gets the signal value
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Gets the duration microseconds.
            /// </summary>
            public long DurationUsecs { get; }

            /// <summary>
            /// Gets the time units.
            /// </summary>
            public long TimeUnits { get; }
        }

        /// <summary>
        /// Provides decoding Methods for the NEC IR Protocol
        /// </summary>
        public static class NecDecoder
        {
            /// <summary>
            /// Decodes the IR pulses according to the NEC protocol.
            /// If the train of pulses does not match the protocol, then it returns a null byte array.
            /// </summary>
            /// <param name="pulses">The pulses.</param>
            /// <returns>The decoded bytes</returns>
            public static byte[] DecodePulses(InfraRedPulse[] pulses)
            {
                const long ShortPulseLengthMin = 560 - 160;
                const long ShortPulseLengthMax = 560 + 140;

                const long BurstSpaceLengthMin = 9000 - 1800;
                const long BurstSpaceLengthMax = 9000 + 1800;

                const long BurstMarkLengthMin = 4500 - 600;
                const long BurstMarkLengthMax = 4500 + 600;

                const int MaxBitLength = 32;

                // from here: https://www.sbprojects.net/knowledge/ir/nec.php
                var startPulseIndex = -1;

                // The first thing we look for is the ACG Space (1) which should be around 9ms
                for (var pulseIndex = 0; pulseIndex < pulses.Length; pulseIndex++)
                {
                    var p = pulses[pulseIndex];
                    if (p.Value == Space && p.DurationUsecs >= BurstSpaceLengthMin && p.DurationUsecs <= BurstSpaceLengthMax)
                    {
                        startPulseIndex = pulseIndex;
                        break;
                    }
                }

                // Return a null result if we could not find ACG Space (1)
                if (startPulseIndex == -1)
                    return null;
                else
                    startPulseIndex += 1;

                // Find the first 3900 to 5000 Mark (0 value, 4.5 Millisecond)
                for (var pulseIndex = startPulseIndex; pulseIndex < pulses.Length; pulseIndex++)
                {
                    var p = pulses[pulseIndex];
                    startPulseIndex = -1;
                    if (p.Value == Mark && p.DurationUsecs >= BurstMarkLengthMin && p.DurationUsecs <= BurstMarkLengthMax)
                    {
                        startPulseIndex = pulseIndex;
                        break;
                    }
                }

                // Return a null result if we could not find the start of the train of pulses
                if (startPulseIndex == -1)
                    return null;
                else
                    startPulseIndex += 1;

                // Verify that the last pulse is a space (1) and and it is a short pulse
                var bits = new BitArray(MaxBitLength);
                var bitCount = 0;

                var lastPulse = pulses[pulses.Length - 1];
                if (lastPulse.Value == false || lastPulse.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax) == false)
                    return null;

                // preallocate the pulse references
                var p1 = default(InfraRedPulse);
                var p2 = default(InfraRedPulse);

                // parse the bits
                for (var pulseIndex = startPulseIndex; pulseIndex < pulses.Length - 1; pulseIndex += 2)
                {
                    // logical 1 is 1 space (1) and 3 marks (0)
                    // logical 0 is 1 space (1) and 1 Mark (0)
                    p1 = pulses[pulseIndex + 0];
                    p2 = pulses[pulseIndex + 1];

                    // Expect a short Space pulse followed by a Mark pulse of variable length
                    if (p1.Value == true && p2.Value == false && p1.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax))
                        bits[bitCount++] = p2.DurationUsecs.IsBetween(ShortPulseLengthMin, ShortPulseLengthMax) ? true : false;

                    if (bitCount >= MaxBitLength)
                        break;
                }

                // Check the message is 4 bytes long
                if (bitCount != 32)
                    return null;

                // Return the result
                var result = new byte[bitCount / 8];
                bits.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        /// Represents event arguments for when a receiver buffer is ready to be decoded.
        /// </summary>
        /// <seealso cref="EventArgs" />
        public class InfraRedSensorDataEventArgs : EventArgs
        {
            internal InfraRedSensorDataEventArgs(InfraRedPulse[] pulses, ReceiverStates state, long trainDurationUsecs)
            {
                Pulses = pulses;
                State = state;
                TrainDurationUsecs = trainDurationUsecs;
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
            public long TrainDurationUsecs { get; }
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
                DurationUsecs = pulse.DurationUsecs;
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
            public long DurationUsecs { get; }

            /// <summary>
            /// Gets the time units.
            /// </summary>
            public long TimeUnits { get; }
        }
    }
}
